﻿/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
using System.Collections.Generic;

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
	using Ploeh.SemanticComparison.Fluent;
	using Sp.Api.Customer.Acceptance;
	using System.Globalization;

	public static class LicenseFacts
	{
		public static class Collection
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
				var licenseData = apiResult.Data.results;
				Assert.NotNull( licenseData );
			}

			[Theory, AutoSoftwarePotentialApiData]
			public static void GetListShouldYieldDataWithEmbeddedCustomer( SpIssueApi api )
			{
				var apiResult = api.GetLicenseList( "$expand=Customer" );

				// It should always be possible to get the list
				Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
				// If the request is OK, there should always be some Data
				Assert.NotNull( apiResult.Data );
				// never null, may be empty
				var licenseData = apiResult.Data.results;

				Assert.NotNull( licenseData );

				// TODO: Ruben, how are we going to test this properly
				Assert.True( licenseData.All( x => x._embedded != null ) ); ; // every  returned entry has an _embedded field
				Assert.True( licenseData.Any( x => x._embedded.Customer != null ) );
				Assert.True( licenseData.Any( x => x._embedded.Customer == null ) );
			}
			
			[Theory, AutoSoftwarePotentialApiData]
			public static void EmbeddedCustomersShouldHaveLinks( SpIssueApi api )
			{
				var apiResult = api.GetLicenseList( "$expand=Customer" );

				// It should always be possible to get the list
				Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
				// If the request is OK, there should always be some Data
				Assert.NotNull( apiResult.Data );
				// never null, may be empty
				var licenseData = apiResult.Data.results;

				Assert.NotNull( licenseData );

				// TODO: Ruben, how are we going to test this properly
				Assert.True( licenseData.All( x => x._embedded != null ) ); ; // every  returned entry has an _embedded field
				Assert.True( licenseData.Any( x => x._embedded.Customer != null ) );
				Assert.True( licenseData.Any( x => x._embedded.Customer == null ) );
			}

			[Theory, AutoSoftwarePotentialApiData]
			public static void GetListShouldBePageable( SpIssueApi api )
			{
				var apiResult = api.GetLicenseList( "$skip=1&$top=1" );

				// It should always be possible to get the list
				Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
				// If the request is OK, there should always be some Data
				Assert.NotNull( apiResult.Data );

				// never null, may be empty
				var licenseData = apiResult.Data.results;

				Assert.NotNull( licenseData );
				// TODO: Ruben, how are we going to test this properly
				Assert.Equal( 1, licenseData.Count );
			}

			[Theory, AutoSoftwarePotentialApiData]
			public static void GetListShouldBeSortable( SpIssueApi api )
			{
				var apiResult = api.GetLicenseList( "$orderby=IssueDate desc" );

				// It should always be possible to get the list
				Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
				// If the request is OK, there should always be some Data
				Assert.NotNull( apiResult.Data );

				// never null, may be empty
				var licenseData = apiResult.Data.results;
				Assert.NotNull( licenseData );

				var resorted = licenseData.OrderByDescending( x => DateTime.Parse(x.IssueDate, CultureInfo.InvariantCulture) ).ToArray();

				Assert.True( Enumerable.SequenceEqual( resorted, licenseData ) );
				//Assert.Equal<IEnumerable<SpIssueApi.License>>( resorted, licenseData);
			}

			[Theory, AutoSoftwarePotentialApiData]
			public static void GetListShouldRejectQueriesWithSelect( SpIssueApi api )
			{
				var apiResult = api.GetLicenseList( "$select=ActivationKey" );

				// It should always be possible to get the list
				Assert.Equal( HttpStatusCode.BadRequest, apiResult.StatusCode );
				Assert.Equal( "Unsupported Parameter: $select", apiResult.StatusDescription );
			}

			[Theory, AutoSoftwarePotentialApiData]
			public static void GetListShouldRejectQueriesWithUnsupportedOrderFields( SpIssueApi api )
			{
				var apiResult = api.GetLicenseList( "$orderby=ProductLabel" );

				// It should always be possible to get the list
				Assert.Equal( HttpStatusCode.BadRequest, apiResult.StatusCode );
				Assert.Contains( "The field OrderBy must match", apiResult.StatusDescription );
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
					Assert.NotEqual( default( string ), license.Selected.IssueDate );
				}

				[Theory, AutoSoftwarePotentialApiData]
				public static void ShouldHaveAWellFormedSelfLink( RandomLicenseFromListFixture item )
				{
					VerifyLinkWellFormed( item, links => links.self );
				}

				[Theory, AutoSoftwarePotentialApiData]
				public static void ShouldHaveAWellFormedCustomerAssignmentLink( RandomLicenseFromListFixture item )
				{
					VerifyLinkWellFormed( item, links => links.customerAssignment );
				}

				static void VerifyLinkWellFormed( RandomLicenseFromListFixture item, Func<SpIssueApi.License.Links, SpIssueApi.License.Link> linkSelector )
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

		public static class Item
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
				var linkedAddress = preSelectedLicense.Selected._links.self.href;
				//Now query the API for that specific license by following the link obtained in the previous step
				var apiResult = api.GetLicense( linkedAddress );
				Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );

				//The license obtained as a separete resource should be identical to the license previously selected from the list
				apiResult.Data.AsSource().OfLikeness<SpIssueApi.License>()
					.Without( p => p._links )
					.Without( p => p._embedded )
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
				var invalidHref = UriHelper.HackLinkReplacingGuidWithAlternateValue( anonymousId, validHref );
				var apiResult = api.GetLicense( invalidHref );
				// We don't want to have landed on an error page that has a StatusCode of 200
				Assert.Equal( HttpStatusCode.NotFound, apiResult.StatusCode );
				// Our final Location should match what we asked for
				Assert.Contains( invalidHref, apiResult.ResponseUri.ToString() );
			}
		}

		public static class CustomerAssignment
		{
			public static class Put
			{
				[Theory, AutoSoftwarePotentialApiData]
				public static void ShouldUpdateCustomerLink( [Frozen] SpIssueApi api, RandomLicenseFromListFixture license,
															 RandomCustomerFromListFixture customer )
				{
					var licenseCustomerAssignmentHref = license.Selected._links.customerAssignment.href;
					var apiResult = api.PutLicenseCustomerAssignment( licenseCustomerAssignmentHref, customer.Selected );
					Assert.Equal( HttpStatusCode.Accepted, apiResult.StatusCode );
					Verify.EventuallyWithBackOff( () =>
					{
						var updated = api.GetLicense( license.Selected._links.self.href );
						Assert.Equal( HttpStatusCode.OK, updated.StatusCode );
						Assert.NotNull( updated.Data._links.customer );
						var customerSelfLink = customer.Selected._links.self;
						Assert.Equal( customerSelfLink.href, updated.Data._links.customer.href );
					} );
				}
			}

			public static class Delete
			{
				[Theory, AutoSoftwarePotentialApiData]
				public static void ShouldResetCustomerLink(
					[Frozen] SpIssueApi api,
					RandomLicenseFromListFixture license,
					RandomCustomerFromListFixture customer )
				{
					//Assign a customer first
					// (This is not strictly necessary, we just want to observe a real user actually seeing the change take place in the License list itself
					Put.ShouldUpdateCustomerLink( api, license, customer );

					//Unassign the customer
					var licenseCustomerAssignmentHref = license.Selected._links.customerAssignment.href;
					var apiResult = api.DeleteLicenseCustomerAssignment( licenseCustomerAssignmentHref );
					Assert.Contains( apiResult.StatusCode, new[] { HttpStatusCode.NoContent, HttpStatusCode.NotFound } ); // TODO assert should be reversed
					Verify.EventuallyWithBackOff( () =>
					{
						var updated = api.GetLicense( license.Selected._links.self.href );
						Assert.Equal( HttpStatusCode.OK, updated.StatusCode );
						Assert.Null( updated.Data._links.customer );
					} );
				}
			}
		}

		// TODO when we support creating licenses via the REST API, this fixture should create one on the fly
		public class RandomLicenseFromListFixture
		{
			readonly SpIssueApi.License _randomItem;

			public RandomLicenseFromListFixture( SpIssueApi api )
			{
				var apiResult = api.GetLicenseList();
				Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
				Assert.True( apiResult.Data.results.Any(), GetType().Name + " requires the target login to have at least one License" );
				_randomItem = apiResult.Data.results.ElementAtRandom();
			}

			public SpIssueApi.License Selected
			{
				get { return _randomItem; }
			}
		}
	}
}