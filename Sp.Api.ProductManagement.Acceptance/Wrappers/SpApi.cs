using RestSharp;
using Sp.Api.ProductManagement.Acceptance.Helpers;
using System;

namespace Sp.Api.ProductManagement.Acceptance.Wrappers
{
	public class SpApi
	{
		readonly string _baseUrl;
		readonly RestClient _client;
		readonly string _password;
		readonly string _username;

		public SpApi( SpApiConfiguration apiConfiguration )
		{
			_baseUrl = apiConfiguration.BaseUrl;
			_client = new RestClient { BaseUrl = _baseUrl };
			_username = apiConfiguration.Username;
			_password = apiConfiguration.Password;

			_client.Authenticator = new WSFederationAuthenticator( _username, _password );
		}

		public IRestResponse<T> Execute<T>( RestRequest request ) where T : new()
		{
			request.Resource = MakeUriRelativeToRestSharpClientBaseUri( request.Resource ).ToString();
			return _client.Execute<T>( request );
		}

		public IRestResponse Execute( RestRequest request )
		{
			request.Resource = MakeUriRelativeToRestSharpClientBaseUri( request.Resource ).ToString();
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