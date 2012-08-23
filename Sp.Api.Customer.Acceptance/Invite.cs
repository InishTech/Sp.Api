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
			public static void ShouldYieldAccepted( SpAuthApi api, RandomCustomerFromListFixture customer, string anonymousVendorName )
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
			public static void ShouldTurnInviteStatusOpen( SpAuthApi api, RandomCustomerFromListFixture customer, string anonymousVendorName )
			{
				var selectedCustomer = customer.Selected;
				var customerInviteHref = selectedCustomer._links.inviteStatus.href;
				string emailToTemplate = "test@inishtech.com";
				char charToChangeCaseOf = emailToTemplate.ToCharArray().Where( Char.IsLower ).ToArray().ElementAtRandom();
				string emailToMutated = emailToTemplate.Replace( charToChangeCaseOf, Char.ToUpper( charToChangeCaseOf ) );
				var customerInvite = new SpCustomerApi.CustomerInvite
				{
					Customer = selectedCustomer,
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
					Assert.Equal( false, result.IsRegistered );
					Assert.Equal( true, result.IsInviteOpen );
					Assert.True( result.LastInviteSentDateTime.HasValue );
					Assert.Equal( DateTime.UtcNow, result.LastInviteSentDateTime.Value, new DateTimeEqualityTolerantComparer( PermittedClientServerClockDrift ) );
					Assert.True( DateTime.UtcNow < result.LastInviteExpiryDateTime );
				} );
			}

			static TimeSpan PermittedClientServerClockDrift
			{
				get { return TimeSpan.FromMinutes( 3 ); }
			}
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