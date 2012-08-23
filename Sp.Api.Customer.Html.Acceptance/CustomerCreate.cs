/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Api.Customer.Html.Acceptance
{
	using OpenQA.Selenium;
	using OpenQA.Selenium.Remote;
	using Sp.Api.Shared;
	using Sp.Test.Html;
	using System;
	using System.Linq;
	using Xunit;
	using Xunit.Extensions;

	public class CustomerCreate
	{
		[Theory, ClassData( typeof( RemoteWebDriverAndAuthenticatingNavigatorProvider<SoftwarePotentialDataFixture> ) )]
		public static void CreateCustomerShouldSucceed( RemoteWebDriver driver, AuthenticatingNavigator navigator )
		{
			using ( driver.FinallyQuitGuard() ) // TODO improve this using http://xunit.codeplex.com/workitem/9798 ( WAS: http://xunit.codeplex.com/discussions/362097 )
			{
				navigator.NavigateWithAuthenticate( driver, "Sp.Web.CustomerManagement/customer/Create" );

				IWebElement nameElement = driver.FindElement( By.Id( "name" ) );
				string anonymousCustomerName = "anonymousName" + Guid.NewGuid();
				nameElement.SendKeys( anonymousCustomerName );

				IWebElement descriptionElement = driver.FindElement( By.Id( "description" ) );
				descriptionElement.SendKeys( "anonymousDescription" );

				IWebElement createButton = driver.FindElement( By.Id( "add-customer" ) );
				createButton.Click();

				var names = driver.FindElementsByClassName( "customerName" );
				Assert.True( names.Any( x => x.Text.Equals( anonymousCustomerName ) ) );
			}
		}
	}
}