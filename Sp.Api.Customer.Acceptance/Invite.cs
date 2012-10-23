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
		public static class PostCustomerInvite
		{
			[Theory, AutoSoftwarePotentialApiData]
			public static void ShouldYieldAccepted( [Frozen] SpAuthApi api, SignedCustomerFixture signedCustomer, string anonymousVendorName )
			{
				var customerInviteHref = signedCustomer.Data._links.inviteStatus.href;
				var customerInvite = new SpCustomerApi.CustomerInvite
				{
					Customer = signedCustomer.Data,
					EmailTo = "test@inishtech.com",
					VendorName = anonymousVendorName,
					RequestId = Guid.NewGuid()
				};

				var apiResult = api.InviteCustomer( customerInviteHref, customerInvite );
				Assert.Equal( HttpStatusCode.Accepted, apiResult.StatusCode );
			}

			[Theory, AutoSoftwarePotentialApiData]
			public static void ShouldTurnInviteStatusOpen( [Frozen] SpAuthApi api, SignedCustomerFixture signedCustomer, string anonymousVendorName )
			{
				var customerInviteHref = signedCustomer.Data._links.inviteStatus.href;
				string emailToTemplate = "test@inishtech.com";
				char charToChangeCaseOf = emailToTemplate.ToCharArray().Where( Char.IsLower ).ToArray().ElementAtRandom();
				string emailToMutated = emailToTemplate.Replace( charToChangeCaseOf, Char.ToUpper( charToChangeCaseOf ) );
				var customerInvite = new SpCustomerApi.CustomerInvite
				{
					Customer = signedCustomer.Data,
					EmailTo = emailToMutated,
					VendorName = anonymousVendorName,
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
					Assert.Equal( "Invited", result.State );
					//Assert.True( result.LastInviteSentDateTime.HasValue );
					//Assert.Equal( DateTime.UtcNow, result.LastInviteSentDateTime.Value, new DateTimeEqualityTolerantComparer( PermittedClientServerClockDrift ) );
					//Assert.True( DateTime.UtcNow < result.LastInviteExpiryDateTime );
				} );
			}

			//static TimeSpan PermittedClientServerClockDrift
			//{
			//	get { return TimeSpan.FromMinutes( 3 ); }
			//}
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
		public static void DuplicatePostCustomerInviteShouldYieldConflict( SpAuthApi api, SignedCustomerFixture signedCustomer, string anonymousVendorName )
		{
			var customerInviteHref = signedCustomer.Data._links.inviteStatus.href;
			var customerInvite = new SpCustomerApi.CustomerInvite
			{
				Customer = signedCustomer.Data,
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