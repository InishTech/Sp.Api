using Sp.Test.Helpers;
using System;
using System.Linq;
using System.Net;
using Xunit;

namespace Sp.Api.Develop.Acceptance
{
    public class ExistingWebHookFixture
    {
        public string Location { get; private set; }
        public SpWebHookApi.WebHookRegistrationModel RegisteredWebHook { get; private set; }
        public ExistingWebHookFixture( SpWebHookApi api, WebHookEvents[] anonymousEvents, string anonymousSecret, Uri anonymousUri )
        {
            RegisteredWebHook = new SpWebHookApi.WebHookRegistrationModel() { Actions = anonymousEvents.Select( x => x.ToString() ).ToArray(), Secret = anonymousSecret, WebHookUri = anonymousUri };

            var response = api.CreateWebHook( RegisteredWebHook );

            Location = response.Headers.Single( x => x.Name == "Location" ).Value.ToString();
            // Verify it has successfully created before proceeding to update
            Verify.EventuallyWithBackOff( () =>
            {
                var apiResult = api.GetWebhook( Location );
                Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
            }, 12 );
        }
    }
}
