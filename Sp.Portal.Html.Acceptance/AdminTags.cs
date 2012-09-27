namespace Sp.Portal.Html.Acceptance
{
	using OpenQA.Selenium;
	using OpenQA.Selenium.Remote;
	using OpenQA.Selenium.Support.UI;
	using Ploeh.AutoFixture;
	using Sp.Api.Shared;
	using Sp.Portal.Acceptance;
	using Sp.Test.Html;
	using System;
	using System.Linq;
	using Xunit;
	using Xunit.Extensions;

	public class AdminTags
	{
		[Theory, ClassData( typeof( RemoteWebDriverAndAuthenticatingNavigatorProvider<SoftwarePotentialPortalDataFixture> ) )]
		public static void ShouldPropagateDeletesAndAddsToMainScreen( RemoteWebDriver driver, AuthenticatingNavigator navigator )
		{
			using ( driver.FinallyQuitGuard() ) // TODO improve this using http://xunit.codeplex.com/workitem/9798 ( WAS: http://xunit.codeplex.com/discussions/362097 )
			{
				navigator.NavigateWithAuthenticate( driver, "tag" );

				// If we cannot respond in 5 seconds for any reason, a human will seriously distrust the software, no excuses
				new WebDriverWait( driver, TimeSpan.FromSeconds( 5 ) ).Until( d => d
					.FindElement( By.Id( "tags" ) )
					.FindElements( By.TagName( "li" ) )
					.Count > 0 );

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

				// Save
				driver.FindElementById( "save_tags" ).Click();
				var tagsAsSubmitted = driver.FindElementsById( "tag_name" ).Select( tagNameEl => tagNameEl.GetAttribute( "value" ).Trim() ).ToArray();

				// Wait for the success report
				new WebDriverWait( driver, TimeSpan.FromSeconds( 3 ) ).Until( d =>
				{
					var messagesElement = d.FindElement( By.Id( "messages" ) );
					return messagesElement.Displayed && -1 != messagesElement.Text.IndexOf( "saved successfully", StringComparison.InvariantCultureIgnoreCase );
				} );

				// Wait for the headings to show, potentially re-clicking home to attempt a reload/refresh if they are not what we expect (as a user might)
				new WebDriverWaitIgnoringNestedTimeouts( driver, TimeSpan.FromSeconds( 3 ) ).Until( d =>
				{
					driver.FindElementById( "logo_link" ).FindElement( By.TagName( "a" ) ).Click();
					return new WebDriverWait( driver, TimeSpan.FromSeconds(1) ).Until( d2 =>
					{
						var tagHeadings = d2.FindElements( By.ClassName( "tag_header" ) ).Select( heading => heading.Text ).ToArray();
						return  tagsAsSubmitted.OrderBy( x => x ).SequenceEqual( tagHeadings.OrderBy( x => x ) );
					} );
				} );
			}
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