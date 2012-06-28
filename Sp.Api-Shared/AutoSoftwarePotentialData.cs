using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using Sp.Api.ProductManagement.Acceptance.Wrappers;
using System;
using System.Configuration;
using System.Net;

namespace Sp.Api.ProductManagement.Acceptance
{
	public class AutoSoftwarePotentialData : AutoDataAttribute
	{
		public AutoSoftwarePotentialData()
			: base( new Fixture()
				.Customize( new SkipSSlCertificateValidationIfRequestedCustomization() )
				.Customize( new SoftwarePotentialApiConfigurationFromAppSettingsCustomization() ) )
		{
		}
	}

	public class SoftwarePotentialApiConfigurationFromAppSettingsCustomization : ICustomization
	{
		void ICustomization.Customize( IFixture fixture )
		{
			fixture.Register( () => new SpApiConfiguration(
				ConfigurationManager.AppSettings[ "Username" ],
				ConfigurationManager.AppSettings[ "Password" ],
				ConfigurationManager.AppSettings[ "BaseUrl" ] ) );

			fixture.Register<SpApiConfiguration, IApiClientFactory>( c => new FixedBaseUrisApiClientFactory( c ) );
			fixture.Register<IApiClientFactory, SpApi>( f => f.CreateSpApiClient() );
			fixture.Register<IApiClientFactory, SpProductManagementApi>( f => f.CreateProductManagementApiClient() );
			fixture.Register<IApiClientFactory, SpLicenseManagementApi>( f => f.CreateLicenseManagementApiClient() );
		}
	}

	public class SkipSSlCertificateValidationIfRequestedCustomization : ICustomization
	{
		void ICustomization.Customize( IFixture fixture )
		{
			if ( Boolean.Parse( ConfigurationManager.AppSettings[ "SkipCertValidation" ] ) )
				ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
		}
	}
}