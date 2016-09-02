/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Api.Portal.Acceptance.Wrappers
{
    using RestSharp;
    using Sp.Api.Shared.Wrappers;
    using Sp.Test.Helpers;

    public abstract class SpPortalApi
    {
        readonly IRestClient _client;

        public SpPortalApi( SpApiConfiguration apiConfiguration )
        {
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

        public IRestResponse SignOff()
        {
            var signOffRequest = new RestRequest( "Authentication/LogOff", Method.GET );
            return _client.Execute( signOffRequest );
        }

    }
}
