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
