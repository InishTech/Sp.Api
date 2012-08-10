using System;
namespace Sp.Api.Customer.Acceptance
{
	using RestSharp;
	using Sp.Api.Shared;
	using Sp.Api.Shared.Wrappers;
	using System.Linq;
	using System.Net;
	using Xunit;
	using Xunit.Extensions;

	public class Invite
	{
		[Theory, AutoSoftwarePotentialApiData]
		public static void PostCustomerInviteShouldYieldAccepted(SpAuthApi api, RandomCustomerFromListFixture customer, string anonymousVendorName)
		{
			//var licenseCustomerAssignmentUrl = license.Selected._links.customerAssignment.AsRelativeUri();
			var apiResult = api.InviteCustomer( ApiPrefix.Auth + "/registration/invite/customer/" + customer.Selected._embedded.Id, customer.Selected, anonymousVendorName, "test@inishtech.com" );
			Assert.Equal( HttpStatusCode.Accepted, apiResult.StatusCode );
		}	
	}
}	