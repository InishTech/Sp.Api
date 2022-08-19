
/* Copyright (c) 2017 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */

using RestSharp;
using Sp.Api.Shared.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sp.Api.Develop.Acceptance
{
    public class SpWebHookApi : SpApi
    {
        readonly string _url;

        public SpWebHookApi( SpApiConfiguration apiConfiguration ) : base( apiConfiguration )
        {
            _url = $"{GetApiPrefix( ApiType.Develop )}/webhook";
        }
        public IRestResponse CreateWebHook( WebHookRegistrationModel webhook )
        {
            var request = new RestRequest( _url, Method.POST );
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody( webhook );
            return Execute( request );
        }

        public IRestResponse UpdateWebHook(string href, WebHookRegistrationModel webhook )
        {
            var request = new RestRequest( href, Method.PUT );
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody( webhook );
            return Execute( request );
        }

        internal IRestResponse<WebHookRegistrationModel> GetWebhook( string href )
        {
            var request = new RestRequest( href );
            return Execute<WebHookRegistrationModel>( request );
        }

        internal IRestResponse<WebHookRegistrationModel> GetWebhookById( Guid id )
        {
            var request = new RestRequest( $"{_url}/{id}" );
            return Execute<WebHookRegistrationModel>( request );
        }

        internal IRestResponse<List<WebHookRegistrationModel>> ListWebHooks(  )
        {
            var request = new RestRequest( _url );
            return Execute<List<WebHookRegistrationModel>>( request );
        }

        public class WebHookRegistrationModel
        {
            public string[] Actions { get; set; }

            public string Secret { get; set; }

            public Uri WebHookUri { get; set; }
            public bool IsPaused { get; set; }
            public Links _links { get; set; }

            public class Links
            {
                public Link self { get; set; }
            }
        }
        public class Link
        {
            public string href { get; set; }
        }
    }

    public class SpInvalidWebHookApi : SpApi
    {
        readonly string _url;

        public SpInvalidWebHookApi( SpApiConfiguration apiConfiguration ) : base( apiConfiguration )
        {
            _url = $"{GetApiPrefix( ApiType.Develop )}/webhook";
        }

        public IRestResponse CreateWebHook( InvalidWebHookRegistrationModel webhook )
        {
            var request = new RestRequest( _url, Method.POST );
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody( webhook );
            return Execute( request );
        }
   
        public class InvalidWebHookRegistrationModel
        {
            public string[] Actions { get; set; }

            public string Secret { get; set; }

            public string WebHookUri { get; set; }
        }
    }
    public enum WebHookEvents
    {
        LicenseIssue = 0,
        LicenseReissue = 1
    }
}
