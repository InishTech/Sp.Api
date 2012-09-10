namespace Sp.Portal.Acceptance
{
	using Ploeh.AutoFixture.Xunit;
	using Sp.Portal.Acceptance.Wrappers;
	using Sp.Test.Helpers;
	using System;
	using System.Linq;
	using System.Net;
	using Xunit;
	using Xunit.Extensions;

	public class CustomerTags
	{
		public class Index
		{
			public class Get
			{
				[Theory, PortalData]
				public static void ShouldAlwaysBeAvailable( SpPortalApi api )
				{
					var response = api.GetTagCollection();
					Assert.Equal( HttpStatusCode.OK, response.StatusCode );
				}

				[Theory, PortalData]
				public static void ShouldHaveAddLink( SpPortalApi api )
				{
					var response = api.GetTagCollection();
					Assert.Equal( HttpStatusCode.OK, response.StatusCode );

					Assert.NotNull( response.Data._links );
					Assert.NotNull( response.Data._links.add );
					Assert.NotEmpty( response.Data._links.add.href );
				}

				[Theory, PortalData]
				public static void ShouldHaveNonEmptyIds( SpPortalApi api )
				{
					var response = api.GetTagCollection();
					Assert.Equal( HttpStatusCode.OK, response.StatusCode );
					Assert.DoesNotContain( Guid.Empty, response.Data.Tags.Select( t => t.Id ) );
				}
			}

			public class Post
			{
				[Theory, PortalData]
				public static void ShouldYieldAccepted( [Frozen]SpPortalApi api, AddLinkFixture addLink, Guid id, string name )
				{
					var apiResponse = api.AddTag( addLink.Href, id, name );
					Assert.Equal( HttpStatusCode.Accepted, apiResponse.StatusCode );
				}

				[Theory, PortalData]
				public static void ShouldEventuallyBeVisible( [Frozen]SpPortalApi api, ExistingTagFixture tag )
				{
					Verify.EventuallyWithBackOff( () =>
					{
						var apiResult = api.GetTagCollection();
						Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
						Assert.True( apiResult.Data.Tags.Any( x => x.Name == tag.Name && tag.Href.Equals( x._links.self.href ) ) );
					} );
				}

				[Theory, PortalData]
				public static void DoubleShouldYieldConflict( [Frozen]SpPortalApi api, AddLinkFixture addLink, Guid id, string name )
				{
					var initialResponse = api.AddTag( addLink.Href, id, name );
					Assert.Equal( HttpStatusCode.Accepted, initialResponse.StatusCode );
					var response = api.AddTag( addLink.Href, id, name );
					Assert.Equal( HttpStatusCode.Conflict, response.StatusCode );
				}

				public class Bad
				{
					[Theory, PortalData]
					public static void MissingNameShouldYieldBadRequest( [Frozen]SpPortalApi api, AddLinkFixture addLink, Guid id )
					{
						var response = api.AddTag( addLink.Href, id, null );
						Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
					}

					[Theory( Skip = "Currently 500 instead due to choice of Serializer" ), PortalData]
					public static void EmptyIdShouldYieldBadRequest( [Frozen]SpPortalApi api, AddLinkFixture addLink, string name )
					{
						var response = api.AddTag( addLink.Href, Guid.Empty, name );
						Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
					}
				}
			}
		}

		public class Put
		{
			[Theory, PortalData]
			public void ShouldYieldAccepted( [Frozen]SpPortalApi api, ExistingTagFixture tag, string newName )
			{
				using ( tag )
				{
					var response = api.RenameTag( tag.Href, newName );
					Assert.Equal( HttpStatusCode.Accepted, response.StatusCode );
				}
			}

			public class Bad
			{
				[Theory, PortalData]
				public static void MissingNameShouldYieldBadRequest( [Frozen]SpPortalApi api, ExistingTagFixture tag )
				{
					using ( tag )
					{
						var response = api.RenameTag( tag.Href, null );
						Assert.Equal( HttpStatusCode.BadRequest, response.StatusCode );
					}
				}
			}

			[Theory, PortalData]
			public static void ShouldEventuallyBeEvident( [Frozen]SpPortalApi api, ExistingTagFixture tag, string newName )
			{
				using ( tag )
				{
					var response = api.RenameTag( tag.Href, newName );
					Assert.Equal( HttpStatusCode.Accepted, response.StatusCode );

					Verify.EventuallyWithBackOff( () =>
					{
						var apiResult = api.GetTagCollection();
						Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
						Assert.True( apiResult.Data.Tags.Any( x => x.Name == newName && tag.Href.Equals( x._links.self.href ) ) );
					} );
				}
			}
		}

		public class Delete
		{
			[Theory, PortalData]
			public void ShouldYieldAccepted( [Frozen]SpPortalApi api, ExistingTagFixture tag )
			{
				var response = api.DeleteTag( tag.Href );
				Assert.Equal( HttpStatusCode.Accepted, response.StatusCode );
			}

			[Theory, PortalData]
			public static void ShouldEventuallyBeRemoved( [Frozen]SpPortalApi api, ExistingTagFixture tag )
			{
				var response = api.DeleteTag( tag.Href );
				Assert.Equal( HttpStatusCode.Accepted, response.StatusCode );

				Verify.EventuallyWithBackOff( () =>
				{
					var apiResult = api.GetTagCollection();
					Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
					Assert.False( apiResult.Data.Tags.Any( x => tag.Href.Equals( x._links.self.href ) ) );
				} );
			}
		}

		public class ExistingTagFixture : IDisposable
		{
			readonly SpPortalApi _api;
			public string Href { get; private set; }
			public string Name { get; private set; }

			public ExistingTagFixture( AddLinkFixture addLink, SpPortalApi api, Guid id, string name )
			{
				Name = name;
				_api = api;
				var response = api.AddTag( addLink.Href, id, name );
				Assert.Equal( HttpStatusCode.Accepted, response.StatusCode );
				Href = Assert.IsType<string>( response.Headers.Single( x => x.Name == "Location" ).Value );
			}

			void IDisposable.Dispose()
			{
				_api.DeleteTag( Href );
			}
		}

		public class AddLinkFixture
		{
			public string Href { get; private set; }

			public AddLinkFixture( SpPortalApi api )
			{
				var response = api.GetTagCollection();
				Assert.Equal( HttpStatusCode.OK, response.StatusCode );
				Href = response.Data._links.add.href;
				Assert.NotEmpty( Href );
			}
		}
	}
}