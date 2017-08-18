/* Copyright (c) 2017 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Test.Html
{
	using OpenQA.Selenium.Chrome;
	using OpenQA.Selenium.Remote;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	public class BrowserDriverDataProvider : TupleTheoryDataProvider<RemoteWebDriver>
	{
		protected override IEnumerable<RemoteWebDriver> Generate()
		{
			yield return new ChromeDriver();
		}
	}

	public static class QuitGuardExtensions
	{
		public static IDisposable FinallyQuitGuard( this RemoteWebDriver that )
		{
			return new LambdaDisposable( that.Quit );
		}

		class LambdaDisposable : IDisposable
		{
			Action _action;

			public LambdaDisposable( Action action )
			{
				_action = action;
			}

			void IDisposable.Dispose()
			{
				if ( _action == null )
					throw new ObjectDisposedException( "Should not be triggering a LambdaDisposable twice" );

				try
				{
					_action();
				}
				finally
				{
					_action = null;
				}
			}
		}
	}

	public abstract class TupleTheoryDataProvider<T1> : TheoryDataProvider
	{
		protected abstract IEnumerable<T1> Generate();

		protected override sealed IEnumerable<object[]> DataSource()
		{
			return Generate().Select( x => new object[] { x } );
		}
	}

	public abstract class TheoryDataProvider : IEnumerable<object[]>
	{
		public IEnumerator<object[]> GetEnumerator()
		{
			return DataSource().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return DataSource().GetEnumerator();
		}

		protected abstract IEnumerable<object[]> DataSource();
	}
}