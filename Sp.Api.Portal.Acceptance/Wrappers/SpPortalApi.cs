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

		public IRestResponse PutCustomerTags( IEnumerable<CustomerTag> tags )
		{
			var request = new RestRequest( "tag", Method.PUT );
			request.RequestFormat = DataFormat.Json;
			request.AddBody( new TagCollection { Tags = tags.ToList() } );
			return Execute( request );
		}

		public IRestResponse PutLicenseTags( string licenseTagsAssignmentHref, IEnumerable<LicenseTag> tags )
		{
			var request = new RestRequest( licenseTagsAssignmentHref, Method.PUT );
			request.RequestFormat = DataFormat.Json;
			request.AddBody( tags );
			return Execute( request );
		}

		public IRestResponse<LicenseCollection> GetLicenses()
		{
			var request = new RestRequest( "License" );
			return Execute<LicenseCollection>( request );
		}

		public IRestResponse<LicenseCollection> GetLicenses( string queryParameters )
		{
			var request = new RestRequest( "License/?" + queryParameters );
			return Execute<LicenseCollection>( request );
		}

		public IRestResponse<License> GetLicense( string href )
		{
			var request = new RestRequest( href );
			return Execute<License>( request );
		}

		public class TagCollection
		{
			public List<CustomerTag> Tags { get; set; }
		}

		public class CustomerTag
		{
			public Guid Id { get; set; }
			public string Name { get; set; }
		}

		public class Link
		{
			public string href { get; set; }
		}

		public class License
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

			public Embedded _embedded { get; set; }
			public Links _links { get; set; }

			public class Embedded
			{
				public List<LicenseTag> Tags { get; set; }
			}

			public class Links
			{
				public Link self { get; set; }
				public Link tags { get; set; }
			}
		}

		public class LicenseTag
		{
			public Guid Id { get; set; }
			public string Value { get; set; }
		}

		public class LicenseCollection
		{
			public List<License> Licenses { get; set; }
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