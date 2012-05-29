namespace Sp.Portal.Acceptance.Wrappers
{
	using System;
	using System.Net;
	using RestSharp;
	using Sp.Portal.Acceptance.Helpers;

	public class SpPortalApi
	{
		readonly string _baseUrl;
		readonly RestClient _client;
		readonly string _username;
		readonly string _password;

		public SpPortalApi( SpPortalConfiguration apiConfiguration )
		{
			_baseUrl = apiConfiguration.BaseUrl;
			_client = new RestClient { BaseUrl = _baseUrl };
			
			_username = apiConfiguration.Username;
			_password = apiConfiguration.Password;

			_client.Authenticator = new WSFederationAuthenticator( _username, _password );
		}

		public IRestResponse<T> Execute<T>( RestRequest request ) where T : new()
		{
			//TODO - fix MakeUriRelativeToRestSharpClientBaseUri
			//request.Resource = MakeUriRelativeToRestSharpClientBaseUri( request.Resource ).ToString();
			return _client.Execute<T>( request );
		}

		public IRestResponse Execute( RestRequest request )
		{
			//TODO - fix MakeUriRelativeToRestSharpClientBaseUri
			//request.Resource = MakeUriRelativeToRestSharpClientBaseUri( request.Resource ).ToString();
			return _client.Execute( request );
		}

		// Required if your BaseUri includes a path (e.g., within InishTech test environments, instances are not always at / on a machine)
		Uri MakeUriRelativeToRestSharpClientBaseUri( string resource )
		{
			return UriHelper.MakeUriRelativeToBase( ClientBaseUri, resource );
		}

		string ClientBaseUri
		{
			get { return _client.BaseUrl; }
		}

		public IRestResponse SignOff()
		{
			var signOffRequest = new RestRequest( "/Authentication/LogOff", Method.GET );
			return _client.Execute( signOffRequest );
		}

	}
}
