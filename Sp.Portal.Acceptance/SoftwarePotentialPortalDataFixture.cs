namespace Sp.Portal.Acceptance
{
	using Ploeh.AutoFixture;
	using Sp.Api.Shared.Wrappers;
	using System.Configuration;

	public class SoftwarePotentialPortalDataFixture : Fixture
	{
		public SoftwarePotentialPortalDataFixture()
		{
			Customize( new SoftwarePotentialApiConfigurationFromAppSettingsCustomization() );
		}
	}

	public class SoftwarePotentialApiConfigurationFromAppSettingsCustomization : ICustomization
	{
		void ICustomization.Customize( IFixture fixture )
		{
			fixture.Register( () => new SpApiConfiguration(
				ConfigurationManager.AppSettings[ "PortalUsername" ],
				ConfigurationManager.AppSettings[ "PortalPassword" ],
				ConfigurationManager.AppSettings[ "PortalBaseUrl" ] ) );
		}
	}
}