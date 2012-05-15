using RestSharp;
using System.Collections.Generic;

namespace Sp.Api.ProductManagement.Acceptance.Wrappers
{
	using System;

	public class SpProductManagementApi : SpApi
	{
		public SpProductManagementApi( SpApiConfiguration apiConfiguration )
			: base( apiConfiguration )
		{
		}

		internal ProductsPage GetProductList()
		{
			var request = new RestRequest( "ProductManagement/" );
			var result = Execute<ProductsPage>( request );
			return result.Data;
		}

		internal IRestResponse<Product> GetProduct( Guid id )
		{
			var request = new RestRequest( "ProductManagement/Product/" + id );
			var result = Execute<Product>( request );
			return result;
		}

		internal IRestResponse<Product> GetProduct( Uri productHref )
		{
			var request = new RestRequest( MakeUriRelativeToRestSharpClientBaseUri( productHref ) );
			var result = Execute<Product>( request );
			return result;
		}

		Uri MakeUriRelativeToRestSharpClientBaseUri( Uri uri )
		{
			Uri clientBaseUriEndingWithSlash = ClientBaseUri.EndsWith( "/" )
				? new Uri( ClientBaseUri )
				: new Uri( ClientBaseUri + "/" );
			var requestUriAbsolute = new Uri( clientBaseUriEndingWithSlash, uri );
			var uriRelativeToClientBaseUri = clientBaseUriEndingWithSlash.MakeRelativeUri( requestUriAbsolute );
			return uriRelativeToClientBaseUri;
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
			}
		}
	}
}