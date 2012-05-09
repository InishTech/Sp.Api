using RestSharp;
using System.Collections.Generic;

namespace Sp.Api.ProductManagement.Acceptance.Wrappers
{
	public class SpProductManagementApi : SpApi
	{
		public SpProductManagementApi( SpApiConfiguration apiConfiguration )
			: base( apiConfiguration )
		{
		}

		internal ProductsPage GetProductList()
		{
			var request = new RestRequest( "ProductManagement" );
			var result = Execute<ProductsPage>( request );
			return result.Data;
		}

		public class ProductsPage
		{
			public List<Product> Products { get; set; }
		}

		public class Product
		{
			public string ReferenceId { get; set; }
			public string Label { get; set; }
			public string Description { get; set; }
		}

		//internal void AccountManagement()
		//{
		//	var request = new RestRequest( "AccountManagement.aspx" );
		//	var result = Execute<object>( request );
		//	Console.WriteLine( "Result:" + result.Content );
		//}
	}
}