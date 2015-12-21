/*
 * Copyright 2016 Wouter Huysentruit
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

using Idento.Core.Configuration;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using System.Diagnostics;
using System.IO;

namespace Owin
{
    internal static class FileServerAppBuilderExtensions
    {
        public static void UseFileServer(this IAppBuilder app, IdentoOptions options)
        {
            var fileServerOptions = new FileServerOptions
            {
                RequestPath = new PathString(""),
#if DEBUG
                FileSystem = new PhysicalFileSystem(GetPublicPath()),
#else
                FileSystem = new EmbeddedResourceFileSystem("Idento.Core.frontend.__build__"),
#endif
                EnableDefaultFiles = true,
                EnableDirectoryBrowsing = false
            };
            fileServerOptions.StaticFileOptions.RequestPath = fileServerOptions.RequestPath;
            fileServerOptions.StaticFileOptions.FileSystem = fileServerOptions.FileSystem;
            fileServerOptions.DefaultFilesOptions.DefaultFileNames = new[] { "index.html" };
            app.UseFileServer(fileServerOptions);

            // Needed to make UseFileServer/UseStaticFiles work until vNext arrives.
            // See http://katanaproject.codeplex.com/wikipage?title=Static%20Files%20on%20IIS
            app.UseStageMarker(PipelineStage.MapHandler);
        }

        private static string GetPublicPath()
        {
            var currentFileName = new StackFrame(true).GetFileName();
            var projectPath = Path.Combine(Path.GetDirectoryName(currentFileName), "..");
            return Path.Combine(projectPath, "frontend", "__build__");
        }
    }
}
