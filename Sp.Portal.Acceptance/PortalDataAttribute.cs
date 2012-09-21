/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Portal.Acceptance
{
	using Ploeh.AutoFixture;
	using Ploeh.AutoFixture.Xunit;
	using System;
	using System.Configuration;
	using System.Net;

	class PortalDataAttribute : AutoDataAttribute
	{
		public PortalDataAttribute()
			: base( new SoftwarePotentialPortalDataFixture()
				.Customize( new SkipSSlCertificateValidationIfRequestedCustomization() ) )
		{
			Fixture.Inject<IFixture>( Fixture );
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
