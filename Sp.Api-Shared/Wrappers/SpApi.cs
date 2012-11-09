/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Api.Shared.Wrappers
{
	using RestSharp;
	using Sp.Test.Helpers;

	public class SpApi
	{
		readonly IRestClient _client;
		readonly SpApiConfiguration _apiConfiguration;

		public SpApi( SpApiConfiguration apiConfiguration )
		{
			_apiConfiguration = apiConfiguration;
			_client = new RelativePathAwareCustomRestClient( apiConfiguration.BaseUrl )
			{
				Authenticator = new WSFederationAuthenticator( apiConfiguration )
			};
		}

		public IRestResponse<T> Execute<T>( RestRequest request ) where T : new()
		{
			return _client.Execute<T>( request );
		}

		public IRestResponse Execute( RestRequest request )
		{
			return _client.Execute( request );
		}

		public string GetApiPrefix( ApiType apiType )
		{
			return _apiConfiguration.GetApiPrefix( apiType );
		}

		public IRestResponse SignOff()
		{
			var signOffRequest = new RestRequest( GetApiPrefix( ApiType.WebApiRoot ) + "/Authentication/LogOff", Method.GET );
			return _client.Execute( signOffRequest );
		}
	}
}