namespace Sp.Web.Acceptance
{
	using System.Net;
	using HtmlAgilityPack;
	using RestSharp;
	using Sp.Web.Acceptance.Wrappers;
	using Xunit;
	using Xunit.Extensions;

	public static class LandingPageFacts
	{
		[Theory, WebData]
		public static void LandingPageShouldReturnHtml( SpWeb webApi )
		{
			var request = new RestRequest( string.Empty );
			request.AddHeader( "Accept", "text/html" );
			var response = webApi.Execute( request );
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
			Assert.True( response.ContentType.StartsWith( "text/html" ) );
		}

		[Theory, WebData]
		public static void LandingPageShouldContainSignedCustomerId( SpWeb webApi )
		{
			var request = new RestRequest( string.Empty );
			request.AddHeader( "Accept", "text/html" );
			var response = webApi.Execute( request );
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
			Assert.True( response.ContentType.StartsWith( "text/html" ) );

			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml( response.Content );
			var node = doc.DocumentNode.SelectSingleNode( "//span[@data-claimid='vendorid']" );
			Assert.NotNull( node );
			Assert.Contains( "999", node.InnerText );
		}

		[Theory, WebData]
		public static void SignoffShouldRedirectBackToSts( SpWeb webApi )
		{
			LandingPageShouldReturnHtml( webApi );

			var result = webApi.SignOff();
			Assert.Equal( HttpStatusCode.OK, result.StatusCode );
			Assert.Contains( "Sp.Portal.Sts", result.ResponseUri.AbsolutePath );
		}
	}
}
