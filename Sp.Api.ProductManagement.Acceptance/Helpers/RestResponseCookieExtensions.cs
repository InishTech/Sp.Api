using System.Net;
using RestSharp;


namespace Sp.Api.ProductManagement.Acceptance.Helpers
{
	static class RestResponseCookieExtensions
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
