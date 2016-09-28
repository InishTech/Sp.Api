/* Copyright (c) 2012 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */
namespace Sp.Api.Portal.Acceptance
{
	using Ploeh.AutoFixture;
	using Ploeh.AutoFixture.Xunit;
	using RestSharp;
	using Sp.Api.Portal.Acceptance.Wrappers;
	using Sp.Test.Helpers;
	using System;
	using System.Linq;
	using System.Net;
	using Xunit;
	using Xunit.Extensions;

	public static class LicenseCustomerTags
	{
		public static class Put
		{
			[Theory, AutoPortalDataAttribute]
			public static void ShouldUpdateTags( [Frozen] SpPortalLicenseApi portalApi, RandomLicenseFromListFixture license, ExistingTagsFixture tags, IFixture fixture )
			{
				var valuesForEachDefinedTag = tags.Tags
					.Select( ct => fixture.Build<SpPortalLicenseApi.LicenseTag>().With( x => x.Id, ct.Id ).CreateAnonymous() )
					.ToList();

				var licenseTagsAssignmentHref = license.Selected._embedded.CustomerTags._links.self.href;
				var apiResult = portalApi.PutLicenseTags( licenseTagsAssignmentHref, valuesForEachDefinedTag );
				Assert.Equal( ResponseStatus.Completed, apiResult.ResponseStatus );
				Assert.Equal( HttpStatusCode.Accepted, apiResult.StatusCode );

				Verify.EventuallyWithBackOff( () =>
				{
					//Retrieve the license again (if it was supported, just GETting the tags subresource would be sufficient to prove the point)
					var updatedLicense = portalApi.GetLicense( license.Selected._links.self.href );

					//Verify that all the tags on the license match
					Assert.Equal( ResponseStatus.Completed, updatedLicense.ResponseStatus );
					Assert.NotNull( updatedLicense.Data._embedded.CustomerTags.results );
					Assert.NotEmpty( updatedLicense.Data._embedded.CustomerTags.results );
					// TOOD use xUnit 1.9 and/or Ploeh.semanticComparison to make this cleaner
					Assert.Equal(
						updatedLicense.Data._embedded.CustomerTags.results.OrderBy( x => x.Id ).Select( x => Tuple.Create( x.Id, x.Value ) ).ToArray(),
						valuesForEachDefinedTag.OrderBy( x => x.Id ).Select( x => Tuple.Create( x.Id, x.Value ) ).ToArray() );
				} );
			}

			public static class Bad
			{
				[Theory, AutoPortalDataAttribute]
				public static void EmptyValuesShouldBeRejected( [Frozen] SpPortalLicenseApi portalApi, RandomLicenseFromListFixture license, ExistingTagsFixture tags )
				{
					var validTagWithValueThatIsEmpty = GenerateLicenseTag( String.Empty, tags );

					var licenseTagsAssignmentHref = license.Selected._embedded.CustomerTags._links.self.href;
					var apiResult = portalApi.PutLicenseTags( licenseTagsAssignmentHref, validTagWithValueThatIsEmpty );
					Assert.Equal( ResponseStatus.Completed, apiResult.ResponseStatus );
					Assert.Equal( HttpStatusCode.BadRequest, apiResult.StatusCode );
				}

				[Theory, AutoPortalDataAttribute]
				public static void TooLongValuesShouldBeRejected( [Frozen] SpPortalLicenseApi portalApi, RandomLicenseFromListFixture license, ExistingTagsFixture tags, IFixture fixture )
				{
					var validTagWithValueThatIsTooLong = GenerateLicenseTag( new String( 'a', 101 ), tags );

					var licenseTagsAssignmentHref = license.Selected._embedded.CustomerTags._links.self.href;
					var apiResult = portalApi.PutLicenseTags( licenseTagsAssignmentHref, validTagWithValueThatIsTooLong );
					Assert.Equal( ResponseStatus.Completed, apiResult.ResponseStatus );
					Assert.Equal( HttpStatusCode.BadRequest, apiResult.StatusCode );
				}

				static SpPortalLicenseApi.LicenseTag[] GenerateLicenseTag( string value, ExistingTagsFixture tags )
				{
					return tags.Tags
						.Select( ct => new Fixture().Build<SpPortalLicenseApi.LicenseTag>()
							.With( x => x.Id, ct.Id )
							.With( x => x.Value, value )
							.CreateAnonymous() )
						.Take( 1 )
						.ToArray();
				}

                [Theory( Skip = "Was working by coincidence, tag functionality removed for now so ignore" ), AutoPortalDataAttribute]
                public static void DuplicateValuesShouldBeRejected( [Frozen] SpPortalLicenseApi portalApi, RandomLicenseFromListFixture license, ExistingTagsFixture tags, IFixture fixture )
				{
					var validTagValue = tags.Tags
						.Select( ct => fixture.Build<SpPortalLicenseApi.LicenseTag>()
							.With( x => x.Id, ct.Id )
							.CreateAnonymous() )
						.Take( 1 )
						.ToArray();

					var theSameValueTwice = validTagValue.Concat( validTagValue ).ToArray();

					var licenseTagsAssignmentHref = license.Selected._embedded.CustomerTags._links.self.href;
					var apiResult = portalApi.PutLicenseTags( licenseTagsAssignmentHref, theSameValueTwice );
					Assert.Equal( ResponseStatus.Completed, apiResult.ResponseStatus );
					Assert.Equal( HttpStatusCode.InternalServerError, apiResult.StatusCode );
				}
			}
		}
	}
}