﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Jhu.Graywulf.Web.Services
{
    public class RestClientSession : IDisposable
    {
        private RestClientBehavior restClientBehavior;

        public CookieContainer Cookies
        {
            get { return restClientBehavior.Cookies; }
        }

        public NameValueCollection Headers
        {
            get { return restClientBehavior.Headers; }
        }

        public HashSet<string> AcceptedHeaders
        {
            get { return restClientBehavior.AcceptedHeaders; }
        }

        public RestClientSession()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.restClientBehavior = new RestClientBehavior();
        }

        public void Dispose()
        {
        }

        public T CreateClient<T>(Uri uri)
        {
            return CreateClient<T>(uri, null);
        }

        public T CreateClient<T>(Uri uri, Uri proxy)
        {
            return CreateChannel<T>(CreateWebHttpBinding(proxy), CreateEndpointAddress(uri));
        }

        private T CreateChannel<T>(WebHttpBinding webHttpBinding, EndpointAddress endpoint)
        {
            var cf = new ChannelFactory<T>(webHttpBinding, endpoint);
            cf.Endpoint.Behaviors.Add(restClientBehavior);
            return cf.CreateChannel();
        }

        private WebHttpBinding CreateWebHttpBinding(Uri proxy)
        {
            var webHttpBinding = new WebHttpBinding(WebHttpSecurityMode.None);

            if (proxy != null)
            {
                webHttpBinding.ProxyAddress = proxy;
                webHttpBinding.UseDefaultWebProxy = false;
            }

            webHttpBinding.MaxReceivedMessageSize = 0x40000000;

            webHttpBinding.ReaderQuotas.MaxArrayLength = 0x7FFFFFFF;
            webHttpBinding.ReaderQuotas.MaxDepth = 0x7FFFFFFF;
            webHttpBinding.ReaderQuotas.MaxStringContentLength = 0x7FFFFFFF;

            webHttpBinding.AllowCookies = false;
            webHttpBinding.TransferMode = TransferMode.Streamed;

            return webHttpBinding;
        }

        private EndpointAddress CreateEndpointAddress(Uri uri)
        {
            return new EndpointAddress(uri);
        }

        public byte[] MakeWebRequest(string url)
        {
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.CookieContainer = Cookies;

            var res = (HttpWebResponse)req.GetResponse();
            Cookies.Add(res.Cookies);

            var stream = res.GetResponseStream();
            var ms = new MemoryStream();
            var buffer = new byte[0x10000];
            while (true)
            {
                var b = stream.Read(buffer, 0, buffer.Length);

                if (b <= 0)
                {
                    break;
                }

                ms.Write(buffer, 0, b);
            }
            
            return ms.ToArray();
        }
    }
}
