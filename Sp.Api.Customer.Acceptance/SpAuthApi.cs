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

	public class SpAuthApi : SpApi
	{
		public SpAuthApi( SpApiConfiguration apiConfiguration )
			: base( apiConfiguration )
		{
		}

		internal IRestResponse InviteCustomer( string inviteLink, SpCustomerApi.CustomerInvite invite )
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

		public class InviteStatus
		{
			public bool IsRegistered { get; set; }
			public bool IsInviteOpen { get; set; }
			public string LastInviteSentTo { get; set; }
			public DateTime? LastInviteSentDateTime { get; set; }
			public DateTime? LastInviteExpiryDateTime { get; set; }
		}
	}
}