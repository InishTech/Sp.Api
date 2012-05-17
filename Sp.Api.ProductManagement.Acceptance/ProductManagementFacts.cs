﻿using Ploeh.AutoFixture.Xunit;
using Sp.Api.ProductManagement.Acceptance.Helpers;
using Sp.Api.ProductManagement.Acceptance.Wrappers;
using System;
using System.Linq;
using System.Net;
using Xunit;
using Xunit.Extensions;

namespace Sp.Api.ProductManagement.Acceptance
{
	public class ProductManagementFacts
	{
		/// <summary>
		/// The master product list yields a JSON response object which contains the Product List collection in a 'Products' value.
		/// </summary>
		/// <remarks>
		/// There are no standard failure conditions for this request - even an empty list returns a well formed result, just with the 'Products' collection empty.
		/// </remarks>
		/// <param name="api">Api wrapper.</param>
		[Theory, AutoSoftwarePotentialData]
		public static void GetProductListShouldYieldData( SpProductManagementApi api )
		{
			var apiResult = api.GetProductList();
			// It should always be possible to get the list
			Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
			// If the request is OK, there should always be some Data
			Assert.NotNull( apiResult.Data );
			// An empty product list is always represented as an empty collection, not null
			Assert.NotNull( apiResult.Data.Products );
		}

		/// <summary>
		/// The master list presents a set of linked child entities. Here we select an arbitrary one from the list and follow its _links.self to get theat resource's data.
		/// </summary>
		/// <remarks>
		/// Success/failure is communicated by the HTTP Status Code being OK		
		/// </remarks>
		/// <param name="api">Api wrapper. [Frozen] so requests involved in getting <paramref name="product"/> can share the authentication work.</param>
		/// <param name="product">Arbitrarily chosen product from the configured user's list (the account needs at least one)</param>
		[Theory, AutoSoftwarePotentialData]
		public static void GetProductFromListShouldYieldData( [Frozen] SpProductManagementApi api, RandomProductFromListFixture product )
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
		public static void GetNonExistingProductShould404( [Frozen] SpProductManagementApi api, RandomProductFromListFixture product, Guid anonymousId )
		{
			string validHref = product.SelectedProduct._links.self.href;
			Uri invalidHref = HackLinkReplacingGuidWithAlternateValue( anonymousId, validHref );
			var apiResult = api.GetProduct( invalidHref );
			Assert.Equal( HttpStatusCode.NotFound, apiResult.StatusCode );
			// Our final Location should match what we asked for (i.e., we don't want to have landed on an error page that has a StatusCode of 200)
			Assert.Contains( invalidHref.ToString(), apiResult.ResponseUri.ToString() );
			// No data should be yielded
			Assert.Null( apiResult.Data );
		}

		/// <summary>
		/// This is purely for the purposes of this low-level test.
		/// Client side should never need to generate or mess with links - the Apis are intended to communicate via standard HAL hypermedia constructs in the _links object. 
		/// </summary>
		/// <returns></returns>
		static Uri HackLinkReplacingGuidWithAlternateValue( Guid replacement, string validHref )
		{
			return new Uri( validHref.Substring( validHref.LastIndexOf( '/' ) ) + replacement.ToString(), UriKind.Relative );
		}

		/// <summary>
		/// Malformed requests (as opposed to items that are Not Found or have Gone) generally yield Bad Request responses. If one sticks to Hypermedia _links, one should not normally encounter these
		/// </summary>
		// Right now this causes an Exception which, instead of the OOTB HTTP 500 instead redirects to ErrorPage.aspx which gives 200. This will be fixed as the intention is that all APIs communicate success/failure info to JSON speakers using Http Status Codes, not HTML error pages
		[Theory, AutoSoftwarePotentialData]
		public static void GetNonGuidProductShould400( [Frozen] SpProductManagementApi api, RandomProductFromListFixture product )
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

		public class RandomProductFromListFixture
		{
			readonly SpProductManagementApi.Product _randomProduct;

			public RandomProductFromListFixture( SpProductManagementApi api )
			{
				var apiResult = api.GetProductList();
				Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
				Assert.True( apiResult.Data.Products.Any(), "RandomProductFromListFixture requires the target login to have at least one Product" );
				_randomProduct = apiResult.Data.Products.ElementAtRandom();
			}

			public SpProductManagementApi.Product SelectedProduct
			{
				get { return _randomProduct; }
			}
		}
	}
}