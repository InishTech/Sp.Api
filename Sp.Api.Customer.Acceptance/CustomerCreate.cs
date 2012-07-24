using Sp.Api.Shared;
using Sp.Test.Helpers;
using System;
using System.Linq;
using System.Net;
using Xunit;
using Xunit.Extensions;

namespace Sp.Api.Customer.Acceptance
{
	public class CustomerCreate
	{
		[Theory, AutoSoftwarePotentialApiData]
		public static void CreateShouldSucceed( SpCustomerApi api )
		{
			var addLink = api.GetList().Data._links.add.href;
			var response = api.AddCustomer( addLink, new SpCustomerApi.CustomerSummary() { Name = "apiNewName", Description = "apiDescription" } );
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
		}

		[Theory, AutoSoftwarePotentialApiData]
		public static void CreateShouldResultInNewCustomer( SpCustomerApi api )
		{
			var addLink = api.GetList().Data._links.add.href;
			string newCustomerName = "apiNewName" + Guid.NewGuid();
			var response = api.AddCustomer( addLink, new SpCustomerApi.CustomerSummary() { Name = newCustomerName, Description = "apiDescription" } );
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );

			Verify.EventuallyWithBackOff( () =>
					Assert.True( api.GetList().Data.Customers.Any( x => x.Name.Equals( newCustomerName ) ) ) );
		}

		/// <summary>
		/// The following are examples of values that will be rejected as invalid.
		/// - Description is Mandatory with a max length of 100 (empty is permitted).
		/// - Label is Mandatory with a Minimum Length of 1 and max of 100.
		/// </summary>
		public static class InvalidData
		{
			public static class Name
			{
				[Theory, AutoSoftwarePotentialApiData]
				public static void PutNullShouldReject( SpCustomerApi api )
				{
					var addLink = api.GetList().Data._links.add.href;
					var response = api.AddCustomer( addLink, new SpCustomerApi.CustomerSummary() { Name = null, Description = "apiDescription" } );
					Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
				}

				[Theory, AutoSoftwarePotentialApiData]
				public static void PutEmptyShouldReject( SpCustomerApi api )
				{
					var addLink = api.GetList().Data._links.add.href;
					var response = api.AddCustomer( addLink, new SpCustomerApi.CustomerSummary() { Name = string.Empty, Description = "apiDescription" } );
					Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
				}

				[Theory, AutoSoftwarePotentialApiData]
				public static void PutExcessivelyLongShouldReject( SpCustomerApi api )
				{
					var addLink = api.GetList().Data._links.add.href;
					var response = api.AddCustomer( addLink, new SpCustomerApi.CustomerSummary() { Name = new String( 'a', 101 ), Description = "apiDescription" } );
					Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
				}
			}

			public static class Description
			{
				[Theory, AutoSoftwarePotentialApiData]
				public static void PutExcessivelyLongShouldReject( SpCustomerApi api )
				{
					var addLink = api.GetList().Data._links.add.href;
					var response = api.AddCustomer( addLink, new SpCustomerApi.CustomerSummary() { Name = "apiName", Description = new String( 'a', 101 ) } );
					Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
				}
			}
		}
	}
}
