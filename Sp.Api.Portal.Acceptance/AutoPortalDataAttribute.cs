/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Api.Portal.Acceptance
{
	using Ploeh.AutoFixture;
	using Ploeh.AutoFixture.Xunit;
	using Sp.Api.Shared.Wrappers;
	using System;
	using System.Configuration;
	using System.Net;

	/// <summary>
	/// This Custom AutoFixture.xUnit.net AutoDataAttribute allows xUnit theories to be provisioned with correctly initialized Api helpers automatically.
	/// See the specific Customizations for further details.
	/// </summary>
	class AutoPortalDataAttribute : AutoDataAttribute
	{
		public AutoPortalDataAttribute()
			: base( new SoftwarePotentialPortalDataFixture()
				.Customize( new SkipSSlCertificateValidationIfRequestedCustomization() ) )
		{
			// Allow tests to get a copy of AutoFixture by adding an IFixture parameter
			Fixture.Inject<IFixture>( Fixture );
		}
	}

	/// <summary>
	/// Wires up SpApiConfiguration instances to be provisioned with Url and Credentials from the config file.
	/// The config file gets the settings injected into it via custom Build Tasks, see build.ps1
	/// </summary>
	public class SoftwarePotentialPortalDataFixture : Fixture
	{
		public SoftwarePotentialPortalDataFixture(  )
		{
			Customize( new SoftwarePotentialApiConfigurationFromAppSettingsCustomization() );			
		}
		
		class SoftwarePotentialApiConfigurationFromAppSettingsCustomization : ICustomization
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

	/// <summary>
	/// We stub out Cert validation in our non-production environments [on an opt-in basis].
	/// </summary>
	class SkipSSlCertificateValidationIfRequestedCustomization : ICustomization
	{
		void ICustomization.Customize( IFixture fixture )
		{
			if ( Boolean.Parse( ConfigurationManager.AppSettings[ "SkipCertValidation" ] ) )
				ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
		}
	}
}