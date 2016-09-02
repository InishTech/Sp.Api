namespace Sp.Api.Portal.Acceptance.Wrappers
{
    using RestSharp;
    using Sp.Api.Shared.Wrappers;
    using System;
    using System.Collections.Generic;

    public class SpPortalActivationApi : SpPortalApi
    {
        public SpPortalActivationApi( SpApiConfiguration apiConfiguration ) : base( apiConfiguration )
        {
        }

        public IRestResponse<Activations> GetActivations()
        {
            var request = new RestRequest( "Activation" );
            return Execute<Activations>( request );
        }

        public IRestResponse<Activations> GetActivations( string queryParameters )
        {
            var request = new RestRequest( "Activation/?" + queryParameters );
            return Execute<Activations>( request );
        }

        public class Link
        {
            public string href { get; set; }
        }

        public class Activation
        {
            public string ActivationKey { get; set; }
            public string DeviceId { get; set; }
            public string DeviceLabel { get; set; }
            public DateTime ActivationTime { get; set; }

            public Links _links { get; set; }
            public class Links
            {
                public Link self { get; set; }
            }
        }

        public class Activations
        {
            public int? __count { get; set; }
            public List<Activation> results { get; set; }
        }
    }
}
