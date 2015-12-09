// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.NuGet.Build.Tasks.Tests
{
    public class ResolvePackagesResult
    {
        public ResolvePackagesResult(
            ITaskItem[] analyzers,
            ITaskItem[] copyLocalItems,
            ITaskItem[] references,
            ITaskItem[] referencedPackages)
        {
            Analyzers = analyzers ?? new ITaskItem[] { };
            CopyLocalItems = copyLocalItems ?? new ITaskItem[] { };
            References = references ?? new ITaskItem[] { };
            ReferencedPackages = referencedPackages ?? new ITaskItem[] { };
        }

        public ITaskItem[] Analyzers { get; }
        public ITaskItem[] CopyLocalItems { get; }
        public ITaskItem[] References { get; }
        public ITaskItem[] ReferencedPackages { get; }
    }
}
