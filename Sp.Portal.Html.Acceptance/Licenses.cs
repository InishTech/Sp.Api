using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Configuration;
using Xunit.Extensions;

namespace Sp.Portal.Html.Acceptance
{
	public class Licenses
	{
		[Theory]
		[ClassData( typeof( BrowserDriverDataProvider ) )]
		public static void LicenseListShouldIncludeAtLeastOneLicense( RemoteWebDriver driver )
		{
			try
			{
				driver.Navigate().GoToUrl( ConfigurationManager.AppSettings[ "PortalBaseUrl" ] + "/license" );				
				WebDriverWait wait = new WebDriverWait( driver, TimeSpan.FromSeconds( 5 ) );
				wait.Until( ( d ) => { return d.FindElement( By.Id( "license-list" ) ).FindElements( By.TagName( "li" ) ).Count >= 1; } );
			}
			finally
			{
				driver.Quit();
			}
		}
	}
}
