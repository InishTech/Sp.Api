
namespace Sp.Api.Customer.Acceptance
{
	using System;
using RestSharp;
using Sp.Api.Shared.Wrappers;

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
	}
}

