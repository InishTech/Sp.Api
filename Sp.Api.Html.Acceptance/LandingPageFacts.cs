namespace Sp.Api.Html.Acceptance
{
	using System.Net;
	using HtmlAgilityPack;
	using RestSharp;
	using Sp.Api.Shared;
	using Sp.Api.Shared.Wrappers;
	using Xunit;
	using Xunit.Extensions;

	//TODO - change this tests to use Selenium
	public static class LandingPageFacts
	{
		[Theory, AutoSoftwarePotentialData]
		public static void LandingPageShouldReturnHtml( SpApi spApi )
		{
			var request = new RestRequest( ApiPrefix.WebApiRoot );
			request.AddHeader( "Accept", "text/html" );
			var response = spApi.Execute( request );
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
			Assert.True( response.ContentType.StartsWith( "text/html" ) );
		}

		[Theory, AutoSoftwarePotentialData]
		public static void LandingPageShouldContainSignedVendorId( SpApi spApi )
		{
			var request = new RestRequest( ApiPrefix.WebApiRoot );
			request.AddHeader( "Accept", "text/html" );
			var response = spApi.Execute( request );
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
			Assert.True( response.ContentType.StartsWith( "text/html" ) );

			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml( response.Content );
			var node = doc.DocumentNode.SelectSingleNode( "//span[@data-claimid='vendorid']" );
			Assert.NotNull( node );
			Assert.Contains( "bff714f1-3c88-40e7-9e78-a73c041ac8eb", node.InnerText );
		}

		[Theory, AutoSoftwarePotentialData]
		public static void SignoffShouldRedirectBackToSts( SpApi spApi )
		{
			LandingPageShouldReturnHtml( spApi );

			var result = spApi.SignOff();
			Assert.Equal( HttpStatusCode.OK, result.StatusCode );
			Assert.Contains( "Sp.Auth.Web", result.ResponseUri.AbsolutePath );
		}
	}
}
