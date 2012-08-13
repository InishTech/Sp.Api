namespace Sp.Api.Customer.Acceptance
{
	using Sp.Api.Shared;
	using Sp.Api.Shared.Wrappers;
	using System.Net;
	using Xunit;
	using Xunit.Extensions;

	public class Invite
	{
		[Theory, AutoSoftwarePotentialApiData]
		public static void PostCustomerInviteShouldYieldAccepted( SpAuthApi api, RandomCustomerFromListFixture customer, string anonymousVendorName )
		{
			//var customerInviteHref = license.Selected._links.customerAssignment.href;
			var customerInviteHref = ApiPrefix.Auth + "/invite/customer/" + customer.Selected._embedded.Id;

			var apiResult = api.InviteCustomer( customerInviteHref, customer.Selected, anonymousVendorName, "test@inishtech.com" );
	
			Assert.Equal( HttpStatusCode.Accepted, apiResult.StatusCode );
		}
	}
}