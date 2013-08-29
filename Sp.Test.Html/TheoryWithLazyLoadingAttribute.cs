using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Extensions;
using Xunit.Sdk;

namespace Sp.Test.Html
{
	public class TheoryWithLazyLoadingAttribute : TheoryAttribute
	{
		/// <summary>
		/// Code copied (mostly) from Xunit.Extensions.Theory.
		/// Lazily iterates over data items (as opposed to OOTB TheoryAttribute behaviour)
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		protected override IEnumerable<ITestCommand> EnumerateTestCommands( IMethodInfo method )
		{
			//Lazily enumerate and convert theory data items to TheoryCommands
			foreach ( object[] dataItems in GetDataWrapped( method.MethodInfo ) )
			{
				IMethodInfo testMethod = method;
				Type[] resolvedTypes = null;

				if ( method.MethodInfo != null && method.MethodInfo.IsGenericMethodDefinition )
				{
					resolvedTypes = ResolveGenericTypesWrapped( method, dataItems );
					testMethod = Reflector.Wrap( method.MethodInfo.MakeGenericMethod( resolvedTypes ) );
				}
				yield return new TheoryCommand( testMethod, dataItems, resolvedTypes );
			}
		}

		#region TheoryAttribute private method caller wrappers
		private Type[] ResolveGenericTypesWrapped( IMethodInfo method, object[] dataItems )
		{
			//HACK - ResolveGenericTypes method is private in the base class (TheoryAttribute), so we use Reflection to call it
			MethodInfo dynMethod = typeof( TheoryAttribute ).GetMethod( "ResolveGenericTypes", BindingFlags.NonPublic | BindingFlags.Static );
			return (Type[])dynMethod.Invoke( this, new object[] { method, dataItems } );
		}

		private IEnumerable<object[]> GetDataWrapped( MethodInfo methodInfo )
		{
			//HACK - GetData method is private in the base class (TheoryAttribute), so we use Reflection to call it
			MethodInfo dynMethod = typeof( TheoryAttribute ).GetMethod( "GetData", BindingFlags.NonPublic | BindingFlags.Static );
			return (IEnumerable<object[]>)dynMethod.Invoke( this, new object[] { methodInfo } );
		}
		#endregion
	}
}