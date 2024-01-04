/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Api.Consume.Acceptance
{
	using AutoFixture.Xunit;
	using Sp.Api.Shared;
	using Sp.Test.Helpers;
	using System;
	using System.Linq;
	using System.Net;
    using System.Reflection;
    using Xunit;
    using Xunit.Extensions;

	public static class Invite
	{
		public static class PostCustomerInvite
		{
			[Theory, AutoSoftwarePotentialApiData]
			public static void ShouldYieldAccepted( [Frozen] SpAuthApi api, FreshOrganizationFixture organization )
			{
				var methodName = MethodBase.GetCurrentMethod().Name;

                var customerInviteHref = organization.InviteStatusLink;
				var customerInvite = new SpAuthApi.CustomerInvite
				{
                    EmailTo = $"ejfqf.{methodName}@inbox.testmail.app",
                    RequestId = Guid.NewGuid()
				};

				var apiResult = api.InviteCustomer( customerInviteHref, customerInvite );
				Assert.Equal( HttpStatusCode.Accepted, apiResult.StatusCode );
			}

			[Theory, AutoSoftwarePotentialApiData]
			public static void ShouldTurnInviteStatusOpenAndUpdateLastInviteTo( [Frozen] SpAuthApi api, FreshOrganizationFixture organization )
			{
                var methodName = MethodBase.GetCurrentMethod().Name;
                var customerInviteHref = organization.InviteStatusLink;
                // TOCONSIDER: Extract fixture to represent anonymous email address
                string emailToTemplate = $"ejfqf.{methodName}@inbox.testmail.app";
                char charToChangeCaseOf = emailToTemplate.ToCharArray().Where( Char.IsLower ).ToArray().ElementAtRandom();
				string emailToMutated = emailToTemplate.Replace( charToChangeCaseOf, Char.ToUpper( charToChangeCaseOf ) );
				var customerInvite = new SpAuthApi.CustomerInvite
				{
					EmailTo = emailToMutated,
					RequestId = Guid.NewGuid()
				};
				var inviteResult = api.InviteCustomer( customerInviteHref, customerInvite );
				Assert.Equal( HttpStatusCode.Accepted, inviteResult.StatusCode );

				Verify.EventuallyWithBackOff( () =>
				{
					var statusResult = api.GetInviteStatus( customerInviteHref );
					Assert.Equal( HttpStatusCode.OK, statusResult.StatusCode );
					var result = statusResult.Data;
					Assert.Equal( "OpenInvitation", result.State );
					Assert.Equal( emailToMutated, result.LastEmailSentTo );
				} );
			}
		}

		[Theory, AutoSoftwarePotentialApiData]
		public static void DuplicatePostCustomerInviteShouldYieldConflict( [Frozen] SpAuthApi api,  FreshOrganizationFixture organization )
		{
            var methodName = MethodBase.GetCurrentMethod().Name;
            var customerInviteHref = organization.InviteStatusLink;
			var customerInvite = new SpAuthApi.CustomerInvite
			{
                EmailTo = $"ejfqf.{methodName}@inbox.testmail.app",
                RequestId = Guid.NewGuid()
			};

			api.InviteCustomer( customerInviteHref, customerInvite );

			var apiResult = api.InviteCustomer( customerInviteHref, customerInvite );

			Assert.Equal( HttpStatusCode.Conflict, apiResult.StatusCode );
		}

		public class FreshOrganizationFixture
		{
			public FreshOrganizationFixture( SpAuthApi api, FreshCustomerFixture customer )
			{
				InviteStatusLink = customer.SignedCustomer._links.inviteStatus.href.EnsureTrailingSlash();
				var addOrganizationLink = customer.SignedCustomer._links.organizationAdd.href.EnsureTrailingSlash();
				var organization = new SpAuthApi.OrganizationCreateModel( customer.SignedCustomer );
				var response = api.CreateOrganization( organization, addOrganizationLink );
				Assert.Equal( HttpStatusCode.Accepted, response.StatusCode );
			}

			public string InviteStatusLink { get; private set; }
		}
	}
}