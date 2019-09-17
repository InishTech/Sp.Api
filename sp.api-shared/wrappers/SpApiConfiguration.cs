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
        readonly string _username;
        readonly string _passowrd;
        readonly string _clientId;
		readonly string _clientSecret;
		readonly string _baseUrl;
        readonly string _authority;
        readonly string _scope;

        public SpApiConfiguration(string username, string password, string clientId, string clientSecret, string baseUrl, string authority, string scope)
		{
            _username = username;
            _passowrd = password;
            _scope = scope;
            _clientId = clientId;
            _clientSecret = clientSecret;
			_baseUrl = baseUrl;
            _authority = authority;
		}

        public string Username
        {
            get { return _username; }
        }

        public string Password
        {
            get { return _passowrd; }
        }

		public string ClientId
		{
			get { return _clientId; }
		}

		public string ClientSecret
		{
			get { return _clientSecret; }
		}

		public string BaseUrl
		{
			get { return _baseUrl; }
		}

        public string Authority
        {
            get { return _authority; }
        }

        public string Scope
        {
            get { return _scope; }
        }

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