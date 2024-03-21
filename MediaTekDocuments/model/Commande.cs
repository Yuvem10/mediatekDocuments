using System;

namespace MediaTekDocuments.model
{
    public class Commande
    {
        public string id { get; }
        public DateTime dateCommande { get; }
        public float montant { get; }
      
        
        public Commande(string id, DateTime dateCommande, float montant)
        {
            this.id = id;
            this.dateCommande = dateCommande;
            this.montant = montant;
        }

    }
}
