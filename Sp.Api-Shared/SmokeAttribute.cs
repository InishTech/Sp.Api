/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
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
}
