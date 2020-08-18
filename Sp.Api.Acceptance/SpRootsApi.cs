using RestSharp;
using Sp.Api.Shared.Wrappers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sp.Api.Acceptance
{
    public class SpRootsApi : SpApi
    {
        public SpRootsApi( SpApiConfiguration apiConfiguration )
            : base( apiConfiguration )
        {
        }

        internal IRestResponse<RootIndexModel> GetRootList()
        {
            var request = new RestRequest( GetApiPrefix( ApiType.WebApiRoot ) );
            return Execute<RootIndexModel>( request );
        }
    }
    public class RootIndexModel
    {
        public RootIndexModel() { }
        public List<RootModel> results { get; set; }
    }

    public class RootModel
    {
        public RootModel() { }
        public Dictionary<string, HalLink> _links { get; set; }
    }

    public class HalLink
    {
        public HalLink()
        {
        }

        public string href { get; set; }
    }

}
