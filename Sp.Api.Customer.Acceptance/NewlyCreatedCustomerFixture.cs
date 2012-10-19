using System;
using System.Linq;
using System.Net;
using RestSharp;
using Sp.Test.Helpers;
using Xunit;

namespace Sp.Api.Customer.Acceptance
{
	public class NewlyCreatedCustomerFixture
	{
		public SpCustomerApi.CustomerSummary Customer { get; private set; }

		public NewlyCreatedCustomerFixture( SpCustomerApi api, string customerName, string customerDescription, Guid requestId )
		{
			//Create a customer
			var response = api.CreateCustomer( CustomerAddHref( api ), new SpCustomerApi.CustomerSummary { Name = customerName, Description = customerDescription, RequestId = requestId } );

			Assert.Equal( HttpStatusCode.Accepted, response.StatusCode );
			string location = Assert.IsType<string>( response.Headers.Single( x => x.Name == "Location" ).Value );

			//Now get the customer
			IRestResponse<SpCustomerApi.CustomerSummary> apiResult = null;
			Verify.EventuallyWithBackOff( () =>
			{
				apiResult = api.GetCustomer( location );
				Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
			} );
			Customer = apiResult.Data;
		}

		static string CustomerAddHref( SpCustomerApi api )
		{
			var customerListResponse = api.GetCustomerList();
			Assert.Equal( HttpStatusCode.OK, customerListResponse.StatusCode );
			return customerListResponse.Data._links.add.href;
		}
	}
}