using System;
using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.manager;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Xml.Linq;

namespace MediaTekDocuments.dal
{
    /// <summary>
    /// Classe d'accès aux données
    /// </summary>
    public class Access
    {
        /// <summary>
        /// adresse de l'API
        /// </summary>
        private static readonly string uriApi = "http://localhost/rest_mediatekdocuments/";
        /// <summary>
        /// instance unique de la classe
        /// </summary>
        private static Access instance = null;
        /// <summary>
        /// instance de ApiRest pour envoyer des demandes vers l'api et recevoir la réponse
        /// </summary>
        private readonly ApiRest api = null;
        /// <summary>
        /// méthode HTTP pour select
        /// </summary>
        private const string GET = "GET";
        /// <summary>
        /// méthode HTTP pour insert
        /// </summary>
        private const string POST = "POST";
        /// <summary>
        /// méthode HTTP pour update
        /// </summary>
        public const string PUT = "PUT";
        /// <summary>
        /// methode HTTP pour delete
        /// </summary>
        public const string DELETE = "DELETE";

        /// <summary>
        /// Méthode privée pour créer un singleton
        /// initialise l'accès à l'API
        /// </summary>
        private Access()
        {
            String authenticationString;
            try
            {
                authenticationString = "admin:adminpwd";
                api = ApiRest.GetInstance(uriApi, authenticationString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Création et retour de l'instance unique de la classe
        /// </summary>
        /// <returns>instance unique de la classe</returns>
        public static Access GetInstance()
        {
            if(instance == null)
            {
                instance = new Access();
            }
            return instance;
        }

        /// <summary>
        /// Retourne tous les genres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            IEnumerable<Genre> lesGenres = TraitementRecup<Genre>(GET, "genre");
            return new List<Categorie>(lesGenres);
        }

        /// <summary>
        /// Retourne tous les rayons à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            IEnumerable<Rayon> lesRayons = TraitementRecup<Rayon>(GET, "rayon");
            return new List<Categorie>(lesRayons);
        }

        /// <summary>
        /// Retourne toutes les catégories de public à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            IEnumerable<Public> lesPublics = TraitementRecup<Public>(GET, "public");
            return new List<Categorie>(lesPublics);
        }

        /// <summary>
        /// retrieve all status from the database
        /// </summary>
        /// <returns>list of status</returns>
        public List<Suivi> GetAllStatus()
        {
            IEnumerable<Suivi> lesStatus = TraitementRecup<Suivi>(GET, "suivi");
            return new List<Suivi>(lesStatus);
        }

        /// <summary>
        /// retrieve the specific status from the database with the id
        /// </summary>5
        public Suivi GetStatus(string id)
        {
            IEnumerable<Suivi> lesStatus = TraitementRecup<Suivi>(GET, "suivi/" + id);
            return new List<Suivi>(lesStatus)[0];
        }

       /// <summary>
       /// retrieve all the orders documents from the database
       /// </summary>
       /// <returns>List<returns>
       public List<CommandeDocument> GetAllCommandeDocuments()
       {
            IEnumerable<CommandeDocument> commandeDocuments = TraitementRecup<CommandeDocument>(GET, "CommandeDocument");
            return new List<CommandeDocument>(commandeDocuments);
       }

        ///<summary>
        /// retrieve order by its id
        /// </summary>
        /// <returns>commande</returns>
        public Commande retrieveCommandeById(string id)
        {
            String jsonIdDocument = convertToJson("id", id);
            IEnumerable<Commande> commande = TraitementRecup<Commande>(GET, "commande/" + jsonIdDocument);
            return new List<Commande>(commande)[0];
        }

        /// <summary>
        /// retrieve orderdocument by its id
        /// </summary>
        public CommandeDocument retrieveCommandeDocumentById(string id)
        {
            String jsonIdDocument = convertToJson("id", id);
            IEnumerable<CommandeDocument> commandeDocument = TraitementRecup<CommandeDocument>(GET, "commandeDocument/" + jsonIdDocument);
 
            return new List<CommandeDocument>(commandeDocument)[0];
        }

