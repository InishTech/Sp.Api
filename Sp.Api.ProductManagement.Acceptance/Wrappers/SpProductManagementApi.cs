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
			return Execute<Product>( request );
		}

		public IRestResponse Put( SpProductManagementApi.Product product )
		{
			var request = new RestRequest( product._links.self.href, Method.PUT );
			request.RequestFormat = DataFormat.Json;
			request.AddBody( product );

			return Execute( request );
		}

		public class ProductsPage
		{
			public List<Product> Products { get; set; }
		}

		public class Product
		{
			public SelfLink _links { get; set; }
			public int _version { get; set; }

			public string ReferenceId { get; set; }
			public string Label { get; set; }
			public string Description { get; set; }
		}
	}
}
