/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Api.Customer.Acceptance
{
	using Sp.Api.Shared;
	using Sp.Test.Helpers;
	using System;
	using System.Linq;
	using System.Net;
	using Xunit;
	using Xunit.Extensions;

	public static class Invite
	{
		public static class PostCustomerInvite
		{
			[Theory, AutoSoftwarePotentialApiData]
			public static void ShouldYieldAccepted( SpAuthApi api, NewlyCreatedOrganizationFixture organization, string anonymousVendorName )
			{
				var customerInviteHref = organization.InviteStatusLink;
				var customerInvite = new SpAuthApi.CustomerInvite
				{
					EmailTo = "test@inishtech.com",
					SiteVendorName = anonymousVendorName,
					RequestId = Guid.NewGuid()
				};

				var apiResult = api.InviteCustomer( customerInviteHref, customerInvite );
				Assert.Equal( HttpStatusCode.Accepted, apiResult.StatusCode );
			}

			[Theory, AutoSoftwarePotentialApiData]
			public static void ShouldTurnInviteStatusOpen( SpAuthApi api, NewlyCreatedOrganizationFixture organization, string anonymousVendorName )
			{
				var customerInviteHref = organization.InviteStatusLink;
				const string emailToTemplate = "test@inishtech.com";
				char charToChangeCaseOf = emailToTemplate.ToCharArray().Where( Char.IsLower ).ToArray().ElementAtRandom();
				string emailToMutated = emailToTemplate.Replace( charToChangeCaseOf, Char.ToUpper( charToChangeCaseOf ) );
				var customerInvite = new SpAuthApi.CustomerInvite
				{
					EmailTo = emailToMutated,
					SiteVendorName = anonymousVendorName,
					RequestId = Guid.NewGuid()
				};
				var inviteResult = api.InviteCustomer( customerInviteHref, customerInvite );
				Assert.Equal( HttpStatusCode.Accepted, inviteResult.StatusCode );

				Verify.EventuallyWithBackOff( () =>
				{
					var statusResult = api.GetInviteStatus( customerInviteHref );
					Assert.Equal( HttpStatusCode.OK, statusResult.StatusCode );
					var result = statusResult.Data;
					Assert.Equal( emailToMutated, result.LastInviteSentTo );
					Assert.Equal( (int)SpAuthApi.InviteState.NotInvited, result.State );
				} );
			}
		}
		
		[Theory, AutoSoftwarePotentialApiData]
		public static void DuplicatePostCustomerInviteShouldYieldConflict( SpAuthApi api, NewlyCreatedOrganizationFixture organization, string anonymousVendorName )
		{
			var customerInviteHref = organization.InviteStatusLink;
			var customerInvite = new SpAuthApi.CustomerInvite
			{
				EmailTo = "test@inishtech.com",
				SiteVendorName = anonymousVendorName,
				RequestId = Guid.NewGuid()
			};

			api.InviteCustomer( customerInviteHref, customerInvite );

			var apiResult = api.InviteCustomer( customerInviteHref, customerInvite );

			Assert.Equal( HttpStatusCode.Conflict, apiResult.StatusCode );
		}
	}
}