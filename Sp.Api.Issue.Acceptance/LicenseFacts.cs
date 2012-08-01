/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */

using Ploeh.SemanticComparison.Fluent;
using Sp.Api.Customer.Acceptance;

namespace Sp.Api.Issue.Acceptance
{
	using System;
	using System.Linq;
	using System.Net;
	using Ploeh.AutoFixture.Xunit;
	using Sp.Api.Shared;
	using Sp.Test.Helpers;
	using Xunit;
	using Xunit.Extensions;

	public static class LicenseFacts
	{
		public static class GetCollection
		{
			/// <summary>
			/// The master license list yields a JSON response object which contains the License List collection in a 'Licenses' value.
			/// </summary>
			/// <remarks>
			/// There are no standard failure conditions for this request - even an empty list returns a well formed result, just with the 'Licenses' collection empty.
			/// </remarks>
			/// <param name="api">Api wrapper.</param>
			[Theory, AutoSoftwarePotentialApiData]
			public static void GetListShouldYieldData( SpIssueApi api )
			{
				var apiResult = api.GetLicenseList();
				// It should always be possible to get the list
				Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
				// If the request is OK, there should always be some Data
				Assert.NotNull( apiResult.Data );
				//-- Portal consumer expects Issue date, activation key, product, version, eval, renewal
				// An empty list is always represented as an empty collection, not null
				Assert.NotNull( apiResult.Data.Licenses );
			}

			public static class ElementFromList
			{
				/// <summary>
				/// The master collection provides a set of summary records. Here we select an arbitrary one from the list and make assertions as to its format.
				/// </summary>
				/// <remarks>
				/// Success/failure is communicated by the HTTP Status Code being OK		
				/// </remarks>
				/// <param name="license">Arbitrarily chosen license from the configured user's list</param>
				[Theory, AutoSoftwarePotentialApiData]
				public static void ShouldContainData( RandomLicenseFromListFixture license )
				{
					// There should always be valid Activation Key
					Assert.NotEmpty( license.Selected.ActivationKey );
					// There should always be a Product Label
					Assert.NotEmpty( license.Selected.ProductLabel );
					// There should always be a Version Label
					Assert.NotEmpty( license.Selected.VersionLabel );
					// There is always an IssueDate
					Assert.NotEqual( default( DateTime ), license.Selected.IssueDate );
				}

				[Theory, AutoSoftwarePotentialApiData]
				public static void ShouldHaveAWellFormedSelfLink( RandomLicenseFromListFixture item )
				{
					VerifyLinkWellFormed( item, links => links.self );
				}

				[Theory, AutoSoftwarePotentialApiData]
				public static void ShouldHaveAValidCustomerAssignmentLink( RandomLicenseFromListFixture item )
				{
					VerifyLinkWellFormed( item, links => links.customerAssignment );
				}

				static void VerifyLinkWellFormed( RandomLicenseFromListFixture item, Func<SpIssueApi.LicenseSummary.Links, SpIssueApi.LicenseSummary.Link> linkSelector )
				{
					var linksSet = item.Selected._links;
					Assert.NotNull( linksSet );
					var linkToVerify = linkSelector( linksSet );
					Assert.NotNull( linkToVerify );
					Assert.NotEmpty( linkToVerify.href );
				}

				[Theory( Skip = "Fields to be exercised by future License creation+migration examples" ), AutoSoftwarePotentialApiData]
				public static void ShouldContainData_UntestedProperties( [Frozen] SpIssueApi api, RandomLicenseFromListFixture license )
				{
					// TODO these are here so the properties are referenced. TODO: Add roundtrip test which verifies that true and false values can propagate
					// There is always a flag indicating the evaluation status
					var dummy = license.Selected.IsEvaluation;
					// TODO these are here so the properties are referenced. TODO: Add roundtrip test which verifies that true and false values can propagate
					// There is always a flag indicating the evaluation status
					var dummy2 = license.Selected.IsRenewable;
					// For a later test
					var dummy3 = license.Selected._links.customer;
				}
			}
		}

