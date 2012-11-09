/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
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
		readonly string _password;
		readonly string _baseUrl;

		public SpApiConfiguration( string username, string password, string baseUrl )
		{
			_username = username;
			_password = password;
			_baseUrl = baseUrl;
		}

		public string Username
		{
			get { return _username; }
		}

		public string Password
		{
			get { return _password; }
		}

		public string BaseUrl
		{
			get { return _baseUrl; }
		}

		// NB this is not the long terma approach - will be using hypermedia and HAL to navigate instead of computing urls the way anything relaiant on this currently does
		// TODO TP 1105 these should be determined by going to an api landing location
		public string GetApiPrefix( ApiType apiType )
		{
			return IsCloudEnvironment ? _azureApiPrefixes[ apiType ] : _iisApiPrefixes[ apiType ];
		}

		bool IsCloudEnvironment
		{
			get
			{
				return BaseUrl.Contains( "softwarepotential.com" ) || BaseUrl.Contains( "cloudapp" );
			}
		}

		readonly Dictionary<ApiType, string> _azureApiPrefixes = new Dictionary<ApiType, string>
		{
			{ ApiType.WebApiRoot, "Home" },
			{ ApiType.Define, "Define" },
			{ ApiType.Issue, "Issue" },
			{ ApiType.Customer, "Customer" },
			{ ApiType.Auth, "Auth" },
		};

		readonly Dictionary<ApiType, string> _iisApiPrefixes = new Dictionary<ApiType, string>
		{
			{ ApiType.WebApiRoot, "Sp.Web" },
			{ ApiType.Define, "Sp.Web.Define" },
			{ ApiType.Issue, "Sp.Web.Issue" },
			{ ApiType.Customer, "Sp.Web.CustomerManagement" },
			{ ApiType.Auth, "Sp.Auth.Web" },
		};
	}

	// NB this is not the long terma approach - will be using hypermedia and HAL to navigate instead of computing urls the way anything relaiant on this currently does
	// TODO TP 1105 these should be determined by going to an api landing location
	public enum ApiType
	{
		WebApiRoot,
		Define,
		Issue,
		Customer,
		Auth
	}
}