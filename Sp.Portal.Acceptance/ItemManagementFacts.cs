﻿namespace Sp.Portal.Acceptance
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using RestSharp;
	using Sp.Portal.Acceptance.Wrappers;
	using Xunit;
	using Xunit.Extensions;

	public static class ItemManagementFacts
	{
		[Theory, PortalData]
		public static void GetItemsShouldYieldData( SpPortalApi portalApi )
		{
			var request = new RestRequest( "/ItemManagement/" );
			var response = portalApi.Execute<ItemsPage>( request );
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
			Assert.Equal( ResponseStatus.Completed, response.ResponseStatus );
			Assert.NotNull( response.Data );
			Assert.NotEmpty( response.Data.Items );
			Assert.NotNull( response.Data.Items.First().Id );
			Assert.NotNull( response.Data.Items.First().Label );
		}

		public class ItemsPage
		{
			public List<Item> Items { get; set; }
		}

		public class Item
		{
			public string Id { get; set; }
			public string Label { get; set; }
		}
	}
}
