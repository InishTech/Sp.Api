/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Test.Helpers
{
	using System;
	using System.Diagnostics;
	using System.Threading;

	public static partial class Verify
	{
		public static void EventuallyWithBackOff( Action action )
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			int maxAttempts = 5;
			for ( int attempt = 0; attempt < maxAttempts; attempt++ )
				try
				{
					action();
					Debug.WriteLine( String.Format( "Assertion succeeded with {0} attempts in {1}", attempt + 1, stopwatch.Elapsed ) );
					return;
				}
				catch ( Exception ex )
				{
					if ( attempt == maxAttempts - 1 )
						throw new TimeoutException( String.Format( "Assertion failed with {0} attempts in {1}", attempt + 1, stopwatch.Elapsed ), ex );
					Thread.Sleep( TimeSpan.FromMilliseconds( 500 * Math.Pow( 2, attempt ) ) );
				}
		}
	}
}