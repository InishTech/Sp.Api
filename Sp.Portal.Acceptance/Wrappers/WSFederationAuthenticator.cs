﻿using System;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using RestSharp;
using RestSharp.Contrib;

namespace Sp.Portal.Acceptance.Wrappers
{
	using Sp.Portal.Acceptance.Helpers;

	class WSFederationAuthenticator : IAuthenticator
	{
		readonly string _username;
		readonly string _password;

		public WSFederationAuthenticator(string username, string password)
		{
			_username = username;
			_password = password;
		}

		public void Authenticate( IRestClient client, IRestRequest request )
		{
			if ( HasFedAuthCookie( client ) )
				return;

			client.CookieContainer = client.CookieContainer ?? new CookieContainer();
			var auth = new InnerAuthenticator( client.BaseUrl, _username, _password);
			var authCookies = auth.SignInAndRetrieveFedAuthCookies();
			client.CookieContainer.Add( new Uri(client.BaseUrl), authCookies );
		}

		static bool HasFedAuthCookie(IRestClient client)
		{
			return client.CookieContainer != null &&
				   client.CookieContainer.GetCookies( new Uri( client.BaseUrl ) ).OfType<Cookie>().Any( c => c.Name == "FedAuth" );
		}

		class InnerAuthenticator
		{
			readonly string _username;
			readonly string _password;
			readonly RestClient _authClient;

			public InnerAuthenticator( string rpBaseUri, string username, string password )
			{
				_username = username;
				_password = password;
				_authClient = new RestClient(rpBaseUri);
			}

			public CookieCollection SignInAndRetrieveFedAuthCookies()
			{
				// An unauthenticated request to the Portal should redirect us to the STS
				var loginPage = _authClient.Execute( new RestRequest( "/", Method.GET ) );
				var stsLoginUri = loginPage.ResponseUri;

				var wresultRequest = AuthenticateAndSignInWithSts( stsLoginUri );
				// Now we've got WS-Federation wsignin message that we should POST back to the Portal
				// NB - Restharp won't save cookies from the first response if there is a redirection, so we need to turn off FollowRedirects
				_authClient.FollowRedirects = false;
				var rpAuthResponse = _authClient.Execute( wresultRequest );
				if ( rpAuthResponse.StatusCode != HttpStatusCode.Found || !rpAuthResponse.Cookies.Any( c => c.Name == "FedAuth" ) )
					throw new InvalidOperationException( string.Format( "login with RP wasn't successful: {0} {1} {2}", rpAuthResponse.ResponseUri, rpAuthResponse.Content, rpAuthResponse.ErrorMessage ) );

				//Now we have FedAuth and FedAuth1 cookies stored in the cookie container
				var restResponseCookies = rpAuthResponse.Cookies.Where( c => c.Name.StartsWith( "FedAuth" ) ).ToList();
				CookieCollection cookieCollection = new CookieCollection();
				restResponseCookies.ForEach( c => cookieCollection.Add( c.ToHttpCookie() ) );
				return cookieCollection;
			}

			/// <summary>
			/// Authenticates with STS.
			/// Returns RestRequest that should be POSTed to the RP.
			/// Throws InvalidOperationException if the provided credentials are incorrect.
			/// </summary>
			/// <returns></returns>
			RestRequest AuthenticateAndSignInWithSts( Uri stsLoginUri )
			{
				// Passive STS authentication
				RestRequest request = new RestRequest( stsLoginUri, Method.POST );
				// NB - we need to temporarily replace existing base URL, as the authentication STS has a different base URL
				var baseUriCopy = _authClient.BaseUrl;
				_authClient.BaseUrl = stsLoginUri.GetLeftPart( UriPartial.Authority );
				request.AddParameter( "Username", _username );
				request.AddParameter( "Password", _password );

				var authenticationResult = _authClient.Execute( request );
				if ( authenticationResult.StatusCode != HttpStatusCode.OK || !authenticationResult.Content.Contains( "wresult" ) )
					throw new InvalidOperationException( string.Format( "Authentication with STS wasn't successful: {0} {1}", authenticationResult.ResponseUri, authenticationResult.Content ) );
				_authClient.BaseUrl = baseUriCopy;

				return PrepareFederationWSignInRequest( _authClient.BaseUrl, authenticationResult.Content );
			}

			/// <summary>
			/// Reads fields from a hidden form that we got as authentication result (including wa, wresult, wctx), and prepares a WS-Federation wsignin message
			/// Browsers are capable of submitting this form automatically, as it contains a piece of JavaScript, but here we must build the request manually.
			/// </summary>
			static RestRequest PrepareFederationWSignInRequest( string baseUri, string authenticationResultContent )
			{
				HtmlDocument doc = new HtmlDocument();
				doc.LoadHtml( authenticationResultContent );
				var rpUri = doc.DocumentNode.SelectSingleNode( "//form" ).GetAttributeValue( "action", "" );
				RestRequest request = new RestRequest( UriHelper.MakeUriRelativeToBase( baseUri, new Uri( rpUri ).PathAndQuery ), Method.POST );
				foreach ( var input in doc.DocumentNode.SelectNodes( "//input[@type='hidden']" ) )
				{
					string name = input.GetAttributeValue( "name", string.Empty );
					string value = HttpUtility.HtmlDecode( input.GetAttributeValue( "value", string.Empty ) );
					//Console.WriteLine( "{0}:{1}", name, value );
					request.AddParameter( name, value );
				}
				request.AddParameter( "Accept", "text/html" );
				return request;
			}
		}
		
	}
}
