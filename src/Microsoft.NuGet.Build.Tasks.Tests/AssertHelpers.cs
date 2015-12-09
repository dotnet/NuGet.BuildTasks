// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Xunit;

namespace Microsoft.NuGet.Build.Tasks.Tests
{
    internal static class AssertHelpers
    {
        public static void AssertCountOf(int expectedCount, IEnumerable<ITaskItem> items)
        {
            Assert.True(items.Count() == expectedCount, 
                $"Expected {expectedCount} items, but actually got {items.Count()} items:" + Environment.NewLine + string.Join(Environment.NewLine, items.Select(i => i.ItemSpec)));
        }

        public static void AssertNoTargetPaths(IEnumerable<ITaskItem> items)
        {
            foreach (var item in items)
            {
                Assert.Equal("", item.GetMetadata("DestinationSubDirectory"));
                Assert.Equal("", item.GetMetadata("TargetPath"));
            }
        }

        public static void AssertConsistentTargetPaths(IEnumerable<ITaskItem> items)
        {
            var mapToItem = new Dictionary<string, ITaskItem>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in items)
            {
                string effectiveTargetPath = item.GetMetadata("TargetPath");

                if (string.IsNullOrEmpty(effectiveTargetPath))
                {
                    effectiveTargetPath = Path.GetFileName(item.ItemSpec);
                }

                ITaskItem conflictingItem;
                if (mapToItem.TryGetValue(effectiveTargetPath, out conflictingItem))
                {
                    Assert.True(conflictingItem == null, $"Item {item.ItemSpec} and {conflictingItem.ItemSpec} have the same TargetPath.");
                }

                mapToItem.Add(effectiveTargetPath, item);
            }
        }
    }
}
