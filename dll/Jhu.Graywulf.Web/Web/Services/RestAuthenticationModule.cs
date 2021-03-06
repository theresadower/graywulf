﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Configuration;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Web;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web.Security;

namespace Jhu.Graywulf.Web.Services
{
    /// <summary>
    /// Implements a module that can authenticate REST web service clients
    /// based on the information in the request header.
    /// </summary>
    class RestAuthenticationModule : Security.AuthenticationModuleBase, IDispatchMessageInspector, IParameterInspector, IDisposable
    {
        #region Constructors and initializers

        public RestAuthenticationModule()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion
        #region WCF service life-cycle methods

        public void Init(Domain domain)
        {
            // (0.) Called by the framework when the authenticator module
            // is created an attached to the WCF service endpoints

            // Create authenticators
            var af = AuthenticationFactory.Create(domain);
            RegisterAuthentications(af.GetAuthentications(AuthenticatorProtocolType.RestRequest));
        }

        #endregion
        #region WCF invocation life-cycle methods

        /// <summary>
        /// Authenticates a request based on the headers
        /// </summary>
        /// <param name="request"></param>
        /// <param name="channel"></param>
        /// <param name="instanceContext"></param>
        /// <returns></returns>
        public object AfterReceiveRequest(ref Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
        {
            // (1.) Called before the WCF operation is invoked

            try
            {
                var authreq = new AuthenticationRequest(WebOperationContext.Current.IncomingRequest);
                var response = Authenticate(authreq);

                // TODO: session handling could be added here, if necessary
                // would be nice to detect user arrival, etc.

                // The return value is passed to BeforeSendReply
                return response;
            }
            catch (Exception ex)
            {
                // Wrap everything into a fault exception so that it
                // is returned to the client nicely packed.

#if BREAKDEBUG
                System.Diagnostics.Debugger.Break();
#endif

                throw new WebFaultException<Exception>(ex, System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public object BeforeCall(string operationName, object[] inputs)
        {
            // (2.)

            // This can be used to authenticate a request based on function parameters
            // These are parsed parameters, not necessarily REST query strings.
            return null;
        }

        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
            // (3.)

            // Required by the interface, not used.
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            // (4.)

            var context = WebOperationContext.Current;
            var response = (AuthenticationResponse)correlationState;

            // Set response headers and cookies created by the authenticators
            // This response doesn't contain any headers when authentication is
            // done "manually" by service functions

            if (response != null)
            {
                response.SetResponseHeaders(context.OutgoingResponse);
            }
        }

        protected override void OnAuthenticated(AuthenticationResponse response)
        {
            // (5.)  Called after successfull authentication

            System.Threading.Thread.CurrentPrincipal = response.Principal;
        }

        protected override void OnAuthenticationFailed(AuthenticationResponse response)
        {
            // (6.)

            // This only means that the custom authenticators could not
            // identify the user, but it still might have been identified by
            // the web server (from Forms ticket, windows authentication, etc.)
            // In this case, the principal provided by the framework needs to
            // be converted to a graywulf principal

            var principal = DispatchPrincipal(System.Threading.Thread.CurrentPrincipal);
            System.Threading.Thread.CurrentPrincipal = principal;
        }

        #endregion
    }
}
