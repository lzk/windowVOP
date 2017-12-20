// This file creates a Google.Apis.Helper namespace and adds the ClientCredentials
// class to that namespace. The code is completely Google's as per the copyright below.

// To authenticate your application to Google Drive (or other Google services) you must fill in
// the information below. The CLIENT_ID represents the application to the Google API and comes
// from the developer API console when an application is registered.

/*
 * Copyright (c) 2012 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except
 * in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software distributed under the License
 * is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
 * or implied. See the License for the specific language governing permissions and limitations under
 * the License.
 */

using System;

namespace Google.Apis.Helper
{
    internal static class ClientCredentials
    {
        /// <summary>
        /// The OAuth2.0 Client ID of your project.
        /// </summary>
        public static readonly string CLIENT_ID = "353488095368-ivo818d9sli1825pu55s1jck7ohqjgv5.apps.googleusercontent.com";//<< YOUR CLIENT ID HERE >>";

        /// <summary>
        /// The OAuth2.0 Client secret of your project.
        /// </summary>
        public static readonly string CLIENT_SECRET = "wgH2M3y3yNfpfUMARTAxYTuk";// << YOUR CLIENT SECRET HERE >>";

        /// <summary>
        /// The OAuth2.0 scopes required by your project.
        /// </summary>
        public static readonly string[] SCOPES = new String[]
        {
            "https://www.googleapis.com/auth/drive.file",
            "https://www.googleapis.com/auth/userinfo.email",
            "https://www.googleapis.com/auth/userinfo.profile"
        };

        /// <summary>
        /// The Redirect URI of your project.
        /// </summary>
        public static readonly string REDIRECT_URI = "urn:ietf:wg:oauth:2.0:oob";
    }

}
