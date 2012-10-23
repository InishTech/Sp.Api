/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Api.Customer.Acceptance
{
	using RestSharp;
	using Sp.Api.Shared.Wrappers;
	using System;
	using System.Collections.Generic;

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

		internal IRestResponse CreateCustomer( CustomerSummary customer )
		{
			var request = new RestRequest( ApiPrefix.Customer + "/customer", Method.POST );
			request.RequestFormat = DataFormat.Json;
			request.AddBody( customer );
			return Execute( request );
		}

		internal IRestResponse<CustomerSummary> GetCustomer( string href )
		{
			var request = new RestRequest( href );
			return Execute<CustomerSummary>( request );
		}

		public IRestResponse PutCustomer( string href, CustomerSummary customer )
		{
			var request = new RestRequest( href, Method.PUT );
			request.RequestFormat = DataFormat.Json;
			request.AddBody( customer );
			return Execute( request );
		}

		public class CustomerSummaryPage
		{
			public List<CustomerSummary> results { get; set; }
		}

		public class CustomerSummary
		{
			public Guid RequestId { get; set; }
			public string Name { get; set; }
			public string Description { get; set; }

			public int _version { get; set; }
			public Links _links { get; set; }

			public JsonSignature _signature { get; set; }

			public _Embedded _embedded { get; set; }

			public class _Embedded
			{
				public Guid Id { get; set; }
				public Guid VendorId { get; set; }
				public CustomerInvite Organization { get; set; }
			}

			public class Links
			{
				public Link self { get; set; }
				public Link inviteStatus { get; set; }
			}
		}

		public class CustomerInvite
		{
			public CustomerSummary Customer { get; set; }
			public string VendorName { get; set; }
			public string EmailTo { get; set; }
			public Guid RequestId { get; set; }

			public class Links
			{
				public Link self { get; set; }
			}

			public Links _links { get; set; }
		}

		public class Link
		{
			public string href { get; set; }
		}

		public class JsonSignature
		{
			public string Value { get; set; }
		}
	}
}