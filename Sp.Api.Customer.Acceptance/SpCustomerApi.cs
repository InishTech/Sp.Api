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

		internal IRestResponse<CustomerSummaryPage> GetCustomerList()
		{
			var request = new RestRequest( ApiPrefix.Customer );
			return Execute<CustomerSummaryPage>( request );
		}

		internal IRestResponse CreateCustomer( string addLink, CustomerSummary customer )
		{
			var request = new RestRequest( addLink, Method.POST );
			request.RequestFormat = DataFormat.Json;
			request.AddBody( customer );
			return Execute( request );
		}

		internal IRestResponse<CustomerSummary> GetCustomer( Uri uri )
		{
			var request = new RestRequest( uri );
			return Execute<CustomerSummary>( request );
		}

		public class CustomerSummaryPage
		{
			public List<CustomerSummary> Customers { get; set; }

			public Links _links { get; set; }

			public class Links
			{
				public Link add { get; set; }
			}
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

			public Uri AsRelativeUri()
			{
				return new Uri( href, UriKind.Relative );
			}
		}
	}
}
