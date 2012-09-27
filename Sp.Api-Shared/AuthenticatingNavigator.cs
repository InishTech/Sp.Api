namespace Sp.Api.Shared
{
	using OpenQA.Selenium;
	using OpenQA.Selenium.Remote;
	using Ploeh.AutoFixture;
	using Sp.Api.Shared.Wrappers;
	using Sp.Test.Html;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	// For now, we do 
	//	[Theory, ClassData( typeof( RemoteWebDriverAndAuthenticatingNavigatorProvider<SoftwarePotentialDataFixture> ) )]
	// Workaround for lack of AutoClassData - if we had such a device, we'd do:
	//	[Theory, AutoSoftwarePotentialData, ClassAutoData( typeof( BrowserDriverDataProvider ) )]
	public class RemoteWebDriverAndAuthenticatingNavigatorProvider<TFixture> : TupleTheoryDataProvider<RemoteWebDriver, AuthenticatingNavigator> where TFixture : Fixture, new()
	{
		protected override IEnumerable<Tuple<RemoteWebDriver, AuthenticatingNavigator>> Generate()
		{
			var bbdp = new BrowserDriverDataProvider();
			var configuration = new TFixture().CreateAnonymous<SpApiConfiguration>();
			var navigator = new AuthenticatingNavigator( configuration );
			return
				from object[] rwd in bbdp
				select Tuple.Create( (RemoteWebDriver)rwd[ 0 ], navigator );
		}
	}

	public abstract class TupleTheoryDataProvider<T1, T2> : TheoryDataProvider
	{
		protected abstract IEnumerable<Tuple<T1, T2>> Generate();

		protected override sealed IEnumerable<object[]> DataSource()
		{
			return Generate().Select( x => new object[] { x.Item1, x.Item2 } );
		}
	}

	public class AuthenticatingNavigator
	{
		readonly SpApiConfiguration _configuration;

		public AuthenticatingNavigator( SpApiConfiguration configuration )
		{
			_configuration = configuration;
		}

		public void NavigateWithAuthenticate( RemoteWebDriver driver, string service )
		{
			driver.Navigate().GoToUrl( _configuration.BaseUrl + "/" + service );
			Authenticate( driver, _configuration.Username, _configuration.Password );
		}

		static void Authenticate( RemoteWebDriver driver, string username, string password )
		{
			IWebElement usernameElement = driver.FindElement( By.Id( "Username" ) );
			usernameElement.SendKeys( username );

			IWebElement passwordElement = driver.FindElement( By.Id( "Password" ) );
			passwordElement.SendKeys( password );

			passwordElement.Submit();
		}
	}
}