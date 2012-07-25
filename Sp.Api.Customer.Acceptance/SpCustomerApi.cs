namespace Sp.Api.Customer.Acceptance
{
	using RestSharp;
	using Sp.Api.Shared.Wrappers;
	using System.Collections.Generic;
	using System;

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

		internal IRestResponse AddCustomer(string addLink, CustomerSummary customer )
		{
			var request = new RestRequest( addLink, Method.POST );
			request.RequestFormat = DataFormat.Json;
			request.AddBody(  customer );
			return Execute( request );
		}

		public class CustomerSummaryPage
		{
			public Links _links { get; set; }
			public class Links
			{
				public Link add { get; set; }
			}
			public List<CustomerSummary> Customers { get; set; }
		}

		public class CustomerSummary
		{
			public string Name { get; set; }
			public string Description { get; set; }
			public bool IsRegistered { get; set; }

			public Links _links { get; set; }

			public class Links
			{
				public Link self { get; set; }
			}
		}

		public class Link
		{
			public string href { get; set; }
		}
	}
}