/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
using System.Linq;
using System.Net;
using RestSharp;
using Xunit;

namespace Sp.Api.Customer.Acceptance
{
	public class ServiceInstanceFixture
	{
		public SpAuthApi.ServiceInstance ServiceInstance { get; private set; }

		public ServiceInstanceFixture( SpAuthApi api )
		{
			var response = api.GetServiceInstances();
			Assert.Equal( ResponseStatus.Completed, response.ResponseStatus );
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
			ServiceInstance = response.Data.results.First();
		}
	}
}