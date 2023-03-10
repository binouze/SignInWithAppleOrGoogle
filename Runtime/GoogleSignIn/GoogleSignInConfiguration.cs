// <copyright file="GoogleSignInConfiguration.cs" company="Google Inc.">
// Copyright (C) 2017 Google Inc. All Rights Reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>

namespace com.binouze
{
    /// <summary>
    /// Configuration properties for Google Sign-In.
    /// </summary>
    public class GoogleSignInConfiguration
    {
        /// <summary>Web client id associated with this app.</summary>
        /// <remarks>Required for requesting auth code or id token.</remarks>
        public string WebClientId = null;
        public string ClientId = null;

        /// <summary>Set to true for getting an auth code when authenticating.
        /// </summary>
        public bool RequestAuthCode = false;

        /// <summary>Set to true to request to reset the refresh token.
        ///   Causes re-consent.
        /// </summary>
        public bool ForceTokenRefresh = false;

        /// <summary>Request email address, requires consent.</summary>
        public bool RequestEmail = false;

        /// <summary>Request id token, requires consent.</summary>
        public bool RequestIdToken = false;

        /// <summary>Request profile, requires consent.</summary>
        public bool RequestProfile = false;
        
        /// <summary>Account name to use when authenticating,
        ///  null indicates use default.</summary>
        public string AccountName = null;
    }
}