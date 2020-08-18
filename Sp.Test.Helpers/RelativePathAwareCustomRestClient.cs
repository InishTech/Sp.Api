/* Copyright (c) 2017 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Test.Helpers
{
	using System;
	using RestSharp;

	public class RelativePathAwareCustomRestClient : RestClient
	{
		public RelativePathAwareCustomRestClient( string baseUrl )
		{
			BaseUrl = new Uri(baseUrl);
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
			return UriHelper.MakeUriRelativeToBase( BaseUrl.ToString(), resource );
		}
	}
}