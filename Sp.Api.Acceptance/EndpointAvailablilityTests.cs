﻿/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Api.Acceptance
{
	using Sp.Api.Shared;
	using Sp.Api.Shared.Wrappers;
	using System;
	using System.Net;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using Xunit;
	using Xunit.Extensions;

	public class EndpointAvailablilityTests
	{
		[HighFrequency]
		[Theory]
        [InlineAutoSoftwarePotentialApiData( ApiType.WebApiRoot )]
        [InlineAutoSoftwarePotentialApiData( ApiType.Issue )]
		[InlineAutoSoftwarePotentialApiData( ApiType.Consume )]
		[InlineAutoSoftwarePotentialApiData( ApiType.Develop )]
		[InlineAutoSoftwarePotentialApiData( ApiType.Analyze )]
		public void Subsystems( ApiType apiType, SpApiConfiguration config )
		{
			var getHtmlPrefix = config.GetHtmlPrefix( apiType );

            var statusCode = HttpHelper.GetStatusCode( config.BaseUrl, getHtmlPrefix);

			Assert.Equal( HttpStatusCode.OK, statusCode );
		}

		[HighFrequency]
		[Theory, AutoSoftwarePotentialApiData]
		public void Nuget( SpApiConfiguration config )
		{
			var statusCode = HttpHelper.GetStatusCode( config.BaseUrl, config.GetHtmlPrefix( ApiType.Nuget ) );

			Assert.Equal( HttpStatusCode.Unauthorized, statusCode );
		}

		[HighFrequency]
		[Theory, AutoSoftwarePotentialApiData]
		public void AuthSubsystem( SpApiConfiguration config )
		{
			var statusCode = HttpHelper.GetStatusCode( config.AuthBaseUrl );

			Assert.Equal( HttpStatusCode.OK, statusCode );
		}

		[HighFrequency]
		[Theory, AutoSoftwarePotentialApiData]
		public void Sts( SpApiConfiguration config )
		{
			var statusCode = HttpHelper.GetStatusCode( config.Authority, ".well-known/openid-configuration" );

			Assert.Equal( HttpStatusCode.OK, statusCode );
		}

		static class HttpHelper
		{
			static readonly HttpClient _client;

			static HttpHelper()
			{
				ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
				_client = new HttpClient();
			}

			public static HttpStatusCode GetStatusCode( string baseUrl, string subsystmePrefix = "")
			{
				var request = new HttpRequestMessage() { RequestUri = new Uri( EnsureTrailingSlash( baseUrl ) + subsystmePrefix ) };
				request.Headers.Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );

				return _client.SendAsync( request ).Result.StatusCode;
			}

			static string EnsureTrailingSlash( string input ) => input.EndsWith( "/" ) ? input : input + "/";
		}
	}
}