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
    [Scope(Feature = "create new transfer between account holder loan accounts")]
    public sealed class CreateNewTransferSteps
    {
        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        private readonly ScenarioContext context;

        private String URL = ConfigurationManager.AppSettings.Get("baseURL");
        private String ROUTE = ConfigurationManager.AppSettings.Get("interanalTransferRoute");

        public CreateNewTransferSteps(ScenarioContext injectedContext)
        {
            context = injectedContext;
        }

      

        [When(@"the User provide a valid (.*), (.*), (.*), (.*), ""(.*)""")]
        public void WhenTheUserProvideAValid(int consumerID, int fromAccountID, int toAccountID, Decimal amount, string startDate)
        {
            Console.WriteLine("Send Post request");
            var client = new RestClient(URL);
            var request = new RestRequest(ROUTE, Method.POST);
            string requestID = new General_Hook().generateUID();
            var b2bAccessToken = ConfigurationManager.AppSettings.Get("b2bAccessToken");

            request.AddUrlSegment("consumerId", consumerID);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("X-Request-ID", requestID);
            request.AddHeader("Authorization", b2bAccessToken);

            // Add request body
            request.AddJsonBody(new
            {
                fromAccountId = fromAccountID.ToString(),
                toAccountId = toAccountID.ToString(),
                amount = amount,
                description = "Loan Transfer",
                transferType = "Regular",
                startDate = startDate
            });

            IRestResponse response = client.Execute(request);
            context.Set(response);
        }

        [Then(@"funds are transfered between account holder's loan accounts")]
        public void ThenFundsAreTransferedBetweenAccountHolderSLoanAccounts()
        {
            var response = context.Get<IRestResponse>();
            var responseStatus = response.StatusCode;
            Console.WriteLine("response Status");
            Console.WriteLine(responseStatus);
            JObject jsonObj = JObject.Parse(response.Content);
            Console.WriteLine(jsonObj);
            Assert.That("Created", Is.EqualTo(responseStatus.ToString()));
            //Assert.That("Conflict", Is.EqualTo(responseStatus.ToString()));
            //JObject jsonObj = JObject.Parse(response.Content);
            //Console.WriteLine(jsonObj);
        }

        [Then(@"loan transfer id is provided to the User")]
        public void ThenLoanTransferIdIsProvidedToTheUser()
        {
            var response = context.Get<IRestResponse>();
            var content = response.Content;
            JObject jsonObj = JObject.Parse(content);
            var loanTransferID = jsonObj["id"].ToString();
            Console.WriteLine(loanTransferID);
        }

        [Then(@"funds are not transfered between account holder's loan accounts")]
        public void ThenFundsAreNotTransferedBetweenAccountHolderSLoanAccounts()
        {
            var response = context.Get<IRestResponse>();
            var responseStatus = response.StatusCode;
            Assert.That("BadRequest", Is.EqualTo(responseStatus.ToString()));
        }

        [Then(@"a message of invalid parameter is provided to the User")]
        public void ThenAMessageOfInvalidParameterIsProvidedToTheUser()
        {
            var response = context.Get<IRestResponse>();
            var content = response.Content;
            JObject jsonObj = JObject.Parse(content);

            string errorMessage = jsonObj["message"].ToString();
            string logError = String.Format("Error Message is \"{0}\"", errorMessage);
            Console.WriteLine(logError);
            var errorCode = jsonObj["code"].ToString();
            Assert.That("201", Is.EqualTo(errorCode));
            Assert.IsNotEmpty(errorMessage);
        }
    }
}
