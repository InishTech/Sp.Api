/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Portal.Acceptance
{
	using Ploeh.AutoFixture;
	using Ploeh.AutoFixture.Xunit;
	using Sp.Portal.Acceptance.Wrappers;
	using Sp.Test.Helpers;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using Xunit;
	using Xunit.Extensions;

	public class CustomerTags
	{
		public static class Index
		{
			public static class Get
			{
				[Theory, AutoPortalDataAttribute]
				public static void ShouldAlwaysBeAvailable( SpPortalApi api )
				{
					var response = api.GetTagCollection();
					Assert.Equal( HttpStatusCode.OK, response.StatusCode );
				}

				[Theory, AutoPortalDataAttribute]
				public static void ShouldHaveNonEmptyIds( SpPortalApi api )
				{
					var response = api.GetTagCollection();
					Assert.Equal( HttpStatusCode.OK, response.StatusCode );
					Assert.DoesNotContain( Guid.Empty, response.Data.Tags.Select( t => t.Id ) );
				}

				[Theory, AutoPortalDataAttribute]
				public static void ShouldHaveNonEmptyNames( SpPortalApi api )
				{
					var response = api.GetTagCollection();
					Assert.Equal( HttpStatusCode.OK, response.StatusCode );
					Assert.False( response.Data.Tags.Any( t => string.IsNullOrEmpty( t.Name ) ) );
				}
			}

			public static class Put
			{
				[Theory, AutoPortalDataAttribute]
				public static void ShouldYieldAccepted( [Frozen]SpPortalApi api, SpPortalApi.CustomerTag[] tags )
				{
					var apiResponse = api.PutCustomerTags( tags );
					Assert.Equal( HttpStatusCode.Accepted, apiResponse.StatusCode );
				}

				[Theory, AutoPortalDataAttribute]
				public static void ShouldEventuallyBeVisible( [Frozen]SpPortalApi api, ExistingTagsFixture tags )
				{
					VerifyCollectionEventuallyGetsUpdatedTo( tags.Tags, api );
				}

				public class Reorder
				{
					[Theory, AutoPortalDataAttribute]
					public static void ShouldEventuallyBeVisible( [Frozen]SpPortalApi api, ExistingTagsFixture tags )
					{
						var updated = tags.Tags.Reverse();
						VerifyPutEventuallyGetsApplied( updated, api );
					}
				}

				public class Rename
				{
					[Theory, AutoPortalDataAttribute]
					public static void ShouldEventuallyBeVisible( [Frozen]SpPortalApi api, ExistingTagsFixture tags )
					{
						var updated = tags.Tags.Select( x => new SpPortalApi.CustomerTag { Id = x.Id, Name = x.Name + "renamed" } );
						VerifyPutEventuallyGetsApplied( updated, api );
					}
				}

				public class Delete
				{
					[Theory, AutoPortalDataAttribute]
					public static void ShouldEventuallyBeVisible( [Frozen]SpPortalApi api, ExistingTagsFixture tags )
					{
						var updated = tags.Tags.Where( ( x, index ) => index != 1 );
						VerifyPutEventuallyGetsApplied( updated, api );
					}
				}

				static void VerifyPutEventuallyGetsApplied( IEnumerable<SpPortalApi.CustomerTag> expected, SpPortalApi api )
				{
					var apiResponse = api.PutCustomerTags( expected );
					Assert.Equal( HttpStatusCode.Accepted, apiResponse.StatusCode );
					VerifyCollectionEventuallyGetsUpdatedTo( expected, api );
				}

				static void VerifyCollectionEventuallyGetsUpdatedTo( IEnumerable<SpPortalApi.CustomerTag> expected, SpPortalApi api )
				{
					Verify.EventuallyWithBackOff( () =>
					{
						var apiResult = api.GetTagCollection();
						Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
						Assert.Equal( expected.Select( x => Tuple.Create( x.Id, x.Name ) ).ToArray(), apiResult.Data.Tags.Select( x => Tuple.Create( x.Id, x.Name ) ).ToArray() );
					} );
				}

				public static class Bad
				{
					[Theory, AutoPortalDataAttribute]
					public static void NameMissingShouldYieldBadRequest( SpPortalApi api, IFixture fixture )
					{
						var badTag = fixture.Build<SpPortalApi.CustomerTag>().With( x => x.Name, null ).CreateAnonymous();

						var response = api.PutCustomerTags( new[] { badTag } );

						Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
					}

					[Theory, AutoPortalDataAttribute]
					public static void TooLongShouldYieldBadRequest( SpPortalApi api, IFixture fixture )
					{
						var badTag = fixture.Build<SpPortalApi.CustomerTag>().With( x => x.Name, new String( 'a', 101 ) ).CreateAnonymous();

						var response = api.PutCustomerTags( new[] { badTag } );

						Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
					}

					[Theory( Skip = "Currently 500 instead due to choice of Serializer" ), AutoPortalDataAttribute]
					public static void IdEmptyShouldYieldBadRequest( SpPortalApi api, IFixture fixture )
					{
						var badTag = fixture.Build<SpPortalApi.CustomerTag>().With( x => x.Id, Guid.Empty ).CreateAnonymous();

						var response = api.PutCustomerTags( new[] { badTag } );

						Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
					}
				}
			}
		}
	}
	public class ExistingTagsFixture : IDisposable
	{
		readonly SpPortalApi _api;

		public SpPortalApi.CustomerTag[] Tags { get; private set; }

		public ExistingTagsFixture( SpPortalApi api, SpPortalApi.CustomerTag[] tags )
		{
			Tags = tags;
			_api = api;
			var response = api.PutCustomerTags( tags );
			Assert.Equal( HttpStatusCode.Accepted, response.StatusCode );
		}

		void IDisposable.Dispose()
		{
			_api.PutCustomerTags( Enumerable.Empty<SpPortalApi.CustomerTag>() );
		}
	}
}