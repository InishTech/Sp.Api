namespace Sp.Api.Shared.Wrappers
{
	using RestSharp;
	using System;
	using System.Collections.Generic;

	public class SpDefineApi : SpApi
	{
		public SpDefineApi( SpApiConfiguration apiConfiguration )
			: base( apiConfiguration )
		{
		}

		internal IRestResponse<ProductsPage> GetProductList()
		{
			var request = new RestRequest( ApiPrefix.Define + "/Product" );
			return Execute<ProductsPage>( request );
		}

		internal IRestResponse<Product> GetProduct( Uri productHref )
		{
			var request = new RestRequest( productHref );
			return Execute<Product>( request );
		}

		public IRestResponse Put( SpDefineApi.Product product )
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

	public class SelfLink
	{
		public Link self { get; set; }

		public class Link
		{
			public string href { get; set; }
		}
	}
}
