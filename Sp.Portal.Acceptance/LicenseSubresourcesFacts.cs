using System.Collections.Generic;
using System.Linq;
using System.Net;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using RestSharp;
using Sp.Portal.Acceptance.Wrappers;
using Sp.Test.Helpers;
using Xunit;
using Xunit.Extensions;

namespace Sp.Portal.Acceptance
{
	public class LicenseSubresourcesFacts
	{
		public class LicenseTags
		{
			public class Put
			{
				[Theory, PortalData]
				public void ShouldUpdateTags( [Frozen] SpPortalApi portalApi, RandomLicenseFromListFixture license, ExistingTagCollectionFixture tags )
				{
					var fixture = new Fixture();
					//Generate random tag values. Reset tag names (they're irrelevant here)
					var tagsWithValues = tags.Tags.Select(
						x => new SpPortalApi.TagWithValue { TagId = x.Id, Name = string.Empty, Value = fixture.CreateAnonymous<string>() } ).ToArray();


					//Pick a random license and get license tags assignment href
					var licenseTagsAssignmentHref = license.Selected._links.tags.href;

					//Put new license tag collection
					var tagsToAddToLicense = new SpPortalApi.TagWithValueCollection { Tags = tagsWithValues.ToList() };
					var apiResult = portalApi.PutLicenseTags( licenseTagsAssignmentHref, tagsToAddToLicense );
					Assert.Equal( ResponseStatus.Completed, apiResult.ResponseStatus );
					Assert.Equal( HttpStatusCode.Accepted, apiResult.StatusCode );

					Verify.EventuallyWithBackOff( () =>
					{
						//Retrieve the license again (or just the tags subresource)
						//Verify that all the tags on the license match
						var updatedLicense = portalApi.GetLicense( license.Selected._links.self.href );
						Assert.NotNull( updatedLicense );
						Assert.NotNull( updatedLicense.Data.Tags );
						Assert.NotEmpty( updatedLicense.Data.Tags );

						//TODO TP 1048 - use Assert.Equal(IEnumerable...) from AssertExtensions (requires Xunit 1.9)
						Assert.True( updatedLicense.Data.Tags.SequenceEqual( tagsToAddToLicense.Tags,
						    new AssertExtensions.FuncEqualityComparer
							    <SpPortalApi.TagWithValue>(
							    ( x, y ) => x.TagId == y.TagId && x.Value == y.Value ) ) );
					} );
				}
			}

			public class ExistingTagCollectionFixture
			{
				SpPortalApi.Tag[] _tags;

				public ExistingTagCollectionFixture( SpPortalApi api, CustomerTags.ExistingTagFixture[] tagsToCreate )
				{
					Verify.EventuallyWithBackOff( () =>
					{
						//Wait until all new customer tags are created
						var apiResult = api.GetTagCollection();
						Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
						Assert.NotNull( apiResult.Data.Tags );
						var tagNamesFromApi = apiResult.Data.Tags.Select( t => t.Name );
						var newlyCreatedTagNames = tagsToCreate.Select( t => t.Name );
						Assert.True( newlyCreatedTagNames.IsSubsetOf( tagNamesFromApi ) );

						_tags = apiResult.Data.Tags
							.Where( x => tagsToCreate.Any( y => y.Name == x.Name ) )
							.ToArray();
					} );
				}


				public SpPortalApi.Tag[] Tags
				{
					get { return _tags; }
				}
			}

			//public class ExistingTagWithValueCollectionFixture : ExistingTagCollectionFixture
			//{
			//	readonly SpPortalApi.TagWithValue[] _tagsWithValues;

			//	public ExistingTagWithValueCollectionFixture( SpPortalApi api, CustomerTags.ExistingTagFixture[] tagsToCreate, Fixture fixture )
			//		: base( api, tagsToCreate )
			//	{

			//		_tagsWithValues = Tags.Select(
			//				x => new SpPortalApi.TagWithValue { TagId = x.Id, Name = x.Name, Value = fixture.CreateAnonymous<string>() } ).ToArray();
			//	}

			//	public SpPortalApi.TagWithValue[] TagsWithValuesWithValues
			//	{
			//		get { return _tagsWithValues; }
			//	}
			//}
		}
	}

	static class EnumerableSubsetExtensions
	{
		public static bool IsSubsetOf<T>( this IEnumerable<T> thats, IEnumerable<T> otherCollection )
		{
			return thats.All( otherCollection.Contains );
		}
	}
}