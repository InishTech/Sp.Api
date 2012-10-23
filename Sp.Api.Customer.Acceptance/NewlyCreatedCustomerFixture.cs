/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
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
			var response = api.CreateCustomer(  new SpCustomerApi.CustomerSummary { Name = customerName, Description = customerDescription, RequestId = requestId } );

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
	}
}