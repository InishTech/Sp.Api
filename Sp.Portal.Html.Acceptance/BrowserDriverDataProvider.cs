using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using System.Configuration;

namespace Sp.Portal.Html.Acceptance
{
	using System;

	public class BrowserDriverDataProvider : SingleItemTheoryDataProvider
	{
		protected override IEnumerable<object> SingleItemDataSource()
		{
			var drivers = new RemoteWebDriver[] 
			{
				new ChromeDriver(),
				new FirefoxDriver( GetSeleniumFirefoxProfile() )
			};
			foreach ( var driver in drivers )
			{
				Authenticate( driver );
				yield return driver;
			}
		}

		static FirefoxProfile GetSeleniumFirefoxProfile()
		{
			const string profileName = "Selenium";
			FirefoxProfileManager profileManager = new FirefoxProfileManager();
			FirefoxProfile profile = profileManager.GetProfile( profileName );
			if ( profile == null )
				throw new InvalidOperationException( "Cannot find Firefox profile " + profileName );
			profile.SetPreference( "browser.ssl_override_behavior", 1 );
			profile.AcceptUntrustedCertificates = false;
			return profile;
		}

		static void Authenticate( RemoteWebDriver driver )
		{
			driver.Navigate().GoToUrl( ConfigurationManager.AppSettings[ "PortalBaseUrl" ] );

			IWebElement username = driver.FindElement( By.Id( "Username" ) );
			username.SendKeys( ConfigurationManager.AppSettings[ "Username" ] );

			IWebElement password = driver.FindElement( By.Id( "Password" ) );
			password.SendKeys( ConfigurationManager.AppSettings[ "Password" ] );

			password.Submit();
		}
	}

	public abstract class TheoryDataProvider : IEnumerable<object[]>
	{
		public IEnumerator<object[]> GetEnumerator()
		{
			return DataSource().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return DataSource().GetEnumerator();
		}

		protected abstract IEnumerable<object[]> DataSource();
	}

	public abstract class SingleItemTheoryDataProvider : TheoryDataProvider
	{
		protected override sealed IEnumerable<object[]> DataSource()
		{
			return SingleItemDataSource().Select( x => new[] { x } );
		}

		protected abstract IEnumerable<object> SingleItemDataSource();
	}
}