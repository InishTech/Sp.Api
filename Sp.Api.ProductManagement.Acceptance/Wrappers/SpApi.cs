using HtmlAgilityPack;
using RestSharp;
using System;
using System.Linq;
using System.Net;

namespace Sp.Api.ProductManagement.Acceptance.Wrappers
{
	public class SpApi
	{
		readonly RestClient _client;
		readonly CookieContainer _cookieContainer;
		readonly string _baseUrl;
		readonly string _username;
		readonly string _password;

		public SpApi( SpApiConfiguration apiConfiguration )
		{
			_baseUrl = apiConfiguration.BaseUrl;
			_cookieContainer = new CookieContainer();
			_username = apiConfiguration.Username;
			_password = apiConfiguration.Password;
			_client = new RestClient();
			_client.BaseUrl = _baseUrl;
		}

		internal void EnsureLoggedIn()
		{
			if ( _client.CookieContainer != null )
				return;

			_client.CookieContainer = _cookieContainer;

			// TODO this should be conditional, i.e. each conversation should only require a single login to take place.
			// This would be an IAuthenticator. However we're likely to be changing our auth approach this placeholder behavior will do for now
			
			var loginPage = _client.Execute( new RestRequest( "login.aspx", Method.GET ) );
			var loginPageHtml = new HtmlDocument();
			loginPageHtml.LoadHtml( loginPage.Content );
			RestRequest request = new RestRequest( "login.aspx", Method.POST );
			request.AddParameter( "_ctl0:ContentPlaceHolder1:m_oLoginBtn.x", "0" );
			request.AddParameter( "_ctl0:ContentPlaceHolder1:m_oLoginBtn.y", "0" );
			request.AddParameter( "_ctl0:ContentPlaceHolder1:m_oUserNameTb", _username );
			request.AddParameter( "_ctl0:ContentPlaceHolder1:m_oPasswordTb", _password );
			request.AddHiddenFieldValueFrom( "__EVENTVALIDATION", loginPageHtml );
			request.AddHiddenFieldValueFrom( "__VIEWSTATE", loginPageHtml );
			
			//Suppress following redirections (HTTP status codes 3xx)
			_client.FollowRedirects = false;
			//Submit the login form
			var loginResult = _client.Execute( request );
			//Client should receive a redirection to Default.aspx page (HTTP Status 302; Location header should point to Default.aspx)
			if ( loginResult.StatusCode != HttpStatusCode.Found ||
				!loginResult.Headers.Any( h => h.Name == "Location" && h.Value.ToString().EndsWith("/Default.aspx") ) )
					throw new Exception( String.Format( "LOGIN FAILED: {0} with status code {1} says: {2}", loginResult.ResponseUri, loginResult.StatusCode, loginResult.Content ) );
			//Restore following redirections
			_client.FollowRedirects = true;
		}

		public IRestResponse<T> Execute<T>( RestRequest request ) where T : new()
		{
			EnsureLoggedIn();
			request.Resource = MakeUriRelativeToRestSharpClientBaseUri( request.Resource ).ToString(  );
			return _client.Execute<T>( request );
		}

		public IRestResponse Execute( RestRequest request ) 
		{
			EnsureLoggedIn();
			request.Resource = MakeUriRelativeToRestSharpClientBaseUri( request.Resource ).ToString();
			return _client.Execute( request );
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

		public override string ToString()
		{
			return string.Format( "{0}( Url: {1}; Username: {2})",
			  typeof( SpApi ).Name, _baseUrl, _username );
		}

		string ClientBaseUri
		{
			get { return  _client.BaseUrl; }
		}
	}

	static class RestRequestFormSubmissionExtensions
	{
		public static void AddHiddenFieldValueFrom( this RestRequest that, string fieldName, HtmlDocument htmlDocument )
		{
			that.AddParameter( fieldName, htmlDocument.HiddenInputValue( fieldName ) );
		}
	}

	static class HtmlDocumentExtensions
	{
		public static string HiddenInputValue( this HtmlDocument page, string fieldName )
		{
			var existingNode = page
				.DocumentNode.Descendants( "input" )
				.SingleOrDefault( i => i.HasAttributeValue( fieldName, "name" ) );
			string existingValue = existingNode == null ? String.Empty : existingNode.Attributes[ "value" ].Value;
			return existingValue;
		}

		public static bool HasAttributeValue( this HtmlNode node, string attributeValue, string attributeName )
		{
			return node.Attributes.Contains( attributeName ) && node.Attributes[ attributeName ].Value == attributeValue;
		}
	}
}