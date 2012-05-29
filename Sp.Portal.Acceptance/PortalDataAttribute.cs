using System;
using System.Configuration;
using System.Net;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using Sp.Portal.Acceptance.Wrappers;

namespace Sp.Portal.Acceptance
{
	class PortalDataAttribute: AutoDataAttribute
	{
		public PortalDataAttribute()
			: base( new Fixture()
				.Customize( new SkipSSlCertificateValidationIfRequestedCustomization() )
				.Customize( new SoftwarePotentialApiConfigurationFromAppSettingsCustomization() ))
		{
		}
	}

	public class SoftwarePotentialApiConfigurationFromAppSettingsCustomization : ICustomization
	{
		void ICustomization.Customize( IFixture fixture )
		{
			fixture.Register( () => new SpPortalConfiguration(
				ConfigurationManager.AppSettings["Username"], 
				ConfigurationManager.AppSettings["Password"],
				ConfigurationManager.AppSettings[ "PortalBaseUrl" ] ) );
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
