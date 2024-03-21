using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    public class CommandeDocument : Commande
    {
        public int nbExemplaire { get; }
        public string idLivreDvd { get; }
        public string statut { get; }

        public CommandeDocument(string id, DateTime dateCommande,  float montant, int nbExemplaire, string idLivreDvd, string statut) : base(id, dateCommande, montant)
        {
            
            this.nbExemplaire = nbExemplaire;
            this.idLivreDvd = idLivreDvd;
            this.statut = statut;
        }
    }
}
