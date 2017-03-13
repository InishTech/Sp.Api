/* Copyright (c) 2017 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
using System;
using Xunit;

namespace Sp.Api.Shared
{
	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Method )]
	class SmokeAttribute : TraitAttribute
	{
		public SmokeAttribute()
			: base( "Smoke", bool.TrueString )
		{
		}
	}

	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Method )]
	class HighFrequencyAttribute : TraitAttribute
	{
		public HighFrequencyAttribute()
			: base( "Frequency", "High" )
		{
		}
	}

	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Method )]
	class MediumFrequencyAttribute : TraitAttribute
	{
		public MediumFrequencyAttribute()
			: base( "Frequency", "Medium" )
		{
		}
	}

	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Method )]
	class LowFrequencyAttribute : TraitAttribute
	{
		public LowFrequencyAttribute()
			: base( "Frequency", "Low" )
		{
		}
	}
}
