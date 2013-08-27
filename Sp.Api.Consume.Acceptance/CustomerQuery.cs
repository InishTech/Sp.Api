using Sp.Api.Shared;
using System.Net;
using Xunit;
using Xunit.Extensions;
using System.Linq;
using System;

namespace Sp.Api.Consume.Acceptance
{
	public static class CustomerQuery
	{
		[Theory, AutoSoftwarePotentialApiData]
		public static void FilterByKnownExternalIdShouldReturnOneOrMoreCustomersWithThatExternalId( SpCustomerApi api, RandomCustomerFromListFixture preSelectedCustomer )
		{
			var response = api.GetCustomerList( "$filter=ExternalId eq '" + preSelectedCustomer.Selected.ExternalId + '"' );
			Assert.NotNull( response.Data.results );
			Assert.False( response.Data.results.Any( x => String.Equals( x.ExternalId, preSelectedCustomer.Selected.ExternalId, StringComparison.OrdinalIgnoreCase /* String field in database will be case-insentistive due to collation configuration */) ) );
		}
	}
}