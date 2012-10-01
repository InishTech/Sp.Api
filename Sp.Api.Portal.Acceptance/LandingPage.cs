﻿/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Api.Portal.Acceptance
{
	using HtmlAgilityPack;
	using RestSharp;
	using Sp.Api.Portal.Acceptance.Wrappers;
	using System.Net;
	using Xunit;
	using Xunit.Extensions;

	public static class LandingPage
	{
		[Theory, AutoPortalDataAttribute]
		public static void LandingPageShouldReturnHtml( SpPortalApi portalApi)
		{
			var request = new RestRequest( string.Empty );
			request.AddHeader( "Accept", "text/html" );
			var response = portalApi.Execute( request );
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
			Assert.True( response.ContentType.StartsWith( "text/html" ) );
		}

		[Theory, AutoPortalDataAttribute]
		public static void LandingPageShouldContainSignedInCustomerId( SpPortalApi portalApi )
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

		[Theory, AutoPortalDataAttribute]
		public static void SignoffShouldRedirectBackToSts( SpPortalApi portalApi )
		{
			LandingPageShouldReturnHtml( portalApi );

			var result = portalApi.SignOff();
			Assert.Equal( HttpStatusCode.OK, result.StatusCode );
			Assert.Contains( "Sp.Auth.Sts", result.ResponseUri.AbsolutePath );
		}
	}
}