/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
using System;
using System.Linq;
using System.Net;
using RestSharp;
using Sp.Api.Shared;
using Xunit;
using Xunit.Extensions;

namespace Sp.Api.Customer.Acceptance
{
	public class ServiceInstanceList
	{
		[Theory, AutoSoftwarePotentialApiData]
		public static void ShouldAlwaysBeAvailable( SpAuthApi api )
		{
			var response = api.GetServiceInstances();
			Assert.Equal( ResponseStatus.Completed, response.ResponseStatus );
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
		}

		[Theory, AutoSoftwarePotentialApiData]
		public static void ShouldHaveExactlyOneItem( SpAuthApi api )
		{
			var response = api.GetServiceInstances();
			Assert.Equal( ResponseStatus.Completed, response.ResponseStatus );
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
			Assert.Single( response.Data.results );
		}

		public class SingleItem
		{
			[Theory, AutoSoftwarePotentialApiData]
			public static void SingleItemShouldHaveData( SpAuthApi api )
			{
				var response = api.GetServiceInstances();
				var item = response.Data.results.Single();

				Assert.NotEqual( Guid.Empty, item.Id );
				Assert.NotNull( item.Name );
				Assert.NotEmpty( item.Name );
			}

			[Theory, AutoSoftwarePotentialApiData]
			public static void SingleItemShouldPertainToPortal( SpAuthApi api )
			{
				var response = api.GetServiceInstances();
				var item = response.Data.results.Single();

				Assert.Contains( "Portal", item.ServiceName );
			}
		}
	}
}
