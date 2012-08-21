namespace Sp.Api.Customer.Acceptance
{
	using Sp.Api.Shared;
	using Sp.Api.Shared.Wrappers;
	using System;
	using System.Net;
	using Xunit;
	using Xunit.Extensions;

	public class Invite
	{
		[Theory, AutoSoftwarePotentialApiData]
		public static void PostCustomerInviteShouldYieldAccepted( SpAuthApi api, RandomCustomerFromListFixture customer, string anonymousVendorName )
		{
			var selectedCustomer = customer.Selected;
			var customerInviteHref = selectedCustomer._links.inviteStatus.href;
			var customerInvite = new SpCustomerApi.CustomerInvite
			{
				Customer = selectedCustomer,
				EmailTo = "test@inishtech.com",
				VendorName = anonymousVendorName,
				RequestId = Guid.NewGuid()
			};

			var apiResult = api.InviteCustomer( customerInviteHref, customerInvite );

			Assert.Equal( HttpStatusCode.Accepted, apiResult.StatusCode );
		}

		[Theory, AutoSoftwarePotentialApiData]
		public static void DuplicatePostCustomerInviteShouldYieldConflict( SpAuthApi api, RandomCustomerFromListFixture customer, string anonymousVendorName )
		{
			var selectedCustomer = customer.Selected;
			var customerInviteHref = selectedCustomer._links.inviteStatus.href;
			var customerInvite = new SpCustomerApi.CustomerInvite
			{
				Customer = selectedCustomer,
				EmailTo = "test@inishtech.com",
				VendorName = anonymousVendorName,
				RequestId = Guid.NewGuid()
			};

			api.InviteCustomer( customerInviteHref, customerInvite );

			var apiResult = api.InviteCustomer( customerInviteHref, customerInvite );

			Assert.Equal( HttpStatusCode.Conflict, apiResult.StatusCode );
		}
	}
}