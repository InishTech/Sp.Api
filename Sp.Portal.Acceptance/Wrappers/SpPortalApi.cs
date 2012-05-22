using System.Net;
using RestSharp;

namespace Sp.Portal.Acceptance.Wrappers
{
	public class SpPortalApi
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

		public IRestResponse<T> Execute<T>( RestRequest request ) where T : new()
		{
			return _client.Execute<T>( request );
		}

		public IRestResponse Execute( RestRequest request )
		{
			return _client.Execute( request );
		}
	}
}
