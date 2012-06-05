namespace Sp.Api.ProductManagement.Acceptance
{
	using System;
	using System.Linq;
	using System.Net;
	using Ploeh.AutoFixture.Xunit;
	using Sp.Api.ProductManagement.Acceptance.Helpers;
	using Sp.Api.ProductManagement.Acceptance.Wrappers;
	using Xunit;
	using Xunit.Extensions;

	public static class LicenseFacts
	{
		public static class GetCollection
		{
			/// <summary>
			/// The master license list yields a JSON response object which contains the License List collection in a 'Licenses' value.
			/// </summary>
			/// <remarks>
			/// There are no standard failure conditions for this request - even an empty list returns a well formed result, just with the 'Licenses' collection empty.
			/// </remarks>
			/// <param name="api">Api wrapper.</param>
			[Theory, AutoSoftwarePotentialData]
			public static void GetListShouldYieldData( SpLicenseManagementApi api )
			{
				var apiResult = api.GetList();
				// It should always be possible to get the list
				Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
				// If the request is OK, there should always be some Data
				Assert.NotNull( apiResult.Data );
				//-- Portal consumer expects Issue date, activation key, product, version, eval, renewal
				// An empty list is always represented as an empty collection, not null
				Assert.NotNull( apiResult.Data.Licenses );
			}

			/// <summary>
			/// The master collection provides a set of summary records. Here we select an arbitrary one from the list and make assertions as to its format.
			/// </summary>
			/// <remarks>
			/// Success/failure is communicated by the HTTP Status Code being OK		
			/// </remarks>
			/// <param name="api">Api wrapper. [Frozen] so requests involved in getting <paramref name="license"/> can share the authentication work.</param>
			/// <param name="license">Arbitrarily chosen product from the configured user's list (the account needs at least one)</param>
			[Theory, AutoSoftwarePotentialData]
			public static void ElementFromListShouldContainData( [Frozen] SpProductManagementApi api, RandomLicenseFromListFixture license )
			{
				// There should always be valid Activation Key
				Assert.NotEmpty( license.SelectedLicense.ActivationKey );
				// There should always be a Product Label
				Assert.NotEmpty( license.SelectedLicense.ProductLabel );
				// There should always be a Version Label
				Assert.NotEmpty( license.SelectedLicense.VersionLabel );
				// There is always an IssueDate
				Assert.NotEqual( default( DateTime ), license.SelectedLicense.IssueDate );
			}

			[Theory(Skip = "Fields to be exercised by future License creation+migration examples"), AutoSoftwarePotentialData]
			public static void ElementFromListShouldContainDataUntestedProperties( [Frozen] SpProductManagementApi api, RandomLicenseFromListFixture license )
			{
				// TODO these are here so the properties are referenced. TODO: Add roundtrip test which verifies that true and false values can propagate
				// There is always a flag indicating the evaluation status
				var dummy = license.SelectedLicense.IsEvaluation;
				// TODO these are here so the properties are referenced. TODO: Add roundtrip test which verifies that true and false values can propagate
				// There is always a flag indicating the evaluation status
				var dummy2 = license.SelectedLicense.IsRenewable;
			}
		}

		// TODO when we support creating licenses via the REST API, this fixture should create one on the fly
		public class RandomLicenseFromListFixture
		{
			readonly SpLicenseManagementApi.LicenseSummary _randomItem;

			public RandomLicenseFromListFixture( SpLicenseManagementApi api )
			{
				var apiResult = api.GetList();
				Assert.Equal( HttpStatusCode.OK, apiResult.StatusCode );
				Assert.True( apiResult.Data.Licenses.Any(), GetType().Name + " requires the target login to have at least one License" );
				_randomItem = apiResult.Data.Licenses.ElementAtRandom();
			}

			public SpLicenseManagementApi.LicenseSummary SelectedLicense
			{
				get { return _randomItem; }
			}
		}
	}
}