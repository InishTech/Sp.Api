using System;
using System.Linq;

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
        }
    }
}
