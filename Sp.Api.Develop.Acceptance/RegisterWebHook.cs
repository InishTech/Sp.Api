using Sp.Api.Shared;
using System;
using System.Linq;
using System.Net;
using Xunit;
using Xunit.Extensions;

namespace Sp.Api.Develop.Acceptance
{
    public class RegisterWebHook
    {
        [Theory, AutoSoftwarePotentialApiData]
        public static void ShouldYieldAcceptedWithALocationHref( SpWebHookApi api, WebHookEvents[] anonymousEvents, string anonymousSecret, Uri anonymousUri )
        {
            var response = api.CreateWebHook( new SpWebHookApi.WebHookRegistrationModel() { Actions = anonymousEvents.Select( x => x.ToString() ).ToArray(), Secret = anonymousSecret, WebHookUri = anonymousUri } );

            Assert.Equal( HttpStatusCode.Accepted, response.StatusCode );
        }

        public class BadRequest
        {

            [Theory, AutoSoftwarePotentialApiData]
            public static void TooShortSecret( SpWebHookApi api, WebHookEvents[] anonymousEvents, Uri anonymousUri )
            {
                var response = api.CreateWebHook( new SpWebHookApi.WebHookRegistrationModel() { Actions = anonymousEvents.Select( x => x.ToString() ).ToArray(), Secret = "short", WebHookUri = anonymousUri } );

                Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
             //   Assert.Contains( "The Shared Secret field is required and must be between 32 and 64 characters.", response.Content );
            }

            [Theory, AutoSoftwarePotentialApiData]
            public static void UnknownAction( SpWebHookApi api, string[] anonymousInvalidActions, string anonymousSecret, Uri anonymousUri )
            {
                var response = api.CreateWebHook( new SpWebHookApi.WebHookRegistrationModel() { Actions = anonymousInvalidActions, Secret = anonymousSecret, WebHookUri = anonymousUri } );

                Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
            //    Assert.Contains( "Unknown WebHook Action: " + anonymousInvalidActions.First() + ". Action must be one of the following known actions", response.Content );
            }

            [Theory, AutoSoftwarePotentialApiData]
            public static void InvalidUri( SpInvalidWebHookApi api, WebHookEvents[] anonymousEvents, string anonymousSecret, string anonymousInvalidUri )
            {
                var response = api.CreateWebHook( new SpInvalidWebHookApi.InvalidWebHookRegistrationModel() { Actions = anonymousEvents.Select( x => x.ToString() ).ToArray(), Secret = anonymousSecret, WebHookUri = anonymousInvalidUri } );

                Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
             //   Assert.Contains( "Invalid URI ", response.Content );
            }
        }
    }
}
