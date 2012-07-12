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

		public SpApi( SpApiConfiguration apiConfiguration )
		{
			_client = new RelativePathAwareCustomRestClient( apiConfiguration.BaseUrl )
			{
				Authenticator = new WSFederationAuthenticator( apiConfiguration.Username, apiConfiguration.Password )
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

		public IRestResponse SignOff()
		{
			var signOffRequest = new RestRequest( ApiPrefix.WebApiRoot + "/Authentication/LogOff", Method.GET );
			return _client.Execute( signOffRequest );
		}
	}
}