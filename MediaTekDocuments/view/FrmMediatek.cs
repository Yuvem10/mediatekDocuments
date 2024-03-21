using System;
using System.Windows.Forms;
using MediaTekDocuments.model;
using MediaTekDocuments.controller;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;

namespace MediaTekDocuments.view

{
    /// <summary>
    /// Classe d'affichage
    /// </summary>
    public partial class FrmMediatek : Form
    {
        #region Commun
        private readonly FrmMediatekController controller;
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();
        private readonly BindingSource bdgStatuts = new BindingSource();
        string globalId = ""; 
        string globalStatut = "";
        bool globalSelected = false;
        

        /// <summary>
        /// Constructeur : création du contrôleur lié à ce formulaire
        /// </summary>
        internal FrmMediatek()
        {
            InitializeComponent();
            this.controller = new FrmMediatekController();
        }

        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesCategories">liste des objets de type Genre ou Public ou Rayon</param>
        /// <param name="bdg">bindingsource contenant les informations</param>
        /// <param name="cbx">combobox à remplir</param>
        public void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }
        #endregion

        #region Onglet Livres
        private readonly BindingSource bdgLivresListe = new BindingSource();
        private List<Livre> lesLivres = new List<Livre>();

        /// <summary>
        /// Ouverture de l'onglet Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controller.GetAllLivres();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
            RemplirLivresListeComplete();
        }


        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirLivresListe(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            dgvLivresListe.DataSource = bdgLivresListe;
            dgvLivresListe.Columns["isbn"].Visible = false;
            dgvLivresListe.Columns["idRayon"].Visible = false;
            dgvLivresListe.Columns["idGenre"].Visible = false;
            dgvLivresListe.Columns["idPublic"].Visible = false;
            dgvLivresListe.Columns["image"].Visible = false;
            dgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresListe.Columns["id"].DisplayIndex = 0;
            dgvLivresListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumRecherche.Text.Equals(""))
            {
                txbLivresTitreRecherche.Text = "";
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbLivresNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    RemplirLivresListe(livres);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbLivresTitreRecherche.Text.Equals(""))
            {
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                txbLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(txbLivresTitreRecherche.Text.ToLower()));
                RemplirLivresListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxLivresGenres.SelectedIndex < 0 && cbxLivresPublics.SelectedIndex < 0 && cbxLivresRayons.SelectedIndex < 0
                    && txbLivresNumRecherche.Text.Equals(""))
                {
                    RemplirLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre</param>
        private void AfficheLivresInfos(Livre livre)
        {
            txbLivresAuteur.Text = livre.Auteur;
            txbLivresCollection.Text = livre.Collection;
            txbLivresImage.Text = livre.Image;
            txbLivresIsbn.Text = livre.Isbn;
            txbLivresNumero.Text = livre.Id;
            txbLivresGenre.Text = livre.Genre;
            txbLivresPublic.Text = livre.Public;
            txbLivresRayon.Text = livre.Rayon;
            txbLivresTitre.Text = livre.Titre;
            string image = livre.Image;
            try
            {
                pcbLivresImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbLivresImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideLivresInfos()
        {
            txbLivresAuteur.Text = "";
            txbLivresCollection.Text = "";
            txbLivresImage.Text = "";
            txbLivresIsbn.Text = "";
            txbLivresNumero.Text = "";
            txbLivresGenre.Text = "";
            txbLivresPublic.Text = "";
            txbLivresRayon.Text = "";
            txbLivresTitre.Text = "";
            pcbLivresImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresGenres.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Genre genre = (Genre)cbxLivresGenres.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresPublics.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Public lePublic = (Public)cbxLivresPublics.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresRayons.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxLivresRayons.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresListe(livres);
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    AfficheLivresInfos(livre);
                }
                catch
                {
                    VideLivresZones();
                }
            }
            else
            {
                VideLivresInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresZones()
        {
            cbxLivresGenres.SelectedIndex = -1;
            cbxLivresRayons.SelectedIndex = -1;
            cbxLivresPublics.SelectedIndex = -1;
            txbLivresNumRecherche.Text = "";
            txbLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = dgvLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresListe(sortedList);
        }
        #endregion

        #region Onglet Dvd
        private readonly BindingSource bdgDvdListe = new BindingSource();
        private List<Dvd> lesDvd = new List<Dvd>();

        /// <summary>
        /// Ouverture de l'onglet Dvds : 
        /// appel des méthodes pour remplir le datagrid des dvd et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controller.GetAllDvd();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="Dvds">liste de dvd</param>
        private void RemplirDvdListe(List<Dvd> Dvds)
        {
            bdgDvdListe.DataSource = Dvds;
            dgvDvdListe.DataSource = bdgDvdListe;
            dgvDvdListe.Columns["idRayon"].Visible = false;
            dgvDvdListe.Columns["idGenre"].Visible = false;
            dgvDvdListe.Columns["idPublic"].Visible = false;
            dgvDvdListe.Columns["image"].Visible = false;
            dgvDvdListe.Columns["synopsis"].Visible = false;
            dgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdListe.Columns["id"].DisplayIndex = 0;
            dgvDvdListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du Dvd dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumRecherche.Text.Equals(""))
            {
                txbDvdTitreRecherche.Text = "";
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>() { dvd };
                    RemplirDvdListe(Dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirDvdListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbDvdTitreRecherche.Text.Equals(""))
            {
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                txbDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(txbDvdTitreRecherche.Text.ToLower()));
                RemplirDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdGenres.SelectedIndex < 0 && cbxDvdPublics.SelectedIndex < 0 && cbxDvdRayons.SelectedIndex < 0
                    && txbDvdNumRecherche.Text.Equals(""))
                {
                    RemplirDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du dvd sélectionné
        /// </summary>
        /// <param name="dvd">le dvd</param>
        private void AfficheDvdInfos(Dvd dvd)
        {
            txbDvdRealisateur.Text = dvd.Realisateur;
            txbDvdSynopsis.Text = dvd.Synopsis;
            txbDvdImage.Text = dvd.Image;
            txbDvdDuree.Text = dvd.Duree.ToString();
            txbDvdNumero.Text = dvd.Id;
            txbDvdGenre.Text = dvd.Genre;
            txbDvdPublic.Text = dvd.Public;
            txbDvdRayon.Text = dvd.Rayon;
            txbDvdTitre.Text = dvd.Titre;
            string image = dvd.Image;
            try
            {
                pcbDvdImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd
        /// </summary>
        private void VideDvdInfos()
        {
            txbDvdRealisateur.Text = "";
            txbDvdSynopsis.Text = "";
            txbDvdImage.Text = "";
            txbDvdDuree.Text = "";
            txbDvdNumero.Text = "";
            txbDvdGenre.Text = "";
            txbDvdPublic.Text = "";
            txbDvdRayon.Text = "";
            txbDvdTitre.Text = "";
            pcbDvdImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdGenres.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdPublics.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdRayons.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                    AfficheDvdInfos(dvd);
                }
                catch
                {
                    VideDvdZones();
                }
            }
            else
            {
                VideDvdInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des Dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdZones()
        {
            cbxDvdGenres.SelectedIndex = -1;
            cbxDvdRayons.SelectedIndex = -1;
            cbxDvdPublics.SelectedIndex = -1;
            txbDvdNumRecherche.Text = "";
            txbDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdZones();
            string titreColonne = dgvDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdListe(sortedList);
        }
        #endregion

        #region Onglet Revues
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        private List<Revue> lesRevues = new List<Revue>();

        /// <summary>
        /// Ouverture de l'onglet Revues : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="revues"></param>
        private void RemplirRevuesListe(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            dgvRevuesListe.DataSource = bdgRevuesListe;
            dgvRevuesListe.Columns["idRayon"].Visible = false;
            dgvRevuesListe.Columns["idGenre"].Visible = false;
            dgvRevuesListe.Columns["idPublic"].Visible = false;
            dgvRevuesListe.Columns["image"].Visible = false;
            dgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvRevuesListe.Columns["id"].DisplayIndex = 0;
            dgvRevuesListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage de la revue dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumRecherche.Text.Equals(""))
            {
                txbRevuesTitreRecherche.Text = "";
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>() { revue };
                    RemplirRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirRevuesListeComplete();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des revues dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbRevuesTitreRecherche.Text.Equals(""))
            {
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                txbRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesRevues.FindAll(x => x.Titre.ToLower().Contains(txbRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxRevuesGenres.SelectedIndex < 0 && cbxRevuesPublics.SelectedIndex < 0 && cbxRevuesRayons.SelectedIndex < 0
                    && txbRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirRevuesListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheRevuesInfos(Revue revue)
        {
            txbRevuesPeriodicite.Text = revue.Periodicite;
            txbRevuesImage.Text = revue.Image;
            txbRevuesDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbRevuesNumero.Text = revue.Id;
            txbRevuesGenre.Text = revue.Genre;
            txbRevuesPublic.Text = revue.Public;
            txbRevuesRayon.Text = revue.Rayon;
            txbRevuesTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbRevuesImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de la reuve
        /// </summary>
        private void VideRevuesInfos()
        {
            txbRevuesPeriodicite.Text = "";
            txbRevuesImage.Text = "";
            txbRevuesDateMiseADispo.Text = "";
            txbRevuesNumero.Text = "";
            txbRevuesGenre.Text = "";
            txbRevuesPublic.Text = "";
            txbRevuesRayon.Text = "";
            txbRevuesTitre.Text = "";
            pcbRevuesImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesGenres.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Genre genre = (Genre)cbxRevuesGenres.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesPublics.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Public lePublic = (Public)cbxRevuesPublics.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesRayons.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxRevuesRayons.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesInfos(revue);
                }
                catch
                {
                    VideRevuesZones();
                }
            }
            else
            {
                VideRevuesInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des revues
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideRevuesZones()
        {
            cbxRevuesGenres.SelectedIndex = -1;
            cbxRevuesRayons.SelectedIndex = -1;
            cbxRevuesPublics.SelectedIndex = -1;
            txbRevuesNumRecherche.Text = "";
            txbRevuesTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = dgvRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirRevuesListe(sortedList);
        }
        #endregion

        #region Onglet Paarutions
        private readonly BindingSource bdgExemplairesListe = new BindingSource();
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();
        const string ETATNEUF = "00001";

        /// <summary>
        /// Ouverture de l'onglet : récupère le revues et vide tous les champs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabReceptionRevue_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            txbReceptionRevueNumero.Text = "";
        }

        /// <summary>
        /// Remplit le dategrid des exemplaires avec la liste reçue en paramètre
        /// </summary>
        /// <param name="exemplaires">liste d'exemplaires</param>
        private void RemplirReceptionExemplairesListe(List<Exemplaire> exemplaires)
        {
            if (exemplaires != null)
            {
                bdgExemplairesListe.DataSource = exemplaires;
                dgvReceptionExemplairesListe.DataSource = bdgExemplairesListe;
                dgvReceptionExemplairesListe.Columns["idEtat"].Visible = false;
                dgvReceptionExemplairesListe.Columns["id"].Visible = false;
                dgvReceptionExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvReceptionExemplairesListe.Columns["numero"].DisplayIndex = 0;
                dgvReceptionExemplairesListe.Columns["dateAchat"].DisplayIndex = 1;
            }
            else
            {
                bdgExemplairesListe.DataSource = null;
            }
        }

        /// <summary>
        /// Recherche d'un numéro de revue et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionRechercher_Click(object sender, EventArgs e)
        {
            if (!txbReceptionRevueNumero.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbReceptionRevueNumero.Text));
                if (revue != null)
                {
                    AfficheReceptionRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                }
            }
        }

        /// <summary>
        /// Si le numéro de revue est modifié, la zone de l'exemplaire est vidée et inactive
        /// les informations de la revue son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbReceptionRevueNumero_TextChanged(object sender, EventArgs e)
        {
            txbReceptionRevuePeriodicite.Text = "";
            txbReceptionRevueImage.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueTitre.Text = "";
            pcbReceptionRevueImage.Image = null;
            RemplirReceptionExemplairesListe(null);
            AccesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheReceptionRevueInfos(Revue revue)
        {
            // informations sur la revue
            txbReceptionRevuePeriodicite.Text = revue.Periodicite;
            txbReceptionRevueImage.Text = revue.Image;
            txbReceptionRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbReceptionRevueNumero.Text = revue.Id;
            txbReceptionRevueGenre.Text = revue.Genre;
            txbReceptionRevuePublic.Text = revue.Public;
            txbReceptionRevueRayon.Text = revue.Rayon;
            txbReceptionRevueTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbReceptionRevueImage.Image = null;
            }
            // affiche la liste des exemplaires de la revue
            AfficheReceptionExemplairesRevue();
        }

        /// <summary>
        /// Récupère et affiche les exemplaires d'une revue
        /// </summary>
        private void AfficheReceptionExemplairesRevue()
        {
            string idDocuement = txbReceptionRevueNumero.Text;
            lesExemplaires = controller.GetExemplairesRevue(idDocuement);
            RemplirReceptionExemplairesListe(lesExemplaires);
            AccesReceptionExemplaireGroupBox(true);
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la réception d'un exemplaire
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces">true ou false</param>
        private void AccesReceptionExemplaireGroupBox(bool acces)
        {
            grpReceptionExemplaire.Enabled = acces;
            txbReceptionExemplaireImage.Text = "";
            txbReceptionExemplaireNumero.Text = "";
            pcbReceptionExemplaireImage.Image = null;
            dtpReceptionExemplaireDate.Value = DateTime.Now;
        }

        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire à insérer)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                // positionnement à la racine du disque où se trouve le dossier actuel
                InitialDirectory = Path.GetPathRoot(Environment.CurrentDirectory),
                Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbReceptionExemplaireImage.Text = filePath;
            try
            {
                pcbReceptionExemplaireImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbReceptionExemplaireImage.Image = null;
            }
        }

        /// <summary>
        /// Enregistrement du nouvel exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!txbReceptionExemplaireNumero.Text.Equals(""))
            {
                try
                {
                    int numero = int.Parse(txbReceptionExemplaireNumero.Text);
                    DateTime dateAchat = dtpReceptionExemplaireDate.Value;
                    string photo = txbReceptionExemplaireImage.Text;
                    string idEtat = ETATNEUF;
                    string idDocument = txbReceptionRevueNumero.Text;
                    Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument);
                    if (controller.CreerExemplaire(exemplaire))
                    {
                        AfficheReceptionExemplairesRevue();
                    }
                    else
                    {
                        MessageBox.Show("numéro de publication déjà existant", "Erreur");
                    }
                }
                catch
                {
                    MessageBox.Show("le numéro de parution doit être numérique", "Information");
                    txbReceptionExemplaireNumero.Text = "";
                    txbReceptionExemplaireNumero.Focus();
                }
            }
            else
            {
                MessageBox.Show("numéro de parution obligatoire", "Information");
            }
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvReceptionExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplaires.OrderBy(o => o.Numero).Reverse().ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplaires.OrderBy(o => o.DateAchat).Reverse().ToList();
                    break;
                case "Photo":
                    sortedList = lesExemplaires.OrderBy(o => o.Photo).ToList();
                    break;
            }
            RemplirReceptionExemplairesListe(sortedList);
        }

        /// <summary>
        /// affichage de l'image de l'exemplaire suite à la sélection d'un exemplaire dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
                string image = exemplaire.Photo;
                try
                {
                    pcbReceptionExemplaireRevueImage.Image = Image.FromFile(image);
                }
                catch
                {
                    pcbReceptionExemplaireRevueImage.Image = null;
                }
            }
            else
            {
                pcbReceptionExemplaireRevueImage.Image = null;
            }
        }
        #endregion





        #region Onglet Commandes Livres
        private readonly BindingSource bdgCommandes = new BindingSource();
        private readonly BindingSource bdgCommandeDocuments = new BindingSource();



        /// <summary>
        /// actions when the order tab is opened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabCommande_Enter(object sender, EventArgs e)
        {
            lesLivres = controller.GetAllLivres();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxBooksGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxBooksPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxBooksRayons);
            fillBooksFullList();
            fillStatusCombo();
            fillOrdersFullList();
            btnCommandeNouveau_Click(sender, e);

        }

        /// <summary>
        /// display the information of the selected line
        /// </summary>
        private void dgvOrdersList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvOrdersList.CurrentCell != null)
            {
                try
                {
                    string idCommande = (string)dgvOrdersList.CurrentRow.Cells[0].Value;
                    CommandeDocument commandeDocument = controller.retrieveCommandeDocumentById(idCommande);
                    Commande commande = controller.retrieveCommandeById(idCommande);
                    txbCommandeId.Text = commande.id.ToString();
                    dtpCommandeDate.Value = commande.dateCommande;
                    txbCommandeMontant.Text = commande.montant.ToString();
                    txbCommandeNbExemplaires.Text = commandeDocument.nbExemplaire.ToString();
                    cbxStatut.SelectedValue = commandeDocument.statut;

                    // set the book title of selected line on the textboxlivrecom
                    int id = dgvOrdersList.Columns["LivreCom"].Index;
                    txbLivreCom.Text = dgvOrdersList.CurrentRow.Cells[id].Value.ToString();


                    // disable the fields
                    txbCommandeId.Enabled = false;
                    dtpCommandeDate.Enabled = false;
                    txbCommandeMontant.Enabled = false;
                    txbCommandeNbExemplaires.Enabled = false;
                    txbLivreCom.Enabled = false;

                    // enabled the cbxstatut
                    cbxStatut.Enabled = true;

                    // disabled the buttons
                    btnCommandeAjouter.Enabled = false;
                    btnCommandeModifier.Enabled = true;
                    btnCommandeSupprimer.Enabled = true;
                    btnCommandeNouvelle.Enabled = true;

                    // set the value of cbxStatut to the global variable
                    globalStatut = cbxStatut.Text;

                    // set the globalSelected to true
                    globalSelected = true;

                }
                catch
                {
                    emptyOrderZone();
                }
            }
            else
            {
                emptyOrderZone();
            }
        }

        /// <summary>
        /// set the id value of the selected line in the datagrid of books
        /// </summary>
        private void dgvLivresCmd_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresCmd.CurrentCell != null)
            {
                try
                {
                    int id = dgvLivresCmd.Columns["id"].Index;
                    globalId = dgvLivresCmd.CurrentRow.Cells[id].Value.ToString();


                    if (!globalSelected)
                    {
                        // set the value of the selected line in the textboxlivrecom
                        int idLivreCom = dgvLivresCmd.Columns["titre"].Index;
                        txbLivreCom.Text = dgvLivresCmd.CurrentRow.Cells[idLivreCom].Value.ToString();
                    }
                    

                }
                catch
                {
                    
                }
            }
            else
            {
                
            }
        }


        /// <summary>
        /// Add a new order to the database
        /// </summary>
        private void btnCommandeAjouter_Click(object sender, EventArgs e)
        {
            if (txbCommandeMontant.Text == "" || txbCommandeNbExemplaires.Text == "")
            {
                MessageBox.Show("Veuillez remplir tous les champs");
            }
            else
            {
                CommandeDocument commandeDocument = new CommandeDocument(txbCommandeId.Text, dtpCommandeDate.Value, int.Parse(txbCommandeMontant.Text), int.Parse(txbCommandeNbExemplaires.Text), globalId, cbxStatut.Text); ;
                controller.AddCommandeDocument(commandeDocument);
                fillOrdersFullList();
            }

            
            
        }

  

        ///<summary>
        /// new order button
        /// </summary>
        private void btnCommandeNouveau_Click(object sender, EventArgs e)
        {
            globalSelected = false;
            emptyOrderZone();
            txbCommandeId.Enabled = true;
            dtpCommandeDate.Enabled = true;
            txbCommandeMontant.Enabled = true;
            txbCommandeNbExemplaires.Enabled = true;

            // set the value "en cours" by default in the status combo
            cbxStatut.SelectedIndex = 0;
            cbxStatut.Enabled = false;

            // set the name of the order
            string orderName = controller.SetNameOrder(true);
            txbCommandeId.Text = orderName;

            

            // disabled the input of the name of the order
            txbCommandeId.Enabled = false;

            btnCommandeAjouter.Enabled = true;
            btnCommandeModifier.Enabled = false;
            btnCommandeSupprimer.Enabled = false;

        }

        /// <summary>
        /// click on the modify button to save the change of the status of the order
        /// </summary>
        private void btnCommandeModifier_Click(object sender, EventArgs e)
        {

            bool refusable = false;
            if (globalStatut == "Livrée" || globalStatut == "Réglée")
            {
                if (cbxStatut.Text == "En cours" || cbxStatut.Text == "Relancée")
                {
                    MessageBox.Show("La commande est déjà " + globalStatut + " elle ne peut pas revenir à une étape intermédiaire");
                    refusable = true;
                }
                else
                {
                    refusable = false;
                }
            }
            if (cbxStatut.Text == "Réglée" && globalStatut != "Livrée")
            {
                MessageBox.Show("Il faut que la commande soit livrée pour être réglée");
                refusable = true;
            }

            if (refusable == false)
            {
                controller.updateOrderStatus(txbCommandeId.Text, cbxStatut.Text);
                fillOrdersFullList();
            }   
        }

        ///<summary>
        ///click on the delete button to delete the order
        /// </summary>
        private void btnCommandeSupprimer_Click(object sender, EventArgs e)
        {
            if (cbxStatut.Text == "Livrée")
            {
                MessageBox.Show("La commande est livrée, elle ne peut pas être supprimée");
            }
            else
            {
                // delete the order
                controller.deleteOrder(txbCommandeId.Text);
                fillOrdersFullList();
                MessageBox.Show("La commande a été supprimée avec succès", "Suppression de commande");
            }
            
        }
       

        /// <summary>
        /// empty the order zone
        /// </summary>
        private void emptyOrderZone()
        {
            txbCommandeId.Text = "";
            dtpCommandeDate.Value = DateTime.Now;
            txbCommandeMontant.Text = "";
            txbCommandeNbExemplaires.Text = "";
            cbxStatut.SelectedIndex = -1;
        }

        ///<summary>
        /// fill the status combo
        /// </summary>
        private void fillStatusCombo()
        {
            List<Suivi> etats = controller.GetAllStatus();
            bdgCommandes.DataSource = etats;
            cbxStatut.DataSource = bdgCommandes;
            cbxStatut.ValueMember = "statut";
            // disable the input of the status combo
            cbxStatut.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        /// <summary>
        /// clear the search and filter zones
        /// </summary>
        private void emptyBooksZone()
        {
            txbBooksNumSearch.Text = "";
            cbxBooksGenres.SelectedIndex = -1;
            cbxBooksRayons.SelectedIndex = -1;
            cbxBooksPublics.SelectedIndex = -1;
            txbBooksTitleSearch.Text = "";
        }


        /// <summary>
        /// display the full list of books
        /// </summary>
        private void fillBooksFullList()
        {
            fillBooksList(lesLivres);
            emptyBooksZone();
        }

        /// <summary>
        /// display the full list of orders
        /// </summary>
        private void fillOrdersFullList()
        {
            int orderCount = controller.GetOrderCount();
            if (orderCount == 0)
            {
                globalSelected = false;
                emptyOrderZone();
                txbCommandeId.Enabled = true;
                dtpCommandeDate.Enabled = true;
                txbCommandeMontant.Enabled = true;
                txbCommandeNbExemplaires.Enabled = true;
                txbLivreCom.Enabled = false;

                // set the value "en cours" by default in the status combo
                cbxStatut.SelectedIndex = 0;
                cbxStatut.Enabled = false;

                // set the name of the order
                string orderName = controller.SetNameOrder(true);
                txbCommandeId.Text = orderName;



                // disabled the input of the name of the order
                txbCommandeId.Enabled = false;

                btnCommandeAjouter.Enabled = true;
                btnCommandeModifier.Enabled = false;
                btnCommandeSupprimer.Enabled = false;
                btnCommandeNouvelle.Enabled = false;
            }
            List<CommandeDocument> commandeDocuments = controller.GetAllCommandeDocumentsLivre();
            fillOrdersList(commandeDocuments);
        }

        /// <summary>
        /// fill the datagrid with the list of orders
        /// </summary>
        /// <param name="commandeDocuments">liste de commandes</param>
        private void fillOrdersList(List<CommandeDocument> commandeDocuments)
        {
            // Effacer les colonnes existantes
            dgvOrdersList.Columns.Clear();

            // Ajouter les colonnes nécessaires
            dgvOrdersList.Columns.Add("Id", "Nom");
            dgvOrdersList.Columns.Add("DateCommande", "Date de commande");
            dgvOrdersList.Columns.Add("Montant", "Montant");
            dgvOrdersList.Columns.Add("NbExemplaire", "Nombre d'exemplaires");
            dgvOrdersList.Columns.Add("LivreCom", "Livre commandé");
            dgvOrdersList.Columns.Add("Statut", "Statut");

            // Remplir les lignes avec les données de la liste commandeDocuments
            foreach (var commande in commandeDocuments)
            {
                Commande currentCommande = controller.retrieveCommandeById(commande.id);
                Document livre = controller.retrieveDocumentById(commande.idLivreDvd);
                string titleBook = livre.Titre;
                dgvOrdersList.Rows.Add(commande.id, currentCommande.dateCommande.ToShortDateString(), currentCommande.montant, commande.nbExemplaire, titleBook ,commande.statut);
            }
        }

        /// <summary>
        /// fill the datagrid with the list of books
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void fillBooksList(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            dgvLivresCmd.DataSource = bdgLivresListe;
            dgvLivresCmd.Columns["isbn"].Visible = false;
            dgvLivresCmd.Columns["idRayon"].Visible = false;
            dgvLivresCmd.Columns["idGenre"].Visible = false;
            dgvLivresCmd.Columns["idPublic"].Visible = false;
            dgvLivresCmd.Columns["image"].Visible = false;
            dgvLivresCmd.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresCmd.Columns["id"].DisplayIndex = 0;
            dgvLivresCmd.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// sort the list of books according to the column clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLivresCmd_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            emptyBooksZone();
            string titreColonne = dgvLivresCmd.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            fillBooksList(sortedList);
        }



        /// <summary>
        /// search for a book by its number
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandPageNumBookSearch_Click(object sender, EventArgs e)
        {
            if (!txbBooksNumSearch.Text.Equals(""))
            {
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbBooksNumSearch.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    fillBooksList(livres);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    fillBooksFullList();
                }
            }
            else
            {
                fillBooksFullList();
            }
        }
        /// <summary>
        /// search for a book by its title and display the result 
        /// during each character added or removed from the search zone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbBooksTitleSearch_TextChanged(object sender, EventArgs e)
        {
            if (!txbBooksTitleSearch.Text.Equals(""))
            {
                cbxBooksGenres.SelectedIndex = -1;
                cbxBooksRayons.SelectedIndex = -1;
                cbxBooksPublics.SelectedIndex = -1;
                txbBooksNumSearch.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(txbBooksTitleSearch.Text.ToLower()));
                fillBooksList(lesLivresParTitre);
            }
            else
            {
                if (cbxBooksGenres.SelectedIndex < 0 && cbxBooksPublics.SelectedIndex < 0 && cbxBooksRayons.SelectedIndex < 0
                    && txbBooksTitleSearch.Text.Equals(""))
                {
                    fillBooksFullList();
                }
            }
        }
        /// <summary>
        /// cancel the search and display the full list of books
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBooksAnnulPublics_Click(object sender, EventArgs e)
        {
            fillBooksFullList();
        }

        /// <summary>
        /// cancel the search and display the full list of books
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBooksAnnulGenres_Click(object sender, EventArgs e)
        {
            fillBooksFullList();
        }

        /// <summary>
        /// cancel the search and display the full list of books
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBooksAnnulRayons_Click(object sender, EventArgs e)
        {
            fillBooksFullList();
        }

        #endregion

        #region Onglet Commandes de DVD

        private readonly BindingSource bdgCommandesDvd = new BindingSource();
        private readonly BindingSource bdgCommandeDocumentsDvd = new BindingSource();



        /// <summary>
        /// actions when the order tab is opened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabCommandeDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controller.GetAllDvd();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxBooksGenresDvd);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxBooksPublicsDvd);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxBooksRayonsDvd);
            fillBooksFullListDvd();
            fillStatusComboDvd();
            fillOrdersFullListDvd();
            btnCommandeNouveauDvd_Click(sender, e);

        }

        /// <summary>
        /// display the information of the selected line
        /// </summary>
        private void dgvOrdersListDvd_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvOrdersListDvd.CurrentCell != null)
            {
                try
                {
                    string idCommande = (string)dgvOrdersListDvd.CurrentRow.Cells[0].Value;
                    CommandeDocument commandeDocument = controller.retrieveCommandeDocumentById(idCommande);
                    Commande commande = controller.retrieveCommandeById(idCommande);
                    txbCommandeIdDvd.Text = commande.id.ToString();
                    dtpCommandeDateDvd.Value = commande.dateCommande;
                    txbCommandeMontantDvd.Text = commande.montant.ToString();
                    txbCommandeNbExemplairesDvd.Text = commandeDocument.nbExemplaire.ToString();
                    cbxStatutDvd.SelectedValue = commandeDocument.statut;

                    // set the book title of selected line on the textboxlivrecom
                    int id = dgvOrdersListDvd.Columns["DvdCom"].Index;
                    txbLivreComDvd.Text = dgvOrdersListDvd.CurrentRow.Cells[id].Value.ToString();


                    // disable the fields
                    txbCommandeIdDvd.Enabled = false;
                    dtpCommandeDateDvd.Enabled = false;
                    txbCommandeMontantDvd.Enabled = false;
                    txbCommandeNbExemplairesDvd.Enabled = false;
                    txbLivreComDvd.Enabled = false;

                    // enabled the cbxstatut
                    cbxStatutDvd.Enabled = true;

                    // disabled the buttons
                    btnCommandeAjouterDvd.Enabled = false;
                    btnCommandeModifierDvd.Enabled = true;
                    btnCommandeSupprimerDvd.Enabled = true;
                    btnCommandeNouvelleDvd.Enabled = true;

                    // set the value of cbxStatut to the global variable
                    globalStatut = cbxStatutDvd.Text;

                    // set the globalSelected to true
                    globalSelected = true;

                }
                catch
                {
                    emptyOrderZoneDvd();
                }
            }
            else
            {
                emptyOrderZoneDvd();
            }
        }
        
        /// <summary>
        /// set the id value of the selected line in the datagrid of books
        /// </summary>
        private void dgvLivresCmdDvd_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresCmdDvd.CurrentCell != null)
            {
                try
                {
                    int id = dgvLivresCmdDvd.Columns["id"].Index;
                    globalId = dgvLivresCmdDvd.CurrentRow.Cells[id].Value.ToString();


                    if (!globalSelected)
                    {
                        // set the value of the selected line in the textboxlivrecom
                        int idLivreCom = dgvLivresCmdDvd.Columns["titre"].Index;
                        txbLivreComDvd.Text = dgvLivresCmdDvd.CurrentRow.Cells[idLivreCom].Value.ToString();
                    }
                    

                }
                catch
                {
                    
                }
            }
            else
            {
                
            }
        }


        /// <summary>
        /// Add a new order to the database
        /// </summary>
        private void btnCommandeAjouterDvd_Click(object sender, EventArgs e)
        {

            if (txbCommandeNbExemplairesDvd.Text == "" || txbCommandeMontantDvd.Text == "")
            {
                MessageBox.Show("Veuillez remplir tous les champs");
            }
            else
            {
                CommandeDocument commandeDocument = new CommandeDocument(txbCommandeIdDvd.Text, dtpCommandeDateDvd.Value, int.Parse(txbCommandeMontantDvd.Text), int.Parse(txbCommandeNbExemplairesDvd.Text), globalId, cbxStatutDvd.Text); ;
                controller.AddCommandeDocument(commandeDocument);
                fillOrdersFullListDvd();
            }
        }

  

        ///<summary>
        /// new order button
        /// </summary>
        private void btnCommandeNouveauDvd_Click(object sender, EventArgs e)
        {
            globalSelected = false;
            emptyOrderZoneDvd();
            txbCommandeIdDvd.Enabled = true;
            dtpCommandeDateDvd.Enabled = true;
            txbCommandeMontantDvd.Enabled = true;
            txbCommandeNbExemplairesDvd.Enabled = true;

            // set the value "en cours" by default in the status combo
            cbxStatutDvd.SelectedIndex = 0;
            cbxStatutDvd.Enabled = false;

            // set the name of the order
            string orderName = controller.SetNameOrder(false);
            txbCommandeIdDvd.Text = orderName;

            

            // disabled the input of the name of the order
            txbCommandeIdDvd.Enabled = false;

            btnCommandeAjouterDvd.Enabled = true;
            btnCommandeModifierDvd.Enabled = false;
            btnCommandeSupprimerDvd.Enabled = false;

        }

        /// <summary>
        /// click on the modify button to save the change of the status of the order
        /// </summary>
        private void btnCommandeModifierDvd_Click(object sender, EventArgs e)
        {

            bool refusable = false;
            if (globalStatut == "Livrée" || globalStatut == "Réglée")
            {
                if (cbxStatutDvd.Text == "En cours" || cbxStatutDvd.Text == "Relancée")
                {
                    MessageBox.Show("La commande est déjà " + globalStatut + " elle ne peut pas revenir à une étape intermédiaire");
                    refusable = true;
                }
                else
                {
                    refusable = false;
                }
            }
            if (cbxStatutDvd.Text == "Réglée" && globalStatut != "Livrée")
            {
                MessageBox.Show("Il faut que la commande soit livrée pour être réglée");
                refusable = true;
            }

            if (refusable == false)
            {
                controller.updateOrderStatus(txbCommandeIdDvd.Text, cbxStatutDvd.Text);
                fillOrdersFullListDvd();
            }   
        }

        ///<summary>
        ///click on the delete button to delete the order
        /// </summary>
        private void btnCommandeSupprimerDvd_Click(object sender, EventArgs e)
        {
            if (cbxStatutDvd.Text == "Livrée")
            {
                MessageBox.Show("La commande est livrée, elle ne peut pas être supprimée");
            }
            else
            {
                // delete the order
                controller.deleteOrder(txbCommandeIdDvd.Text);
                fillOrdersFullListDvd();
                MessageBox.Show("La commande a été supprimée avec succès", "Suppression de commande");
            }
            
        }
       

        /// <summary>
        /// empty the order zone
        /// </summary>
        private void emptyOrderZoneDvd()
        {
            txbCommandeIdDvd.Text = "";
            dtpCommandeDateDvd.Value = DateTime.Now;
            txbCommandeMontantDvd.Text = "";
            txbCommandeNbExemplairesDvd.Text = "";
            cbxStatutDvd.SelectedIndex = -1;
        }

        ///<summary>
        /// fill the status combo
        /// </summary>
        private void fillStatusComboDvd()
        {
            List<Suivi> etats = controller.GetAllStatus();
            bdgCommandesDvd.DataSource = etats;
            cbxStatutDvd.DataSource = bdgCommandesDvd;
            cbxStatutDvd.ValueMember = "statut";
            // disable the input of the status combo
            cbxStatutDvd.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        /// <summary>
        /// clear the search and filter zones
        /// </summary>
        private void emptyBooksZoneDvd()
        {
            txbBooksNumSearchDvd.Text = "";
            cbxBooksGenresDvd.SelectedIndex = -1;
            cbxBooksRayonsDvd.SelectedIndex = -1;
            cbxBooksPublicsDvd.SelectedIndex = -1;
            txbBooksTitleSearchDvd.Text = "";
        }


        /// <summary>
        /// display the full list of books
        /// </summary>
        private void fillBooksFullListDvd()
        {
            fillBooksListDvd(lesDvd);
            emptyBooksZoneDvd();
        }

        /// <summary>
        /// display the full list of orders
        /// </summary>
        private void fillOrdersFullListDvd()
        {
            int orderCount = controller.GetOrderCount();
            if (orderCount == 0)
            {
                globalSelected = false;
                emptyOrderZoneDvd();
                txbCommandeIdDvd.Enabled = true;
                dtpCommandeDateDvd.Enabled = true;
                txbCommandeMontantDvd.Enabled = true;
                txbCommandeNbExemplairesDvd.Enabled = true;
                txbLivreComDvd.Enabled = false;

                // set the value "en cours" by default in the status combo
                cbxStatutDvd.SelectedIndex = 0;
                cbxStatutDvd.Enabled = false;

                // set the name of the order
                string orderName = controller.SetNameOrder(false);
                txbCommandeIdDvd.Text = orderName;



                // disabled the input of the name of the order
                txbCommandeIdDvd.Enabled = false;

                btnCommandeAjouterDvd.Enabled = true;
                btnCommandeModifierDvd.Enabled = false;
                btnCommandeSupprimerDvd.Enabled = false;
                btnCommandeNouvelleDvd.Enabled = false;
            }
            List<CommandeDocument> commandeDocuments = controller.GetAllCommandeDocumentsDvd();
            fillOrdersListDvd(commandeDocuments);
        }

        /// <summary>
        /// fill the datagrid with the list of orders
        /// </summary>
        /// <param name="commandeDocuments">liste de commandes</param>
        private void fillOrdersListDvd(List<CommandeDocument> commandeDocuments)
        {
            // Effacer les colonnes existantes
            dgvOrdersListDvd.Columns.Clear();

            // Ajouter les colonnes nécessaires
            dgvOrdersListDvd.Columns.Add("Id", "Nom");
            dgvOrdersListDvd.Columns.Add("DateCommande", "Date de commande");
            dgvOrdersListDvd.Columns.Add("Montant", "Montant");
            dgvOrdersListDvd.Columns.Add("NbExemplaire", "Nombre d'exemplaires");
            dgvOrdersListDvd.Columns.Add("DvdCom", "Dvd commandé");
            dgvOrdersListDvd.Columns.Add("Statut", "Statut");

            // Remplir les lignes avec les données de la liste commandeDocuments
            foreach (var commande in commandeDocuments)
            {
                Commande currentCommande = controller.retrieveCommandeById(commande.id);
                Document dvd = controller.retrieveDocumentById(commande.idLivreDvd);
                string titleDvd = dvd.Titre;
                dgvOrdersListDvd.Rows.Add(commande.id, currentCommande.dateCommande.ToShortDateString(), currentCommande.montant, commande.nbExemplaire, titleDvd ,commande.statut);
            }
        }

        /// <summary>
        /// fill the datagrid with the list of books
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void fillBooksListDvd(List<Dvd> dvd)
        {
            bdgDvdListe.DataSource = dvd;
            dgvLivresCmdDvd.DataSource = bdgDvdListe;
            
            dgvLivresCmdDvd.Columns["idRayon"].Visible = false;
            dgvLivresCmdDvd.Columns["idGenre"].Visible = false;
            dgvLivresCmdDvd.Columns["idPublic"].Visible = false;
            dgvLivresCmdDvd.Columns["image"].Visible = false; 
            dgvLivresCmdDvd.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresCmdDvd.Columns["id"].DisplayIndex = 0;
            dgvLivresCmdDvd.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// sort the list of books according to the column clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLivresCmdDvd_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            emptyBooksZoneDvd();
            string titreColonne = dgvLivresCmdDvd.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Synopsis":
                    sortedList = lesDvd.OrderBy(o => o.Synopsis).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            fillBooksListDvd(sortedList);
        }



        /// <summary>
        /// search for a book by its number
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandPageNumBookSearchDvd_Click(object sender, EventArgs e)
        {
            if (!txbBooksNumSearchDvd.Text.Equals(""))
            {
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbBooksNumSearchDvd.Text));
                if (dvd != null)
                {
                    List<Dvd> dvds = new List<Dvd>() { dvd };
                    fillBooksListDvd(dvds);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    fillBooksFullListDvd();
                }
            }
            else
            {
                fillBooksFullListDvd();
            }
        }
        /// <summary>
        /// search for a book by its title and display the result 
        /// during each character added or removed from the search zone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbBooksTitleSearchDvd_TextChanged(object sender, EventArgs e)
        {
            if (!txbBooksTitleSearchDvd.Text.Equals(""))
            {
                cbxBooksGenresDvd.SelectedIndex = -1;
                cbxBooksRayonsDvd.SelectedIndex = -1;
                cbxBooksPublicsDvd.SelectedIndex = -1;
                txbBooksNumSearchDvd.Text = "";
                List<Dvd> lesDvdsParTitre;
                lesDvdsParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(txbBooksTitleSearchDvd.Text.ToLower()));
                fillBooksListDvd(lesDvdsParTitre);
            }
            else
            {
                if (cbxBooksGenresDvd.SelectedIndex < 0 && cbxBooksPublicsDvd.SelectedIndex < 0 && cbxBooksRayonsDvd.SelectedIndex < 0
                    && txbBooksTitleSearchDvd.Text.Equals(""))
                {
                    fillBooksFullListDvd();
                }
            }
        }
        /// <summary>
        /// cancel the search and display the full list of books
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBooksAnnulPublicsDvd_Click(object sender, EventArgs e)
        {
            fillBooksFullListDvd();
        }

        /// <summary>
        /// cancel the search and display the full list of books
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBooksAnnulGenresDvd_Click(object sender, EventArgs e)
        {
            fillBooksFullListDvd();
        }

        /// <summary>
        /// cancel the search and display the full list of books
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBooksAnnulRayonsDvd_Click(object sender, EventArgs e)
        {
            fillBooksFullListDvd();
        }

        #endregion









        private void txbLivresNumRecherche_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgvLivresListe_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void grpLivresRecherche_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void label63_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label68_Click(object sender, EventArgs e)
        {

        }

        private void dgvOrdersList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label69_Click(object sender, EventArgs e)
        {

        }
    }
}
