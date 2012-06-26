using System;
using System.Diagnostics;
using System.Threading;

namespace Sp.Api.ProductManagement.Acceptance.Helpers
{
	static partial class Verify
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

