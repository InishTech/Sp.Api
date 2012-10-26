/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */

namespace Sp.Api.Customer.Acceptance
{
	using Ploeh.AutoFixture.Xunit;
	using Sp.Api.Shared;
	using Sp.Test.Helpers;
	using System;
	using System.Linq;
	using System.Net;
	using Xunit;
	using Xunit.Extensions;

	public static class Invite
	{
		static Guid VendorInvitableInstanceId
		{
			//TODO TP 1043 - retrieve instance resource and get instance id from there
			get { return new Guid( "0C37CB62-36B8-4752-ACD8-4C31FC059D66" ); }
		}

		public static class PostCustomerInvite
		{
			[Theory, AutoSoftwarePotentialApiData]
			public static void ShouldYieldAccepted( [Frozen] SpAuthApi api, NewlyCreatedOrganizationFixture organization, string anonymousVendorName )
			{
				var customerInviteHref = organization.InviteStatusLink;
				var customerInvite = new SpAuthApi.CustomerInvite
				{
					EmailTo = "test@inishtech.com",
					InstanceId = VendorInvitableInstanceId,
					RequestId = Guid.NewGuid()
				};

				var apiResult = api.InviteCustomer( customerInviteHref, customerInvite );
				Assert.Equal( HttpStatusCode.Accepted, apiResult.StatusCode );
			}

			[Theory, AutoSoftwarePotentialApiData]
			public static void ShouldTurnInviteStatusOpen( [Frozen] SpAuthApi api, NewlyCreatedOrganizationFixture organization, string anonymousVendorName )
			{
				var customerInviteHref = organization.InviteStatusLink;
				const string emailToTemplate = "test@inishtech.com";
				char charToChangeCaseOf = emailToTemplate.ToCharArray().Where( Char.IsLower ).ToArray().ElementAtRandom();
				string emailToMutated = emailToTemplate.Replace( charToChangeCaseOf, Char.ToUpper( charToChangeCaseOf ) );
				var customerInvite = new SpAuthApi.CustomerInvite
				{
					EmailTo = emailToMutated,
					InstanceId = VendorInvitableInstanceId,
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
					Assert.Equal( "OpenInvitation", result.State );
				} );
			}
		}

		public class SignedCustomerFixture
		{
			readonly SpCustomerApi.CustomerSummary _data;

			public SignedCustomerFixture( SpCustomerApi api, RandomCustomerFromListFixture customer )
			{
				var apiResult = api.GetCustomer( customer.Selected._links.self.href );
				Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
				Assert.NotNull( apiResult.Data );
				_data = apiResult.Data;
			}

			public SpCustomerApi.CustomerSummary Data
			{
				get { return _data; }
			}
		}

		[Theory, AutoSoftwarePotentialApiData]
		public static void DuplicatePostCustomerInviteShouldYieldConflict( [Frozen] SpAuthApi api, NewlyCreatedOrganizationFixture organization, string anonymousVendorName )
		{
			var customerInviteHref = organization.InviteStatusLink;
			var customerInvite = new SpAuthApi.CustomerInvite
			{
				EmailTo = "test@inishtech.com",
				InstanceId = VendorInvitableInstanceId,
				RequestId = Guid.NewGuid()
			};

			api.InviteCustomer( customerInviteHref, customerInvite );

			var apiResult = api.InviteCustomer( customerInviteHref, customerInvite );

			Assert.Equal( HttpStatusCode.Conflict, apiResult.StatusCode );
		}

		public class NewlyCreatedOrganizationFixture
		{
			public NewlyCreatedOrganizationFixture( SpCustomerApi api, NewlyCreatedCustomerFixture customer )
			{
				//TODO TP 1043 - arguably inviteStatus link should be in the organization resource rather than in the customer
				InviteStatusLink = customer.Customer._links.inviteStatus.href;
				var organization = new SpCustomerApi.OrganizationCreateModel( customer.Customer );
				var response = api.CreateOrganization( organization );
				Assert.Equal( HttpStatusCode.Accepted, response.StatusCode );
			}

			public string InviteStatusLink { get; private set; }
		}
	}
}