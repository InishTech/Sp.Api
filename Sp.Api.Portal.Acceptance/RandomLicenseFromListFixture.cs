/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Api.Portal.Acceptance
{
	using RestSharp;
	using Sp.Api.Portal.Acceptance.Wrappers;
	using Sp.Test.Helpers;
	using System.Linq;
	using System.Net;
	using Xunit;

	public class RandomLicenseFromListFixture
	{
		readonly SpPortalApi.License _randomItem;

		public RandomLicenseFromListFixture( SpPortalApi api )
		{
			var apiResult = api.GetLicenses();
			Assert.Equal( ResponseStatus.Completed, apiResult.ResponseStatus );
			Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
			Assert.True( apiResult.Data.results.Any(), GetType().Name + " requires the target login to have at least one License" );
			_randomItem = apiResult.Data.results.ElementAtRandom();
		}

		public SpPortalApi.License Selected
		{
			get { return _randomItem; }
		}
	}
}