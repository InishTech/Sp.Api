/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Api.Customer.Html.Acceptance
{
	using OpenQA.Selenium;
	using OpenQA.Selenium.Remote;
	using OpenQA.Selenium.Support.UI;
	using Ploeh.AutoFixture.Kernel;
	using Ploeh.AutoFixture.Xunit;
	using Sp.Api.Shared;
	using Sp.Api.Shared.Wrappers;
	using Sp.Test.Html;
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Reflection;
	using Xunit.Extensions;

	public class CustomerList
	{
		[Theory, ClassData( typeof( RemoteWebDriverAndAuthenticatingNavigatorProvider<SoftwarePotentialDataFixture> ) )]
		public static void ShouldIncludeAtLeastOneCustomer( RemoteWebDriver driver, AuthenticatingNavigator navigator )
		{
			using ( driver.FinallyQuitGuard() ) // TODO improve this using http://xunit.codeplex.com/workitem/9798 ( WAS: http://xunit.codeplex.com/discussions/362097 )
			{
				navigator.NavigateWithAuthenticate( driver, "Sp.Web.CustomerManagement" );
				// If we cannot respond in 5 seconds for any reason, a human will seriously distrust the software, no excuses
				WebDriverWait wait = new WebDriverWait( driver, TimeSpan.FromSeconds( 5 ) );
				wait.Until( d => d
					.FindElement( By.Id( "customer-list" ) )
					.FindElements( By.TagName( "li" ) )
					.Count > 0 );
			}
		}

		[Theory]
		[ClassAutoData( typeof( XProvider ) )]
		public static void Scenario( X x, string y )
		{
			Console.WriteLine( x + "," + y );
		}

		public class ClassAutoDataAttribute : CompositeDataAttribute
		{
			public ClassAutoDataAttribute( Type @class )
				: base( new DataAttribute[] { new ClassDataAttribute( @class ), new AutoDataAttributeHack() } )
			{
			}

			public override IEnumerable<object[]> GetData( System.Reflection.MethodInfo methodUnderTest, Type[] parameterTypes )
			{
				return
					from result in Attributes.First().GetData( methodUnderTest, parameterTypes ).ToArray()
					select result.Concat( Attributes.ElementAt( 1 ).GetData( methodUnderTest, parameterTypes.Skip( result.Length ).ToArray() ).First() ).ToArray();
			}
		}

		public class AutoDataAttributeHack : AutoDataAttribute
		{
			public override IEnumerable<object[]> GetData( MethodInfo methodUnderTest, Type[] parameterTypes )
			{
				if ( methodUnderTest == null )
				{
					throw new ArgumentNullException( "methodUnderTest" );
				}

				// BEGIN DIFF
				var specimens = new List<object>();
				var methodParams = methodUnderTest.GetParameters();
				foreach ( var p in methodParams.Skip( methodParams.Length - parameterTypes.Length ) )
				// END DIFF
				{
					this.CustomizeFixture( p );

					var specimen = this.Resolve( p );
					specimens.Add( specimen );
				}

				return new[] { specimens.ToArray() };
			}

			private void CustomizeFixture( ParameterInfo p )
			{
				var dummy = false;
				var customizeAttributes = p.GetCustomAttributes( typeof( CustomizeAttribute ), dummy ).OfType<CustomizeAttribute>();
				foreach ( var ca in customizeAttributes )
				{
					var c = ca.GetCustomization( p );
					this.Fixture.Customize( c );
				}
			}

			private object Resolve( ParameterInfo p )
			{
				var context = new SpecimenContext( this.Fixture.Compose() );
				return context.Resolve( p );
			}
		}

		class XProvider : TupleTheoryDataProvider<X>
		{
			protected override IEnumerable<X> Generate()
			{
				return Enumerable.Repeat( X.Create(), 2 );
			}
		}

		public class X
		{
			private X() { }

			public static X Create()
			{
				return new X();
			}
		}
	}
}