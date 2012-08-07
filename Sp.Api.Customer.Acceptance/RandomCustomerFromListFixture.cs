/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Api.Customer.Acceptance
{
	using RestSharp;
	using Sp.Test.Helpers;
	using System.Linq;
	using System.Net;
	using Xunit;

	public class RandomCustomerFromListFixture
	{
		readonly SpCustomerApi.CustomerSummary _randomItem;

		public RandomCustomerFromListFixture( SpCustomerApi api )
		{
			var apiResult = api.GetCustomerList();
			Assert.Equal( ResponseStatus.Completed, apiResult.ResponseStatus );
			Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
			Assert.NotNull(apiResult.Data.Customers);
			Assert.True( apiResult.Data.Customers.Any(), GetType().Name + " requires the target login to have at least one Customer" );
			_randomItem = apiResult.Data.Customers.ElementAtRandom();
		}

		public SpCustomerApi.CustomerSummary Selected
		{
			get { return _randomItem; }
		}
	}
}