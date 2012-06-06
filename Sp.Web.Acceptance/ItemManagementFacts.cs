namespace Sp.Portal.Acceptance
{
	using System.Collections.Generic;
	using System.Configuration;
	using System.Linq;
	using System.Net;
	using Ploeh.AutoFixture;
	using RestSharp;
	using Sp.Web.Acceptance;
	using Sp.Web.Acceptance.Wrappers;
	using Xunit;
	using Xunit.Extensions;

	public static class ItemManagementFacts
	{
		[Theory, WebData]
		public static void GetItemsShouldYieldData( SpWeb webApi )
		{
			var request = new RestRequest( "ItemManagement/" );
			var response = webApi.Execute<ItemsPage>( request );
			Assert.Equal( ResponseStatus.Completed, response.ResponseStatus );
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
			Assert.NotNull( response.Data );
			Assert.NotEmpty( response.Data.Items );
			Assert.NotNull( response.Data.Items.First().Id );
			Assert.NotNull( response.Data.Items.First().Label );
		}

		[Fact]
		public static void GetFromHttpShouldHopToHttps( )
		{
			var autoFixture = new Fixture();
			autoFixture.Customize( new SoftwarePotentialWebHttpConfigurationCustomization() );
			autoFixture.Customize( new SkipSSlCertificateValidationIfRequestedCustomization() );
			var webApi = autoFixture.CreateAnonymous<SpWeb>();

			var request = new RestRequest( "ItemManagement/" );
			var response = webApi.Execute<ItemsPage>( request );
			Assert.Equal( ResponseStatus.Completed, response.ResponseStatus );
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
			Assert.Contains( "https", response.ResponseUri.AbsoluteUri );
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
		
		public class SoftwarePotentialWebHttpConfigurationCustomization : ICustomization
		{
			void ICustomization.Customize( IFixture fixture )
			{
				// Use guest credentials - should authenticate, but not give CanSeeItems claim
				// Use guest credentials - should authenticate, but not give CanSeeItems claim
				fixture.Register( ( string username, string password ) => new SpWebConfiguration(
					username,
					password,
					ConfigurationManager.AppSettings[ "WebBaseUrl" ].Replace( "https", "http" ) ) );
			}
		}
	}
}
