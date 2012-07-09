﻿/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
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

		internal IRestResponse<LicensesSummaryPage> GetList()
		{
			var request = new RestRequest( ApiPrefix.Issue + "/License" );
			return Execute<LicensesSummaryPage>( request );
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