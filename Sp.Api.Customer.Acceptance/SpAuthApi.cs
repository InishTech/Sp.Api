
namespace Sp.Api.Customer.Acceptance
{
	using RestSharp;
	using Sp.Api.Shared.Wrappers;

	public class SpAuthApi : SpApi
	{
		public SpAuthApi( SpApiConfiguration apiConfiguration )
			: base( apiConfiguration )
		{
		}

		internal IRestResponse InviteCustomer( string inviteLink, Sp.Api.Customer.Acceptance.SpCustomerApi.CustomerSummary customer, string vendorName, string emailTo )
		{
			var request = new RestRequest( inviteLink, Method.POST );
			request.RequestFormat = DataFormat.Json;
			request.AddBody( customer );
			request.AddParameter( "vendorName", vendorName );
			request.AddParameter( "emailTo", emailTo );
			return Execute( request );
		}
	}
}

