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
			// TODO: this line goes when RestSharp has an IAuthenticator registered
			api.ExecuteLogin();
			Assert.NotEmpty( api.GetProductList().Products );
		}
	}
}