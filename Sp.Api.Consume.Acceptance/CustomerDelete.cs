using Ploeh.AutoFixture.Xunit;
using Sp.Api.Shared;
using Sp.Test.Helpers;
using System.Net;
using Xunit;
using Xunit.Extensions;

namespace Sp.Api.Consume.Acceptance
{
	public static class CustomerDelete
	{
		[Theory, AutoSoftwarePotentialApiData]
		public static void DeleteCustomerShouldYieldAccepted( [Frozen] SpCustomerApi api, FreshCustomerFixture freshCustomerFixture )
		{
			var linkedAddress = freshCustomerFixture.SignedCustomer._links.self;
			var apiResult = api.DeleteCustomer( linkedAddress.href );
			Assert.Equal( HttpStatusCode.Accepted, apiResult.StatusCode );
		}

		[Theory, AutoSoftwarePotentialApiData]
		public static void GetDeletedCustomerShould404( [Frozen] SpCustomerApi api, FreshCustomerFixture freshCustomerFixture )
		{
			var linkedAddress = freshCustomerFixture.SignedCustomer._links.self;
			var apiResult = api.DeleteCustomer( linkedAddress.href );
			Verify.EventuallyWithBackOff( () =>
				{
					apiResult = api.GetCustomer( linkedAddress.href );
					Assert.Equal( HttpStatusCode.NotFound, apiResult.StatusCode );
				} );
		}

		[Theory, AutoSoftwarePotentialApiData]
		public static void UpdateDeletedCustomerShould500( [Frozen] SpCustomerApi api, string anonymousName, string anonymousExternalId, FreshCustomerFixture freshCustomerFixture )
		{
			var linkedAddress = freshCustomerFixture.SignedCustomer._links.self;
			var apiResult = api.DeleteCustomer( linkedAddress.href );

			var updatedCustomer = freshCustomerFixture.SignedCustomer;
			updatedCustomer.Name = anonymousName;
			updatedCustomer.ExternalId = anonymousExternalId;
			updatedCustomer._version++;
			apiResult = api.PutCustomer( linkedAddress.href, updatedCustomer );
			Assert.Equal( HttpStatusCode.InternalServerError, apiResult.StatusCode );
		}
	}
}
