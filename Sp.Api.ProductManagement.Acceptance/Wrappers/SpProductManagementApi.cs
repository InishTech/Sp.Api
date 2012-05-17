namespace Sp.Api.ProductManagement.Acceptance.Wrappers
{
	using RestSharp;
	using System;
	using System.Collections.Generic;

	public class SpProductManagementApi : SpApi
	{
		public SpProductManagementApi( SpApiConfiguration apiConfiguration )
			: base( apiConfiguration )
		{
		}

		internal IRestResponse<ProductsPage> GetProductList()
		{
			var request = new RestRequest( "ProductManagement/" );
			return Execute<ProductsPage>( request );
		}

		internal IRestResponse<Product> GetProduct( Uri productHref )
		{
			var request = new RestRequest( productHref );
			var result = Execute<Product>( request );
			return result;
		}

		public class ProductsPage
		{
			public List<Product> Products { get; set; }
		}

		public class Product
		{
			public SelfLink _links { get; set; }

			public string ReferenceId { get; set; }
			public string Label { get; set; }
			public string Description { get; set; }
		}

		public class SelfLink
		{
			public Link self { get; set; }

			public class Link
			{
				public string href { get; set; }

				public Uri AsRelativeUri()
				{
					return new Uri( href, UriKind.Relative );
				}
			}
		}
	}
}