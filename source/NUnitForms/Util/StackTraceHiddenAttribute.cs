// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NETFRAMEWORK  // https://github.com/dotnet/designs/blob/main/accepted/2020/net5/net5.md#preprocessor-symbols
#pragma warning disable IDE0130
namespace System.Diagnostics;
#pragma warning restore IDE0130

/// <summary>
/// Hides the attributed method or type from stack traces in debugging and exception reports.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Struct, Inherited = false)]
public sealed class StackTraceHiddenAttribute : Attribute
{
}

#endif
