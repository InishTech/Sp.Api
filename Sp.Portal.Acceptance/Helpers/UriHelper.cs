using System;

namespace Sp.Portal.Acceptance.Helpers
{
	class UriHelper
	{
		/// <summary>
		/// Returns resource Uri relative to the base Uri.
		/// If resource Uri contains a segment that is also present in baseUri, it will be removed in the result
		/// Example:
		/// baseUri=http://somehost/MyApp1, resource=MyApp/user/1 => result=/user/1
		/// baseUri=http://somehost, resource=MyApp/user/1 => result=/MyApp/user/1
		/// baseUri=http://somehost/MyApp1, resource=/ => result=/
		/// </summary>
		/// <param name="baseUri">Base Uri</param>
		/// <param name="resource">Relative Uri of the resource</param>
		/// <returns></returns>
		public static Uri MakeUriRelativeToBase( string baseUri, string resource )
		{
			//TODO - we should check if resource is a relative Uri
			//TODO - we should check if resource is just '/' and in such case return baseUri only
			//TODO - we should check if resource starts with '/' (this may chop off everything after Authority part in baseUri)
			//if ( resource.StartsWith( "/" ) )
			//    resource = resource.Substring( 1, resource.Length - 1 );

			Uri baseUriEndingWithSlash = baseUri.EndsWith( "/" )
				? new Uri( baseUri )
				: new Uri( baseUri + "/" );
			var requestUriAbsolute = new Uri( baseUriEndingWithSlash, resource );
			var uriRelativeToClientBaseUri = baseUriEndingWithSlash.MakeRelativeUri( requestUriAbsolute );
			return uriRelativeToClientBaseUri;
		}

	}
}
