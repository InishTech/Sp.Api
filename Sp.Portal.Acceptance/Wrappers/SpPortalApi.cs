namespace Sp.Portal.Acceptance.Wrappers
{
	using System;
	using System.Linq;
	using System.Net;
	using HtmlAgilityPack;
	using RestSharp;
	using RestSharp.Contrib;

	public class SpPortalApi
	{
		readonly string _baseUrl;
		readonly CookieContainer _cookieContainer;
		readonly RestClient _client;
		readonly string _username;
		readonly string _password;

		public SpPortalApi( SpPortalConfiguration apiConfiguration )
		{
			_baseUrl = apiConfiguration.BaseUrl;
			_cookieContainer = new CookieContainer();
			_client = new RestClient { BaseUrl = _baseUrl };

			_username = apiConfiguration.Username;
			_password = apiConfiguration.Password;
		}

		public IRestResponse<T> Execute<T>( RestRequest request ) where T : new()
		{
			EnsureLoggedIn();
			return _client.Execute<T>( request );
		}

		public IRestResponse Execute( RestRequest request )
		{
			EnsureLoggedIn();
			return _client.Execute( request );
		}

		void EnsureLoggedIn()
		{
			if ( _client.CookieContainer != null )
				return;

			_client.CookieContainer = _cookieContainer;

			// TODO this should be conditional, i.e. each conversation should only require a single login to take place.
			// This would be an IAuthenticator. However we're likely to be changing our auth approach this placeholder behavior will do for now

			// An unauthenticated request to the Portal should redirect us to the STS
			var loginPage = _client.Execute( new RestRequest( "/", Method.GET ) );
			var stsLoginUri = loginPage.ResponseUri;

			var wresultRequest = AuthenticateAndSignInWithSts( stsLoginUri );
			// Now we've got WS-Federation wsignin message that we should POST back to the Portal
			// NB - Restharp won't save cookies from the first response if there is a redirection, so we need to turn off FollowRedirects
			_client.FollowRedirects = false;
			var rpAuthResponse = _client.Execute( wresultRequest );
			if ( rpAuthResponse.StatusCode != HttpStatusCode.Found || !rpAuthResponse.Cookies.Any(c => c.Name == "FedAuth") )
				throw new InvalidOperationException( string.Format( "login with RP wasn't successful: {0} {1} {2}", rpAuthResponse.ResponseUri, rpAuthResponse.Content, rpAuthResponse.ErrorMessage ) );
			_client.FollowRedirects = true;

			//Now we have FedAuth cookie stored in the cookie container
		}

		// Required if your BaseUri includes a path (e.g., within InishTech test environments, instances are not always at / on a machine)
		Uri MakeUriRelativeToRestSharpClientBaseUri( string resource )
		{
			Uri clientBaseUriEndingWithSlash = ClientBaseUri.EndsWith( "/" )
				? new Uri( ClientBaseUri )
				: new Uri( ClientBaseUri + "/" );
			var requestUriAbsolute = new Uri( clientBaseUriEndingWithSlash, resource );
			var uriRelativeToClientBaseUri = clientBaseUriEndingWithSlash.MakeRelativeUri( requestUriAbsolute );
			return uriRelativeToClientBaseUri;
		}

		string ClientBaseUri
		{
			get { return _client.BaseUrl; }
		}

		/// <summary>
		/// Authenticates with STS.
		/// Returns RestRequest that should be POSTed to the RP.
		/// Throws InvalidOperationException if the provided credentials are incorrect.
		/// </summary>
		/// <param name="stsLoginUri"></param>
		/// <returns></returns>
		RestRequest AuthenticateAndSignInWithSts( Uri stsLoginUri )
		{
			// Passive STS authentication
			RestRequest request = new RestRequest( stsLoginUri, Method.POST );
			// NB - we need to temporarily replace existing base URL, as the authentication STS has a different base URL
			var baseUriCopy = _client.BaseUrl;
			_client.BaseUrl = stsLoginUri.GetLeftPart( UriPartial.Authority );
			request.AddParameter( "Username", _username );
			request.AddParameter( "Password", _password );

			var authenticationResult = _client.Execute( request );
			if ( authenticationResult.StatusCode != HttpStatusCode.OK || !authenticationResult.Content.Contains( "wresult" ) )
				throw new InvalidOperationException( string.Format( "login wasn't successful: {0} {1}", authenticationResult.ResponseUri, authenticationResult.Content ) );
			_client.BaseUrl = baseUriCopy;

			return PrepareFederationWSignInRequest( authenticationResult );
		}

		RestRequest PrepareFederationWSignInRequest( IRestResponse loginResult )
		{
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml( loginResult.Content );
			var rpUri = doc.DocumentNode.SelectNodes( "//form" )[ 0 ].GetAttributeValue( "action", "" );
			RestRequest request = new RestRequest( MakeUriRelativeToRestSharpClientBaseUri( new Uri( rpUri ).PathAndQuery ), Method.POST );
			foreach ( var input in doc.DocumentNode.SelectNodes( "//input[@type='hidden']" ) )
			{
				string name = input.GetAttributeValue( "name", string.Empty );
				string value = HttpUtility.HtmlDecode( input.GetAttributeValue( "value", string.Empty ) );
				//Console.WriteLine( "{0}:{1}", name, value );
				request.AddParameter( name, value );
			}
			request.AddParameter( "Accept", "text/html" );
			request.AddParameter( "Referer", loginResult.ResponseUri );
			return request;
		}

		public void SignOff()
		{
			var signOffRequest = new RestRequest( "/Authentication/LogOff", Method.GET );
			var result = _client.Execute( signOffRequest );
			if(result.StatusCode != HttpStatusCode.OK || !result.ResponseUri.AbsolutePath.Contains( "Sp.Portal.Sts" ))
				throw new InvalidOperationException( string.Format( "logout wasn't successful: {0} {1}", result.ResponseUri, result.Content ) );
		}

	}
}
