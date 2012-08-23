/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
using System.Collections.Generic;
namespace Sp.Test.Helpers
{
	using System;

	public class DateTimeEqualityTolerantComparer : IEqualityComparer<DateTime>
	{
		readonly long _toleranceTicks;

		public DateTimeEqualityTolerantComparer( TimeSpan tolerance )
		{
			_toleranceTicks = tolerance.Ticks;
		}

		public Boolean Equals( DateTime x, DateTime y )
		{
			var diff = x - y;
			return Math.Abs( diff.Ticks ) <= _toleranceTicks;
		}

		public Int32 GetHashCode( DateTime obj )
		{
			// We arent reliant on the hashing being efficient as we only have small datasets so we make this ultra-stable
			// See http://stackoverflow.com/questions/98033/wrap-a-delegate-in-an-iequalitycomparer
			return 0;
		}
	}
}

