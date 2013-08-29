/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Test.Html
{
	using OpenQA.Selenium.Chrome;
	using OpenQA.Selenium.Firefox;
	using OpenQA.Selenium.IE;
	using OpenQA.Selenium.Remote;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	public class BrowserDriverDataProvider : TupleTheoryDataProvider<RemoteWebDriver>
	{
		protected override IEnumerable<RemoteWebDriver> Generate()
		{
			// Setting IE as default desktop build driver as it tends to be the most buggy and Firefox / Chrome are consistently used by devs
			yield return new InternetExplorerDriver();
#if !DEBUG
			yield return new FirefoxDriver( GetSeleniumFirefoxProfile() );
			yield return new ChromeDriver();
#endif
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

	public static class QuitGuardExtensions
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

	public abstract class TupleTheoryDataProvider<T1> : TheoryDataProvider
	{
		protected abstract IEnumerable<T1> Generate();

		protected override sealed IEnumerable<object[]> DataSource()
		{
			return Generate().Select( x => new object[] { x } );
		}
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
}