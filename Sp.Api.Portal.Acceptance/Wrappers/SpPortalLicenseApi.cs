namespace Sp.Api.Portal.Acceptance.Wrappers
{
    using RestSharp;
    using Sp.Api.Shared.Wrappers;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SpPortalLicenseApi : SpPortalApi
    {
        public SpPortalLicenseApi( SpApiConfiguration apiConfiguration ) : base( apiConfiguration )
        {
        }

        internal IRestResponse<CustomerTags> GetCustomerTags()
        {
            var request = new RestRequest( "tag" );
            return Execute<CustomerTags>( request );
        }

        public IRestResponse PutCustomerTags( IEnumerable<CustomerTag> tags )
        {
            var request = new RestRequest( "tag", Method.PUT );
            request.RequestFormat = DataFormat.Json;
            request.AddBody( tags.ToList() );
            return Execute( request );
        }

        public IRestResponse PutLicenseTags( string licenseTagsAssignmentHref, IEnumerable<LicenseTag> tags )
        {
            var request = new RestRequest( licenseTagsAssignmentHref, Method.PUT );
            request.RequestFormat = DataFormat.Json;
            request.AddBody( tags );
            return Execute( request );
        }

        public IRestResponse<Licenses> GetLicenses()
        {
            var request = new RestRequest( "License" );
            return Execute<Licenses>( request );
        }

        public IRestResponse<Licenses> GetLicenses( string queryParameters )
        {
            var request = new RestRequest( "License/?" + queryParameters );
            return Execute<Licenses>( request );
        }

        public IRestResponse<License> GetLicense( string href )
        {
            var request = new RestRequest( href );
            return Execute<License>( request );
        }

        public class CustomerTags
        {
            public List<CustomerTag> results { get; set; }
        }

        public class CustomerTag
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class Link
        {
            public string href { get; set; }
        }

        public class License
        {
            public string ActivationKey { get; set; }

            public string ProductLabel { get; set; }
            public string VersionLabel { get; set; }

            public string IssueDate { get; set; }
            public bool IsEvaluation { get; set; }
            public bool IsRenewable { get; set; }
            //public Guid? CustomerId { get; set; }
            public bool IsActivatable { get; set; }
            public string ActivatableMessage { get; set; }

            public Embedded _embedded { get; set; }
            public Links _links { get; set; }

            public class Embedded
            {
                public LicenseTags CustomerTags { get; set; }
            }

            public class Links
            {
                public Link self { get; set; }
            }
        }

        public class LicenseTags
        {
            public List<LicenseTag> results { get; set; }

            public Links _links { get; set; }

            public class Links
            {
                public Link self { get; set; }
            }
        }

        public class LicenseTag
        {
            public Guid Id { get; set; }
            public string Value { get; set; }
        }

        public class Licenses
        {
            public int? __count { get; set; }
            public List<License> results { get; set; }
        }
        
    }
}
