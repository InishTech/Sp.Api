﻿namespace Sp.Portal.Html.Acceptance
{
	using OpenQA.Selenium;
	using OpenQA.Selenium.Remote;
	using OpenQA.Selenium.Support.UI;
	using System;
	using System.Configuration;
	using Xunit.Extensions;

	public class LicenseList
	{
		[Theory]
		[ClassData( typeof( BrowserDriverDataProvider ) )]
		public static void ShouldIncludeAtLeastOneLicense( RemoteWebDriver driver )
		{
			using ( driver.FinallyQuitGuard() ) // TODO improve this using http://xunit.codeplex.com/workitem/9798 ( WAS: http://xunit.codeplex.com/discussions/362097 )
			{
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
}