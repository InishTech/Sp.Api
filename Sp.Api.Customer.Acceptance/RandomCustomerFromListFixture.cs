using System.Linq;
using System.Net;
using Sp.Test.Helpers;
using Xunit;

namespace Sp.Api.Customer.Acceptance
{
	public class RandomCustomerFromListFixture
	{
		readonly SpCustomerApi.CustomerSummary _randomItem;

		public RandomCustomerFromListFixture( SpCustomerApi api )
		{
			var apiResult = api.GetCustomerList();
			Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
			Assert.True( apiResult.Data.Customers.Any(), GetType().Name + " requires the target login to have at least one Customer" );
			_randomItem = apiResult.Data.Customers.ElementAtRandom();
		}

		public SpCustomerApi.CustomerSummary Selected
		{
			get { return _randomItem; }
		}
	}
}