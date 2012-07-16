/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Portal.Acceptance
{
	using System;
	using System.Configuration;
	using Ploeh.AutoFixture;
	using RestSharp;
	using Sp.Portal.Acceptance.Wrappers;
	using Xunit;
	using Sp.Api.Shared.Wrappers;

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
				fixture.Register( ( string username, string password ) => new SpApiConfiguration(
					username,
					password,
					ConfigurationManager.AppSettings[ "PortalBaseUrl" ] ) );
			}
		}
	}
}