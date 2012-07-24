using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Sp.Api.Shared;
using Sp.Test.Html;
using System;
using Xunit.Extensions;
using System.Linq;
using Xunit;

namespace Sp.Api.Customer.Html.Acceptance
{
	public class CustomerCreate
	{
		[Theory, ClassData( typeof( RemoteWebDriverAndAuthenticatingNavigatorProvider<SoftwarePotentialDataFixture> ) )]
		public static void CreateCustomerShouldSuceed( RemoteWebDriver driver, AuthenticatingNavigator navigator )
		{
			using ( driver.FinallyQuitGuard() ) // TODO improve this using http://xunit.codeplex.com/workitem/9798 ( WAS: http://xunit.codeplex.com/discussions/362097 )
			{
				navigator.NavigateWithAuthenticate( driver, "Sp.Web.CustomerManagement" );

				IWebElement nameElement = driver.FindElement( By.Id( "name" ) );
				string customerName = "name" + Guid.NewGuid();
				nameElement.SendKeys( customerName );

				IWebElement descriptionElement = driver.FindElement( By.Id( "description" ) );
				descriptionElement.SendKeys( "description" );

				IWebElement createBtn = driver.FindElement( By.Id( "add-customer" ) );
				createBtn.Click();

				var names = driver.FindElementsByClassName( "customerName" );
				Assert.True( names.Any( x => x.Text.Equals( customerName ) ) );

			}
		}
	}
}
