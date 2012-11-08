using System.Net;
using Ploeh.AutoFixture.Xunit;
using Sp.Api.Shared;
using Xunit;
using Xunit.Extensions;

namespace Sp.Api.Customer.Acceptance
{
	public class OrganizationCreate
	{
		[Theory, AutoSoftwarePotentialApiData]
		public static void ShouldYieldAccepted( [Frozen] SpCustomerApi api, FreshCustomerFixture customer )
		{
			var organization = new SpCustomerApi.OrganizationCreateModel( customer.SignedCustomer );
			var response = api.CreateOrganization( organization );
			Assert.Equal( HttpStatusCode.Accepted, response.StatusCode );
		}
	}


}
