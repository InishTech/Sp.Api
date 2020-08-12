using IdentityModel.Client;
using RestSharp;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;

namespace Sp.Test.Helpers
{
    public static class RestRequestAuthExtensions
    {
        public static void AddAuthorizationHeader( this IRestRequest that )
        {
            that.AddHeader( "Authorization", "bearer " + GetAccessToken() );
        }

        static string GetAccessToken()
        {
            var client = new HttpClient();
            var disco = client.GetDiscoveryDocumentAsync( new DiscoveryDocumentRequest
            {
                Address = ConfigurationManager.AppSettings[ "Authority" ].ToLower(),
                Policy =
                    {
                        RequireHttps =  false
                    }
            } ).Result;

            if ( disco.IsError )
                throw new System.Exception( disco.Error );
            var clientId = ConfigurationManager.AppSettings[ "ClientId" ];
            var secret = ConfigurationManager.AppSettings[ "ClientSecret" ];
            var tokenResponse = client.RequestClientCredentialsTokenAsync( new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = clientId,
                ClientSecret = secret,
                Scope = ConfigurationManager.AppSettings[ "scope" ]
            } ).Result;
            if ( tokenResponse.IsError )
                throw new System.Exception( tokenResponse.Error );
            return tokenResponse.AccessToken;
        }
    }
}