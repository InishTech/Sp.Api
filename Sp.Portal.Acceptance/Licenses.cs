using System.Net;
using Sp.Portal.Acceptance.Wrappers;
using Xunit;
using Xunit.Extensions;
using System.Linq;
using System.Collections.Generic;

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

                VerifyResponse( apiResult );
            }

            [Theory, PortalData]
            public void GetShouldAllowPaging( SpPortalApi api )
            {
                var firstQuery = api.GetLicenseList( "$skip=0&$top=1" );
                var secondQuery = api.GetLicenseList( "$skip=1&$top=1" );

                SpPortalApi.LicenseSummary first = VerifyResponse( firstQuery ).Single();
                SpPortalApi.LicenseSummary second = VerifyResponse( secondQuery ).Single();

                Assert.NotNull( first );
                Assert.NotNull( second );
                Assert.NotEqual( first._links.self, second._links.self );
            }

            [Theory, PortalData]
            public void GetShouldRespondToUnsupportedQueriesWithBadRequestAndStatusDescription( SpPortalApi api )
            {
                Dictionary<string, string> expectedFailures = new Dictionary<string, string>()
                {
                    {"$top=101","The field Top must be between 1 and 100."},
                    {"$top=NaN","The field Top must be between 1 and 100."},
                    {"$top=","The Top field is required."},         // a specified query field must be provided
                    {"$orderby=","The OrderBy field is required."}  // a specified query field must be provided
                };

                foreach (var expectedFailure in expectedFailures)
                {
                    var invalidQueryResult = api.GetLicenseList( expectedFailure.Key );
                    Assert.Equal( HttpStatusCode.BadRequest, invalidQueryResult.StatusCode );
                    Assert.Equal( expectedFailure.Value, invalidQueryResult.StatusDescription );
                }
            }

            private List<SpPortalApi.LicenseSummary> VerifyResponse( RestSharp.IRestResponse<SpPortalApi.LicensesSummaryPage> apiResult )
            {
                // It should always be possible to get the list
                Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
                // If the request is OK, there should always be some Data
                Assert.NotNull( apiResult.Data );
                //-- Portal consumer expects Issue date, activation key, product, version, eval, renewal
                // An empty list is always represented as an empty collection, not null
                Assert.NotNull( apiResult.Data.Licenses );

                return apiResult.Data.Licenses;
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
                // There are always tags - they might be empty though
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