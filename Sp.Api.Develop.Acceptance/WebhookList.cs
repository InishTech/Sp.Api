using Sp.Api.Shared;
using Sp.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Xunit;
using Xunit.Extensions;

namespace Sp.Api.Develop.Acceptance
{
    public class WebHookList
    {
        [Theory, AutoSoftwarePotentialApiData]
        [HighFrequency]
        public static void ShouldBeAbleToListWebHooks( SpWebHookApi api)
        {
            Verify.EventuallyWithBackOff( () =>
            {
                var apiResult = api.ListWebHooks();
                Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );

            } );
        }

        [Theory, AutoSoftwarePotentialApiData]
        public static void ShouldBeAbleToGetExistingWebHook( SpWebHookApi api, ExistingWebHookFixture existingWebHook )
        {
            Verify.EventuallyWithBackOff( () =>
            {
                var apiResult = api.GetWebhook( existingWebHook.Location );
                Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );


            } );
        }
    }
}
