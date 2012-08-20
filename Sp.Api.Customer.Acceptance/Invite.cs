namespace Sp.Api.Customer.Acceptance
{
	using Sp.Api.Shared;
	using Sp.Api.Shared.Wrappers;
	using System.Net;
	using Xunit;
	using Xunit.Extensions;
	using System;

	public class Invite
	{
		[Theory, AutoSoftwarePotentialApiData]
		public static void PostCustomerInviteShouldYieldAccepted( SpAuthApi api, RandomCustomerFromListFixture customer, string anonymousVendorName )
		{
			//var customerInviteHref = license.Selected._links.customerAssignment.href;
			var customerInvite = new SpCustomerApi.CustomerInvite() { Customer = customer.Selected, EmailTo = "test@inishtech.com", VendorName = anonymousVendorName, RequestId = Guid.NewGuid() };
			var customerInviteHref = ApiPrefix.Auth + "/registration/invite/" + customer.Selected._embedded.Id + "/customer/";

			var apiResult = api.InviteCustomer( customerInviteHref, customerInvite);

			Assert.Equal( HttpStatusCode.Accepted, apiResult.StatusCode );
		}

		[Theory, AutoSoftwarePotentialApiData]
		public static void DuplicatePostCustomerInviteShouldYieldConflict( SpAuthApi api, RandomCustomerFromListFixture customer, string anonymousVendorName )
		{
			//var customerInviteHref = license.Selected._links.customerAssignment.href;
			var customerInvite = new SpCustomerApi.CustomerInvite() { Customer = customer.Selected, EmailTo = "test@inishtech.com", VendorName = anonymousVendorName, RequestId = Guid.NewGuid() };
			var customerInviteHref = ApiPrefix.Auth + "/registration/invite/" + customer.Selected._embedded.Id + "/customer/";
		
			api.InviteCustomer( customerInviteHref, customerInvite);

			var apiResult = api.InviteCustomer( customerInviteHref, customerInvite );

			Assert.Equal( HttpStatusCode.Conflict, apiResult.StatusCode );
		}
	}
}