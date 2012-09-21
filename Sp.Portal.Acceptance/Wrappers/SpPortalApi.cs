/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Portal.Acceptance.Wrappers
{
	using RestSharp;
	using Sp.Api.Shared.Wrappers;
	using Sp.Test.Helpers;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class SpPortalApi
	{
		readonly IRestClient _client;
		readonly string _username;
		readonly string _password;

		public SpPortalApi( SpApiConfiguration apiConfiguration )
		{
			_client = new RelativePathAwareCustomRestClient( apiConfiguration.BaseUrl );

			_username = apiConfiguration.Username;
			_password = apiConfiguration.Password;

			_client.Authenticator = new WSFederationAuthenticator( _username, _password );
		}

		internal IRestResponse<TagCollection> GetTagCollection()
		{
			var request = new RestRequest( "tag" );
			return Execute<TagCollection>( request );
		}

		public IRestResponse PutTagCollection( IEnumerable<SpPortalApi.Tag> tags )
		{
			var request = new RestRequest( "tag", Method.PUT );
			request.RequestFormat = DataFormat.Json;
			request.AddBody( new TagCollection { Tags = tags.ToList() } );
			return Execute( request );
		}

		public IRestResponse PutLicenseTags( string licenseTagsAssignmentHref, TagWithValueCollection tags )
		{
			var request = new RestRequest( licenseTagsAssignmentHref, Method.PUT ) { RequestFormat = DataFormat.Json };
			request.AddBody( tags );
			return Execute( request );
		}

		public IRestResponse<LicensesSummaryPage> GetLicenseList()
		{
			var request = new RestRequest( "License" );
			return Execute<LicensesSummaryPage>( request );
		}

		public IRestResponse<LicenseSummary> GetLicense( string href )
		{
			var request = new RestRequest( href );
			return Execute<LicenseSummary>( request );
		}

		public class TagCollection
		{
			public List<Tag> Tags { get; set; }
		}

		public class Tag
		{
			public Guid Id { get; set; }
			public string Name { get; set; }
		}

		public class Link
		{
			public string href { get; set; }
		}

		public class LicenseSummary
		{
			public string ActivationKey { get; set; }

			public string ProductLabel { get; set; }
			public string VersionLabel { get; set; }

			public string IssueDate { get; set; }
			public bool IsEvaluation { get; set; }
			public bool IsRenewable { get; set; }
			//public Guid? CustomerId { get; set; }
			public bool IsActivatable { get; set; }
			public string ActivatableMessage { get; set; }

			public Dictionary<string, string> Tags { get; set; }

			public Links _links { get; set; }

			public class Links
			{
				public Link self { get; set; }
				public Link tags { get; set; }
			}
		}

		public class LicensesSummaryPage
		{
			public List<LicenseSummary> Licenses { get; set; }
		}

		public class TagWithValueCollection
		{
			public Dictionary<string, string> Tags { get; set; }
		}

		public IRestResponse<T> Execute<T>( RestRequest request ) where T : new()
		{
			return _client.Execute<T>( request );
		}

		public IRestResponse Execute( RestRequest request )
		{
			return _client.Execute( request );
		}

		public IRestResponse SignOff()
		{
			var signOffRequest = new RestRequest( "Authentication/LogOff", Method.GET );
			return _client.Execute( signOffRequest );
		}
	}
}