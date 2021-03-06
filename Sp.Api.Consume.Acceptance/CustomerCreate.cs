﻿/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */

namespace Sp.Api.Consume.Acceptance
{
	using Sp.Api.Shared;
	using Sp.Test.Helpers;
	using System;
	using System.Linq;
	using System.Net;
	using Xunit;
	using Xunit.Extensions;

	public class CustomerCreate
	{
		[Theory, AutoSoftwarePotentialApiData]
		public static void ShouldYieldAcceptedWithALocationHref( SpCustomerApi api, string anonymousCustomerName, string anonymousCustomerExternalId, Guid anonymousRequestId )
		{
			var response = api.CreateCustomer( new SpCustomerApi.CustomerSummary() { Name = anonymousCustomerName, ExternalId = anonymousCustomerExternalId, RequestId = anonymousRequestId } );
			
			Assert.Equal( HttpStatusCode.Accepted, response.StatusCode );
			string result = Assert.IsType<string>( response.Headers.Single( x => x.Name == "Location" ).Value );

			Assert.NotEmpty( result );
		}

		public class Fresh
		{
			[Smoke]
			[MediumFrequency]
			[Theory, AutoSoftwarePotentialApiData]
			public static void ShouldEventuallyBeGettable( SpCustomerApi api, string anonymousCustomerName, string anonymousCustomerExternalId, Guid anonymousRequestId )
			{
				string createdAtLocation = PutPendingCreate( api, anonymousCustomerName, anonymousCustomerExternalId, anonymousRequestId );

				Verify.EventuallyWithBackOff( () =>
				{
					var apiResult = api.GetCustomer( createdAtLocation );
					Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
					var resultData = apiResult.Data;
				} );
			}

			static string PutPendingCreate( SpCustomerApi api, string anonymousCustomerName, string anonymousCustomerExternalId, Guid anonymousRequestId )
			{
				var response = api.CreateCustomer( new SpCustomerApi.CustomerSummary() { Name = anonymousCustomerName, ExternalId = anonymousCustomerExternalId, RequestId = anonymousRequestId } );
				Assert.Equal( HttpStatusCode.Accepted, response.StatusCode );
				var createdAtLocation = Assert.IsType<string>( response.Headers.Single( x => x.Name == "Location" ).Value );
				return createdAtLocation;
			}
		}

		/// <summary>
		/// The following are examples of values that will be rejected as invalid.
		/// - Name is Mandatory with a Minimum Length of 1 and max of 100.
		/// - Description is Mandatory with a max length of 100 (empty is permitted).
		/// </summary>
		public static class InvalidData
		{
			public static class Name
			{
				[Theory, AutoSoftwarePotentialApiData]
				public static void PutNullShouldReject( SpCustomerApi api, string anonymousExternalId )
				{
					var response = api.CreateCustomer( new SpCustomerApi.CustomerSummary() { Name = null, ExternalId = anonymousExternalId } );
					Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
				}

				[Theory, AutoSoftwarePotentialApiData]
				public static void PutEmptyShouldReject( SpCustomerApi api, string anonymousExternalId )
				{
					var response = api.CreateCustomer( new SpCustomerApi.CustomerSummary() { Name = string.Empty, ExternalId = anonymousExternalId } );
					Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
				}

				[Theory, AutoSoftwarePotentialApiData]
				public static void PutExcessivelyLongShouldReject( SpCustomerApi api, string anonymousExternalId )
				{
					var response = api.CreateCustomer( new SpCustomerApi.CustomerSummary() { Name = new String( 'a', 101 ), ExternalId = anonymousExternalId } );
					Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
				}
			}

			public static class ExternalId
			{
				[Theory, AutoSoftwarePotentialApiData]
				public static void PutExcessivelyLongShouldReject( SpCustomerApi api, string anonymousName )
				{
					var response = api.CreateCustomer( new SpCustomerApi.CustomerSummary() { Name = anonymousName, ExternalId = new String( 'a', 101 ) } );
					Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
				}
			}
		}
	}
}