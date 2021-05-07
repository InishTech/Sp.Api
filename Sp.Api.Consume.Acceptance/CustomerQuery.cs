using Sp.Api.Shared;
using System.Linq;
using Xunit;
using Xunit.Extensions;

namespace Sp.Api.Consume.Acceptance
{
	public static class CustomerQuery
    {
        [Theory, AutoSoftwarePotentialApiData]
        public static void FilterByKnownExternalIdShouldReturnOneOrMoreCustomersWithThatExternalId( SpCustomerApi api, RandomCustomerFromListFixture preSelectedCustomer )
        {
            var id = preSelectedCustomer.Selected.ExternalId;
            var response = api.GetCustomerList( "$filter=ExternalId eq '" + id + '"' );
            Assert.True( response.Data.results.All( x => id == x.ExternalId ), "Did not find '" + id + "' in: " + string.Join( ", ", from r in response.Data.results select r.ExternalId ) );
        }
  
        [Theory, AutoSoftwarePotentialApiData]
        public static void ShouldBeAbleToCalculateInlineCount( SpCustomerApi api, RandomCustomerFromListFixture preSelectedCustomer )
        {
            var filteredCustomers = api.GetCustomerList( "$inlinecount=allpages&$top=1" );
            var allCustomers = api.GetCustomerList();
            Assert.Equal( allCustomers.Data.results.Count(), filteredCustomers.Data.__count ); ;
        }

        [Theory, AutoSoftwarePotentialApiData]
        public static void ShouldBePageable( SpCustomerApi api )
        {
            var firstRequest = api.GetCustomerList( "$skip=0&$top=1" );
            var secondRequest = api.GetCustomerList( "$skip=1&$top=1" );

            var firstCustomer = Assert.Single( firstRequest.Data.results );
            var secondCustomer = Assert.Single( secondRequest.Data.results );
            Assert.NotEqual( firstCustomer.Name, secondCustomer.Name );
        }

        [Theory, AutoSoftwarePotentialApiData]
        public static void ShouldBeSortable( SpCustomerApi api )
        {
            var apiResult = api.GetCustomerList( "$orderby=Name desc" );

            var customers = apiResult.Data.results;
            var resorted = customers.OrderByDescending( x => x.Name ).ToArray();
            Assert.True( resorted.SequenceEqual( customers ) );
        }
    }
}