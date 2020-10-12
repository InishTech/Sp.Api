using AutoFixture.Xunit;
using Sp.Api.Shared;
using System.Net;
using Xunit;
using Xunit.Extensions;

namespace Sp.Api.Consume.Acceptance
{
	public class OrganizationCreate
	{
		[Theory, AutoSoftwarePotentialApiData]
		public static void ShouldYieldAccepted( [Frozen] SpAuthApi api, FreshCustomerFixture customer )
		{
			var organization = new SpAuthApi.OrganizationCreateModel( customer.SignedCustomer );
			var addOrganizationLink = customer.SignedCustomer._links.organizationAdd.href;
			var response = api.CreateOrganization( organization, addOrganizationLink );
			Assert.Equal( HttpStatusCode.Accepted, response.StatusCode );
		}
	}
}