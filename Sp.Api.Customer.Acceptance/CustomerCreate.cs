namespace Sp.Api.Customer.Acceptance
{
	using Sp.Api.Shared;
	using Sp.Test.Helpers;
	using System;
	using System.Linq;
	using System.Net;
	using Xunit;
	using Xunit.Extensions;

	public class CustomerCreate
	{
		[Theory, AutoSoftwarePotentialApiData]
		public static void ShouldYieldOk( SpCustomerApi api, string anonymousCustomerName, string anonymousCustomerDescription)
		{
			var addLink = GetCustomerAddHref( api );
			var response = api.CreateCustomer( addLink, new SpCustomerApi.CustomerSummary() { Name = anonymousCustomerName, Description = anonymousCustomerDescription } );
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
		}

		[Theory, AutoSoftwarePotentialApiData]
		public static void ShouldResultInNewCustomer( SpCustomerApi api, string anonymousCustomerName, string anonymousCustomerDescription )
		{
			var addLink = GetCustomerAddHref( api );
			var response = api.CreateCustomer( addLink, new SpCustomerApi.CustomerSummary() { Name = anonymousCustomerName, Description = anonymousCustomerDescription } );
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );

			Verify.EventuallyWithBackOff( () =>
				Assert.True( api.GetCustomerList().Data.Customers.Any( x => x.Name.Equals( anonymousCustomerName ) ) ) );
		}

		/// <summary>
		/// The following are examples of values that will be rejected as invalid.
		/// - Name is Mandatory with a Minimum Length of 1 and max of 100.
		/// - Description is Mandatory with a max length of 100 (empty is permitted).
		/// </summary>
		public static class InvalidData
		{
			public static class Name
			{
				[Theory, AutoSoftwarePotentialApiData]
				public static void PutNullShouldReject( SpCustomerApi api, string anonymousDescription )
				{
					var addLink = GetCustomerAddHref( api );
					var response = api.CreateCustomer( addLink, new SpCustomerApi.CustomerSummary() { Name = null, Description = anonymousDescription } );
					Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
				}

				[Theory, AutoSoftwarePotentialApiData]
				public static void PutEmptyShouldReject( SpCustomerApi api, string anonymousDescription )
				{
					var addLink = GetCustomerAddHref( api );
					var response = api.CreateCustomer( addLink, new SpCustomerApi.CustomerSummary() { Name = string.Empty, Description = anonymousDescription } );
					Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
				}

				[Theory, AutoSoftwarePotentialApiData]
				public static void PutExcessivelyLongShouldReject( SpCustomerApi api, string anonymousDescription )
				{
					var addLink = GetCustomerAddHref( api );
					var response = api.CreateCustomer( addLink, new SpCustomerApi.CustomerSummary() { Name = new String( 'a', 101 ), Description = anonymousDescription } );
					Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
				}
			}

			public static class Description
			{
				[Theory, AutoSoftwarePotentialApiData]
				public static void PutExcessivelyLongShouldReject( SpCustomerApi api, string anonymousName )
				{
					var addLink = GetCustomerAddHref( api );
					var response = api.CreateCustomer( addLink, new SpCustomerApi.CustomerSummary() { Name = anonymousName, Description = new String( 'a', 101 ) } );
					Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
				}
			}
		}

		static string GetCustomerAddHref( SpCustomerApi api )
		{
			var customerListResponse = api.GetCustomerList();
			Assert.Equal( HttpStatusCode.OK, customerListResponse.StatusCode );
			return customerListResponse.Data._links.add.href;
		}
	}
}