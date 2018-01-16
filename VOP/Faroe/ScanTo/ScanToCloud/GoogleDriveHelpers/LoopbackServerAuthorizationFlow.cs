/*
Copyright 2011 Google Inc

Licensed under the Apache License, Version 2.0(the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using DotNetOpenAuth.OAuth2;
using System.Threading;
using VOP;

namespace Google.Apis.Helper
{
    /// <summary>
    /// A native authorization flow which uses a listening local loopback socket to fetch the authorization code.
    /// </summary>
    /// <remarks>Might not work if blocked by the system firewall.</remarks>
    public class LoopbackServerAuthorizationFlow : INativeAuthorizationFlow
    {
        private const string LoopbackCallback = "http://localhost:{0}/{1}/authorize/";
        private static string _code = null;

        /// <summary>
        /// Returns a random, unused port.
        /// </summary>
        private static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            try
            {
                listener.Start();
                return ((IPEndPoint)listener.LocalEndpoint).Port;
            }
            finally
            {
                listener.Stop();
            }
        }

        /// <summary>
        /// Handles an incoming WebRequest.
        /// </summary>
        /// <param name="context">The request to handle.</param>
        /// <param name="appName">Name of the application handling the request.</param>
        /// <returns>The authorization code, or null if the process was cancelled.</returns>
        private string HandleRequest(HttpListenerContext context)
        {
            Win32.OutputDebugString("HandleRequest===>Enter");
            try
            {
                // Check whether we got a successful response:
                string code = context.Request.QueryString["code"];
                if (!string.IsNullOrEmpty(code))
                {
                    Win32.OutputDebugString("HandleRequest===LEave, code");
                    return code;
                }

                // Check whether we got an error response:
                string error = context.Request.QueryString["error"];
                if (!string.IsNullOrEmpty(error))
                {
                    Win32.OutputDebugString("HandleRequest===LEave, error");
                    return null; // Request cancelled by user.
                }

                // The response is unknown to us. Choose a different authentication flow.
                throw new NotSupportedException(
                    "Received an unknown response: " + Environment.NewLine + context.Request.RawUrl);
            }
            catch (Exception ex)
            {
                Win32.OutputDebugString(ex.Message);
            }
            finally
            {
                // Write a response.
                using (var writer = new StreamWriter(context.Response.OutputStream))
                {
                    string response = "VOP OAuth Authentication";
                    writer.WriteLine(response);
                    writer.Flush();
                }
                context.Response.OutputStream.Close();
                context.Response.Close();
            }
            Win32.OutputDebugString("HandleRequest===LEave");
            return null;
        }

        private static void ListenerCallback(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;
            HttpListenerContext context = null;
            try
            {
                // Call EndGetContext to complete the asynchronous operation.
                context = listener.EndGetContext(result);

                try
                {
                    // Check whether we got a successful response:
                    string code = context.Request.QueryString["code"];
                    if (!string.IsNullOrEmpty(code))
                    {
                        _code = code;
                    }

                    // Check whether we got an error response:
                    string error = context.Request.QueryString["error"];
                    if (!string.IsNullOrEmpty(error))
                    {
                        // Request cancelled by user.
                    }

                    // The response is unknown to us. Choose a different authentication flow.
                    throw new NotSupportedException(
                        "Received an unknown response: " + Environment.NewLine + context.Request.RawUrl);
                }
                catch (Exception ex)
                {
                    Win32.OutputDebugString(ex.Message);
                }
                finally
                {
                    // Write a response.
                    using (var writer = new StreamWriter(context.Response.OutputStream))
                    {
                        string response = "VOP OAuth Authentication";
                        writer.WriteLine(response);
                        writer.Flush();
                    }
                    context.Response.OutputStream.Close();
                    context.Response.Close();
                }
            }
            catch (Exception ex)
            {
                Win32.OutputDebugString(ex.Message);
            }
        }
        public string RetrieveAuthorization(UserAgentClient client, IAuthorizationState authorizationState)
        {
            Win32.OutputDebugString("RetrieveAuthorization===Enter");
            if (!HttpListener.IsSupported)
            {
                Win32.OutputDebugString("HttpListener is not supported by this platform.");
                throw new NotSupportedException("HttpListener is not supported by this platform.");
            }

            // Create a HttpListener for the specified url.
            string url = string.Format(LoopbackCallback, GetRandomUnusedPort(), Google.Apis.Util.Utilities.ApplicationName);
            authorizationState.Callback = new Uri(url);
            
            var webserver = new HttpListener();
            webserver.Prefixes.Add(url);
            
            // Retrieve the authorization url.
            Uri authUrl = client.RequestUserAuthorization(authorizationState);
            
            try
            {
                // Start the webserver.
                webserver.Start();

                Win32.OutputDebugString(authUrl.ToString());
                // Open the browser.
                Process.Start(authUrl.ToString());

                // Wait for the incoming connection, then handle the request.

                IAsyncResult result = webserver.BeginGetContext(new AsyncCallback(ListenerCallback), webserver);

                result.AsyncWaitHandle.WaitOne(10000);                

                return _code;

                //return HandleRequest(webserver.GetContext());
            }
            catch (HttpListenerException ex)
            {
                Win32.OutputDebugString("The HttpListener threw an exception.");
                Win32.OutputDebugString(ex.Message);
                throw new NotSupportedException("The HttpListener threw an exception.", ex);
            }
            finally
            {
                // Stop the server after handling the one request.
                webserver.Stop();
            }
            Win32.OutputDebugString("RetrieveAuthorization===Leave");
        }
    }
}
