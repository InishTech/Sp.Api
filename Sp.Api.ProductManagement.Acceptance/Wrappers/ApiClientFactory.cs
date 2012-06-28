namespace Sp.Api.ProductManagement.Acceptance.Wrappers
{
	public interface IApiClientFactory
	{
		SpApi CreateSpApiClient();
		SpProductManagementApi CreateProductManagementApiClient();
		SpLicenseManagementApi CreateLicenseManagementApiClient();
	}

	public class FixedBaseUrisApiClientFactory : IApiClientFactory
	{
		readonly SpApiConfiguration _configuration;

		public FixedBaseUrisApiClientFactory( SpApiConfiguration configuration )
		{
			_configuration = configuration;
		}

		SpApiConfiguration CreateConfigurationForBaseUrl( string baseUrl )
		{
			return new SpApiConfiguration( _configuration.Username, _configuration.Password, baseUrl );
		}

		public SpApi CreateSpApiClient()
		{
			var configurationForBaseUrl = CreateConfigurationForBaseUrl( _configuration.BaseUrl + "/Sp.Web" );
			return new SpApi( configurationForBaseUrl );
		}

		public SpProductManagementApi CreateProductManagementApiClient()
		{
			var configurationForBaseUrl = CreateConfigurationForBaseUrl( _configuration.BaseUrl + "/Sp.Web.Define" );
			return new SpProductManagementApi( configurationForBaseUrl );
		}

		public SpLicenseManagementApi CreateLicenseManagementApiClient()
		{
			var configurationForBaseUrl = CreateConfigurationForBaseUrl( _configuration.BaseUrl + "/Sp.Web.Issue" );
			return new SpLicenseManagementApi( configurationForBaseUrl );
		}
	}
}