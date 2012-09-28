namespace Sp.Portal.Html.Acceptance
{
	using OpenQA.Selenium;
	using OpenQA.Selenium.Remote;
	using OpenQA.Selenium.Support.UI;
	using Ploeh.AutoFixture;
	using Sp.Api.Shared;
	using Sp.Portal.Acceptance;
	using Sp.Test.Helpers;
	using Sp.Test.Html;
	using System;
	using System.Collections.ObjectModel;
	using System.Linq;
	using Xunit;
	using Xunit.Extensions;

	public class LicenseTags
	{
		[Theory, ClassData( typeof( RemoteWebDriverAndAuthenticatingNavigatorProvider<SoftwarePotentialPortalDataFixture> ) )]
		public static void ShouldEachBeEditableViaDialog( RemoteWebDriver driver, AuthenticatingNavigator navigator )
		{
			using ( driver.FinallyQuitGuard() ) // TODO improve this using http://xunit.codeplex.com/workitem/9798 ( WAS: http://xunit.codeplex.com/discussions/362097 )
			{
				navigator.NavigateWithAuthenticate( driver, "tag" );

				var tagNames = EnsureWeHaveTagsDefined( driver );

				// wait for any updates to propagate to the licenses page
				AwaitSyncingOfSubmittedTags( tagNames, driver );

				// Pop up the editor for a random one
				Func<ReadOnlyCollection<IWebElement>> findLicenseRows = () => driver.FindElementsByCssSelector( "#license-list tr" );
				var licenseRowToEdit = findLicenseRows().ElementAtRandom();
				var randomTagEditButton = licenseRowToEdit.FindElement( By.ClassName( "tag_editor_dialog_launcher" ) );
				var editedLicenseKey = randomTagEditButton.GetAttribute( "data-key" );
				Func<IWebElement> findLicenseRowEditedOrDefault = () => findLicenseRows().SingleOrDefault( x => -1 != x.Text.IndexOf( editedLicenseKey ) );
				randomTagEditButton.Click();

				var tagEditorEl = driver.FindElementById( "tag_editor" );

				// Check the tag names are as we just set them up
				var tagLabelsOnDialog = tagEditorEl.FindElements( By.TagName( "label" ) )
					.Where( x => x.GetAttribute( "for" ) == "tag" )
					.Select( x => x.Text.TrimEnd( ':' ) );
				Assert.Equal( string.Join( "|", tagNames ), string.Join( "|", tagLabelsOnDialog ) );

				// Lash in some values
				var tagTextInputs = tagEditorEl.FindElements( By.Name( "tag" ) );
				foreach ( var tagTextInputEl in tagTextInputs )
				{
					tagTextInputEl.Clear();
					tagTextInputEl.SendKeys( new Fixture().CreateAnonymous<string>( "value" ) );
				}
				var valuesInputted = tagEditorEl.FindElements( By.Name( "tag" ) ).Select( x => x.GetAttribute( "value" ).Trim() ).ToArray();
				var valuesExpected = string.Join( "|", valuesInputted );

				// Save
				driver.FindElementById( "save_tags" ).Click();

				// Verify inputted values show on page without any refreshes
				new WebDriverWait( driver, TimeSpan.FromSeconds( 3 ) ).Until( d2 =>
				{
					var valuesAsFound = string.Join( "|", findLicenseRowEditedOrDefault().FindElements( By.TagName( "td" ) ).Select( x => x.Text ) );
					return -1 != valuesAsFound.IndexOf( valuesExpected );
				} );

				// Verify everything can show when we get everything fresh from the server
				new WebDriverWaitIgnoringNestedTimeouts( driver, TimeSpan.FromSeconds( 4 ) ).Until( d =>
				{
					driver.FindElementById( "logo_link" ).FindElement( By.TagName( "a" ) ).Click();
					return new WebDriverWait( driver, TimeSpan.FromSeconds( 2 ) ).Until( d2 =>
					{
						var editedRow = findLicenseRowEditedOrDefault();
						if (editedRow == null)
							return false;
						var valuesAsFound = string.Join( "|", editedRow.FindElements( By.TagName( "td" ) ).Select( x => x.Text ) );
						return -1 != valuesAsFound.IndexOf( valuesExpected );
					} );
				} );
			}
		}

		static string[] EnsureWeHaveTagsDefined( RemoteWebDriver driver )
		{
			// The page doesn't enable the Add button until we have loaded
			new WebDriverWait( driver, TimeSpan.FromSeconds( 2 ) ).Until( d => d.FindElement( By.Id( "add_new_tag" ) ).Enabled );

			// Delete all except the first three
			var existing = driver.FindElementsById( "delete_tag" );
			foreach ( IWebElement item in existing.Skip( 3 ) )
				item.Click();

			// Top up to 3 if necessary
			if ( existing.Count < 3 )
				foreach ( var newTag in new Fixture().CreateMany<string>( "filler", 3 - existing.Count ) )
				{
					// Add a fresh one; give it a name
					driver.FindElementById( "add_new_tag" ).Click();
					var newTagInputElement = driver.FindElementsById( "tag_name" ).Last();
					newTagInputElement.SendKeys( newTag );
				}

			// Save and note them
			return AdminTags.SaveAndRecordSubmittedTags( driver );
		}

		public static void AwaitSyncingOfSubmittedTags( string[] tagsAsSubmitted, RemoteWebDriver driver )
		{
			// Wait for the headings to show, potentially re-clicking home to attempt a reload/refresh if they are not what we expect (as a user might)
			new WebDriverWaitIgnoringNestedTimeouts( driver, TimeSpan.FromSeconds( 3 ) ).Until( d =>
			{
				driver.FindElementByCssSelector( "#logo_link a" ).Click();
				return new WebDriverWait( driver, TimeSpan.FromSeconds( 1 ) ).Until( d2 =>
				{
					var tagHeadings = driver.FindElementsByClassName( "tag_header" ).Select( heading => heading.Text ).ToArray();
					return tagsAsSubmitted.SequenceEqual( tagHeadings );
				} );
			} );
		}

		class WebDriverWaitIgnoringNestedTimeouts : WebDriverWait
		{
			public WebDriverWaitIgnoringNestedTimeouts( IWebDriver driver, TimeSpan timeout )
				: base( driver, timeout )
			{
				IgnoreExceptionTypes( typeof( TimeoutException ) );
			}
		}
	}
}