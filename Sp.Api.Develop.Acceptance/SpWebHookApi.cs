
/* Copyright (c) 2017 Inish Technology Ventures Limited.  All rights reserved.
 * 
 * This code is licensed under the BSD 3-Clause License included with this source
 * 
 * FOR DETAILS, SEE https://github.com/InishTech/Sp.Api/wiki/License */

using RestSharp;
using Sp.Api.Shared.Wrappers;
using System;
using System.Linq;

namespace Sp.Api.Develop.Acceptance
{
    public class SpWebHookApi : SpApi
    {
        public SpWebHookApi(SpApiConfiguration apiConfiguration) : base(apiConfiguration)
        {

        }
        public IRestResponse CreateWebHook(WebHookRegistrationModel webhook)
        {
            var request = new RestRequest(GetApiPrefix(ApiType.Develop) + "/webhook", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(webhook);
            return Execute(request);
        }

        public class WebHookRegistrationModel
        {
            public string EventId { get; set; }

            public string Description { get; set; }

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
