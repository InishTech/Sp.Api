using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Sp.Portal.Acceptance
{
	static class AssertExtensions
	{
		//public static void SequenceEqual<T, T2>( IEnumerable<T> left, IEnumerable<T> right, Func<T, T2> keySelector, Func<T, T, bool> comparer )
		//{
		//	Assert.Equal( left.OrderBy( keySelector ), right.OrderBy( keySelector ), new FuncEqualityComparer<T>( comparer ) );
		//}

		public static void SortedSequenceEqual<T, T2>( IEnumerable<T> left, IEnumerable<T> right, Func<T, T2> keySelector )
		{
			Assert.Equal( left.OrderBy( keySelector ), right.OrderBy( keySelector ) );
		}

		public static void Contains<T>( T expected, IEnumerable<T> collection, Func<T, T, bool> comparer )
		{
			Assert.Contains( expected, collection, new FuncEqualityComparer<T>( comparer ) );
		}

		// http://stackoverflow.com/a/3719617
		public class FuncEqualityComparer<T> : IEqualityComparer<T>
		{
			readonly Func<T, T, bool> _comparer;
			readonly Func<T, int> _hash;

			public FuncEqualityComparer( Func<T, T, bool> comparer )
				: this( comparer, t => 0 ) // NB Cannot assume anything about how e.g., t.GetHashCode() interacts with the comparer's behavior
			{
			}

			public FuncEqualityComparer( Func<T, T, bool> comparer, Func<T, int> hash )
			{
				_comparer = comparer;
				_hash = hash;
			}

			public bool Equals( T x, T y )
			{
				return _comparer( x, y );
			}

			public int GetHashCode( T obj )
			{
				return _hash( obj );
			}
		}
	}
}
