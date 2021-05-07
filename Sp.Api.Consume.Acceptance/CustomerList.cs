namespace Sp.Api.Consume.Acceptance
{
	using System;
	using Sp.Api.Shared;
	using System.Net;
	using Xunit;
	using Xunit.Extensions;

	public static class CustomerList
	{
		[Theory, AutoSoftwarePotentialApiData]
		public static void ShouldAlwaysBeAvailable( SpCustomerApi api )
		{
			var response = api.GetCustomerList();
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
		}   

		[Smoke]
		[MediumFrequency]
		[Theory, AutoSoftwarePotentialApiData]
		public static void ShouldHaveAtLeastOneItem( SpCustomerApi api )
		{
			var response = api.GetCustomerList();
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
			Assert.NotEmpty( response.Data.results );
		}

		public static class RandomItem
		{
			[Theory, AutoSoftwarePotentialApiData]
			public static void ShouldHaveAName( RandomCustomerFromListFixture item )
			{
				Assert.NotEmpty( item.Selected.Name );
			}

			[Theory, AutoSoftwarePotentialApiData]
			public static void ShouldHaveANonNullDescription( RandomCustomerFromListFixture item )
			{
				Assert.NotNull( item.Selected.ExternalId );
			}

			[Theory, AutoSoftwarePotentialApiData]
			public static void ShouldHaveAValidSelfLink( RandomCustomerFromListFixture item )
			{
				VerifyLinkValid( item, links => links.self );
			}

			static void VerifyLinkValid( RandomCustomerFromListFixture item, Func<SpCustomerApi.CustomerSummary.Links, SpCustomerApi.Link> linkSelector )
			{
				var linksSet = item.Selected._links;
				Assert.NotNull( linksSet );
				var linkToVerify = linkSelector( linksSet );
				VerifyLink.LinkWellFormed( linkToVerify );
			}
		}
	}

	static class VerifyLink
	{
		public static void LinkWellFormed( SpCustomerApi.Link link )
		{
			Assert.NotNull( link );
			Assert.NotNull( link.href );
			Assert.NotEmpty( link.href );
		}
	}
}