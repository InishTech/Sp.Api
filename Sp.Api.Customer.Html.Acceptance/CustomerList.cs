/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Api.Customer.Html.Acceptance
{
	using OpenQA.Selenium;
	using OpenQA.Selenium.Remote;
	using OpenQA.Selenium.Support.UI;
	using Ploeh.AutoFixture.Xunit;
	using Sp.Api.Shared;
	using Sp.Api.Shared.Wrappers;
	using Sp.Test.Html;
	using System;
	using Xunit.Extensions;

	public class CustomerList
	{
		[Theory, ClassData( typeof( RemoteWebDriverAndAuthenticatingNavigatorProvider<SoftwarePotentialDataFixture> ) )]
		public static void ShouldIncludeAtLeastOneCustomer( RemoteWebDriver driver, AuthenticatingNavigator navigator ) 
		{
			using ( driver.FinallyQuitGuard() ) // TODO improve this using http://xunit.codeplex.com/workitem/9798 ( WAS: http://xunit.codeplex.com/discussions/362097 )
			{
				navigator.NavigateWithAuthenticate( driver, "Sp.Web.CustomerManagement" );
				// If we cannot respond in 5 seconds for any reason, a human will seriously distrust the software, no excuses
				WebDriverWait wait = new WebDriverWait( driver, TimeSpan.FromSeconds( 5 ) );
				wait.Until( d => d
					.FindElement( By.Id( "customer-list" ) )
					.FindElements( By.TagName( "li" ) )
					.Count > 0 );
			}
		}
	}
}