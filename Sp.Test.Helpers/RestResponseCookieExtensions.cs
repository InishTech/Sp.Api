/* Copyright (c) 2017 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Test.Helpers
{
	using System.Net;
	using RestSharp;

	public static class RestResponseCookieExtensions
	{
		public static Cookie ToHttpCookie( this RestResponseCookie restResponseCookie )
		{
			return new Cookie( restResponseCookie.Name, restResponseCookie.Value, RemoveTrailingSlash( restResponseCookie.Path ), restResponseCookie.Domain ) { Expires = restResponseCookie.Expires, Expired = restResponseCookie.Expired, HttpOnly = restResponseCookie.HttpOnly };
		}

		static string RemoveTrailingSlash( string path )
		{
			if ( string.IsNullOrEmpty( path ) )
				return path;

			if ( path[ path.Length - 1 ] == '/' )
				return path.Substring( 0, path.Length - 1 );
			return path;
		}
	}
}