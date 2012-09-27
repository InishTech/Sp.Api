using RestSharp;
using Sp.Portal.Acceptance.Wrappers;
using Sp.Test.Helpers;
using System.Linq;
using System.Net;
using Xunit;

namespace Sp.Portal.Acceptance
{
	public class RandomLicenseFromListFixture
	{
		readonly SpPortalApi.License _randomItem;

		public RandomLicenseFromListFixture( SpPortalApi api )
		{
			var apiResult = api.GetLicenses();
			Assert.Equal( ResponseStatus.Completed, apiResult.ResponseStatus );
			Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
			Assert.True( apiResult.Data.Licenses.Any(), GetType().Name + " requires the target login to have at least one License" );
			_randomItem = apiResult.Data.Licenses.ElementAtRandom();
		}

		public SpPortalApi.License Selected
		{
			get { return _randomItem; }
		}
	}
}