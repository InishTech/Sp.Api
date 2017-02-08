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
        public static void ShouldYieldAcceptedWithALocationHref(SpWebHookApi api, string anonymousId, string anonymousDescription, string anonymousSecret, Uri anonymousUri)
        {
            var response = api.CreateWebHook(new SpWebHookApi.WebHookRegistrationModel() { Id = anonymousId, Description = anonymousDescription, Secret = anonymousSecret, WebHookUri = anonymousUri.ToString() });

            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        }

        public class BadRequest
        {
            [Theory, AutoSoftwarePotentialApiData]
            public static void TooShortSecret(SpWebHookApi api, string anonymousId, string anonymousDescription, Uri anonymousUri)
            {
                var response = api.CreateWebHook(new SpWebHookApi.WebHookRegistrationModel() { Id = anonymousId, Description = anonymousDescription, Secret = "short", WebHookUri = anonymousUri.ToString() });


                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                Assert.Contains("The field Secret ", response.Content);
            }

            [Theory, AutoSoftwarePotentialApiData]
            public static void InvalidUri(SpWebHookApi api, string anonymousId, string anonymousDescription, string anonymousSecret, string anonymousUri)
            {
                var response = api.CreateWebHook(new SpWebHookApi.WebHookRegistrationModel() { Id = anonymousId, Description = anonymousDescription, Secret = anonymousSecret, WebHookUri = anonymousUri });

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                Assert.Contains("Invalid URI ", response.Content);
            }
        }
    }
}
