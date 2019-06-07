using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Microsoft.NuGet.Build.Tasks.Tests
{
    public class ContentItemTests
    {
        // https://github.com/dotnet/project-system/issues/3042
        [Fact]
        public void Issue3042()
        {
            var result = NuGetTestHelpers.ResolvePackagesWithJsonFileContents(
                Json.Json.ContentFiles_assets,
                ".NETFramework,Version=v4.7.2",
                runtimeIdentifier: null,
                createTemporaryFolderForPackages: false,
                projectLanguage: "C#");

            var contentItemNames = result.ContentItems.Select(t => t.ItemSpec);

            var expectedItems = new[]
            {
                @"contentFiles\cs\any\Container.Generated.tt",
                @"contentFiles\cs\any\Container.cs",
                @"contentFiles\cs\any\ImTools.cs",
                @"contentFiles\cs\any\Registrations.ttinclude",
                @"contentFiles\any\any\MyAwesomeFile.txt",
                @"contentFiles\cs\net472\Alpha.cs",
                @"contentFiles\any\net472\AnyNet472.txt"
            };

            foreach (var item in expectedItems)
            {
                var expectedPath = Path.Combine(@"C:\alpha\packages\DryIocZero\4.0.0-preview-04", item);
                Assert.Contains(expectedPath, contentItemNames, StringComparer.OrdinalIgnoreCase);
            }

            Assert.Equal(expected: 7, actual: result.ContentItems.Length);
        }
    }
}
