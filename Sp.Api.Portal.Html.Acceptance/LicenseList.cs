/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Api.Portal.Html.Acceptance
{
	using OpenQA.Selenium.Remote;
	using OpenQA.Selenium.Support.UI;
	using Sp.Api.Shared;
	using Sp.Api.Portal.Acceptance;
	using Sp.Test.Html;
	using System;
	using Xunit.Extensions;

	public class LicenseList
	{
		[TheoryWithLazyLoading, ClassData( typeof( RemoteWebDriverAndAuthenticatingNavigatorProvider<SoftwarePotentialPortalDataFixture> ) )]
		public static void ShouldIncludeAtLeastOneLicense( RemoteWebDriver driver, AuthenticatingNavigator navigator ) 
		{
			using ( driver.FinallyQuitGuard() ) // TODO improve this using http://xunit.codeplex.com/workitem/9798 ( WAS: http://xunit.codeplex.com/discussions/362097 )
			{
				navigator.NavigateWithAuthenticate( driver, "license" );
				
				// Even when cold, 5 seconds is a long time to wait
				WebDriverWait wait = new WebDriverWait( driver, TimeSpan.FromSeconds( 5 ) );
				wait.Until( d => driver.FindElementsByCssSelector( "#license-list tr" ).Count > 0 );
			}
		}
	}
}