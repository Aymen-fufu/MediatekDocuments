﻿using System;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MediaTekDocuments.manager
{
    /// <summary>
    /// Classe indépendante d'accès à une api rest avec éventuellement une "basic authorization"
    /// </summary>
    class ApiRest
    {
        /// <summary>
        /// unique instance de la classe
        /// </summary>
        private static ApiRest instance = null;
        /// <summary>
        /// Objet de connexion à l'api
        /// </summary>
        private readonly HttpClient httpClient;
        /// <summary>
        /// Canal http pour l'envoi du message et la récupération de la réponse
        /// </summary>
        private HttpResponseMessage httpResponse;

        /// <summary>
        /// Constructeur privé pour préparer la connexion (éventuellement sécurisée)
        /// </summary>
        /// <param name="uriApi">adresse de l'api</param>
        /// <param name="authenticationString">chaîne d'authentification</param>
        private ApiRest(String uriApi, String authenticationString = "")
        {
            httpClient = new HttpClient() { BaseAddress = new Uri(uriApi) };
            // prise en compte dans l'url de l'authentificaiton (basic authorization), si elle n'est pas vide
            if (!String.IsNullOrEmpty(authenticationString))
            {
                String base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + base64EncodedAuthenticationString);
            }
        }

        /// <summary>
        /// Crée une instance unique de la classe
        /// </summary>
        /// <param name="uriApi">adresse de l'api</param>
        /// <param name="authenticationString">chaîne d'authentificatio (login:pwd)</param>
        /// <returns></returns>
        public static ApiRest GetInstance(String uriApi, String authenticationString)
        {
            if (instance == null)
            {
                instance = new ApiRest(uriApi, authenticationString);
            }
            return instance;
        }

        /// <summary>
        /// Envoi une demande à l'API et récupère la réponse
        /// </summary>
        /// <param name="methode">verbe http (GET, POST, PUT, DELETE)</param>
        /// <param name="message">message à envoyer dans l'URL</param>
        /// <param name="parametres">contenu de variables à mettre dans body</param>
        /// <returns>liste d'objets (select) ou liste vide (ok) ou null si erreur</returns>
        public JObject RecupDistant(string methode, string message, String parametres)
        {
            try
            {
                // transformation des paramètres pour les mettre dans le body
                StringContent content = null;
                if (!(parametres is null))
                {
                    // Toujours utiliser application/x-www-form-urlencoded comme pour les autres méthodes
                    content = new StringContent(parametres, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
                    Console.WriteLine($"Content-Type: {content.Headers.ContentType}");
                }

                // envoi du message et attente de la réponse
                switch (methode)
                {
                    case "GET":
                        httpResponse = httpClient.GetAsync(message).Result;
                        break;
                    case "POST":
                        httpResponse = httpClient.PostAsync(message, content).Result;
                        break;
                    case "PUT":
                        httpResponse = httpClient.PutAsync(message, content).Result;
                        break;
                    case "DELETE":
                        httpResponse = httpClient.DeleteAsync(message).Result;
                        break;
                    default:
                        return new JObject { ["code"] = "400", ["message"] = "Méthode HTTP non supportée" };
                }

                // Récupération du contenu de la réponse
                string responseContent = httpResponse.Content.ReadAsStringAsync().Result;
                Console.WriteLine($"Status Code: {httpResponse.StatusCode}");
                Console.WriteLine($"Response Content: {responseContent}");

                // Si nous avons une réponse JSON valide
                try
                {
                    return JsonConvert.DeserializeObject<JObject>(responseContent);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur de désérialisation : {ex.Message}");
                    return new JObject
                    {
                        ["code"] = "500",
                        ["message"] = "Erreur lors de la désérialisation de la réponse",
                        ["result"] = new JArray()
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur dans RecupDistant : {ex.Message}");
                Console.WriteLine($"Stack trace : {ex.StackTrace}");
                return new JObject
                {
                    ["code"] = "500",
                    ["message"] = ex.Message,
                    ["result"] = new JArray()
                };
            }
        }

    }
}
