#region Copyright (c) 2026-2026 Smurf.IV

/********************************************************************************************************************
'
' Modernisation 2026-2026 Smurf.IV
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
using NUnit.Framework;
using NUnit.WinForms.Util;


namespace NUnit.WinForms.Test;

[TestFixture]
public class UtilTest
{
    [Test]
    public void SafeRemoveSpacesTest()
    {
        Assert.AreEqual(string.Empty, Strings.SafeRemoveSpaces(null));
        Assert.AreEqual(string.Empty, Strings.SafeRemoveSpaces("   "));
        Assert.AreEqual("HelloWorld", Strings.SafeRemoveSpaces("Hello World"));
        Assert.AreEqual("abc123", Strings.SafeRemoveSpaces(" a b c 1 2 3 "));
    }

    [Test]
    public void TypesHasPublicPropertiesTest()
    {
        Assert.IsTrue(Types.HasPublicProperties(typeof(SampleWithProperties)));
        Assert.IsFalse(Types.HasPublicProperties(typeof(SampleWithoutProperties)));
    }

    [Test]
    public void TypesGetEventNamesTest()
    {
        ICollection<string> eventNames = Types.GetEventNames(typeof(SampleWithEvents));
        Assert.IsTrue(eventNames.Contains("CustomEvent"));
    }

    [Test]
    public void EventHelperCallTest()
    {
        var target = new SampleMethodTarget();
        object result = EventHelper.Call(target, "AddNumbers", [2, 3]);
        Assert.AreEqual(5, result);
    }

    [Test]
    public void ThrowHelperNullChecksTest()
    {
        object notNull = new();
        Assert.DoesNotThrow(() => ThrowHelper.ThrowIfNull(notNull));

        Assert.Throws<ArgumentNullException>(() => ThrowHelper.ThrowIfNull(null));
        Assert.Throws<ArgumentNullException>(() => ThrowHelper.ThrowArgumentNullException());
        Assert.Throws<ArgumentNullException>(() => ThrowHelper.ThrowArgumentNullException("paramName"));
        Assert.Throws<ArgumentNullException>(() => ThrowHelper.ThrowArgumentNullException("paramName", "custom message"));
    }

    [Test]
    public void ThrowHelperArgumentExceptionsTest()
    {
        Assert.Throws<ArgumentException>(() => ThrowHelper.ThrowArgumentException("paramName", "message"));
        Assert.Throws<ArgumentOutOfRangeException>(() => ThrowHelper.ThrowArgumentOutOfRangeException("param", -1, "must be non-negative"));
        Assert.Throws<InvalidOperationException>(() => ThrowHelper.ThrowInvalidOperationException("invalid state"));
        Assert.Throws<NotSupportedException>(() => ThrowHelper.ThrowNotSupportedException("unsupported"));
    }

    private class SampleWithProperties
    {
        public string Properties => "props";
    }

    private class SampleWithoutProperties
    {
        public string Other => "other";
    }

    private class SampleWithEvents
    {
        public event EventHandler? CustomEvent;

        protected virtual void OnCustomEvent(EventArgs e)
        {
            CustomEvent?.Invoke(this, e);
        }
    }

    private class SampleMethodTarget
    {
        public int AddNumbers(int a, int b)
        {
            return a + b;
        }
    }
}
