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
		public static void ShouldYieldAccepted( [Frozen] SpCustomerApi api, NewlyCreatedCustomerFixture customer )
		{
			var organization = new SpCustomerApi.OrganizationCreateModel( customer.Customer );
			var response = api.CreateOrganization( organization );
			Assert.Equal( HttpStatusCode.Accepted, response.StatusCode );
		}
	}


}
