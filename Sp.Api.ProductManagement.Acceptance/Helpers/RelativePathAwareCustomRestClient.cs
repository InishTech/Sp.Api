namespace Sp.Api.ProductManagement.Acceptance.Helpers
{
	using System;
	using RestSharp;

	class RelativePathAwareCustomRestClient : RestClient
	{
		public RelativePathAwareCustomRestClient( string baseUrl )
		{
			BaseUrl = baseUrl;
		}

		public override IRestResponse<T> Execute<T>( IRestRequest request )
		{
			request.Resource = MakeUriRelativeToRestSharpClientBaseUri( request.Resource ).ToString();
			return base.Execute<T>( request );
		}

		public override IRestResponse Execute( IRestRequest request )
		{
			request.Resource = MakeUriRelativeToRestSharpClientBaseUri( request.Resource ).ToString();
			return base.Execute( request );
		}

		// Required if your BaseUri includes a path (e.g., within InishTech test environments, instances are not always at / on a machine)
		Uri MakeUriRelativeToRestSharpClientBaseUri( string resource )
		{
			return UriHelper.MakeUriRelativeToBase( BaseUrl, resource );
		}
	}
}