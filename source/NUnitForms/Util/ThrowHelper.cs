#region Copyright (c) : 2026-2026 Smurf.IV

/********************************************************************************************************************
'
' 2026-2026 Smurf.IV
' All rights reserved.
'
' Redistribution and use in source and binary forms, with or without modification, are permitted provided
' that the following conditions are met:
'
' * Redistributions of source code must retain the above copyright notice, this list of conditions and the
' 	following disclaimer.
'
' * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and
' 	the following disclaimer in the documentation and/or other materials provided with the distribution.
'
' * Neither the name of the author nor the names of its contributors may be used to endorse or
' 	promote products derived from this software without specific prior written permission.
'
' THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
' WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
' PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
' ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
' LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
' INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
' OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN
' IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
'
' ******************************************************************************************************************/

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using NUnit.Extensions.Forms.Exceptions;

// ReSharper disable UnusedMember.Global


namespace NUnit.Extensions.Forms.Util;


/// <summary>
/// Centralized throw helpers that keep exception construction on a cold path.
/// </summary>
/// <remarks>
/// Methods marked with <see cref="DoesNotReturnAttribute"/> only throw, which keeps calling methods
/// smaller and more likely to be inlined. Throw helpers also use <see cref="MethodImplOptions.NoInlining"/>
/// and <see cref="StackTraceHiddenAttribute"/> so the cold path stays out of the hot method and stack traces.
/// Prefer <see cref="ThrowIfNull"/> for statements and
/// <c>value ?? ThrowHelper.ThrowArgumentNullException(value)</c> for assignments
/// (<see cref="CallerArgumentExpressionAttribute"/> supplies the parameter name).
/// Use <see cref="ThrowNullReferenceException(string?)"/> only when preserving an existing
/// <see cref="NullReferenceException"/> contract.
/// Generic <c>T</c> overloads exist for expression contexts (<c>??</c>, switch expression arms).
/// Visible to sibling assemblies via <c>InternalsVisibleTo</c>; not part of the public API surface.
/// </remarks>
[StackTraceHidden]
internal static class ThrowHelper
{
    #region Null checks
    /// <summary>
    /// Throws <see cref="ArgumentNullException"/> when <paramref name="argument"/> is <see langword="null"/>.
    /// </summary>
    /// <param name="argument">The argument to validate.</param>
    /// <param name="paramName">The parameter name; captured from the call site when omitted.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfNull(
        [NotNull] object? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is null)
        {
            ThrowArgumentNull(paramName);
        }
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowArgumentNull(string? paramName)
        => throw new ArgumentNullException(paramName);

    /// <summary>
    /// Throws <see cref="ArgumentNullException"/> with no parameter name.
    /// </summary>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowArgumentNullException()
        => throw new ArgumentNullException();

    /// <summary>
    /// Throws <see cref="ArgumentNullException"/> for the specified parameter name.
    /// </summary>
    /// <param name="paramName">The name of the parameter that was <see langword="null"/>.</param>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowArgumentNullException(string? paramName)
        => ThrowArgumentNull(paramName);

    /// <summary>
    /// Throws <see cref="ArgumentNullException"/> with a parameter name and message.
    /// </summary>
    /// <param name="paramName">The name of the parameter that was <see langword="null"/>.</param>
    /// <param name="message">The error message.</param>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowArgumentNullException(string? paramName, string? message)
        => throw new ArgumentNullException(paramName, message);

    /// <summary>
    /// Throws <see cref="ArgumentNullException"/> with no parameter name for expression forms.
    /// </summary>
    /// <typeparam name="T">The expected type of the expression.</typeparam>
    /// <returns>Never returns; the return type exists only for use in expressions.</returns>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowArgumentNullException<T>()
        => throw new ArgumentNullException();

    /// <summary>
    /// Throws <see cref="ArgumentNullException"/> and satisfies expression forms such as
    /// <c>value ?? ThrowHelper.ThrowArgumentNullException(value)</c>.
    /// </summary>
    /// <typeparam name="T">The expected non-null type of the expression.</typeparam>
    /// <param name="argument">The null argument; used for type inference and parameter-name capture.</param>
    /// <param name="paramName">The parameter name; captured from the call site when omitted.</param>
    /// <returns>Never returns; the return type exists only for use in expressions.</returns>
    /// <remarks>
    /// For <see cref="string"/> operands, call with an explicit type argument
    /// (<c>ThrowArgumentNullException&lt;string&gt;(value)</c>) so overload resolution does not
    /// pick the void <see cref="ThrowArgumentNullException(string?)"/> helper.
    /// Pass <paramref name="paramName"/> explicitly when the argument expression is not the
    /// parameter name (for example <c>sender as T</c>).
    /// </remarks>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowArgumentNullException<T>(T? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        => throw new ArgumentNullException(paramName);

    /// <summary>
    /// Throws <see cref="ArgumentNullException"/> with a message for expression forms.
    /// </summary>
    /// <typeparam name="T">The expected non-null type of the expression.</typeparam>
    /// <param name="paramName">The name of the parameter that was <see langword="null"/>.</param>
    /// <param name="message">The error message.</param>
    /// <returns>Never returns; the return type exists only for use in expressions.</returns>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowArgumentNullException<T>(string? paramName, string? message)
        => throw new ArgumentNullException(paramName, message);
    #endregion

    #region Argument
    /// <summary>
    /// Throws <see cref="ArgumentException"/> with the specified message and parameter name.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="paramName">The name of the parameter that caused the exception.</param>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowArgumentException(string? message, string? paramName)
        => throw new ArgumentException(message, paramName);

    /// <summary>
    /// Throws <see cref="ArgumentException"/> with the specified message.
    /// </summary>
    /// <param name="message">The error message.</param>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowArgumentException(string? message)
        => throw new ArgumentException(message);

    /// <summary>
    /// Throws <see cref="ArgumentException"/> for expression forms.
    /// </summary>
    /// <typeparam name="T">The expected type of the expression.</typeparam>
    /// <param name="message">The error message.</param>
    /// <param name="paramName">The name of the parameter that caused the exception.</param>
    /// <returns>Never returns; the return type exists only for use in expressions.</returns>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowArgumentException<T>(string? message, string? paramName)
        => throw new ArgumentException(message, paramName);

    /// <summary>
    /// Throws <see cref="ArgumentException"/> for expression forms.
    /// </summary>
    /// <typeparam name="T">The expected type of the expression.</typeparam>
    /// <param name="message">The error message.</param>
    /// <returns>Never returns; the return type exists only for use in expressions.</returns>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowArgumentException<T>(string? message)
        => throw new ArgumentException(message);
    #endregion

    #region Argument out of range
    /// <summary>
    /// Throws <see cref="ArgumentOutOfRangeException"/> with no parameter name.
    /// </summary>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowArgumentOutOfRangeException()
        => throw new ArgumentOutOfRangeException();

    /// <summary>
    /// Throws <see cref="ArgumentOutOfRangeException"/> for the specified parameter name.
    /// </summary>
    /// <param name="paramName">The name of the parameter that was out of range.</param>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowArgumentOutOfRangeException(string? paramName)
        => throw new ArgumentOutOfRangeException(paramName);

    /// <summary>
    /// Throws <see cref="ArgumentOutOfRangeException"/> with a parameter name and message.
    /// </summary>
    /// <param name="paramName">The name of the parameter that was out of range.</param>
    /// <param name="message">The error message.</param>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowArgumentOutOfRangeException(string? paramName, string? message)
        => throw new ArgumentOutOfRangeException(paramName, message);

    /// <summary>
    /// Throws <see cref="ArgumentOutOfRangeException"/> with an actual value and message.
    /// </summary>
    /// <param name="paramName">The name of the parameter that was out of range.</param>
    /// <param name="actualValue">The value of the argument that caused the exception.</param>
    /// <param name="message">The error message.</param>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowArgumentOutOfRangeException(string? paramName, object? actualValue, string? message)
        => throw new ArgumentOutOfRangeException(paramName, actualValue, message);

    /// <summary>
    /// Throws <see cref="ArgumentOutOfRangeException"/> for expression forms with no arguments.
    /// </summary>
    /// <typeparam name="T">The expected type of the expression.</typeparam>
    /// <returns>Never returns; the return type exists only for use in expressions.</returns>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowArgumentOutOfRangeException<T>()
        => throw new ArgumentOutOfRangeException();

    /// <summary>
    /// Throws <see cref="ArgumentOutOfRangeException"/> for expression forms (e.g. switch arms).
    /// </summary>
    /// <typeparam name="T">The expected type of the expression.</typeparam>
    /// <param name="paramName">The name of the parameter that was out of range.</param>
    /// <returns>Never returns; the return type exists only for use in expressions.</returns>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowArgumentOutOfRangeException<T>(string? paramName)
        => throw new ArgumentOutOfRangeException(paramName);

    /// <summary>
    /// Throws <see cref="ArgumentOutOfRangeException"/> for expression forms with a message.
    /// </summary>
    /// <typeparam name="T">The expected type of the expression.</typeparam>
    /// <param name="paramName">The name of the parameter that was out of range.</param>
    /// <param name="message">The error message.</param>
    /// <returns>Never returns; the return type exists only for use in expressions.</returns>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowArgumentOutOfRangeException<T>(string? paramName, string? message)
        => throw new ArgumentOutOfRangeException(paramName, message);

    /// <summary>
    /// Throws <see cref="ArgumentOutOfRangeException"/> for expression forms with an actual value and message.
    /// </summary>
    /// <typeparam name="T">The expected type of the expression.</typeparam>
    /// <param name="paramName">The name of the parameter that was out of range.</param>
    /// <param name="actualValue">The value of the argument that caused the exception.</param>
    /// <param name="message">The error message.</param>
    /// <returns>Never returns; the return type exists only for use in expressions.</returns>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowArgumentOutOfRangeException<T>(string? paramName, object? actualValue, string? message)
        => throw new ArgumentOutOfRangeException(paramName, actualValue, message);
    #endregion

    #region Invalid operation
    /// <summary>
    /// Throws <see cref="InvalidOperationException"/> with the specified message.
    /// </summary>
    /// <param name="message">The error message.</param>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowInvalidOperationException(string? message)
        => throw new InvalidOperationException(message);

    /// <summary>
    /// Throws <see cref="InvalidOperationException"/> for expression forms.
    /// </summary>
    /// <typeparam name="T">The expected type of the expression.</typeparam>
    /// <param name="message">The error message.</param>
    /// <returns>Never returns; the return type exists only for use in expressions.</returns>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowInvalidOperationException<T>(string? message)
        => throw new InvalidOperationException(message);
    #endregion

    #region Null reference (legacy contracts)
    /// <summary>
    /// Throws <see cref="NullReferenceException"/> with the specified message.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <remarks>
    /// Prefer <see cref="ThrowArgumentNullException(string?)"/> for new parameter validation.
    /// Use this only to preserve existing NRE behaviour at legacy call sites.
    /// </remarks>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowNullReferenceException(string? message)
        => throw new NullReferenceException(message);

    /// <summary>
    /// Throws <see cref="NullReferenceException"/> for use in expressions that require a typed result.
    /// </summary>
    /// <typeparam name="T">The expected non-null type of the expression.</typeparam>
    /// <param name="message">The error message.</param>
    /// <returns>Never returns; the return type exists only for use in expressions.</returns>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowNullReferenceException<T>(string? message)
        => throw new NullReferenceException(message);
    #endregion

    #region Not supported
    /// <summary>
    /// Throws <see cref="NotSupportedException"/> with no message.
    /// </summary>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowNotSupportedException()
        => throw new NotSupportedException();

    /// <summary>
    /// Throws <see cref="NotSupportedException"/> with the specified message.
    /// </summary>
    /// <param name="message">The error message.</param>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowNotSupportedException(string? message)
        => throw new NotSupportedException(message);

    /// <summary>
    /// Throws <see cref="NotSupportedException"/> for expression forms.
    /// </summary>
    /// <typeparam name="T">The expected type of the expression.</typeparam>
    /// <returns>Never returns; the return type exists only for use in expressions.</returns>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowNotSupportedException<T>()
        => throw new NotSupportedException();

    /// <summary>
    /// Throws <see cref="NotSupportedException"/> for expression forms.
    /// </summary>
    /// <typeparam name="T">The expected type of the expression.</typeparam>
    /// <param name="message">The error message.</param>
    /// <returns>Never returns; the return type exists only for use in expressions.</returns>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowNotSupportedException<T>(string? message)
        => throw new NotSupportedException(message);
    #endregion

    #region Not implemented
    /// <summary>
    /// Throws <see cref="NotImplementedException"/> with no message.
    /// </summary>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowNotImplementedException()
        => throw new NotImplementedException();

    /// <summary>
    /// Throws <see cref="NotImplementedException"/> with the specified message.
    /// </summary>
    /// <param name="message">The error message.</param>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowNotImplementedException(string? message)
        => throw new NotImplementedException(message);

    /// <summary>
    /// Throws <see cref="NotImplementedException"/> for expression forms.
    /// </summary>
    /// <typeparam name="T">The expected type of the expression.</typeparam>
    /// <returns>Never returns; the return type exists only for use in expressions.</returns>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowNotImplementedException<T>()
        => throw new NotImplementedException();

    /// <summary>
    /// Throws <see cref="NotImplementedException"/> for expression forms.
    /// </summary>
    /// <typeparam name="T">The expected type of the expression.</typeparam>
    /// <param name="message">The error message.</param>
    /// <returns>Never returns; the return type exists only for use in expressions.</returns>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowNotImplementedException<T>(string? message)
        => throw new NotImplementedException(message);
    #endregion

    #region Object disposed
    /// <summary>
    /// Throws <see cref="ObjectDisposedException"/> for the specified object name.
    /// </summary>
    /// <param name="objectName">The name of the disposed object.</param>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowObjectDisposedException(string? objectName)
        => throw new ObjectDisposedException(objectName);

    /// <summary>
    /// Throws <see cref="ObjectDisposedException"/> for expression forms.
    /// </summary>
    /// <typeparam name="T">The expected type of the expression.</typeparam>
    /// <param name="objectName">The name of the disposed object.</param>
    /// <returns>Never returns; the return type exists only for use in expressions.</returns>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowObjectDisposedException<T>(string? objectName)
        => throw new ObjectDisposedException(objectName);
    #endregion

    #region Invalid cast
    /// <summary>
    /// Throws <see cref="InvalidCastException"/> with no message.
    /// </summary>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowInvalidCastException()
        => throw new InvalidCastException();

    /// <summary>
    /// Throws <see cref="InvalidCastException"/> with the specified message.
    /// </summary>
    /// <param name="message">The error message.</param>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowInvalidCastException(string? message)
        => throw new InvalidCastException(message);

    /// <summary>
    /// Throws <see cref="InvalidCastException"/> for expression forms.
    /// </summary>
    /// <typeparam name="T">The expected type of the expression.</typeparam>
    /// <returns>Never returns; the return type exists only for use in expressions.</returns>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowInvalidCastException<T>()
        => throw new InvalidCastException();

    /// <summary>
    /// Throws <see cref="InvalidCastException"/> for expression forms.
    /// </summary>
    /// <typeparam name="T">The expected type of the expression.</typeparam>
    /// <param name="message">The error message.</param>
    /// <returns>Never returns; the return type exists only for use in expressions.</returns>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowInvalidCastException<T>(string? message)
        => throw new InvalidCastException(message);
    #endregion

    #region Win32
    /// <summary>
    /// Throws <see cref="Win32Exception"/> using the last Win32 error.
    /// </summary>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowWin32Exception()
        => throw new Win32Exception();

    /// <summary>
    /// Throws <see cref="Win32Exception"/> for the specified error code.
    /// </summary>
    /// <param name="error">The Win32 error code.</param>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowWin32Exception(int error)
        => throw new Win32Exception(error);

    /// <summary>
    /// Throws <see cref="Win32Exception"/> with a message (uses the last Win32 error).
    /// </summary>
    /// <param name="message">The error message.</param>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowWin32Exception(string? message)
        => throw new Win32Exception(message);

    /// <summary>
    /// Throws <see cref="Win32Exception"/> for expression forms.
    /// </summary>
    /// <typeparam name="T">The expected type of the expression.</typeparam>
    /// <returns>Never returns; the return type exists only for use in expressions.</returns>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowWin32Exception<T>()
        => throw new Win32Exception();

    /// <summary>
    /// Throws <see cref="Win32Exception"/> for expression forms.
    /// </summary>
    /// <typeparam name="T">The expected type of the expression.</typeparam>
    /// <param name="error">The Win32 error code.</param>
    /// <returns>Never returns; the return type exists only for use in expressions.</returns>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowWin32Exception<T>(int error)
        => throw new Win32Exception(error);
    #endregion

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowInvalidEnumArgumentException<T>(string? valueName, int value, Type type)
        => throw new InvalidEnumArgumentException(valueName, value, type);

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowNoSuchControlException<T>(string? name)
        => throw new NoSuchControlException(name);

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowNoSuchControlException(string? name)
        => throw new NoSuchControlException(name);

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowAmbiguousNameException<T>(string? name)
        => throw new AmbiguousNameException(name);

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowFormsTestAssertionException(string s)
        => throw new FormsTestAssertionException(s);

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowControlNotVisibleException(string? name)
        => throw new ControlNotVisibleException(name);

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowControlNotEnabledException(string? name)
        => throw new ControlNotEnabledException(name);

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowKeyboardSequenceException()
        => throw new KeyboardSequenceException();

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowIndexOutOfRangeException(string s)
        => throw new IndexOutOfRangeException(s);

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowMissingMethodException(string toString, string s)
        => throw new MissingMethodException(toString, s);

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T KeyNotFoundException<T>(string? s)
        => throw new KeyNotFoundException(s);
}
