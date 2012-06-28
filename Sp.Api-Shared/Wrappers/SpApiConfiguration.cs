namespace Sp.Api.Shared.Wrappers
{
	public class SpApiConfiguration
	{
		readonly string _username;
		readonly string _password;
		readonly string _baseUrl;

		public SpApiConfiguration( string username, string password )
			: this( username, password, "https://web.softwarepotential.com" )
		{
		}

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

	static class ApiPrefix
	{
		public const string WebApiRoot = "Sp.Web";
		public const string Define = "Sp.Web.Define";
		public const string Issue = "Sp.Web.Issue";
	}
}