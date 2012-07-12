namespace Sp.Api.Customer.Acceptance
{
	using Sp.Api.Shared;
	using Sp.Test.Helpers;
	using System.Linq;
	using System.Net;
	using Xunit;
	using Xunit.Extensions;

	public static class CustomerList
	{
		[Theory, AutoSoftwarePotentialData]
		public static void ShouldAlwaysBeAvailable( SpCustomerApi api )
		{
			var response = api.GetList();
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
		}

		[Theory, AutoSoftwarePotentialData]
		public static void ShouldHaveAtLeastOneItem( SpCustomerApi api )
		{
			var response = api.GetList();
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
			Assert.NotEmpty( response.Data.Customers );
		}

		public static class Items
		{
			[Theory, AutoSoftwarePotentialData]
			public static void ShouldHaveAName( RandomCustomerFromListFixture item )
			{
				Assert.NotEmpty( item.Selected.Name );
			}

			[Theory, AutoSoftwarePotentialData]
			public static void ShouldHaveANonNullDescription( RandomCustomerFromListFixture item )
			{
				Assert.NotNull( item.Selected.Description );
			}

			// For now, it's always false; When this becomes variable, this pinning test will be replaced with other more appropriate ones
			[Theory, AutoSoftwarePotentialData]
			public static void ShouldHaveAnIsRegisteredField( RandomCustomerFromListFixture item )
			{
				Assert.False( item.Selected.IsRegistered );
			}

			public class RandomCustomerFromListFixture
			{
				readonly SpCustomerApi.CustomerSummary _randomItem;

				public RandomCustomerFromListFixture( SpCustomerApi api )
				{
					var apiResult = api.GetList();
					Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
					Assert.True( apiResult.Data.Customers.Any(), GetType().Name + " requires the target login to have at least one Customer" );
					_randomItem = apiResult.Data.Customers.ElementAtRandom();
				}

				public SpCustomerApi.CustomerSummary Selected
				{
					get { return _randomItem; }
				}
			}
		}
	}
}