namespace Sp.Api.Customer.Acceptance
{
	using RestSharp;
	using Sp.Api.Shared.Wrappers;
	using System.Collections.Generic;

	public class SpCustomerApi : SpApi
	{
		public SpCustomerApi( SpApiConfiguration apiConfiguration )
			: base( apiConfiguration )
		{
		}

		internal IRestResponse<CustomerSummaryPage> GetList()
		{
			var request = new RestRequest( ApiPrefix.Customer );
			return Execute<CustomerSummaryPage>( request );
		}

		public class CustomerSummaryPage
		{
			public List<CustomerSummary> Customers { get; set; }
		}

		public class CustomerSummary
		{
			public string Name { get; set; }
			public string Description { get; set; }
			public bool IsRegistered { get; set; }
		}
	}
}