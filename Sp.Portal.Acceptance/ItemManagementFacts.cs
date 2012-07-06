namespace Sp.Portal.Acceptance
{
	using System.Collections.Generic;
	using System.Configuration;
	using System.Linq;
	using System.Net;
	using Ploeh.AutoFixture;
	using RestSharp;
	using Sp.Portal.Acceptance.Wrappers;
	using Xunit;
	using Xunit.Extensions;

	public static class ItemManagementFacts
	{
		[Theory, PortalData]
		public static void GetItemsShouldYieldData( SpPortalApi portalApi )
		{
			var request = new RestRequest( "ItemManagement/" );
			var response = portalApi.Execute<ItemsPage>( request );
			Assert.Equal( ResponseStatus.Completed, response.ResponseStatus );
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
			Assert.NotNull( response.Data );
			Assert.NotEmpty( response.Data.Items );
			Assert.NotNull( response.Data.Items.First().Id );
			Assert.NotNull( response.Data.Items.First().Label );
		}

		[Fact]
		public static void GetItemsWithoutCanSeeItemsClaimShouldReturnHttpStatusForbidden(  )
		{
			var autoFixture = new Fixture();
			autoFixture.Customize( new SoftwarePotentialApiGuestCredentialsConfigurationCustomization() );
			autoFixture.Customize( new SkipSSlCertificateValidationIfRequestedCustomization() );
			var portalApi = autoFixture.CreateAnonymous<SpPortalApi>();

			var request = new RestRequest( "ItemManagement/" );
			var response = portalApi.Execute<ItemsPage>( request );
			Assert.Equal( HttpStatusCode.Forbidden, response.StatusCode );
		}

		public class ItemsPage
		{
			public List<Item> Items { get; set; }
		}

		public class Item
		{
			public string Id { get; set; }
			public string Label { get; set; }
		}

		public class SoftwarePotentialApiGuestCredentialsConfigurationCustomization : ICustomization
		{
			void ICustomization.Customize( IFixture fixture )
			{
				// Use guest credentials - should authenticate, but not give CanSeeItems claim
				fixture.Register( ( string username, string password ) => new SpPortalConfiguration(
					"guest",
					ConfigurationManager.AppSettings[ "Password" ],
					ConfigurationManager.AppSettings[ "PortalBaseUrl" ] ) );
			}
		}
	}
}