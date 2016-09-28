using System.Threading;

namespace Sp.Api.Portal.Html.Acceptance
{
	using OpenQA.Selenium;
	using OpenQA.Selenium.Remote;
	using OpenQA.Selenium.Support.UI;
	using Ploeh.AutoFixture;
	using Sp.Api.Portal.Acceptance;
	using Sp.Api.Shared;
	using Sp.Test.Html;
	using System;
	using System.Linq;
	using Xunit.Extensions;

	public static class AdminTags
	{
		[TheoryWithLazyLoading( Skip = "Tag facilities disabled" ), ClassData( typeof( RemoteWebDriverAndAuthenticatingNavigatorProvider<SoftwarePotentialPortalDataFixture> ) )]
		static void DeletesAndAddsShouldBeEvidentOnRefresh( RemoteWebDriver driver, AuthenticatingNavigator navigator )
		{
			using ( driver.FinallyQuitGuard() ) // TODO improve this using http://xunit.codeplex.com/workitem/9798 ( WAS: http://xunit.codeplex.com/discussions/362097 )
			{
				navigator.NavigateWithAuthenticate( driver, "tag" );

				// Once the 'Add New Tag' button becomes enabled, we know that all tags have been loaded
				WebDriverWait shortWait = new WebDriverWait( driver, TimeSpan.FromSeconds( 2 ) );
				shortWait.Until( d => d.FindElement( By.Id( "add_new_tag" ) ).Enabled );
				//TODO TP 1650 - an enabled 'Add New Tag' button doesn't guarantee that the rendering is finished in Chrome; doing some unconditional wait...
				Thread.Sleep( 1000 );

				// Delete all except the first (if there is one)
				foreach ( IWebElement item in driver.FindElements( By.CssSelector( "button.delete" ) ).Skip( 1 ) )
					item.Click();

				shortWait.Until( d => d.FindElements( By.CssSelector( "input.tag_name" ) ).Count <= 1 );

				// Add a fresh one; give it a name
				driver.FindElement( By.Id( "add_new_tag" ) ).Click();
				var newTagInputElement = driver.FindElements( By.CssSelector( "input.tag_name" ) ).Last();
				newTagInputElement.Clear();
				newTagInputElement.Click();
				var newTagName = new Fixture().CreateAnonymous<string>();
				newTagInputElement.SendKeys( newTagName );

				string[] tagsAsSubmitted = SaveAndRecordSubmittedTags( driver );

				// Need a retry in case the initial load loads data which does not include the (eventually consistent) changes
				new WebDriverWaitIgnoringNestedTimeouts( driver, TimeSpan.FromSeconds( 7 ) ).Until( _ =>
				{
					driver.Navigate().Refresh();
					// Verify reloading the page shows the same tags pretty quickly
					return shortWait.Until( d =>
						d.FindElement( By.Id( "add_new_tag" ) ).Enabled
						&& tagsAsSubmitted.SequenceEqual( d.FindElements( By.CssSelector( "input.tag_name" ) ).Select( tagNameEl => tagNameEl.GetAttribute( "value" ).Trim() ) ) );
				} );
			}
		}

		public static string[] SaveAndRecordSubmittedTags( RemoteWebDriver driver )
		{
			// Save
			driver.FindElement( By.Id( "save_tags" ) ).Click();

			// Stash the values we entered
			var tagsAsSubmitted = driver.FindElements( By.CssSelector( "input.tag_name" ) ).Select( tagNameEl => tagNameEl.GetAttribute( "value" ).Trim() ).ToArray();

			// Wait for the success report
			new WebDriverWait( driver, TimeSpan.FromSeconds( 10 ) ).Until( d =>
			{
				var messagesElement = driver.FindElement( By.Id( "messages" ) );
				return messagesElement.Displayed && -1 != messagesElement.Text.IndexOf( "saved successfully", StringComparison.InvariantCultureIgnoreCase );
			} );

			return tagsAsSubmitted;
		}
	}
}