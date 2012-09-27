namespace Sp.Portal.Html.Acceptance
{
	using OpenQA.Selenium;
	using OpenQA.Selenium.Remote;
	using OpenQA.Selenium.Support.UI;
	using Sp.Api.Shared;
	using Sp.Portal.Acceptance;
	using Sp.Test.Html;
	using System;
	using Xunit.Extensions;
	using System.Linq;
	using Ploeh.AutoFixture;

	public class AdminTags
	{
		[Theory, ClassData( typeof( RemoteWebDriverAndAuthenticatingNavigatorProvider<SoftwarePotentialPortalDataFixture> ) )]
		public static void ShouldHandleDeletesAndAdds( RemoteWebDriver driver, AuthenticatingNavigator navigator )
		{
			using ( driver.FinallyQuitGuard() ) // TODO improve this using http://xunit.codeplex.com/workitem/9798 ( WAS: http://xunit.codeplex.com/discussions/362097 )
			{
				navigator.NavigateWithAuthenticate( driver, "tag" );
				// If we cannot respond in 5 seconds for any reason, a human will seriously distrust the software, no excuses
				WebDriverWait wait = new WebDriverWait( driver, TimeSpan.FromSeconds( 5 ) );
				wait.Until( d => d
					.FindElement( By.Id( "tags" ) )
					.FindElements( By.TagName( "li" ) )
					.Count > 0 );

				// Delete all except the first
				foreach ( IWebElement item in driver.FindElementsById( "delete_tag" ).Skip( 1 ) )
					item.Click();

				driver.FindElementById( "add_new_tag" ).Click();

				var newTagNameElement = driver.FindElementsById( "tag_name" ).Last();
				newTagNameElement.Clear();
				newTagNameElement.Click();
				var newTagName = new Fixture().CreateAnonymous<string>();
				driver.Keyboard.SendKeys( newTagName );

				driver.FindElementById( "save_tags" ).Click();

				WebDriverWait waitForSave = new WebDriverWait( driver, TimeSpan.FromSeconds( 3 ) );
				waitForSave.Until( d =>
				{
					var messagesElement = d.FindElement( By.Id( "messages" ) );
					return messagesElement.Displayed && -1 != messagesElement.Text.IndexOf( "saved successfully", StringComparison.InvariantCultureIgnoreCase );
				} );
			}
		}
	}
}