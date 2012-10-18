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

		public class CustomerInvite
		{
			public string SiteVendorName { get; set; }
			public string EmailTo { get; set; }
			public Guid RequestId { get; set; }
		}

		public class InviteStatus
		{
			public string LastInviteSentTo { get; set; }
			//NB - this should be of enum type InviteState, but RestSharp deserializer seems to have problems with that
			public int State { get; set; }
		}

		public enum InviteState
		{
			NotInvited,
			OpenInvitation,
			ExpiredInvitation,
			Redeemed
		}
	}
}