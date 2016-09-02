using RestSharp;
using Sp.Api.Portal.Acceptance.Wrappers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;
using Xunit.Extensions;

namespace Sp.Api.Portal.Acceptance
{
    public static class Activations
    {
        public static class IndexGet
        {
            [Theory, AutoPortalData]
            public static void ShouldYieldResults( SpPortalActivationApi api )
            {
                var apiResult = api.GetActivations();
                VerifyResponse( apiResult );
            }

            [Theory, AutoPortalData]
            public static void ShouldAllowPaging( SpPortalActivationApi api )
            {
                var firstQuery = api.GetActivations( "$skip=0&$top=1" );
                var secondQuery = api.GetActivations( "$skip=1&$top=1" );

                SpPortalActivationApi.Activation first = VerifyResponse( firstQuery ).Single();
                SpPortalActivationApi.Activation second = VerifyResponse( secondQuery ).Single();

                Assert.NotNull( first );
                Assert.NotNull( second );
                Assert.NotEqual( first._links.self.href, second._links.self.href );
            }

            [Theory, AutoPortalData]
            public static void ShouldAllowCounting( SpPortalActivationApi api )
            {
                var apiResult = api.GetActivations( "$inlinecount=allpages&$top=1" );

                VerifyResponse( apiResult, shouldHaveCount: true );
                Assert.True( apiResult.Data.__count > 1 );
                Assert.Equal( 1, apiResult.Data.results.Count );
            }

            [Theory, AutoPortalData]
            public static void ShouldAllowOrdering( SpPortalActivationApi api )
            {
                var apiResult = api.GetActivations( "$orderby=ActivationTime desc" );

                var data = VerifyResponse( apiResult );

                Assert.NotEmpty( data );
                var ordered = data.OrderByDescending( x => x.ActivationTime ).ToArray();

                Assert.Equal( ordered.AsEnumerable(), data.AsEnumerable() );
            }

            [Theory, AutoPortalData]
            public static void ShouldRespondToUnsupportedQueriesWithBadRequestAndStatusDescription( SpPortalActivationApi api )
            {
                var expectedFailures = new Dictionary<string, string>()
                {
                    {"$top=1001","The field Top must be between 1 and 1000."},
                    {"$top=NaN","The field Top must be between 1 and 1000."},
                    {"$top=","The Top field is required."},         // a specified query field must be provided
				    {"$orderby=","The OrderBy field is required."}  // a specified query field must be provided
				};

                foreach ( var expectedFailure in expectedFailures )
                {
                    var invalidQueryResult = api.GetActivations( expectedFailure.Key );
                    Assert.Equal( HttpStatusCode.BadRequest, invalidQueryResult.StatusCode );
                    Assert.Equal( expectedFailure.Value, invalidQueryResult.StatusDescription );
                }
            }

            static SpPortalActivationApi.Activation[] VerifyResponse( IRestResponse<SpPortalActivationApi.Activations> apiResult, bool shouldHaveCount = false )
            {
                // It should always be possible to get the list
                Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
                // If the request is OK, there should always be some Data
                Assert.NotNull( apiResult.Data );
                Assert.NotNull( apiResult.Data.results );

                Assert.Equal( shouldHaveCount, apiResult.Data.__count.HasValue );

                return apiResult.Data.results.ToArray();
            }
        }
    }
}