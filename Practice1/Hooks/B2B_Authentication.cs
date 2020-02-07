using System;
using System.Configuration;
using RestSharp;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Practice1.Hooks
{
    class B2B_Authentication
    {
        public string getB2BAuthorizationToken()
        {
            var getToken = true;
            var b2bAccessToken = ConfigurationManager.AppSettings.Get("b2bAccessToken");
            var b2bAccessTokenExpiry = ConfigurationManager.AppSettings.Get("b2bAccessTokenExpiry");

            if (b2bAccessToken == "" || b2bAccessToken == null || b2bAccessTokenExpiry == "" || b2bAccessTokenExpiry == null)
            {
                Console.WriteLine("'Token or expiry date are missing'");
            }
            else if (Convert.ToDateTime(b2bAccessTokenExpiry) <= DateTime.Now)
            {
                Console.WriteLine("'Token is expired'");
            }
            else
            {
                getToken = false;
                Console.WriteLine("'Token and expiry date are all good'");
            }

            if (getToken)
            {
                Console.WriteLine("Fetching b2bAccessToken");
                // get tokenEndPoint by sending Get request to loginURL
                string loginURL = ConfigurationManager.AppSettings.Get("loginURL");
                var client = new RestClient(loginURL);
                var request = new RestRequest(Method.GET);
                request.AddHeader("Content-Type", "application/json");
                IRestResponse response = client.Execute(request);

                Assert.That(System.Net.HttpStatusCode.OK, Is.EqualTo(response.StatusCode));
                JObject jsonObj = JObject.Parse(response.Content);
                string tokenEndpoint = jsonObj["token_endpoint"].ToString();
                Console.WriteLine("tokenEndpoint = " + tokenEndpoint);

                // Send Post request to tokenEndPoint
                string b2bClientID = ConfigurationManager.AppSettings.Get("b2bClientID");
                string b2bClientSecret = ConfigurationManager.AppSettings.Get("b2bClientSecret");
                string grant_type = "client_credentials";
                string scope = "openid";

                client = new RestClient(tokenEndpoint);
                var tokenRequest = new RestRequest(Method.POST);
                tokenRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                tokenRequest.AddParameter("application/x-www-form-urlencoded", $"grant_type={grant_type}&client_id={b2bClientID}&client_secret={b2bClientSecret}&scope={scope}", ParameterType.RequestBody);
                IRestResponse tokenResponse = client.Execute(tokenRequest);

                JObject tokenJsonObj = JObject.Parse(tokenResponse.Content);
                Assert.That(System.Net.HttpStatusCode.OK, Is.EqualTo(tokenResponse.StatusCode));

                // get access token and other details from tokenResponse
                //string tokenExpiresIn = tokenJsonObj["expires_in"].ToString();
                double tokenExpiresIn = Convert.ToDouble(tokenJsonObj["expires_in"].ToString());


                string accessToken = tokenJsonObj["access_token"].ToString();
                string tokenType = tokenJsonObj["token_type"].ToString();
                string bearer = tokenType + " " + accessToken;

                // set b2bAccessToken and token expiry as global parameter to configuration file : app.config

                ConfigurationManager.AppSettings.Set("b2bAccessToken", bearer);
                var tokenExpiry = DateTime.Now.AddSeconds(tokenExpiresIn);
                ConfigurationManager.AppSettings.Set("b2bAccessTokenExpiry", tokenExpiry.ToString());
                return bearer;
            }
            else
            {
                Console.WriteLine("Token is still valid");
                return ConfigurationManager.AppSettings.Get("b2bAccessToken");

            }

        }
    }
}
