/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Test.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public static class CollectionExtensions
	{
		public static T ElementAtRandom<T>( this ICollection<T> thats )
		{
			var random = new Random();
			return thats.ElementAt( random.Next( thats.Count ) );
		}
	}
}