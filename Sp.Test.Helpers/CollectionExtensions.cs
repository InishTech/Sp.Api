﻿namespace Sp.Test.Helpers
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