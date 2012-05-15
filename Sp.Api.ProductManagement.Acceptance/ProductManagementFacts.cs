using System;
using System.Net;
using Ploeh.AutoFixture;
using Sp.Api.ProductManagement.Acceptance.Helpers;
using Sp.Api.ProductManagement.Acceptance.Wrappers;
using Xunit;
using Xunit.Extensions;

namespace Sp.Api.ProductManagement.Acceptance
{
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
			Uri existingProductHref = GetRandomProductHref( api );

			var apiResult = api.GetProduct( existingProductHref );
			Assert.NotNull( apiResult.Data );
			Assert.NotEmpty( apiResult.Data.ReferenceId );
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

		static Uri GetRandomProductHref( SpProductManagementApi api )
		{
			var apiResult = api.GetProductList();
			if ( apiResult.Products == null )
				throw new InvalidOperationException( "No products found" );
			var randomProduct = apiResult.Products.ElementAtRandom();
			Console.WriteLine( randomProduct ); 
			return new Uri(randomProduct._links.self.href, UriKind.Relative );
		}
	}
}