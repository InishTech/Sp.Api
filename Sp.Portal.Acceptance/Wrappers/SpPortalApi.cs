using System.Net;
using RestSharp;

namespace Sp.Portal.Acceptance.Wrappers
{
	class SpPortalApi
	{
		readonly string _baseUrl;
		readonly CookieContainer _cookieContainer;
		readonly RestClient _client;

		public SpPortalApi( SpPortalConfiguration apiConfiguration )
		{
			_baseUrl = apiConfiguration.BaseUrl;
			_cookieContainer = new CookieContainer();
			_client = new RestClient { BaseUrl = _baseUrl, CookieContainer = _cookieContainer };
		}
	}
}
