/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Api.Issue
{
	using RestSharp;
	using Sp.Api.Shared.Wrappers;
	using System;
	using System.Collections.Generic;

	public class SpIssueApi : SpApi
	{
		public SpIssueApi( SpApiConfiguration apiConfiguration )
			: base( apiConfiguration )
		{
		}

		internal IRestResponse<LicensesSummaryPage> GetLicenseList()
		{
			var request = new RestRequest( ApiPrefix.Issue + "/License" );
			return Execute<LicensesSummaryPage>( request );
		}

		internal IRestResponse<LicenseSummary> GetLicense( Uri uri )
		{
			var request = new RestRequest( uri );
			return Execute<LicenseSummary>( request );
		}

		public IRestResponse PutLicenseCustomerAssignment( Uri licenseUri, Uri customerUri )
		{
			var request = new RestRequest( licenseUri, Method.PUT );
			request.RequestFormat = DataFormat.Json;
			request.AddBody( new { href = customerUri.ToString(  ) } );

			return Execute( request );
		}

		public class LicensesSummaryPage
		{
			public List<LicenseSummary> Licenses { get; set; }
		}

		public class LicenseSummary
		{
			// External identifier as used in activation
			public string ActivationKey { get; set; }

			// Name of Product this is a license for at the time of issue (underlying stable ProductReferenceId as used on Activated License may be different)
			public string ProductLabel { get; set; }
			// Name of Version this is a license for at the time of issue (underlying stable VersionReferenceId as used on Activated License may be different)
			public string VersionLabel { get; set; }

			public DateTime IssueDate { get; set; }
			public bool IsEvaluation { get; set; }
			public bool IsRenewable { get; set; }
		}
	}
}