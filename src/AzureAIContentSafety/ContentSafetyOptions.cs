﻿using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureAIContentSafety
{
    [ServiceConfiguration]
    public class ContentSafetyOptions
    {
        public string ContentSafetySubscriptionKey { get; set; }
        public string ContentSafetyEndpoint { get; set; }
    }
}