﻿namespace Sp.Portal.Acceptance.Wrappers
{
	public class SpPortalConfiguration
	{
		readonly string _username;
		readonly string _password;
		readonly string _baseUrl;

		public SpPortalConfiguration( string username, string password )
			: this( username, password, "https://portal.softwarepotential.com" )
		{
		}

		public SpPortalConfiguration( string username, string password, string baseUrl )
		{
			_baseUrl = baseUrl;
			_password = password;
			_username = username;
		}

		public string BaseUrl
		{
			get { return _baseUrl; }
		}

		public string Username
		{
			get { return _username; }
		}

		public string Password
		{
			get { return _password; }
		}

	}
}