// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.NuGet.Build.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Microsoft.NuGet.Build.Tasks.Tests.Helpers;

namespace Microsoft.NuGet.Build.Tasks.Tests
{
    internal static class NuGetTestHelpers
    {
        public static ResolvePackagesResult ResolvePackagesWithJsonFileContents(
            string projectLockJsonFileContents,
            string targetMoniker,
            string runtimeIdentifier,
            string projectLanguage = null,
            bool allowFallbackOnTargetSelection = false,
            DirectoryExists directoryExists = null,
            FileExists fileExists = null,
            TryGetRuntimeVersion tryGetRuntimeVersion = null,
            bool includeFrameworkReferences = true,
            string projectJsonFileContents = null)
        {
            var filePaths = GetFilePaths(projectLockJsonFileContents).ToArray();
            var tempPackagesFolder = Path.Combine(Path.GetTempPath(), ".nuget", "packages");
            var nugetPackageDirectory = new TempRoot(tempPackagesFolder);

            if (fileExists == null)
            {
                CreateFiles(nugetPackageDirectory, filePaths);
            }

            var rootDirectory = new TempRoot();
            using (rootDirectory)
            using (nugetPackageDirectory)
            {
                var projectLockJsonFile = CreateFile(rootDirectory, "project.lock.json");
                projectLockJsonFile.WriteAllText(projectLockJsonFileContents);

                if (projectJsonFileContents != null)
                {
                    var projectJsonFile = CreateFile(rootDirectory, "project.json");
                    projectJsonFile.WriteAllText(projectJsonFileContents);
                }

                return ResolvePackages(
                    targetMoniker,
                    runtimeIdentifier,
                    projectLockJsonFile.Path,
                    tempPackagesFolder,
                    projectLanguage,
                    allowFallbackOnTargetSelection,
                    directoryExists,
                    fileExists,
                    tryGetRuntimeVersion,
                    includeFrameworkReferences);
            }
        }

        public static ResolvePackagesResult ResolvePackages(
            string targetMoniker,
            string runtimeIdentifier,
            string projectLockFile,
            string packagesDirectory,
            string projectLanguage = null,
            bool allowFallbackOnTargetSelection = false,
            DirectoryExists directoryExists = null,
            FileExists fileExists = null,
            TryGetRuntimeVersion tryGetRuntimeVersion = null,
            bool includeFrameworkReferences = true)
        {
            ResolveNuGetPackageAssets task = new ResolveNuGetPackageAssets(directoryExists, fileExists, tryGetRuntimeVersion);
            var sw = new StringWriter();
            task.BuildEngine = new MockBuildEngine(sw);

            task.AllowFallbackOnTargetSelection = allowFallbackOnTargetSelection;
            task.IncludeFrameworkReferences = includeFrameworkReferences;
            task.NuGetPackagesDirectory = packagesDirectory;
            task.RuntimeIdentifier = runtimeIdentifier;
            task.ProjectLockFile = projectLockFile;
            task.ProjectLanguage = projectLanguage;
            task.TargetMonikers = new ITaskItem[] { new TaskItem(targetMoniker) };

            if (!task.Execute())
            {
                throw new PackageResolutionTestException(sw.ToString());
            }

            var analyzers = task.ResolvedAnalyzers;
            var copyLocalItems = task.ResolvedCopyLocalItems;
            var references = task.ResolvedReferences;
            var referencedPackages = task.ReferencedPackages;

            return new ResolvePackagesResult(analyzers, copyLocalItems, references, referencedPackages);
        }

        public static void CreateFiles(TempRoot root, string[] filenames)
        {
            foreach (var filename in filenames)
            {
                CreateFile(root, filename);
            }
        }

        public static TempFile CreateFile(TempRoot root, string filename)
        {
            var directory = root.CreateDirectory().CreateDirectory(System.IO.Path.GetDirectoryName(filename));
            return directory.CreateFile(System.IO.Path.GetFileName(filename));
        }

        private static IEnumerable<string> GetFilePaths(string jsonFileContents)
        {
            JObject lockFile;
            using (var streamReader = new StringReader(jsonFileContents))
            {
                lockFile = JObject.Load(new JsonTextReader(streamReader));
            }

            var packages = lockFile["libraries"].Children();

            foreach (var package in packages)
            {
                var packageNameParts = (package as JProperty)?.Name.Split('/');
                if (packageNameParts == null)
                {
                    continue;
                }

                var packageName = packageNameParts[0];
                var packageVersion = packageNameParts[1];

                foreach (var file in package.Children()
                                    .SelectMany(x => x["files"].Children())
                                    .Select(x => x.ToString()))
                {
                    yield return Path.Combine(packageName, packageVersion, file.Replace('/', '\\'));
                }
            }
        }
    }
}
