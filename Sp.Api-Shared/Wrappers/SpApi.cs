/* Copyright (c) 2017 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Api.Shared.Wrappers
{
    using IdentityModel.Client;
    using RestSharp;
    using RestSharp.Serializers;
    using Sp.Test.Helpers;
    using System.Net.Http;

    public class SpApi
    {
        readonly IRestClient _client;
        readonly SpApiConfiguration _apiConfiguration;

        public SpApi( SpApiConfiguration apiConfiguration )
        {
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            _apiConfiguration = apiConfiguration;
            _client = new RelativePathAwareCustomRestClient( apiConfiguration.BaseUrl );
            _client.AddDefaultHeader( "Authorization", string.Format( "Bearer {0}", GetAccessToken() ) );
        }

        string GetAccessToken()
        {
            var client = new HttpClient();
            var disco = client.GetDiscoveryDocumentAsync( _apiConfiguration.Authority.ToLower() ).Result;

            if ( disco.IsError )
                throw new System.Exception( disco.Error );

            var tokenResponse = client.RequestClientCredentialsTokenAsync( new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = _apiConfiguration.ClientId,
                ClientSecret = _apiConfiguration.ClientSecret,
                Scope = _apiConfiguration.Scope
            } ).Result;
            if ( tokenResponse.IsError )
                throw new System.Exception( tokenResponse.Error );
            return tokenResponse.AccessToken;
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
    }
}