/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
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
	}

	// NB this is not the long terma approach - will be using hypermedia and HAL to navigate instead of computing urls the way anything relaiant on this currently does
	// TODO TP 1105 these should be determined by going to an api landing location
	static class ApiPrefix
	{
		public const string WebApiRoot = "Sp.Web";
		public const string Define = "Sp.Web.Define";
		public const string Issue = "Sp.Web.Issue";
		public const string Customer = "Sp.Web.CustomerManagement";
		public const string Auth = "Sp.Auth.Web";
	}
}