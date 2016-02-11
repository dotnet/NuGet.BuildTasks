// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Linq;
using Xunit;

namespace Microsoft.NuGet.Build.Tasks.Tests.ProjectReferences
{
    public class ProjectReferenceTests
    {
        [Fact]
        public void ProjectReferenceToXProjWithAssetsButNotWithAPathFails()
        {
            var exception = Assert.Throws<ExceptionFromResource>(
                () => NuGetTestHelpers.ResolvePackagesWithJsonFileContents(Resources.LockFileWithXProjReference, ".NETFramework,Version=v4.5.2", "win"));

            Assert.Equal(nameof(Strings.MissingProjectReference), exception.ResourceName);
            AssertHelpers.PathEndsWith(@"XProjClassLib\XProjClassLib.xproj", exception.MessageArgs[0]);
        }

        [Fact]
        public void ProjectReferenceToProjectWithNoMSBuildProjectFailsGracefully()
        {
            var exception = Assert.Throws<ExceptionFromResource>(
                () => NuGetTestHelpers.ResolvePackagesWithJsonFileContents(Resources.LockFileMissingMSBuildProjectThatProvidesAssets, ".NETFramework,Version=v4.5.2", "win"));

            Assert.Equal(nameof(Strings.MissingMSBuildPathInProjectPackage), exception.ResourceName);
            Assert.Equal(@"XProjClassLib", exception.MessageArgs[0]);
        }

        [Fact]
        public void ProjectReferenceToXProjWithAssetsAndPathSucceeds()
        {
            var referenceToXProj = new TaskItem(@"..\XProjClassLib\XProjClassLib.xproj");
            referenceToXProj.SetMetadata("OutputBasePath", "XProjOutputDirectory");

            var result = NuGetTestHelpers.ResolvePackagesWithJsonFileContents(
                Resources.LockFileWithXProjReference,
                ".NETFramework,Version=v4.5.2",
                "win",
                projectReferencesCreatingPackages: new[] { referenceToXProj });

            Assert.Empty(result.Analyzers);
            AssertHelpers.PathEndsWith(@"XProjOutputDirectory\net452\XProjClassLib.dll", result.CopyLocalItems.Single().ItemSpec);
            AssertHelpers.PathEndsWith(@"XProjOutputDirectory\net452\XProjClassLib.dll", result.References.Single().ItemSpec);
            Assert.All(result.References, r => Assert.Equal(ResolveNuGetPackageAssets.NuGetSourceType_Project, r.GetMetadata(ResolveNuGetPackageAssets.NuGetSourceType)));
            Assert.Empty(result.ReferencedPackages);
        }

        [Fact]
        public void ProjectReferenceToCSProjWithoutAssetsAndNoPathSucceeds()
        {
            var result = NuGetTestHelpers.ResolvePackagesWithJsonFileContents(Resources.LockFileWithCSProjReference, ".NETFramework,Version=v4.5.2", "win");

            // The lock file doesn't contain any assets for csproj references, so we should produce nothing
            Assert.Empty(result.Analyzers);
            Assert.Empty(result.CopyLocalItems);
            Assert.Empty(result.References);
            Assert.Empty(result.ReferencedPackages);
        }
    }
}
