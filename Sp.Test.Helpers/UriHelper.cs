/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Test.Helpers
{
	using System;

	public class UriHelper
	{
		/// <summary>
		/// Returns resource Uri relative to the base Uri.
		/// If resource Uri starts with a segment that is also present in baseUri, it will be removed in the result
		/// Example:
		/// baseUri=http://somehost/MyApp1, resource=/MyApp/user/1 => result=user/1
		/// baseUri=http://somehost, resource=/MyApp/user/1 => result=MyApp/user/1
		/// baseUri=http://somehost/MyApp1, resource=/MyApp => result=
		/// baseUri=http://somehost/MyApp1, resource=SomeResource => result=SomeResource
		/// </summary>
		/// <param name="baseUri">Base Uri</param>
		/// <param name="resource">Relative Uri of the resource</param>
		/// <returns></returns>
		public static Uri MakeUriRelativeToBase( string baseUri, string resource )
		{
			//TODO - we should check if resource is a relative Uri
			//TODO - fix requests starting with / which are already relative, e.g.:
			//baseUri=http://somehost/MyApp1, resource=/SomeResource => result=SomeResource (at the moment returns htp://somehost/SomeResource)
			//baseUri=http://somehost/MyApp1, resource=/ => result= (at the moment returns htp://somehost/)

			Uri baseUriEndingWithSlash = baseUri.EndsWith( "/" )
				? new Uri( baseUri )
				: new Uri( baseUri + "/" );
			var requestUriAbsolute = new Uri( baseUriEndingWithSlash, resource );
			var uriRelativeToClientBaseUri = baseUriEndingWithSlash.MakeRelativeUri( requestUriAbsolute );
			return uriRelativeToClientBaseUri;
		}

	}
}