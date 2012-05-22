namespace Sp.Portal.Acceptance
{
	using System.Net;
	using RestSharp;
	using Sp.Portal.Acceptance.Wrappers;
	using Xunit;
	using Xunit.Extensions;

	public static class LandingPageFacts
	{
		[Theory, PortalData]
		public static void LandingPageShouldReturnHtml( SpPortalApi portalApi)
		{
			var request = new RestRequest( "/" );
			request.AddHeader( "Accept", "text/html" );
			var response = portalApi.Execute( request );
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
			Assert.True( response.ContentType.StartsWith( "text/html" ) );
		}
	}
}
