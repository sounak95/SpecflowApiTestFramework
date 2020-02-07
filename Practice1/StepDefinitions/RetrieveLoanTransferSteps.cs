using System;
using RestSharp;
using TechTalk.SpecFlow;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Configuration;
using LaserPro.Test.Hooks;

namespace Practice1.StrpDefinitions
{
    [Binding]
    [Scope(Feature = "retrieve all loan transfer for an account holder")]
    public sealed class RetrieveLoanTransferSteps
    {
        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        private readonly ScenarioContext context;
        private String URL = ConfigurationManager.AppSettings.Get("baseURL");
        private String ROUTE = ConfigurationManager.AppSettings.Get("interanalTransferRoute");

        public RetrieveLoanTransferSteps(ScenarioContext injectedContext)
        {
            context = injectedContext;
        }

        [When(@"the User provide valid (.*)")]
        public void WhenTheUserProvideValid(int consumerID)
        {
            Console.WriteLine("Get request to fetch loan transfer details for consumerID = " + consumerID);
            var client = new RestClient(URL);
            var request = new RestRequest(ROUTE, Method.GET);
            string requestID = new General_Hook().generateUID();
            var b2bAccessToken = ConfigurationManager.AppSettings.Get("b2bAccessToken");

            request.AddUrlSegment("consumerId", consumerID);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("X-Request-ID", requestID);
            request.AddHeader("Authorization", b2bAccessToken);
            IRestResponse response = client.Execute(request);
            context.Set(response);
        }

        [Then(@"all loan transfer details are  provided to the User")]
        public void ThenAllLoanTransferDetailsAreProvidedToTheUser()
        {
            var response = context.Get<IRestResponse>();
            var responseStatus = response.StatusCode;
            Assert.That("OK", Is.EqualTo(responseStatus.ToString()));
            JObject jsonObj = JObject.Parse(response.Content);
            Console.WriteLine(jsonObj);
        }

        [When(@"the User provide an invalid (.*)")]
        public void WhenTheUserProvideAnInvalid(int consumerID)
        {
            Console.WriteLine("Get request to fetch loan transfer details for invalid consumerID = " + consumerID);
            var client = new RestClient(URL);
            var request = new RestRequest(ROUTE, Method.GET);
            string requestID = new General_Hook().generateUID();
            var b2bAccessToken = ConfigurationManager.AppSettings.Get("b2bAccessToken");

            request.AddUrlSegment("consumerId", consumerID);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("X-Request-ID", requestID);
            request.AddHeader("Authorization", b2bAccessToken);
            IRestResponse response = client.Execute(request);
            context.Set(response);
        }

        [Then(@"there is no loan transfer details available")]
        public void ThenThereIsNoLoanTransferDetailsAvailable()
        {
            var response = context.Get<IRestResponse>();
            var responseStatus = response.StatusCode;
            Assert.That("BadRequest", Is.EqualTo(responseStatus.ToString()));
        }

        [Then(@"'(.*)' message is provided to the user")]
        public void ThenMessageIsProvidedToTheUser(string errorMessage)
        {
            var response = context.Get<IRestResponse>();
            var content = response.Content;
            JObject jsonObj = JObject.Parse(content);
            var actualMessage = jsonObj["message"].ToString();
            Assert.That(errorMessage, Is.EqualTo(actualMessage));
        }

    }
}
