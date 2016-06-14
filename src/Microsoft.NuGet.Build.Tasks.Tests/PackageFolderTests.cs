using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.NuGet.Build.Tasks.Tests
{
    public class PackageFolderTests
    {
        [Fact]
        public void ResolveWithLockFileWithPackageFolders()
        {
            var result = NuGetTestHelpers.ResolvePackagesWithJsonFileContents(
                Json.Json.LockFileWithWithSpecifiedPackageFolders,
                ".NETFramework,Version=v4.5",
                runtimeIdentifier: null,
                createTemporaryFolderForPackages: false);

            Assert.Equal(@"C:\PackageFolder\Newtonsoft.Json\8.0.3\lib\net45\Newtonsoft.Json.dll", result.References.Single().ItemSpec);
        }
    }
}
