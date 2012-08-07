namespace Sp.Api.Customer.Acceptance
{
	using Ploeh.AutoFixture.Xunit;
	using Ploeh.SemanticComparison.Fluent;
	using Sp.Api.Shared;
	using Sp.Test.Helpers;
	using System;
	using System.Net;
	using Xunit;
	using Xunit.Extensions;

	public static class CustomerGet
	{
		/// <summary>
		/// The master list presents a set of linked child entities. Here we select an arbitrary one from the list and follow its _links.self to get that resource's 			
		/// </summary>
		/// <remarks>
		/// Success/failure is communicated by the HTTP Status Code being OK		
		/// </remarks>
		/// <param name="api">Api wrapper. [Frozen] so requests involved in getting <paramref name="preSelectedCustomer"/> can share the authentication work.</param>
		/// <param name="preSelectedCustomer">Arbitrarily chosen customer from the configured user's list</param>
		[Theory, AutoSoftwarePotentialApiData]
		public static void GetCustomerShouldContainData( [Frozen] SpCustomerApi api, RandomCustomerFromListFixture preSelectedCustomer )
		{
			var linkedAddress = preSelectedCustomer.Selected._links.self;
			//Now query the API for that specific customer by following the link obtained in the previous step
			var apiResult = api.GetCustomer( linkedAddress.href );
			Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
			//The customer obtained as a separete resource should be identical to the customer previously selected from the list
			apiResult.Data.AsSource().OfLikeness<SpCustomerApi.CustomerSummary>()
				.Without( p => p._links )
				.With( x => x._embedded )
					.EqualsWhen( ( x, y ) => x._embedded.AsSource().OfLikeness<SpCustomerApi.CustomerSummary._Embedded>().Equals( y._embedded ) )
				.Without( p => p._signature )
				.ShouldEqual( preSelectedCustomer.Selected );
		}

		/// <summary>
		/// Normal usage should just involve following _links as illustrated in GetProductFromListShouldYieldData. Here we simulate what would happen if the item [had not yet become accessible|had been deleted]
		/// </summary>
		/// <param name="api">Api wrapper. [Frozen] so requests involved in getting <paramref name="customer"/> can share the authentication work.</param>
		/// <param name="customer">Arbitrarily chosen customer from the configured user's list</param>
		/// <param name="anonymousId">Random id</param>
		[Theory, AutoSoftwarePotentialApiData]
		public static void GetNonExistingCustomerShould404( [Frozen] SpCustomerApi api, RandomCustomerFromListFixture customer, Guid anonymousId )
		{
			string validHref = customer.Selected._links.self.href;
			var invalidHref = UriHelper.HackLinkReplacingGuidWithAlternateValue( anonymousId, validHref );
			var apiResult = api.GetCustomer( invalidHref );
			// We don't want to have landed on an error page that has a StatusCode of 200
			Assert.Equal( HttpStatusCode.NotFound, apiResult.StatusCode );
			// Our final Location should match what we asked for
			Assert.Contains( invalidHref.ToString(), apiResult.ResponseUri.ToString() );
		}
	}
}