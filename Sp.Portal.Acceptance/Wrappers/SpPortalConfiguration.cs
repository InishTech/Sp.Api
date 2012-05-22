namespace Sp.Portal.Acceptance.Wrappers
{
	public class SpPortalConfiguration
	{
		readonly string _baseUrl;

		public SpPortalConfiguration()
			: this( "https://portal.softwarepotential.com" )
		{
		}

		public SpPortalConfiguration(string baseUrl )
		{
			_baseUrl = baseUrl;
		}

		public string BaseUrl
		{
			get { return _baseUrl; }
		}
	}
}