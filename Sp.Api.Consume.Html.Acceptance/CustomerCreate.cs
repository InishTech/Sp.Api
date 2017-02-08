/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Api.Consume.Html.Acceptance
{
	using OpenQA.Selenium;
	using OpenQA.Selenium.Remote;
	using Sp.Api.Shared;
	using Sp.Test.Html;
	using System;
	using Xunit.Extensions;
	using OpenQA.Selenium.Support.UI;

	public class CustomerCreate
	{
		[TheoryWithLazyLoading, ClassData( typeof( RemoteWebDriverAndAuthenticatingNavigatorProvider<SoftwarePotentialDataFixture> ) )]
		public static void CreateCustomerShouldSucceed( RemoteWebDriver driver, AuthenticatingNavigator navigator )
		{
			using ( driver.FinallyQuitGuard() ) // TODO improve this using http://xunit.codeplex.com/workitem/9798 ( WAS: http://xunit.codeplex.com/discussions/362097 )
			{
				navigator.NavigateWithAuthenticate( driver, "Sp.Web.Consume/Customer/Create" );

				WebDriverWait wait = new WebDriverWait( driver, TimeSpan.FromSeconds( 5 ) );
				wait.Until( d => d.FindElement( By.Id( "create-view" ) ) );
				IWebElement nameElement = driver.FindElement( By.Id( "name" ) );
				string anonymousCustomerName = "anonymousName" + Guid.NewGuid();
				nameElement.SendKeys( anonymousCustomerName );

				IWebElement externalIdElement = driver.FindElement( By.Id( "externalId" ) );
				externalIdElement.SendKeys( "anonymousExternalId" );

				IWebElement createButton = driver.FindElement( By.Id( "add-customer" ) );
				createButton.Click();

				wait.Until( d => d
					.FindElement( By.Id( "messages" ) )
					.Text.Contains( "Customer created successfully" ) );
			}
		}
	}
}