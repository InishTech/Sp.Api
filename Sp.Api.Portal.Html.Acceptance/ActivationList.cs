using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Sp.Api.Portal.Acceptance;
using Sp.Api.Shared;
using Sp.Test.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit.Extensions;

namespace Sp.Api.Portal.Html.Acceptance
{
    public static class ActivationList
    {
        [TheoryWithLazyLoading, ClassData( typeof( RemoteWebDriverAndAuthenticatingNavigatorProvider<SoftwarePotentialPortalDataFixture> ) )]
        public static void ShouldIncludeAtLeastOneLicense( RemoteWebDriver driver, AuthenticatingNavigator navigator )
        {
            using ( driver.FinallyQuitGuard() ) // TODO improve this using http://xunit.codeplex.com/workitem/9798 ( WAS: http://xunit.codeplex.com/discussions/362097 )
            {
                navigator.NavigateWithAuthenticate( driver, "activation" );

                // Even when cold, 5 seconds is a long time to wait
                WebDriverWait wait = new WebDriverWait( driver, TimeSpan.FromSeconds( 5 ) );
                wait.Until( d => driver.FindElementsByCssSelector( "#activeDevicesTable tr" ).Count > 0 );
            }
        }
    }
}
