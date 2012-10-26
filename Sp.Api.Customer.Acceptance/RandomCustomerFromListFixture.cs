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

	/// <summary>
	/// <para>Customers are normally located by obtaining a list and selecting from that, or by searching.</para>
	/// <para>This Fixture Selects a random item from the set that the Test Suite's credentials grants us access to.</para>
	/// </summary>
	/// <remarks>It is assumed that there is at least one customer in the list on the service for the test Suite credentials.</remarks>
	public class RandomCustomerFromListFixture
	{
		public SpCustomerApi.CustomerSummary Selected { get; private set; }

		public RandomCustomerFromListFixture( SpCustomerApi api )
		{
			var apiResult = api.GetCustomerList();
			Assert.Equal( ResponseStatus.Completed, apiResult.ResponseStatus );
			Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
			Assert.NotNull( apiResult.Data.results );
			Assert.True( apiResult.Data.results.Any(), GetType().Name + " requires the target login to have at least one Customer" );
			Selected = apiResult.Data.results.ElementAtRandom();
		}
	}

	/// <summary>
	/// <para>This Fixture follows the link and does a Get from an arbitrarily selected customer from the list.</para>
	/// </summary>
	/// <remarks>
	/// <para><see cref="RandomCustomerFromListFixture"/> for preconditions.</para>
	/// <para>The data differs from that in the list available from the index in that the individual GET yields a signature too, which is important for sending Invitations and Assigning Licenses.</para>
	/// </remarks>
	public class GetRandomCustomerFixture
	{
		public SpCustomerApi.CustomerSummary DataFromGet { get; private set; }

		public GetRandomCustomerFixture( SpCustomerApi api, RandomCustomerFromListFixture list )
		{
			//Now query the API for that specific customer by following the self link from the item in the Index obtained in the previous step
			var linkedAddress = list.Selected._links.self;
			var apiResult = api.GetCustomer( linkedAddress.href );
			Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
			DataFromGet = apiResult.Data;
		}
	}
}