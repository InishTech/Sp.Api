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
		public static void ShouldYieldAccepted( [Frozen] SpCustomerApi api, CustomerPut.GetRandomCustomerFixture customer )
		{
			var organization = new SpCustomerApi.OrganizationCreateModel( customer.DataFromGet );
			var response = api.CreateOrganization( organization );
			Assert.Equal( HttpStatusCode.Accepted, response.StatusCode );
		}
	}

	public class NewlyCreatedOrganizationFixture
	{
		public NewlyCreatedOrganizationFixture( SpCustomerApi api, CustomerPut.GetRandomCustomerFixture existingCustomer )
		{
			//TODO TP 1043 - arguably inviteStatus link should be in the organization resource rather than in the customer
			InviteStatusLink = existingCustomer.DataFromGet._links.inviteStatus.href;
			var organization = new SpCustomerApi.OrganizationCreateModel( existingCustomer.DataFromGet );
			var response = api.CreateOrganization( organization );
			Assert.Equal( HttpStatusCode.Accepted, response.StatusCode );
		}

		public string InviteStatusLink { get; private set; }
	}
}
