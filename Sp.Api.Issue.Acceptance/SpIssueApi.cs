/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Api.Issue
{
	using RestSharp;
	using Sp.Api.Customer.Acceptance;
	using Sp.Api.Shared.Wrappers;
	using System.Collections.Generic;

	public class SpIssueApi : SpApi
	{
		public SpIssueApi( SpApiConfiguration apiConfiguration )
			: base( apiConfiguration )
		{
		}

		internal IRestResponse<Licenses> GetLicenseList()
		{
			return GetLicenseList( "" );
		}

		internal IRestResponse<Licenses> GetLicenseList(string query)
		{
			var request = new RestRequest( ApiPrefix.Issue + "/License?"+query );
			return Execute<Licenses>( request );
		}

		internal IRestResponse<License> GetLicense( string href )
		{
			var request = new RestRequest( href );
			return Execute<License>( request );
		}

		public IRestResponse PutLicenseCustomerAssignment( string licenseCustomerAssignmentHref, SpCustomerApi.CustomerSummary customer )
		{
			var request = new RestRequest( licenseCustomerAssignmentHref, Method.PUT );
			request.RequestFormat = DataFormat.Json;
			request.AddBody( customer );

			return Execute( request );
		}

		public IRestResponse DeleteLicenseCustomerAssignment( string licenseCustomerAssignmentHref )
		{
			var request = new RestRequest( licenseCustomerAssignmentHref, Method.DELETE );

			return Execute( request );
		}

		public class Licenses
		{
			public int? __count { get; set; }
			public List<License> results { get; set; }
		}
		
		public class License
		{
			// External identifier as used in activation
			public string ActivationKey { get; set; }

			// Name of Product this is a license for at the time of issue (underlying stable ProductReferenceId as used on Activated License may be different)
			public string ProductLabel { get; set; }
			// Name of Version this is a license for at the time of issue (underlying stable VersionReferenceId as used on Activated License may be different)
			public string VersionLabel { get; set; }

			public string IssueDate { get; set; }
			public bool IsEvaluation { get; set; }
			public bool IsRenewable { get; set; }

			public bool IsActivatable { get; set; }

			public Links _links { get; set; }
			public Embedded _embedded { get; set; }

			public class Links
			{
				public Link self { get; set; }
				public Link customer { get; set; }
				public Link customerAssignment { get; set; }
			}

			public class Link
			{
				public string href { get; set; }
			}

			public class Embedded
			{
				public Customer Customer { get; set; }
			}
		}

		public class Customer
		{
			public string Name { get; set; }
			public string Description { get; set; }
		}
	}
}