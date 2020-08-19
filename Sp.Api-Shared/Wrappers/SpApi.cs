/* Copyright (c) 2017 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Api.Shared.Wrappers
{
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
        }

        public IRestResponse<T> Execute<T>( IRestRequest request ) where T : new()
        {
            request.AddAuthorizationHeader();
            return _client.Execute<T>( request );
        }

        public IRestResponse Execute( IRestRequest request )
        {
            request.AddAuthorizationHeader();
            return _client.Execute( request );
        }

        public string GetApiPrefix( ApiType apiType )
        {
            return _apiConfiguration.GetApiPrefix( apiType );
        }
    }
}