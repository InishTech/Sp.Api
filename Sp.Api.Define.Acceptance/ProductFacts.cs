/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Api.Define.Acceptance
{
	using Sp.Api.Shared;
	using Sp.Api.Shared.Wrappers;
	using Sp.Test.Helpers;
	using Ploeh.AutoFixture.Xunit;
	using RestSharp;
	using System;
	using System.Linq;
	using System.Net;
	using Xunit;
	using Xunit.Extensions;

	public class ProductFacts
	{
		public static class GetCollection
		{
			/// <summary>
			/// The master product list yields a JSON response object which contains the Product List collection in a 'Products' value.
			/// </summary>
			/// <remarks>
			/// There are no standard failure conditions for this request - even an empty list returns a well formed result, just with the 'Products' collection empty.
			/// </remarks>
			/// <param name="api">Api wrapper.</param>
			[Theory, AutoSoftwarePotentialData]
			public static void GetProductListShouldYieldData( SpDefineApi api )
			{
				var apiResult = api.GetProductList();
				// It should always be possible to get the list
				Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
				// If the request is OK, there should always be some Data
				Assert.NotNull( apiResult.Data );
				// An empty product list is always represented as an empty collection, not null
				Assert.NotNull( apiResult.Data.Products );
			}
		}

		public static class GetItem
		{
			/// <summary>
			/// The master list presents a set of linked child entities. Here we select an arbitrary one from the list and follow its _links.self to get that resource's data.
			/// </summary>
			/// <remarks>
			/// Success/failure is communicated by the HTTP Status Code being OK		
			/// </remarks>
			/// <param name="api">Api wrapper. [Frozen] so requests involved in getting <paramref name="product"/> can share the authentication work.</param>
			/// <param name="product">Arbitrarily chosen product from the configured user's list (the account needs at least one)</param>
			[Theory, AutoSoftwarePotentialData]
			public static void GetProductFromListShouldYieldData( [Frozen] SpDefineApi api, RandomProductFromListFixture product )
			{
				Uri linkedAddress = product.SelectedProduct._links.self.AsRelativeUri();
				var apiResult = api.GetProduct( linkedAddress );
				// It should always be possible to get the data
				Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
				// Our final Location should match what we asked for (i.e., we don't want to have landed on an error page that has a StatusCode of 200)
				Assert.Contains( linkedAddress.ToString(), apiResult.ResponseUri.ToString() );
				// There should always be valid Data
				Assert.NotNull( apiResult.Data );
				// There should always be a reference
				Assert.NotEmpty( apiResult.Data.ReferenceId );
				// There should always be a label
				Assert.NotEmpty( apiResult.Data.Label );
			}

			/// <summary>
			/// Normal usage should just involve following _links as illustrated in GetProductFromListShouldYieldData. Here we simulate what would happen if the item [had not yet become accessible|had been deleted]
			/// TODO illustrate this better by doing a DELETE
			/// </summary>
			/// <param name="api">Api wrapper. [Frozen] so requests involved in getting <paramref name="product"/> can share the authentication work.</param>
			/// <param name="product">Arbitrarily chosen product from the configured user's list (the account needs at least one)</param>
			[Theory, AutoSoftwarePotentialData]
			public static void GetNonExistingProductShould404( [Frozen] SpDefineApi api, RandomProductFromListFixture product, Guid anonymousId )
			{
				string validHref = product.SelectedProduct._links.self.href;
				Uri invalidHref = HackLinkReplacingGuidWithAlternateValue( anonymousId, validHref );
				var apiResult = api.GetProduct( invalidHref );
				// We don't want to have landed on an error page that has a StatusCode of 200
				Assert.Equal( HttpStatusCode.NotFound, apiResult.StatusCode );
				// Our final Location should match what we asked for
				Assert.Contains( invalidHref.ToString(), apiResult.ResponseUri.ToString() );
			}

			/// <summary>
			/// This is purely for the purposes of this low-level test.
			/// Client side should never need to generate or mess with links - the Apis are intended to communicate via standard HAL hypermedia constructs in the _links object. 
			/// </summary>
			/// <returns></returns>
			static Uri HackLinkReplacingGuidWithAlternateValue( Guid replacement, string validHref )
			{
				return new Uri( validHref.Substring( 0, validHref.LastIndexOf( '/' ) + 1 ) + replacement.ToString(), UriKind.Relative );
			}

			public static class BadRequests
			{
				/// <summary>
				/// Malformed requests (as opposed to items that are Not Found or have Gone) generally yield Bad Request responses. If one sticks to Hypermedia _links, one should not normally encounter these
				/// </summary>
				// Right now this causes an Exception which, instead of the OOTB HTTP 500 instead redirects to ErrorPage.aspx which gives 200. This will be fixed as the intention is that all APIs communicate success/failure info to JSON speakers using Http Status Codes, not HTML error pages
				[Theory( Skip = "TODO - at the moment this request returns HTTP 500" ), AutoSoftwarePotentialData]
				public static void GetNonGuidProductShould400( [Frozen] SpDefineApi api, RandomProductFromListFixture product )
				{
					Uri misformattedHackedUri = new Uri( product.SelectedProduct._links.self.href + "broken", UriKind.Relative );
					var apiResult = api.GetProduct( misformattedHackedUri );
					// TODO move this after the next assert when this one starts passing
					// Our final Location should match what we asked for (i.e., we don't want to have landed on an error page that has a StatusCode of 200)
					Assert.Contains( misformattedHackedUri.ToString(), apiResult.ResponseUri.ToString() );
					// We should know there's a problem
					Assert.Equal( HttpStatusCode.BadRequest, apiResult.StatusCode );
					// No data should be yielded
					Assert.Null( apiResult.Data );
				}
			}
		}

		/// <summary>
		/// The master list presents a set of linked child entities. Here we select an arbitrary one from the list and follow its _links.self to get and then PUT an updated version of that resource's data.
		/// </summary>
		/// <remarks>
		/// <para>Success/failure is communicated by the HTTP Status Code being OK.</para>
		/// <para>Note that update may not be processed immediately hence usage of <c>Verify.EventuallyWithBackOff</c>.</para>
		/// </remarks>
		/// <param name="api">Api wrapper. [Frozen] so requests involved in getting <paramref name="product"/> can share the authentication work.</param>
		/// <param name="product">Arbitrarily chosen product from the configured user's list (the account needs at least one)</param>
		public class PutItem
		{
			[Theory, AutoSoftwarePotentialData]
			public static void PutProductFromListWithUpdatedDescriptionShouldRoundtrip( [Frozen] SpDefineApi api, RandomProductFromListFixture product, string updatedValue )
			{
				product.SelectedProduct.Description = updatedValue;

				Assert.Equal( HttpStatusCode.OK, product.PutSelectedProduct().StatusCode );

				Verify.EventuallyWithBackOff( () =>
					Assert.Equal( updatedValue, product.GetSelectedProductAgain().Description ) );
			}

			[Theory, AutoSoftwarePotentialData]
			public static void PutProductFromListWithUpdatedLabelShouldRoundtrip( [Frozen] SpDefineApi api, RandomProductFromListFixture product, string updatedValue )
			{
				product.SelectedProduct.Label = updatedValue;

				Assert.Equal( HttpStatusCode.OK, product.PutSelectedProduct().StatusCode );

				Verify.EventuallyWithBackOff( () =>
					Assert.Equal( updatedValue, product.GetSelectedProductAgain().Label ) );
			}

			/// <summary>
			/// The following are examples of values that will be rejected as invalid.
			/// - Description is Mandatory with a max length of 100 (empty is permitted).
			/// - Label is Mandatory with a Minimum Length of 1 and max of 100.
			/// </summary>
			public static class InvalidData
			{
				public static class Label
				{
					[Theory, AutoSoftwarePotentialData]
					public static void PutNullShouldReject( [Frozen] SpDefineApi api, RandomProductFromListFixture product )
					{
						product.SelectedProduct.Label = null;
						Assert.Equal( HttpStatusCode.BadRequest, product.PutSelectedProduct().StatusCode );
					}

					[Theory, AutoSoftwarePotentialData]
					public static void PutEmptyShouldReject( [Frozen] SpDefineApi api, RandomProductFromListFixture product )
					{
						product.SelectedProduct.Label = string.Empty;
						Assert.Equal( HttpStatusCode.BadRequest, product.PutSelectedProduct().StatusCode );
					}

					[Theory, AutoSoftwarePotentialData]
					public static void PutExcessivelyLongShouldReject( [Frozen] SpDefineApi api, RandomProductFromListFixture product )
					{
						product.SelectedProduct.Label = new String( 'a', 101 );
						Assert.Equal( HttpStatusCode.BadRequest, product.PutSelectedProduct().StatusCode );
					}
				}

				public static class Description
				{
					/// <summary>
					/// While the Description can be left Empty, one is not permitted to submit a null value.
					/// </summary>
					[Theory( Skip = "TP 1109" ), AutoSoftwarePotentialData]
					public static void PutNullDescriptionShouldReject( [Frozen] SpDefineApi api, RandomProductFromListFixture product )
					{
						product.SelectedProduct.Description = null;
						Assert.Equal( HttpStatusCode.BadRequest, product.PutSelectedProduct().StatusCode );
					}

					[Theory, AutoSoftwarePotentialData]
					public static void PutExcessivelyLongDescriptionShouldReject( [Frozen] SpDefineApi api, RandomProductFromListFixture product )
					{
						product.SelectedProduct.Description = new String( 'a', 101 );
						Assert.Equal( HttpStatusCode.BadRequest, product.PutSelectedProduct().StatusCode );
					}
				}
			}
		}

		public class RandomProductFromListFixture
		{
			readonly SpDefineApi.Product _randomProduct;
			readonly SpDefineApi _api;

			public RandomProductFromListFixture( SpDefineApi api )
			{
				_api = api;
				var apiResult = api.GetProductList();
				Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
				Assert.True( apiResult.Data.Products.Any(), "RandomProductFromListFixture requires the target login to have at least one Product" );
				_randomProduct = apiResult.Data.Products.ElementAtRandom();
			}

			public SpDefineApi.Product SelectedProduct
			{
				get { return _randomProduct; }
			}

			public IRestResponse PutSelectedProduct()
			{
				return _api.Put( SelectedProduct );
			}

			public SpDefineApi.Product GetSelectedProductAgain()
			{
				Uri linkedAddress = SelectedProduct._links.self.AsRelativeUri();
				var apiResult = _api.GetProduct( linkedAddress );
				Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
				return apiResult.Data;
			}
		}
	}
}