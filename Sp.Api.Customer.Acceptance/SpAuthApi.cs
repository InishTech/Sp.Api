/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */

using System.Collections.Generic;

namespace Sp.Api.Customer.Acceptance
{
	using RestSharp;
	using Sp.Api.Shared.Wrappers;
	using System;

	public class SpAuthApi : SpApi
	{
		public SpAuthApi( SpApiConfiguration apiConfiguration )
			: base( apiConfiguration )
		{
		}

		internal IRestResponse InviteCustomer( string inviteLink, CustomerInvite invite )
		{
			var request = new RestRequest( inviteLink, Method.POST );
			request.RequestFormat = DataFormat.Json;
			request.AddBody( invite );
			return Execute( request );
		}

		public IRestResponse<InviteStatus> GetInviteStatus( string inviteStatusLink )
		{
			var request = new RestRequest( inviteStatusLink );
			request.RequestFormat = DataFormat.Json;
			return Execute<InviteStatus>( request );
		}

		public IRestResponse<ServiceInstanceIndexModel> GetServiceInstances()
		{
			var request = new RestRequest( GetApiPrefix( ApiType.Auth ) + "/Registration/Instance" );
			return Execute<ServiceInstanceIndexModel>( request );
		}

		public class CustomerInvite
		{
			public Guid InstanceId { get; set; }
			public string EmailTo { get; set; }
			public Guid RequestId { get; set; }
		}

		public class InviteStatus
		{
			public string State { get; set; }
			public string LastEmailSentTo { get; set; }
		}

		public class ServiceInstanceIndexModel
		{
			public List<ServiceInstance> results { get; set; }
		}

		public class ServiceInstance
		{
			public Guid Id { get; set; }
			public string Name { get; set; }
			public string ServiceName { get; set; }
		}
	}
}