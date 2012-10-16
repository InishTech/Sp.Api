namespace Sp.Api.Portal.Html.Acceptance
{
	using OpenQA.Selenium;
	using OpenQA.Selenium.Remote;
	using OpenQA.Selenium.Support.UI;
	using Ploeh.AutoFixture;
	using Sp.Api.Shared;
	using Sp.Api.Portal.Acceptance;
	using Sp.Test.Html;
	using System;
	using System.Linq;
	using Xunit;
	using Xunit.Extensions;

	public class AdminTags
	{
		[Theory, ClassData( typeof( RemoteWebDriverAndAuthenticatingNavigatorProvider<SoftwarePotentialPortalDataFixture> ) )]
		public static void DeletesAndAddsShouldBeEvidentOnRefresh( RemoteWebDriver driver, AuthenticatingNavigator navigator )
		{
			using ( driver.FinallyQuitGuard() ) // TODO improve this using http://xunit.codeplex.com/workitem/9798 ( WAS: http://xunit.codeplex.com/discussions/362097 )
			{
				navigator.NavigateWithAuthenticate( driver, "tag" );

				// We don't enable the Add button until we have loaded
				WebDriverWait shortWait = new WebDriverWait( driver, TimeSpan.FromSeconds( 2 ) );
				shortWait.Until( d => driver.FindElementById( "add_new_tag" ).Enabled );

				// Delete all except the first (if there is one)
				foreach ( IWebElement item in driver.FindElementsById( "delete_tag" ).Skip( 1 ) )
					item.Click();

				// Add a fresh one; give it a name
				driver.FindElementById( "add_new_tag" ).Click();
				var newTagInputElement = driver.FindElementsById( "tag_name" ).Last();
				newTagInputElement.Clear();
				newTagInputElement.Click();
				var newTagName = new Fixture().CreateAnonymous<string>();
				newTagInputElement.SendKeys( newTagName );

				string[] tagsAsSubmitted = SaveAndRecordSubmittedTags( driver );

				// Verify reloading the page shows the same tags pretty quickly
				driver.Navigate().Refresh();
				shortWait.Until( d =>
					driver.FindElementById( "add_new_tag" ).Enabled
					&& tagsAsSubmitted.SequenceEqual( driver.FindElementsById( "tag_name" ).Select( tagNameEl => tagNameEl.GetAttribute( "value" ).Trim() ) ) );
			}
		}

		public static string[] SaveAndRecordSubmittedTags( RemoteWebDriver driver )
		{
			// Save
			driver.FindElementById( "save_tags" ).Click();

			// Stash the values we entered
			var tagsAsSubmitted = driver.FindElementsById( "tag_name" ).Select( tagNameEl => tagNameEl.GetAttribute( "value" ).Trim() ).ToArray();

			// Wait for the success report
			new WebDriverWait( driver, TimeSpan.FromSeconds( 5 ) ).Until( d =>
			{
				var messagesElement = driver.FindElementById( "messages" );
				return messagesElement.Displayed && -1 != messagesElement.Text.IndexOf( "saved successfully", StringComparison.InvariantCultureIgnoreCase );
			} );

			return tagsAsSubmitted;
		}
	}
}