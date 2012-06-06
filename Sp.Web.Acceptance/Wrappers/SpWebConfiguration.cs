namespace Sp.Web.Acceptance.Wrappers
{
	public class SpWebConfiguration
	{
		readonly string _username;
		readonly string _password;
		readonly string _baseUrl;

		public SpWebConfiguration( string username, string password )
			: this( username, password, "https://srv.softwarepotential.com" )
		{
		}

		public SpWebConfiguration( string username, string password, string baseUrl )
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
}