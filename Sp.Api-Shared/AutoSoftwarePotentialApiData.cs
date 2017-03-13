/* Copyright (c) 2017 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Api.Shared
{
	using Ploeh.AutoFixture;
	using Ploeh.AutoFixture.Xunit;
	using System;
	using System.Configuration;
	using System.Net;

	public class AutoSoftwarePotentialApiData : AutoDataAttribute
	{
		public AutoSoftwarePotentialApiData()
			: this( new Fixture()
				.Customize( new SkipSSlCertificateValidationIfRequestedCustomization() )
				.Customize( new SoftwarePotentialApiConfigurationFromAppSettingsCustomization() ) )
		{
		}

		AutoSoftwarePotentialApiData( IFixture fixture )
			: base( fixture )
		{
			fixture.Inject( fixture );
		}
	}

	public class InlineAutoSoftwarePotentialApiDataAttribute : InlineAutoDataAttribute
	{
		public InlineAutoSoftwarePotentialApiDataAttribute( params object[] args )
			: base( new AutoSoftwarePotentialApiData(), args )
		{
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