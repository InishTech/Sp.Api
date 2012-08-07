namespace Sp.Api.Customer.Acceptance
{
	using System;
	using Sp.Api.Shared;
	using System.Net;
	using Xunit;
	using Xunit.Extensions;

	public static class CustomerList
	{
		[Theory, AutoSoftwarePotentialApiData]
		public static void ShouldAlwaysBeAvailable( SpCustomerApi api )
		{
			var response = api.GetCustomerList();
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
		}

		[Theory, AutoSoftwarePotentialApiData]
		public static void ShouldHaveALinkForAdd( SpCustomerApi api )
		{
			var response = api.GetCustomerList();
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
			Assert.NotNull( response.Data._links );
			Assert.NotEmpty( response.Data._links.add.href );
		}

		[Theory, AutoSoftwarePotentialApiData]
		public static void ShouldHaveAtLeastOneItem( SpCustomerApi api )
		{
			var response = api.GetCustomerList();
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
			Assert.NotEmpty( response.Data.Customers );
		}

		public static class RandomItem
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

			[Theory, AutoSoftwarePotentialApiData]
			public static void ShouldHaveAValidSelfLink( RandomCustomerFromListFixture item )
			{
				VerifyLinkValid( item, links => links.self );
			}

			static void VerifyLinkValid( RandomCustomerFromListFixture item, Func<SpCustomerApi.CustomerSummary.Links, SpCustomerApi.Link> linkSelector )
			{
				var linksSet = item.Selected._links;
				Assert.NotNull( linksSet );
				var linkToVerify = linkSelector( linksSet );
				Assert.NotNull(linkToVerify);
				Assert.NotEmpty( linkToVerify.href );
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