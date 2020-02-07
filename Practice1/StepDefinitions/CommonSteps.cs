using System;
using TechTalk.SpecFlow;
using Practice1.Hooks;

namespace Practice1.StrpDefinitions
{
    [Binding]
    public sealed class CommonSteps
    {
        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        private readonly ScenarioContext context;

        public CommonSteps(ScenarioContext injectedContext)
        {
            context = injectedContext;
        }

        [Given(@"the User is registered with the bank")]
        public void GivenTheUserIsRegisteredWithTheBank()
        {
            Console.WriteLine("Checking If User has valid Access token or not");
            new B2B_Authentication().getB2BAuthorizationToken();

        }

    }
}
