// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.NuGet.Build.Tasks.Tests.Content
{
    public class ContentTests
    {
        [Fact]
        public void ContentWithBuildActionForCSharp()
        {
            var result = NuGetTestHelpers.ResolvePackagesWithJsonFileContents(
                Resources.LockFileWithBuildActionForCSharp,
                ".NETFramework,Version=v4.5.2",
                runtimeIdentifier: null,
                projectLanguage: "C#");

            var contentItem = result.ContentItems.Single();

            AssertHelpers.PathEndsWith("ContentFolder\\Content.txt", contentItem.ItemSpec);
            Assert.Equal("ContentFolder\\Content.txt", contentItem.GetMetadata("TargetPath"));
        }

        [Fact]
        public void FileWithContentBuildActionForCSharpConsumedInVisualBasic()
        {
            var result = NuGetTestHelpers.ResolvePackagesWithJsonFileContents(
                Resources.LockFileWithBuildActionForCSharp,
                ".NETFramework,Version=v4.5.2",
                runtimeIdentifier: null,
                projectLanguage: "VB");

            Assert.Empty(result.ContentItems);
        }
    }
}
