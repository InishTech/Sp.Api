namespace Sp.Portal.Acceptance
{
	using System.Net;
	using HtmlAgilityPack;
	using RestSharp;
	using Sp.Portal.Acceptance.Wrappers;
	using Xunit;
	using Xunit.Extensions;

	public static class LandingPageFacts
	{
		[Theory, PortalData]
		public static void LandingPageShouldReturnHtml( SpPortalApi portalApi)
		{
			var request = new RestRequest( string.Empty );
			request.AddHeader( "Accept", "text/html" );
			var response = portalApi.Execute( request );
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
			Assert.True( response.ContentType.StartsWith( "text/html" ) );
		}

		[Theory, PortalData]
		public static void LandingPageShouldContainSignedCustomerId( SpPortalApi portalApi )
		{
			var request = new RestRequest( string.Empty );
			request.AddHeader( "Accept", "text/html" );
			var response = portalApi.Execute( request );
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
			Assert.True( response.ContentType.StartsWith( "text/html" ) );

			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml( response.Content );
			var node = doc.DocumentNode.SelectSingleNode( "//span[@data-claimid='customerid']" );
			Assert.NotNull( node );
			Assert.Contains( "18840545-4947-49e3-9639-dd13d9dbd615", node.InnerText );
		}

		[Theory, PortalData]
		public static void SignoffShouldRedirectBackToSts( SpPortalApi portalApi )
		{
			LandingPageShouldReturnHtml( portalApi );

			var result = portalApi.SignOff();
			Assert.Equal( HttpStatusCode.OK, result.StatusCode );
			Assert.Contains( "Sp.Auth.Web", result.ResponseUri.AbsolutePath );
		}
	}
}
