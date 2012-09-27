/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Portal.Acceptance
{
	using Sp.Portal.Acceptance.Wrappers;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using Xunit;
	using Xunit.Extensions;
	using RestSharp;

	public static class Licenses
	{
		public static class IndexGet
		{
			[Theory, AutoPortalDataAttribute]
			public static void ShouldYieldResults( SpPortalApi api )
			{
				var apiResult = api.GetLicenses();

				VerifyResponse( apiResult );
			}

			[Theory, AutoPortalDataAttribute]
			public static void ShouldAllowPaging( SpPortalApi api )
			{
				var firstQuery = api.GetLicenses( "$skip=0&$top=1" );
				var secondQuery = api.GetLicenses( "$skip=1&$top=1" );

				SpPortalApi.License first = VerifyResponse( firstQuery ).Single();
				SpPortalApi.License second = VerifyResponse( secondQuery ).Single();

				Assert.NotNull( first );
				Assert.NotNull( second );
				Assert.NotEqual( first._links.self, second._links.self );
			}

			[Theory, AutoPortalDataAttribute]
			public static void ShouldRespondToUnsupportedQueriesWithBadRequestAndStatusDescription( SpPortalApi api )
			{
				var expectedFailures = new Dictionary<string, string>()
				{
				    {"$top=101","The field Top must be between 1 and 100."},
				    {"$top=NaN","The field Top must be between 1 and 100."},
				    {"$top=","The Top field is required."},         // a specified query field must be provided
				    {"$orderby=","The OrderBy field is required."}  // a specified query field must be provided
				};

				foreach ( var expectedFailure in expectedFailures )
				{
					var invalidQueryResult = api.GetLicenses( expectedFailure.Key );
					Assert.Equal( HttpStatusCode.BadRequest, invalidQueryResult.StatusCode );
					Assert.Equal( expectedFailure.Value, invalidQueryResult.StatusDescription );
				}
			}

			static SpPortalApi.License[] VerifyResponse( IRestResponse<SpPortalApi.LicenseCollection> apiResult )
			{
				// It should always be possible to get the list
				Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
				// If the request is OK, there should always be some Data
				Assert.NotNull( apiResult.Data );
				//-- Portal consumer expects Issue date, activation key, product, version, eval, renewal
				// An empty list is always represented as an empty collection, not null
				Assert.NotNull( apiResult.Data.Licenses );

				return apiResult.Data.Licenses.ToArray();
			}
		}

		public static class Get
		{
			[Theory, AutoPortalDataAttribute]
			public static void ShouldContainData( RandomLicenseFromListFixture license )
			{
				// There should always be valid Activation Key
				Assert.NotEmpty( license.Selected.ActivationKey );
				// There should always be a Product Label
				Assert.NotEmpty( license.Selected.ProductLabel );
				// There should always be a Version Label
				Assert.NotEmpty( license.Selected.VersionLabel );
				// There is always an IssueDate
				Assert.NotEmpty( license.Selected.IssueDate );
				// There are always embedded elements (even if no explicit $expand is specified)
				Assert.NotNull( license.Selected._embedded.Tags );
				// There are always tags - they might be empty though
				Assert.NotNull( license.Selected._embedded.Tags );
			}

			[Theory, AutoPortalDataAttribute]
			public static void ShouldIncludeSelfLink( RandomLicenseFromListFixture license )
			{
				Assert.NotNull( license.Selected._links.self );
				Assert.NotEmpty( license.Selected._links.self.href );
			}

			[Theory, AutoPortalDataAttribute]
			public static void ShouldIncludeLicenseTagsAssignmentLink( RandomLicenseFromListFixture license )
			{
				Assert.NotNull( license.Selected._links.tags );
				Assert.NotEmpty( license.Selected._links.tags.href );
			}
		}
	}
}