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

namespace Sp.Api.Consume.Acceptance
{
	public class FreshCustomerFixture
	{
		public SpCustomerApi.CustomerSummary SignedCustomer { get; private set; }

		public FreshCustomerFixture( SpCustomerApi api, string customerName, string customerDescription, Guid requestId )
		{
			var response = api.CreateCustomer(  new SpCustomerApi.CustomerSummary { Name = customerName, Description = customerDescription, RequestId = requestId } );

			Assert.Equal( HttpStatusCode.Accepted, response.StatusCode );
			string location = Assert.IsType<string>( response.Headers.Single( x => x.Name == "Location" ).Value );

			var signedCustomer = GetCustomer( api, location );
			SignedCustomer = signedCustomer.Data;
		}

		static IRestResponse<SpCustomerApi.CustomerSummary> GetCustomer( SpCustomerApi api, string location )
		{
			var apiResult = default(IRestResponse<SpCustomerApi.CustomerSummary>);
			Verify.EventuallyWithBackOff( () =>
			{
				apiResult = api.GetCustomer( location );
				Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
			} );
			return apiResult;
		}
	}
}