		public static class GetItem
		{
			/// <summary>
			/// /// The master list presents a set of linked child entities. Here we select an arbitrary one from the list and follow its _links.self to get that resource's 			/// </summary>
			/// <remarks>
			/// Success/failure is communicated by the HTTP Status Code being OK		
			/// </remarks>
			/// <param name="api">Api wrapper. [Frozen] so requests involved in getting <paramref name="preSelectedLicense"/> can share the authentication work.</param>
			/// <param name="preSelectedLicense">Arbitrarily chosen license from the configured user's list</param>
			[Theory, AutoSoftwarePotentialApiData]
			public static void GetLicenseShouldContainData( [Frozen] SpIssueApi api, RandomLicenseFromListFixture preSelectedLicense )
			{
				var linkedAddress = preSelectedLicense.Selected._links.self.AsRelativeUri();
				//Now query the API for that specific license by following the link obtained in the previous step
				var apiResult = api.GetLicense( linkedAddress );
				Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );

				//The license obtained as a separete resource should be identical to the license previously selected from the list
				apiResult.Data.AsSource().OfLikeness<SpIssueApi.LicenseSummary>()
					.Without(p => p._links)
					.ShouldEqual( preSelectedLicense.Selected );
			}

			/// <summary>
			/// Normal usage should just involve following _links as illustrated in GetProductFromListShouldYieldData. Here we simulate what would happen if the item [had not yet become accessible|had been deleted]
			/// </summary>
			/// <param name="api">Api wrapper. [Frozen] so requests involved in getting <paramref name="license"/> can share the authentication work.</param>
			/// <param name="license">Arbitrarily chosen license from the configured user's list</param>
			/// <param name="anonymousId">Random id</param>
			/// TODO - consider where to place low-level infrastructure tests
			[Theory, AutoSoftwarePotentialApiData]
			public static void GetNonExistingLicenseShould404( [Frozen] SpIssueApi api, RandomLicenseFromListFixture license, Guid anonymousId )
			{
				string validHref = license.Selected._links.self.href;
				Uri invalidHref = UriHelper.HackLinkReplacingGuidWithAlternateValue( anonymousId, validHref );
				var apiResult = api.GetLicense( invalidHref );
				// We don't want to have landed on an error page that has a StatusCode of 200
				Assert.Equal( HttpStatusCode.NotFound, apiResult.StatusCode );
				// Our final Location should match what we asked for
				Assert.Contains( invalidHref.ToString(), apiResult.ResponseUri.ToString() );
			}
		}

		public static class PutCustomer
		{
			[Theory, AutoSoftwarePotentialApiData]
			public static void PutCustomerAssignmentShouldUpdateCustomerLink( [Frozen] SpIssueApi api, RandomLicenseFromListFixture license, RandomCustomerFromListFixture customer )
			{
				var licenseCustomerAssignmentUrl = license.Selected._links.customerAssignment.AsRelativeUri();
				var apiResult = api.PutLicenseCustomerAssignment( licenseCustomerAssignmentUrl, customer.Selected );
				Assert.Equal( HttpStatusCode.Accepted, apiResult.StatusCode );
				Verify.EventuallyWithBackOff( () =>
				{
					var updated = api.GetLicense( license.Selected._links.self.AsRelativeUri() );
					Assert.Equal( HttpStatusCode.OK, updated.StatusCode );
					Assert.NotNull( updated.Data._links.customer );
					var updatedCustomerSelfLink = customer.Selected._links.self;
					Assert.Equal( updatedCustomerSelfLink.AsRelativeUri(), updated.Data._links.customer.AsRelativeUri() );
				} );
			}
		}

		// TODO when we support creating licenses via the REST API, this fixture should create one on the fly
		public class RandomLicenseFromListFixture
		{
			readonly SpIssueApi.LicenseSummary _randomItem;

			public RandomLicenseFromListFixture( SpIssueApi api )
			{
				var apiResult = api.GetLicenseList();
				Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
				Assert.True( apiResult.Data.Licenses.Any(), GetType().Name + " requires the target login to have at least one License" );
				_randomItem = apiResult.Data.Licenses.ElementAtRandom();
			}

			public SpIssueApi.LicenseSummary Selected
			{
				get { return _randomItem; }
			}
		}
	}
}