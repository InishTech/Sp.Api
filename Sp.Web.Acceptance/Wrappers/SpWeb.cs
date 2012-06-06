using RestSharp;
using Sp.Web.Acceptance.Helpers;
using System;

namespace Sp.Web.Acceptance.Wrappers
{
	public class SpWeb
	{
		readonly string _baseUrl;
		readonly RestClient _client;

		public SpWeb( SpWebConfiguration apiConfiguration )
		{
			_baseUrl = apiConfiguration.BaseUrl;
			_client = new RestClient { BaseUrl = _baseUrl };	
		}

		public IRestResponse<T> Execute<T>( RestRequest request ) where T : new()
		{
			request.Resource = MakeUriRelativeToRestSharpClientBaseUri( request.Resource ).ToString();
			return _client.Execute<T>( request );
		}

		public IRestResponse Execute( RestRequest request )
		{
			request.Resource = MakeUriRelativeToRestSharpClientBaseUri( request.Resource ).ToString();
			return _client.Execute( request );
		}

		// Required if your BaseUri includes a path (e.g., within InishTech test environments, instances are not always at / on a machine)
		Uri MakeUriRelativeToRestSharpClientBaseUri( string resource )
		{
			return UriHelper.MakeUriRelativeToBase( ClientBaseUri, resource );
		}

		string ClientBaseUri
		{
			get { return _client.BaseUrl; }
		}	
	}
}