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

		internal IRestResponse CreateCustomer( string addLink, CustomerSummary customer )
		{
			var request = new RestRequest( addLink, Method.POST );
			request.RequestFormat = DataFormat.Json;
			request.AddBody( customer );
			return Execute( request );
		}

		internal IRestResponse CreateOrganization( OrganizationCreateModel organization )
		{
			const string addLink = ApiPrefix.Auth + "/Registration/Organization";
			var request = new RestRequest( addLink, Method.POST );
			request.RequestFormat = DataFormat.Json;
			request.AddBody( organization );
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
			public List<CustomerSummary> Customers { get; set; }

			public Links _links { get; set; }

			public class Links
			{
				public Link add { get; set; }
			}
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
			}

			public class Links
			{
				public Link self { get; set; }
				public Link inviteStatus { get; set; }
				public Link organizationAdd { get; set; }
			}
		}

	
		public class OrganizationCreateModel
		{
			public _Embedded _embedded { get; set; }

			public class _Embedded
			{
				public CustomerSummary Customer { get; set; }
			};

			public OrganizationCreateModel( CustomerSummary customer )
			{
				_embedded = new _Embedded { Customer = customer };
			}
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