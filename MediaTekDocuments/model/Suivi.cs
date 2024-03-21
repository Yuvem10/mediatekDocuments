using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    public class Suivi
    {
        public string statut { get; }
       
        public Suivi(string statut)
        {
            this.statut = statut;
        }

    }
}
