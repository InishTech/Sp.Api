/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Portal.Html.Acceptance
{
	using OpenQA.Selenium;
	using OpenQA.Selenium.Chrome;
	using OpenQA.Selenium.Firefox;
	using OpenQA.Selenium.Remote;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Linq;

	public class BrowserDriverDataProvider : SingleItemTheoryDataProvider
	{
		protected override IEnumerable<object> SingleItemDataSource()
		{
			var drivers = new RemoteWebDriver[] 
			{
				new ChromeDriver(),
				new FirefoxDriver( GetSeleniumFirefoxProfile() )
			};

			return drivers
				.Do( Authenticate );
		}

		static void Authenticate( RemoteWebDriver driver )
		{
			driver.Navigate().GoToUrl( ConfigurationManager.AppSettings[ "PortalBaseUrl" ] );

			IWebElement username = driver.FindElement( By.Id( "Username" ) );
			username.SendKeys( ConfigurationManager.AppSettings[ "Username" ] );

			IWebElement password = driver.FindElement( By.Id( "Password" ) );
			password.SendKeys( ConfigurationManager.AppSettings[ "Password" ] );

			password.Submit();
		}

		static FirefoxProfile GetSeleniumFirefoxProfile()
		{
			const string profileName = "Selenium";
			FirefoxProfileManager profileManager = new FirefoxProfileManager();
			FirefoxProfile profile = profileManager.GetProfile( profileName );
			if ( profile == null )
				throw new InvalidOperationException( "Cannot find Firefox profile " + profileName );
			profile.SetPreference( "browser.ssl_override_behavior", 1 );
			profile.AcceptUntrustedCertificates = false;
			return profile;
		}
	}

	static class QuitGuardExtensions
	{
		public static IDisposable FinallyQuitGuard( this RemoteWebDriver that )
		{
			return new LambdaDisposable( that.Quit );
		}

		class LambdaDisposable : IDisposable
		{
			Action _action;

			public LambdaDisposable( Action action )
			{
				_action = action;
			}

			void IDisposable.Dispose()
			{
				if ( _action == null )
					throw new ObjectDisposedException( "Should not be triggering a LambdaDisposable twice" );

				try
				{
					_action();
				}
				finally
				{
					_action = null;
				}
			}
		}
	}

	public abstract class SingleItemTheoryDataProvider : TheoryDataProvider
	{
		protected override sealed IEnumerable<object[]> DataSource()
		{
			return SingleItemDataSource().Select( x => new[] { x } );
		}

		protected abstract IEnumerable<object> SingleItemDataSource();
	}

	public abstract class TheoryDataProvider : IEnumerable<object[]>
	{
		public IEnumerator<object[]> GetEnumerator()
		{
			return DataSource().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return DataSource().GetEnumerator();
		}

		protected abstract IEnumerable<object[]> DataSource();
	}

	static class DoExtensions
	{
		public static IEnumerable<T> Do<T>( this IEnumerable<T> thats, Action<T> action )
		{
			foreach ( var item in thats )
			{
				action( item );
				yield return item;
			}
		}
	}
}