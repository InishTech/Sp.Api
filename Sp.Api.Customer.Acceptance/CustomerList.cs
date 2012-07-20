namespace Sp.Api.Customer.Acceptance
{
	using Sp.Api.Shared;
	using System.Net;
	using Xunit;
	using Xunit.Extensions;

	public static class CustomerList
	{
		[Theory, AutoSoftwarePotentialApiData]
		public static void ShouldAlwaysBeAvailable( SpCustomerApi api )
		{
			var response = api.GetList();
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
		}

		[Theory, AutoSoftwarePotentialApiData]
		public static void ShouldHaveAtLeastOneItem( SpCustomerApi api )
		{
			var response = api.GetList();
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
			Assert.NotEmpty( response.Data.Customers );
		}

		public static class Items
		{
			[Theory, AutoSoftwarePotentialApiData]
			public static void ShouldHaveAName( RandomCustomerFromListFixture item )
			{
				Assert.NotEmpty( item.Selected.Name );
			}

			[Theory, AutoSoftwarePotentialApiData]
			public static void ShouldHaveANonNullDescription( RandomCustomerFromListFixture item )
			{
				Assert.NotNull( item.Selected.Description );
			}

			// For now, it's always false; When this becomes variable, this pinning test will be replaced with other more appropriate ones
			[Theory, AutoSoftwarePotentialApiData]
			public static void ShouldHaveAnIsRegisteredField( RandomCustomerFromListFixture item )
			{
				Assert.False( item.Selected.IsRegistered );
			}
		}
	}
}