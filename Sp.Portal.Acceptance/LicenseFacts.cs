using System;
using System.Net;
using Sp.Portal.Acceptance.Wrappers;
using Xunit;
using Xunit.Extensions;

namespace Sp.Portal.Acceptance
{
	public class LicenseFacts
	{
		public class Index
		{
			[Theory, PortalData]
			public void GetShouldYieldResults( SpPortalApi api )
			{
				var apiResult = api.GetLicenseList();
				// It should always be possible to get the list
				Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
				// If the request is OK, there should always be some Data
				Assert.NotNull( apiResult.Data );
				//-- Portal consumer expects Issue date, activation key, product, version, eval, renewal
				// An empty list is always represented as an empty collection, not null
				Assert.NotNull( apiResult.Data.Licenses );
			}
		}

		public class Get
		{
			[Theory, PortalData]
			public static void ShouldContainData( RandomLicenseFromListFixture license )
			{
				// There should always be valid Activation Key
				Assert.NotEmpty( license.Selected.ActivationKey );
				// There should always be a Product Label
				Assert.NotEmpty( license.Selected.ProductLabel );
				// There should always be a Version Label
				Assert.NotEmpty( license.Selected.VersionLabel );
				// There is always an IssueDate
				Assert.NotEmpty( license.Selected.IssueDate );
			}

			[Theory, PortalData]
			public void ShouldIncludeLicenseTagsElement( RandomLicenseFromListFixture license )
			{
				// There should always be valid Activation Key
				Assert.NotNull( license.Selected.Tags );
			}


			[Theory, PortalData]
			public void ShouldIncludeSelfLink( RandomLicenseFromListFixture license )
			{
				Assert.NotNull( license.Selected._links.self );
				Assert.NotEmpty( license.Selected._links.self.href );
			}

			[Theory, PortalData]
			public void ShouldIncludeLicenseTagsAssignmentLink( RandomLicenseFromListFixture license )
			{
				Assert.NotNull( license.Selected._links.tags );
				Assert.NotEmpty( license.Selected._links.tags.href );
			}
		}
	}
}
