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

        public SpApiConfiguration( string username, string password, string clientId, string clientSecret, string baseUrl, string authBaseUrl, string authority, string scope, string skipCertValidation)
        {
            Username = username;
            Password = password;
            Scope = scope;
            ClientId = clientId;
            ClientSecret = clientSecret;
            BaseUrl = baseUrl;
			AuthBaseUrl = authBaseUrl;
			Authority = authority;
            RequireHttps = !bool.Parse( skipCertValidation );
        }

        public string Username { get; }

        public string Password { get; }

        public string ClientId { get; }

        public string ClientSecret { get; }

        public string BaseUrl { get; set; }

		public string AuthBaseUrl { get; }

		public string Authority { get; }

        public string Scope { get; }

        public bool RequireHttps { get; }

		// NB this is not the long term approach - will be using hypermedia and HAL to navigate instead of computing urls the way anything relaiant on this currently does
		// TODO TP 1105 these should be determined by going to an api landing location
		public string GetApiPrefix( ApiType apiType ) => IsCloudEnvironment( apiType, false) ? _azureApiPrefixes[ apiType ] : _iisOrKestrelApiPrefixes[ apiType ];
		public string GetHtmlPrefix( ApiType apiType ) => IsCloudEnvironment(apiType, true) ? _azureHtmlPrefixes[ apiType ] : _iisOrKestrelHtmlPrefixes[ apiType ];

		bool IsCloudEnvironment( ApiType apiType, bool isHtml)
        {

            var returnValue = BaseUrl.Contains( "softwarepotential.com" ) || BaseUrl.Contains( "cloudapp" );

            if( isHtml && !returnValue && _subsystemPorts.ContainsKey(apiType) )
			{
				BaseUrl = $"{BaseUrl}:{ _subsystemPorts[apiType]}";
			}

            if( isHtml && returnValue && _subdomainUrls.ContainsKey( apiType ) )
            {
                BaseUrl = BaseUrl.Replace( "srv", _subdomainUrls[ apiType ] );
            }

            return returnValue;

        }

		readonly Dictionary<ApiType, string> _azureApiPrefixes = new Dictionary<ApiType, string>
		{
			{ ApiType.WebApiRoot, "Home" },
			{ ApiType.Issue, "IssueApi" },
			{ ApiType.Consume, "ConsumeApi" },
			{ ApiType.Develop, "DevelopApi" }
		};

		readonly Dictionary<ApiType, string> _iisOrKestrelApiPrefixes = new Dictionary<ApiType, string>
		{
			{ ApiType.WebApiRoot, "Sp.Api.Web" },
			{ ApiType.Issue, "Sp.Api.Issue" },
			{ ApiType.Consume, "Sp.Api.Consume" },
			{ ApiType.Develop, "Sp.Api.Develop" },
			{ ApiType.Auth, "api" }
		};

		readonly Dictionary<ApiType, string> _azureHtmlPrefixes = new Dictionary<ApiType, string>
		{
            { ApiType.Auth, "Auth" },
            { ApiType.WebApiRoot, "Home" },
			{ ApiType.Issue, "Issue" },
			{ ApiType.Consume, "Consume" },
			{ ApiType.Develop, "Develop" },
			{ ApiType.Nuget, "Nuget" },
			{ ApiType.Analyze, "Analyze" }
		};

		readonly Dictionary<ApiType, string> _iisOrKestrelHtmlPrefixes = new Dictionary<ApiType, string>
		{
			{ ApiType.Auth, "auth" },
			{ ApiType.WebApiRoot, "web" },
			{ ApiType.Issue, "Sp.Web.Issue" },
			{ ApiType.Consume, "Sp.Web.Consume" },
			{ ApiType.Develop, "Sp.Web.Develop" },
			{ ApiType.Nuget, "Sp.Web.Develop.Nuget" },
			{ ApiType.Analyze, "Sp.Web.Analyze" }
		};

        readonly Dictionary<ApiType, string> _subsystemPorts = new Dictionary<ApiType, string>
        {
            { ApiType.Auth, "5003" },
            { ApiType.WebApiRoot, "5011" }
        };

        readonly Dictionary<ApiType, string> _subdomainUrls = new Dictionary<ApiType, string>
        {
            { ApiType.Auth, "auth"},
            { ApiType.WebApiRoot, "web" }
        };

    }

    // NB this is not the long terma approach - will be using hypermedia and HAL to navigate instead of computing urls the way anything relaiant on this currently does
    // TODO TP 1105 these should be determined by going to an api landing location
    public enum ApiType
    {
        WebApiRoot,
        Issue,
        Consume,
        Auth,
        Develop,
		Nuget,
		Analyze
    }
}