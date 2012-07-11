/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Portal.Html.Acceptance
{
	using OpenQA.Selenium;
	using OpenQA.Selenium.Remote;
	using OpenQA.Selenium.Support.UI;
	using System;
	using System.Configuration;
	using Xunit.Extensions;
	using Sp.Test.Html;

	public class LicenseList
	{
		[Theory]
		[ClassData( typeof( BrowserDriverDataProvider ) )]
		public static void ShouldIncludeAtLeastOneLicense( RemoteWebDriver driver )
		{
			using ( driver.FinallyQuitGuard() ) // TODO improve this using http://xunit.codeplex.com/workitem/9798 ( WAS: http://xunit.codeplex.com/discussions/362097 )
			{
				driver.Authenticate();
				driver.Navigate().GoToUrl( ConfigurationManager.AppSettings[ "PortalBaseUrl" ] + "/license" );
				// If we cannot respond in 5 seconds for any reason, a human will seriously distrust the software, no excuses
				WebDriverWait wait = new WebDriverWait( driver, TimeSpan.FromSeconds( 5 ) );
				wait.Until( d => d
					.FindElement( By.Id( "license-list" ) )
					.FindElements( By.TagName( "li" ) )
					.Count > 0 );
			}
		}
	}
	static class RemoteWebDriverExtensions
	{
		public static void Authenticate( this RemoteWebDriver driver )
		{
			driver.Navigate().GoToUrl( ConfigurationManager.AppSettings[ "PortalBaseUrl" ] );

			IWebElement username = driver.FindElement( By.Id( "Username" ) );
			username.SendKeys( ConfigurationManager.AppSettings[ "Username" ] );

			IWebElement password = driver.FindElement( By.Id( "Password" ) );
			password.SendKeys( ConfigurationManager.AppSettings[ "Password" ] );

			password.Submit();
		}
	}
}