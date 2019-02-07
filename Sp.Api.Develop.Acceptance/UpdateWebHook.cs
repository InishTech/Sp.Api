using Newtonsoft.Json;
using Sp.Api.Shared;
using Sp.Test.Helpers;
using System;
using System.Linq;
using System.Net;
using Xunit;
using Xunit.Extensions;

namespace Sp.Api.Develop.Acceptance
{
    public class UpdateWebHook
    {
        [Theory, AutoSoftwarePotentialApiData]
        public static void UnmodifiedShouldYieldAccepted( SpWebHookApi api, ExistingWebHookFixture existingWebHook )
        {
            var response = api.UpdateWebHook( existingWebHook.Location, existingWebHook.RegisteredWebHook );

            Assert.Equal( HttpStatusCode.Accepted, response.StatusCode );
        }

        [Theory, AutoSoftwarePotentialApiData]
        public static void ModifiedShouldYieldAccepted( SpWebHookApi api, ExistingWebHookFixture existingWebHook, WebHookEvents[] anonymousEvents, string anonymousSecret, Uri anonymousUri )
        {
            var updated = new SpWebHookApi.WebHookRegistrationModel() { Actions = anonymousEvents.Select( x => x.ToString() ).Distinct().ToArray(), Secret = anonymousSecret, WebHookUri = anonymousUri };
            var response = api.UpdateWebHook( existingWebHook.Location, updated );
             
            Assert.Equal( HttpStatusCode.Accepted, response.StatusCode );

            Verify.EventuallyWithBackOff( () =>
            {
                var apiResult = api.GetWebhook( existingWebHook.Location );
                Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
                var updatedWebhook = JsonConvert.DeserializeObject<SpWebHookApi.WebHookRegistrationModel>( apiResult.Content );
                Assert.Equal( updated.Actions.Distinct(), updatedWebhook.Actions );
                Assert.Equal( updated.WebHookUri, updatedWebhook.WebHookUri );
                Assert.Equal( updated.IsPaused, updatedWebhook.IsPaused );

                // For security reasons the shared secret is not returned so this value cannot be covered in acceptance tests (it is fully covered by internal unit and integration)
            } );
        }

        
    }
}
