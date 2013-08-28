using System.Collections.Generic;
namespace Sp.Api.Portal.Html.Acceptance
{
	using OpenQA.Selenium;
	using OpenQA.Selenium.Remote;
	using OpenQA.Selenium.Support.UI;
	using Ploeh.AutoFixture;
	using Sp.Api.Shared;
	using Sp.Api.Portal.Acceptance;
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

				var tagNames = EnsureWeHaveSomeTagsDefined( driver );

				// wait for any updates to propagate to the licenses page - the column headings at the top need to have synced up
				AwaitSyncingOfSubmittedTags( tagNames, driver );

				// Wait for page to populate and then click edit to pop up the editor for a random license row
				IWebElement licenseRowToEdit = (new WebDriverWait( driver, TimeSpan.FromSeconds( 5 ) ).Until( d2 =>
				{
					ReadOnlyCollection<IWebElement> loadedRows = driver.FindElementsByCssSelector( "#license-list tr" );
					if ( loadedRows.Count == 0 ) // Not loaded yet
						return null;
					return loadedRows.ElementAtRandom();
				} ));
				var randomTagEditButton = licenseRowToEdit.FindElement( By.ClassName( "tag_editor_dialog_launcher" ) );
				var editedLicenseActivationKey = randomTagEditButton.GetAttribute( "data-key" );
				randomTagEditButton.Click();

				var tagEditorEl = driver.FindElementById( "tag_editor" );

				// Check the tag names in the popup are as we just set them up (i.e. same as we entered (and also same as column headings on license list))
				var tagLabelsOnDialog = tagEditorEl.FindElements( By.CssSelector( "label[for='tag']" ) ).Select( x => x.Text.TrimEnd( ':' ) ).ToArray();
				Assert.Equal( tagNames, tagLabelsOnDialog );

				// Lash in some values and hit Save
				var tagTextInputs = tagEditorEl.FindElements( By.Name( "tag" ) );
				foreach ( var tagTextInputEl in tagTextInputs )
				{
					tagTextInputEl.Clear();
					tagTextInputEl.SendKeys( new Fixture().CreateAnonymous<string>( "value" ) );
				}
				string[] valuesInputted = tagEditorEl.FindElements( By.Name( "tag" ) ).Select( x => x.GetAttribute( "value" ).Trim() ).ToArray();
				driver.FindElementById( "save_tags" ).Click();

				// Verify inputted values show on page without any refreshes
				Func<bool> tableShowsValuesInputtedForSelectedLicense = () => 
					driver
						.FindElementsByXPath( "//*[@id='license-list']/tr[contains(.,'" + editedLicenseActivationKey + "')]/td" )
						.Select( x => x.Text )
						.ContainsSubsequence( valuesInputted );
				// Page re-renders row asynchronously in response to preceding Click(), which can yank out the IWebElements from underneath us
				new WebDriverWaitIgnoringStaleness( driver, TimeSpan.FromSeconds( 5 ) ).Until( _ => tableShowsValuesInputtedForSelectedLicense() );

				// Verify everything can show when we get everything fresh from the server
				new WebDriverWaitIgnoringNestedTimeouts( driver, TimeSpan.FromSeconds( 15 ) ).Until( _ =>
				{
					// Force refresh by hitting nav home link
					driver.FindElementByCssSelector( ".section_title a" ).Click();
					return new WebDriverWait( driver, TimeSpan.FromSeconds( 5 ) ).Until( __ => tableShowsValuesInputtedForSelectedLicense() );
				} );
			}
		}

		static string[] EnsureWeHaveSomeTagsDefined( RemoteWebDriver driver )
		{
			// The page doesn't enable the Add button until we have loaded
			new WebDriverWait( driver, TimeSpan.FromSeconds( 5 ) ).Until( _ => driver.FindElementById( "add_new_tag" ).Enabled );

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

		static void AwaitSyncingOfSubmittedTags( string[] tagsAsSubmitted, RemoteWebDriver driver )
		{
			// Wait for the headings to show, potentially re-clicking home to attempt a reload/refresh if they are not what we expect (as a user might)
			new WebDriverWaitIgnoringNestedTimeouts( driver, TimeSpan.FromSeconds( 3 ) ).Until( d =>
			{
				driver.FindElementByCssSelector( ".section_title a" ).Click();
				return new WebDriverWait( driver, TimeSpan.FromSeconds( 1 ) ).Until( d2 =>
				{
					var tagHeadings = driver.FindElementsByClassName( "tag_header" ).Select( heading => heading.Text ).ToArray();
					return tagsAsSubmitted.SequenceEqual( tagHeadings );
				} );
			} );
		}

		class WebDriverWaitIgnoringStaleness : WebDriverWait
		{
			public WebDriverWaitIgnoringStaleness( IWebDriver driver, TimeSpan timeout )
				: base( driver, timeout )
			{
				IgnoreExceptionTypes( typeof( StaleElementReferenceException ) );
			}
		}
	}

	class WebDriverWaitIgnoringNestedTimeouts : WebDriverWait
	{
		public WebDriverWaitIgnoringNestedTimeouts( IWebDriver driver, TimeSpan timeout )
			: base( driver, timeout )
		{
			IgnoreExceptionTypes( typeof( WebDriverTimeoutException ) );
		}
	}
	
	static class SubSequenceExtensions
	{
		// http://stackoverflow.com/a/7334462/11635
		public static bool ContainsSubsequence<T>( this IEnumerable<T> parent, IEnumerable<T> target )
		{
			var pattern = target.ToArray();
			var source = new LinkedList<T>();
			foreach ( var element in parent )
			{
				source.AddLast( element );
				if ( source.Count == pattern.Length )
				{
					if ( source.SequenceEqual( pattern ) )
						return true;
					source.RemoveFirst();
				}
			}
			return false;
		}
	}
}