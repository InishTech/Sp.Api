/* Copyright (c) 2017 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */

using System.Collections.Generic;

namespace Sp.Api.Shared.Wrappers
{
    public class SpApiConfiguration
    {

        public SpApiConfiguration( string username, string password, string clientId, string clientSecret, string baseUrl, string authority, string scope )
        {
            Username = username;
            Password = password;
            Scope = scope;
            ClientId = clientId;
            ClientSecret = clientSecret;
            BaseUrl = baseUrl;
            Authority = authority;
        }

        public string Username { get; }

        public string Password { get; }

        public string ClientId { get; }

        public string ClientSecret { get; }

        public string BaseUrl { get; }

        public string Authority { get; }

        public string Scope { get; }

        // NB this is not the long term approach - will be using hypermedia and HAL to navigate instead of computing urls the way anything relaiant on this currently does
        // TODO TP 1105 these should be determined by going to an api landing location
        public string GetApiPrefix( ApiType apiType )
        {
            return IsCloudEnvironment ? _azureApiPrefixes[ apiType ] : _iisApiPrefixes[ apiType ];
        }

        bool IsCloudEnvironment
        {
            get { return BaseUrl.Contains( "softwarepotential.com" ) || BaseUrl.Contains( "cloudapp" ); }
        }

        readonly Dictionary<ApiType, string> _azureApiPrefixes = new Dictionary<ApiType, string>
        {
            { ApiType.WebApiRoot, "Home" },
            { ApiType.Define, "Define" },
            { ApiType.Issue, "Issue" },
            { ApiType.Consume, "Consume" },
            { ApiType.Auth, "Auth" },
            { ApiType.Develop, "Develop" }
        };

        readonly Dictionary<ApiType, string> _iisApiPrefixes = new Dictionary<ApiType, string>
        {
            { ApiType.WebApiRoot, "Sp.Web" },
            { ApiType.Define, "Sp.Web.Define" },
            { ApiType.Issue, "Sp.Web.Issue" },
            { ApiType.Consume, "Sp.Api.Consume" },
            { ApiType.Auth, "Sp.Auth.Web" },
            { ApiType.Develop, "Sp.Web.Develop" }
        };
    }

    // NB this is not the long terma approach - will be using hypermedia and HAL to navigate instead of computing urls the way anything relaiant on this currently does
    // TODO TP 1105 these should be determined by going to an api landing location
    public enum ApiType
    {
        WebApiRoot,
        Define,
        Issue,
        Consume,
        Auth,
        Develop
    }
}