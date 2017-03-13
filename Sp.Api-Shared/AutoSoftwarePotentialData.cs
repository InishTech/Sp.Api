/* Copyright (c) 2017 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Api.Shared
{
	using Ploeh.AutoFixture;
	using Ploeh.AutoFixture.Xunit;
	using Sp.Api.Shared.Wrappers;
	using System;
	using System.Configuration;

	public class AutoSoftwarePotentialData : AutoDataAttribute
	{
		public AutoSoftwarePotentialData()
			: base( new SoftwarePotentialDataFixture(  ) )
		{
		}
	}

	public class SoftwarePotentialDataFixture : Fixture
	{
		public SoftwarePotentialDataFixture()
		{
			Customize( new SoftwarePotentialApiConfigurationFromAppSettingsCustomization() );
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
		}
	}
}