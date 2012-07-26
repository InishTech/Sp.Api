using System;

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

			public Links _links { get; set; }

			public class Links
			{
				public Link self { get; set; }
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
}