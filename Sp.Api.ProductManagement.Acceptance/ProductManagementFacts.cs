using Sp.Api.ProductManagement.Acceptance.Wrappers;
using Xunit;
using Xunit.Extensions;

namespace Sp.Api.ProductManagement.Acceptance
{
	using System;
	using System.Net;
	using Ploeh.AutoFixture;

	public static class ProductManagementFacts
	{
		[Theory, AutoSoftwarePotentialData]
		public static void ProductList( SpProductManagementApi api )
		{
			var apiResult = api.GetProductList();
			Assert.NotNull( apiResult );
			Assert.NotEmpty( apiResult.Products );
		}

		[Theory, AutoSoftwarePotentialData]
		public static void GetExistingProduct( SpProductManagementApi api )
		{
			//TODO - remove hardcoding of wellKnownProductId; load product list and select a random item
			Guid wellKnownProductId = new Guid( "49cbc9f6-ef16-4c49-af20-496eabbbd92b" );
			var apiResult = api.GetProduct( wellKnownProductId );
			Assert.NotNull( apiResult.Data );
			Assert.Equal( "AidansMed", apiResult.Data.Label );
		}

		[Theory, AutoSoftwarePotentialData]
		public static void GetNonExistingProduct( SpProductManagementApi api )
		{
			var fixture = new Fixture();
			Guid anonymousProductId = fixture.CreateAnonymous<Guid>();
			var apiResult = api.GetProduct( anonymousProductId );
			Assert.NotNull( apiResult );
			Assert.Equal( HttpStatusCode.NotFound, apiResult.StatusCode );
		}
	}
}