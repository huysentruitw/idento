/*
 * Copyright 2016 CogniStreamer, Wouter Huysentruit
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace Idento
{
    internal static class IdentoConstants
    {
        public const string ClientId = "@@IdentoClientId@@";
        public const string AdminUserName = "admin";
        public const string DefaultAdminPassword = "admin";
        public const string ManagerCookieScheme = "Microsoft.AspNet.Identity.Application";
        public const string ManagerOidcScheme = "IdentoManagerOidc";
        public const string ManagerRoleName = "idento_administrator";
    }
}
