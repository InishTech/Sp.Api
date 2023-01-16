/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Api.Acceptance
{
    using System;
    using System.Net;
	using RestSharp;
	using Sp.Api.Shared;
	using Sp.Api.Shared.Wrappers;
	using Xunit;
	using Xunit.Extensions;

	public static class RootLinksFacts
	{
		[Theory, AutoSoftwarePotentialApiData]
		public static void ShouldAlwaysBeAvailable( SpRootsApi api )
		{
			var response = api.GetRootList();
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
		}
		[Theory, AutoSoftwarePotentialApiData]
		public static void ShouldContainAtLeastOneResult( SpRootsApi api )
		{
			var response = api.GetRootList();
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
			Assert.NotEmpty( response.Data.results );
		}

		[Theory, AutoSoftwarePotentialApiData]
		public static void ShouldContainLinksToKnownEndpoints( SpRootsApi api )
		{
			var response = api.GetRootList();
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
			Assert.NotNull( response.Data.results[ 0 ]._links[ "Auth" ] );
			Assert.NotNull( response.Data.results[ 0 ]._links[ "Consume" ] );
			Assert.NotNull( response.Data.results[ 0 ]._links[ "ConsumeApi" ] );
			Assert.NotNull( response.Data.results[ 0 ]._links[ "Issue" ] );
			Assert.NotNull( response.Data.results[ 0 ]._links[ "IssueApi" ] );
			Assert.NotNull( response.Data.results[ 0 ]._links[ "Develop" ] );
			Assert.NotNull( response.Data.results[ 0 ]._links[ "DevelopApi" ] );
			Assert.NotNull( response.Data.results[ 0 ]._links[ "Pay" ] );
			Assert.NotNull( response.Data.results[ 0 ]._links[ "Analyze" ] );
		}
	}
}