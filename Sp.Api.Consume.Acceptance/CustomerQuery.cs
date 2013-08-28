using Sp.Api.Shared;
using System.Linq;
using Xunit;
using Xunit.Extensions;

namespace Sp.Api.Consume.Acceptance
{
	public static class CustomerQuery
	{
		[Theory, AutoSoftwarePotentialApiData]
		static void FilterByKnownExternalIdShouldReturnOneOrMoreCustomersWithThatExternalId( SpCustomerApi api, RandomCustomerFromListFixture preSelectedCustomer )
		{
			var id = preSelectedCustomer.Selected.ExternalId;
			var response = api.GetCustomerList( "$filter=ExternalId eq '" + id + '"' );
			Assert.True( response.Data.results.All( x => id == x.ExternalId ), "Did not find '" + id + "' in: " + string.Join( ", ", from r in response.Data.results select r.ExternalId ) );
		}
	}
}