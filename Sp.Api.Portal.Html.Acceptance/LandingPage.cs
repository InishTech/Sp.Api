using System;
using System.Configuration;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Sp.Api.Portal.Acceptance;
using Sp.Api.Shared;
using Sp.Test.Html;
using Xunit;
using Xunit.Extensions;

namespace Sp.Api.Portal.Html.Acceptance
{
	public class LandingPage
	{
		[TheoryWithLazyLoading, ClassData( typeof( RemoteWebDriverAndAuthenticatingNavigatorProvider<SoftwarePotentialPortalDataFixture> ) )]
		public static void ShouldPresentLicenseList( RemoteWebDriver driver, AuthenticatingNavigator navigator )
		{
			using ( driver.FinallyQuitGuard() ) // TODO improve this using http://xunit.codeplex.com/workitem/9798 ( WAS: http://xunit.codeplex.com/discussions/362097 )
			{
				navigator.NavigateWithAuthenticate( driver, "" );

				WebDriverWait wait = new WebDriverWait( driver, TimeSpan.FromSeconds( 5 ) );
				wait.Until( d => driver.Title.Equals( "Licenses" ) );
			}
		}

		[TheoryWithLazyLoading, ClassData( typeof( RemoteWebDriverAndAuthenticatingNavigatorProvider<SoftwarePotentialPortalDataFixture> ) )]
		public static void ShouldContainCustomerUsername( RemoteWebDriver driver, AuthenticatingNavigator navigator )
		{
			using ( driver.FinallyQuitGuard() ) // TODO improve this using http://xunit.codeplex.com/workitem/9798 ( WAS: http://xunit.codeplex.com/discussions/362097 )
			{
				navigator.NavigateWithAuthenticate( driver, "" );
                WebDriverWait wait = new WebDriverWait( driver, TimeSpan.FromSeconds( 5 ) );
                var userSettingsMenu = wait.Until( d => driver.FindElementByCssSelector( "button[name='settings']" ) );
                userSettingsMenu.Click();
                var usernameElement = wait.Until( d => driver.FindElementByCssSelector( "li[id='username']" ) );

				Assert.Contains( ConfigurationManager.AppSettings[ "PortalUsername" ], usernameElement.Text );
			}
		}

		[TheoryWithLazyLoading, ClassData( typeof( RemoteWebDriverAndAuthenticatingNavigatorProvider<SoftwarePotentialPortalDataFixture> ) )]
		public static void SignoffShouldRedirectToHostRoot( RemoteWebDriver driver, AuthenticatingNavigator navigator )
		{
			using ( driver.FinallyQuitGuard() ) // TODO improve this using http://xunit.codeplex.com/workitem/9798 ( WAS: http://xunit.codeplex.com/discussions/362097 )
			{
				navigator.NavigateWithAuthenticate( driver, "" );
				WebDriverWait wait = new WebDriverWait( driver, TimeSpan.FromSeconds( 5 ) );
                var userSettingsMenu = wait.Until( d => driver.FindElementByCssSelector( "button[name='settings']" ) );
                userSettingsMenu.Click();
                var logoffButton = wait.Until( d => driver.FindElementByCssSelector( "a[name='logoff']" ) );

				var portalUrl = new Uri( driver.Url );
				logoffButton.Click();

				var signoffLandingUrl = new Uri( driver.Url );
				Assert.Equal( portalUrl.Authority, signoffLandingUrl.Authority );
			}
		}
	}
}
