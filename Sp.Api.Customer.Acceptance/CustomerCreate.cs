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
	}
}
