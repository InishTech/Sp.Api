/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Api.Portal.Acceptance
{
	using System.Collections.Generic;
	using System.Configuration;
	using System.Linq;
	using System.Net;
	using Ploeh.AutoFixture;
	using RestSharp;
	using Sp.Api.Portal.Acceptance.Wrappers;
	using Xunit;
	using Xunit.Extensions;
	using Sp.Api.Shared.Wrappers;

	public static class ItemManagement
	{
		[Theory, AutoPortalDataAttribute]
		public static void GetItemsShouldYieldData( SpPortalApi portalApi )
		{
			var request = new RestRequest( "ItemManagement/" );
			var response = portalApi.Execute<ItemsPage>( request );
			Assert.Equal( HttpStatusCode.OK, response.StatusCode );
			Assert.Equal( ResponseStatus.Completed, response.ResponseStatus );
			Assert.NotNull( response.Data );
			Assert.NotEmpty( response.Data.Items );
			Assert.NotNull( response.Data.Items.First().Id );
			Assert.NotNull( response.Data.Items.First().Label );
		}

		[Fact]
		public static void GetItemsWithoutCanSeeItemsClaimShouldReturnHttpStatusForbidden(  )
		{
			var autoFixture = new Fixture();
			autoFixture.Customize( new SoftwarePotentialApiNonPrivilegedUserConfigurationCustomization() );
			autoFixture.Customize( new SkipSSlCertificateValidationIfRequestedCustomization() );
			var portalApi = autoFixture.CreateAnonymous<SpPortalApi>();

			var request = new RestRequest( "ItemManagement/" );
			var response = portalApi.Execute<ItemsPage>( request );
			Assert.Equal( HttpStatusCode.Forbidden, response.StatusCode );
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

		public class SoftwarePotentialApiNonPrivilegedUserConfigurationCustomization : ICustomization
		{
			const string PortalUserWithouthCanSeeItemsPermission = "customer2@inishtech.com";

			void ICustomization.Customize( IFixture fixture )
			{
				fixture.Register( ( string username, string password ) => new SpApiConfiguration(
					PortalUserWithouthCanSeeItemsPermission,
					ConfigurationManager.AppSettings[ "PortalPassword" ],
					ConfigurationManager.AppSettings[ "PortalBaseUrl" ] ) );
			}
		}
	}
}