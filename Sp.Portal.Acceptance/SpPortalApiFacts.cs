namespace Sp.Portal.Acceptance
{
	using System;
	using System.Configuration;
	using Ploeh.AutoFixture;
	using RestSharp;
	using Sp.Portal.Acceptance.Wrappers;
	using Xunit;

	public static class SpPortalApiFacts
	{
		[Fact]
		public static void AuthenticationWithIncorrectCredentialsShouldThrow()
		{
			var autoFixture = new Fixture();
			autoFixture.Customize( new SoftwarePotentialApiIncorrectCredentialsConfigurationCustomization() );
			autoFixture.Customize( new SkipSSlCertificateValidationIfRequestedCustomization() );
			var portalApi = autoFixture.CreateAnonymous<SpPortalApi>();

			var request = new RestRequest( string.Empty );
			request.AddHeader( "Accept", "text/html" );

			Assert.Throws<InvalidOperationException>( () =>
				portalApi.Execute( request ) );
		}

		public class SoftwarePotentialApiIncorrectCredentialsConfigurationCustomization : ICustomization
		{
			void ICustomization.Customize( IFixture fixture )
			{
				// Use random strings for username and password
				fixture.Register( ( string username, string password ) => new SpPortalConfiguration(
					username,
					password,
					ConfigurationManager.AppSettings[ "PortalBaseUrl" ] ) );
			}
		}
	}
}
