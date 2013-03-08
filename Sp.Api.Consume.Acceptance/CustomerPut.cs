/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Api.Consume.Acceptance
{
	using Ploeh.AutoFixture;
	using Ploeh.AutoFixture.Xunit;
	using Ploeh.SemanticComparison.Fluent;
	using Sp.Api.Shared;
	using Sp.Test.Helpers;
	using System;
	using System.Net;
	using Xunit;
	using Xunit.Extensions;

	public static class CustomerPut
	{
		[Theory, AutoSoftwarePotentialApiData]
		public static void UnmodifiedShouldYieldAccepted( [Frozen] SpCustomerApi api, IFixture fixture )
		{
			HandleTightLoopEdgeCase( () =>
				fixture.Do( ( GetRandomCustomerFixture customer ) =>
				{
					SpCustomerApi.CustomerSummary customerData = customer.DataFromGet;
					var apiResult = api.PutCustomer( customerData._links.self.href, customerData );
					Assert.Equal( HttpStatusCode.Accepted, apiResult.StatusCode );
				} ) );
		}

		[Theory, AutoSoftwarePotentialApiData]
		public static void ModifiedShouldYieldAccepted( [Frozen] SpCustomerApi api, string anonymousName, string anonymousExternalId, IFixture fixture )
		{
			var customerData = default( SpCustomerApi.CustomerSummary );
			HandleTightLoopEdgeCase( () =>
				fixture.Do( ( GetRandomCustomerFixture customer ) =>
				{
					customerData = customer.DataFromGet;
					customerData.Name = anonymousName;
					customerData.ExternalId = anonymousExternalId;
					var apiResult = api.PutCustomer( customerData._links.self.href, customerData );
					Assert.Equal( HttpStatusCode.Accepted, apiResult.StatusCode );
				} ) );

			VerifyGetCustomerReflectsAcceptedChanges( api, customerData );
		}

		public static class PutConflictingChangesToSameBaseData
		{
			[Theory, AutoSoftwarePotentialApiData]
			public static void NonMatchingChangesShouldYieldConflict( [Frozen] SpCustomerApi api, string anonymousName1, string anonymousExternalId1, string anonymousName2, string anonymousExternalId2, IFixture fixture )
			{
				var customerData = default( SpCustomerApi.CustomerSummary );
				HandleTightLoopEdgeCase( () =>
					fixture.Do( ( GetRandomCustomerFixture customer ) =>
					{
						customerData = customer.DataFromGet;
						customerData.Name = anonymousName1;
						customerData.ExternalId = anonymousExternalId1;
						var firstResult = api.PutCustomer( customerData._links.self.href, customerData );
						Assert.Equal( HttpStatusCode.Accepted, firstResult.StatusCode );
					} ) );

				customerData.Name = anonymousName2;
				customerData.ExternalId = anonymousExternalId2;
				var apiResult = api.PutCustomer( customerData._links.self.href, customerData );
				Assert.Equal( HttpStatusCode.Conflict, apiResult.StatusCode );

				// Our second edits should not have been applied, so revert
				customerData.Name = anonymousName1;
				customerData.ExternalId = anonymousExternalId1;
				VerifyGetCustomerReflectsAcceptedChanges( api, customerData );
			}

			/// <summary>
			/// <para>The concurrency checking is actually performed based on a _version reference number that is kept alongside the data.</para>
			/// <para>This means the very attempt to edit (even if setting the same values as the previous editor has submitted) will be refused.</para>
			/// </summary>
			[Theory, AutoSoftwarePotentialApiData]
			public static void MatchingChangesShouldYieldConflict( [Frozen] SpCustomerApi api,  string anonymousName, string anonymousExternalId, IFixture fixture )
			{
				var customerData = default( SpCustomerApi.CustomerSummary );
				HandleTightLoopEdgeCase( () =>
					fixture.Do( ( GetRandomCustomerFixture customer ) =>
					{
						customerData = customer.DataFromGet;
						customerData.Name = anonymousName;
						customerData.ExternalId = anonymousExternalId;
						var firstResult = api.PutCustomer( customerData._links.self.href, customerData );
						Assert.Equal( HttpStatusCode.Accepted, firstResult.StatusCode );
					} ) );

				var apiResult = api.PutCustomer( customerData._links.self.href, customerData );
				Assert.Equal( HttpStatusCode.Conflict, apiResult.StatusCode );

				VerifyGetCustomerReflectsAcceptedChanges( api, customerData );
			}
		}

		/// <summary>
		/// In the course of our testing, we need to be assured the changes actually get applied.
		/// In the standard programming model, it is safe to assume success once one has attained an HttpStatusCode.Accept on the PUT - in the rare case of conflicts, this will be detected and handled in the normal way.
		/// For the purposes of the test, we are actually prepared to hang around to double-check that they really do get applied.
		/// </summary>
		static void VerifyGetCustomerReflectsAcceptedChanges( SpCustomerApi api, SpCustomerApi.CustomerSummary customerData )
		{
			Verify.EventuallyWithBackOff( () =>
			{
				var apiResult = api.GetCustomer( customerData._links.self.href );
				Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );

				apiResult.Data.AsSource().OfLikeness<SpCustomerApi.CustomerSummary>()
					.OmitAutoComparison()
					.WithDefaultEquality( x => x.ExternalId )
					.WithDefaultEquality( x => x.Name )
					.ShouldEqual( customerData );
			} );
		}

		/// <summary>
		/// In the normal course of events, a GET should yield up-to-date information. When running a set of tests in sequence, all pumping messages through, in some cases the GET can yield stale data.
		/// This can cause an update to yield a Conflict even on the initial PUT.
		/// The solution in this case is to effectively restart the transaction back at the start, taking the latest version of the data.
		/// </summary>
		/// <param name="action"></param>
		static void HandleTightLoopEdgeCase( Action action )
		{
			Verify.EventuallyWithBackOff( action );
		}
	}
}