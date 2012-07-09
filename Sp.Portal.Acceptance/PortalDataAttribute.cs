/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Portal.Acceptance
{
	using System;
	using System.Configuration;
	using System.Net;
	using Ploeh.AutoFixture;
	using Ploeh.AutoFixture.Xunit;
	using Sp.Portal.Acceptance.Wrappers;

	class PortalDataAttribute : AutoDataAttribute
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
