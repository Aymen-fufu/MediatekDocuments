using System;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier CommandeLivreDvd (réunit les informations des tables Commande et CommandeDocument)
    /// </summary>
    public class CommandeLivreDvd : Commande
    {
        
        public int NbExemplaire { get; }
        public string IdLivreDvd { get; }
        public string IdSuivi { get; }
        public string Stade { get; }

        public CommandeLivreDvd(string id, DateTime dateCommande, double montant, int nbExemplaire, string idLivreDvd, string idSuivi, string stade) : base(id, dateCommande, montant)
        {
            
            NbExemplaire = nbExemplaire;
            IdLivreDvd = idLivreDvd;
            IdSuivi = idSuivi;
            Stade = stade;
        }
    }
}