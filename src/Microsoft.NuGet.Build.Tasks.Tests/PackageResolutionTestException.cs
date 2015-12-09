// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Runtime.Serialization;

namespace Microsoft.NuGet.Build.Tasks.Tests
{
    /// <summary>
    /// An exception thrown if a helper <see cref="NuGetTestHelpers"/> says package resolution failed.
    /// </summary>
    [Serializable]
    internal class PackageResolutionTestException : Exception
    {
        public PackageResolutionTestException()
        {
        }

        public PackageResolutionTestException(string message) : base(message)
        {
        }

        public PackageResolutionTestException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PackageResolutionTestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}