        /// <summary>
        /// add a new order document
        /// </summary>
        public bool AddCommandeDocument(CommandeDocument commandeDocument)
        {
            String jsonCommandeDocument = JsonConvert.SerializeObject(commandeDocument, new CustomDateTimeConverter()); 
            try
            {
                Console.WriteLine(jsonCommandeDocument);
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<CommandeDocument> liste = TraitementRecup<CommandeDocument>(POST, "commandedocument/" + jsonCommandeDocument);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// set the name of the order with retrieve the numbers of the order
        /// </summary>
        public string SetNameOrder(bool livre)
        {
            IEnumerable<Commande> commande = TraitementRecup<Commande>(GET, "commande");
            List<Commande> liste = new List<Commande>(commande);

            if (livre)
            {
                List<CommandeDocument> listeCommande = GetAllCommandeDocumentsLivre();

                if (listeCommande.Count == 0)
                {
                    return "CMDL1";
                }

                string id = listeCommande[listeCommande.Count - 1].id;
                string number = id.Substring(4);
                int numberInt = Int32.Parse(number);
                numberInt++;
                string newId = "CMDL" + numberInt;
                return newId;     
            }
            else
            {
                List<CommandeDocument> listeCommande = GetAllCommandeDocumentsDvd();

                if (listeCommande.Count == 0)
                {
                    return "CMDD1";
                }


                string id = listeCommande[listeCommande.Count - 1].id;
                string number = id.Substring(4);
                int numberInt = Int32.Parse(number);
                numberInt++;
                string newId = "CMDD" + numberInt;
                return newId;
            }
            
            
        }

        /// <summary>
        /// get commandedocument dvd
        /// </summary>
        public List<CommandeDocument> GetAllCommandeDocumentsDvd()
        {
            // retrieve all the order document where the id starts with CMDD
            IEnumerable<CommandeDocument> commandeDocument = TraitementRecup<CommandeDocument>(GET, "commandedocument");
            List<CommandeDocument> listeDvd = new List<CommandeDocument>(commandeDocument);
            List<CommandeDocument> liste = new List<CommandeDocument>();
            foreach (CommandeDocument commande in listeDvd)
            {
                if (commande.id.StartsWith("CMDD"))
                {
                    liste.Add(commande);
                }
            }
            return liste;
        }

        /// <summary>
        /// get commandedocument livre
        /// </summary>
        public List<CommandeDocument> GetAllCommandeDocumentsLivre()
        {
            // retrieve all the order document where the id starts with CMDL
            IEnumerable<CommandeDocument> commandeDocument = TraitementRecup<CommandeDocument>(GET, "commandedocument");
            List<CommandeDocument> listeLivre = new List<CommandeDocument>(commandeDocument);
            List<CommandeDocument> liste = new List<CommandeDocument>();
            foreach (CommandeDocument commande in listeLivre)
            {
                if (commande.id.StartsWith("CMDL"))
                {
                    liste.Add(commande);
                }
            }
            return liste;
        }


        /// <summary>
        /// retrieve livre by its id
        /// </summary>
        public Document retrieveDocumentById(string id)
        {
            String jsonIdDocument = convertToJson("id", id);
            IEnumerable<Document> livre = TraitementRecup<Document>(GET, "document/" + jsonIdDocument);
            return new List<Document>(livre)[0];
        }

        /// <summary>
        /// get order count
        /// </summary>
        public int GetOrderCount()
        {
            IEnumerable<Commande> commande = TraitementRecup<Commande>(GET, "commande");
            List<Commande> liste = new List<Commande>(commande);
            return liste.Count;
        }

        /// <summary>
        /// update the status of the order
        /// </summary>
        public bool updateOrderStatus(string idCommande, string idStatus)
        {
            String jsonIdStatus = convertToJson("statut", idStatus);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Commande> liste = TraitementRecup<Commande>(PUT, "commandedocument/" + idCommande + "/" + jsonIdStatus);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        ///<summary>
        ///delete a command document
        /// </summary>
        public bool deleteOrder(string idCommande)
        {
            // convert the id to json
            String jsonIdCommande = convertToJson("id", idCommande);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<CommandeDocument> liste = TraitementRecup<CommandeDocument>(DELETE, "commande/" + jsonIdCommande);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// retrieve the id of the current order for adding a new orderdocument
        /// </summary>
        public string GetIdCommande(string idDocuement)
        {
            String jsonIdDocument = convertToJson("id", idDocuement);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                IEnumerable<Commande> commande = TraitementRecup<Commande>(GET, "commande/" + jsonIdDocument);
                return new List<Commande>(commande)[0].id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            
        }


        /// <summary>
        /// add commande
        /// </summary>
        public bool AddCommande(Commande commande)
        {
            String jsonCommande = JsonConvert.SerializeObject(commande, new CustomDateTimeConverter());
            try
            {
                Console.WriteLine(jsonCommande);
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Commande> liste = TraitementRecup<Commande>(POST, "commande/" + jsonCommande);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Retourne toutes les livres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            List<Livre> lesLivres = TraitementRecup<Livre>(GET, "livre");
            return lesLivres;
        }

        /// <summary>
        /// Retourne toutes les dvd à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            List<Dvd> lesDvd = TraitementRecup<Dvd>(GET, "dvd");
            return lesDvd;
        }

        /// <summary>
        /// Retourne toutes les revues à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            List<Revue> lesRevues = TraitementRecup<Revue>(GET, "revue");
            return lesRevues;
        }

        /// <summary>
        /// Retrieve the list of all the commands from the database
        /// </summary>
        /// <returns>list of documents</returns>
    


        /// <summary>
        /// Retourne les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocument">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocument)
        {
            String jsonIdDocument = convertToJson("id", idDocument);
            List<Exemplaire> lesExemplaires = TraitementRecup<Exemplaire>(GET, "exemplaire/" + jsonIdDocument);
            return lesExemplaires;
        }

        /// <summary>
        /// ecriture d'un exemplaire en base de données
        /// </summary>
        /// <param name="exemplaire">exemplaire à insérer</param>
        /// <returns>true si l'insertion a pu se faire (retour != null)</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            String jsonExemplaire = JsonConvert.SerializeObject(exemplaire, new CustomDateTimeConverter());
            try {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Exemplaire> liste = TraitementRecup<Exemplaire>(POST, "exemplaire/" + jsonExemplaire);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false; 
        }

   
        /// <summary>
        /// Traitement de la récupération du retour de l'api, avec conversion du json en liste pour les select (GET)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methode">verbe HTTP (GET, POST, PUT, DELETE)</param>
        /// <param name="message">information envoyée</param>
        /// <returns>liste d'objets récupérés (ou liste vide)</returns>
        private List<T> TraitementRecup<T> (String methode, String message)
        {
            List<T> liste = new List<T>();
            try
            {
                JObject retour = api.RecupDistant(methode, message);
                // extraction du code retourné
                String code = (String)retour["code"];
                if (code.Equals("200"))
                {
                    // dans le cas du GET (select), récupération de la liste d'objets
                    if (methode.Equals(GET))
                    {
                        String resultString = JsonConvert.SerializeObject(retour["result"]);
                        // construction de la liste d'objets à partir du retour de l'api
                        liste = JsonConvert.DeserializeObject<List<T>>(resultString, new CustomBooleanJsonConverter());
                    }
                }
                else
                {
                    Console.WriteLine("code erreur = " + code + " message = " + (String)retour["message"]);
                }
            }catch(Exception e)
            {
                Console.WriteLine("Erreur lors de l'accès à l'API : "+e.Message);
                Environment.Exit(0);
            }
            return liste;
        }

        /// <summary>
        /// Convertit en json un couple nom/valeur
        /// </summary>
        /// <param name="nom"></param>
        /// <param name="valeur"></param>
        /// <returns>couple au format json</returns>
        private String convertToJson(Object nom, Object valeur)
        {
            Dictionary<Object, Object> dictionary = new Dictionary<Object, Object>();
            dictionary.Add(nom, valeur);
            return JsonConvert.SerializeObject(dictionary);
        }

        /// <summary>
        /// Modification du convertisseur Json pour gérer le format de date
        /// </summary>
        private sealed class CustomDateTimeConverter : IsoDateTimeConverter
        {
            public CustomDateTimeConverter()
            {
                base.DateTimeFormat = "yyyy-MM-dd";
            }
        }

        /// <summary>
        /// Modification du convertisseur Json pour prendre en compte les booléens
        /// classe trouvée sur le site :
        /// https://www.thecodebuzz.com/newtonsoft-jsonreaderexception-could-not-convert-string-to-boolean/
        /// </summary>
        private sealed class CustomBooleanJsonConverter : JsonConverter<bool>
        {
            public override bool ReadJson(JsonReader reader, Type objectType, bool existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                return Convert.ToBoolean(reader.ValueType == typeof(string) ? Convert.ToByte(reader.Value) : reader.Value);
            }

            public override void WriteJson(JsonWriter writer, bool value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }

    }
}
