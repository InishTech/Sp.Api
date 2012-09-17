using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using RestSharp;
using Sp.Portal.Acceptance.Wrappers;
using Sp.Test.Helpers;
using Xunit;
using Xunit.Extensions;

namespace Sp.Portal.Acceptance
{
	public class LicenseCustomerTags
	{
		public class Put
		{
			[Theory, PortalData]
			public void ShouldUpdateTags( [Frozen] SpPortalApi portalApi, RandomLicenseFromListFixture license, ExistingTagCollectionFixture tags )
			{
				var tagsWithValues = GenerateAnonymousValuesForKnownTags( tags.Tags );

				//Pick a random license and get license tags assignment href
				var licenseTagsAssignmentHref = license.Selected._links.tags.href;

				//Put new license tag collection
				var tagsToAddToLicense = new SpPortalApi.TagWithValueCollection { Tags = tagsWithValues };
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

					//TODO - use Assert.Equal(IEnumerable...) from AssertExtensions (requires Xunit 1.9)
					Assert.True( updatedLicense.Data.Tags.SequenceEqual( tagsToAddToLicense.Tags ) );
				} );
			}

			static Dictionary<string, string> GenerateAnonymousValuesForKnownTags( IEnumerable<SpPortalApi.Tag> tags )
			{
				var fixture = new Fixture();
				return tags.ToDictionary( x => x.Id.ToString(), x => fixture.CreateAnonymous<string>() );
			}

			[Theory, PortalData]
			public void TooLongValuesShouldBeRejected( [Frozen] SpPortalApi portalApi, RandomLicenseFromListFixture license, ExistingTagCollectionFixture tags )
			{
				var tagsWithValues = GenerateInsanelyLongAnonymousValuesForKnownTags( tags.Tags );

				//Pick a random license and get license tags assignment href
				var licenseTagsAssignmentHref = license.Selected._links.tags.href;

				//Put new license tag collection
				var tagsToAddToLicense = new SpPortalApi.TagWithValueCollection { Tags = tagsWithValues };
				var apiResult = portalApi.PutLicenseTags( licenseTagsAssignmentHref, tagsToAddToLicense );
				Assert.Equal( ResponseStatus.Completed, apiResult.ResponseStatus );
				Assert.Equal( HttpStatusCode.BadRequest, apiResult.StatusCode );
			}

			static Dictionary<string, string> GenerateInsanelyLongAnonymousValuesForKnownTags( IEnumerable<SpPortalApi.Tag> tags )
			{
				const int insaneStringLength = 101;
				var fixture = new Fixture();
				return tags.ToDictionary(
					x => x.Id.ToString(),
					x => ConstrainedStringGenerator.CreateAnonymous( insaneStringLength, insaneStringLength, fixture )
				);
			}

			static class ConstrainedStringGenerator
			{
				public static string CreateAnonymous( int minimumLength, int maximumLength, IFixture fixture )
				{
					var sb = new StringBuilder();

					do
					{
						sb.Append( fixture.CreateAnonymous<string>() );
					}
					while ( sb.Length < minimumLength );

					if ( sb.Length > maximumLength )
					{
						return sb.ToString().Substring( 0, maximumLength );
					}

					return sb.ToString();
				}
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
	}

	static class EnumerableSubsetExtensions
	{
		public static bool IsSubsetOf<T>( this IEnumerable<T> thats, IEnumerable<T> otherCollection )
		{
			return new HashSet<T>( thats ).IsSubsetOf( otherCollection );
		}
	}
}