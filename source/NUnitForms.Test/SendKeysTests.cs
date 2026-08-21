#region Copyright (c) 2003-2007, Luke T. Maxon : 2026-2026 Smurf.IV

/********************************************************************************************************************
'
' Copyright (c) 2003-2007, Luke T. Maxon
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
using System.Windows.Forms;
using FakeItEasy;
using NUnit.Extensions.Forms.SendKey;
using NUnit.Extensions.Forms.Win32Interop;
using NUnit.Framework;

namespace NUnit.Extensions.Forms.TestApplications;

[TestFixture]
public class SendKeysTests : MockingTestFixture
{
    private AlternateSendKeys keyboardSendKeys;
    private ISendKeyboardInput keyboardInput;
    private ISendKeysParserFactory parserFactory;
    private ISendKeysParser parser;
    private IntPtr window;

    [SetUp]
    public void SetUp()
    {
        keyboardInput = NewMock<ISendKeyboardInput>();
        parserFactory = NewMock<ISendKeysParserFactory>();
        parser = NewMock<ISendKeysParser>();

        window = new IntPtr(0x12345);
        keyboardSendKeys = new AlternateSendKeys(keyboardInput, parserFactory, window);
    }

    [TearDown]
    public void TearDown()
    {
        keyboardSendKeys.Dispose();
    }

    [Test]
    public void SendWait_SingleCharLowerCase()
    {
        StubFormatter("b", "b", "", Keys.None);

        ExpectKeyDownAndRelease(Keys.B);

        keyboardSendKeys.SendWait("b");
    }

    [Test]
    public void SendWait_SingleCharUpperCase()
    {
        StubFormatter("B", "B", "", Keys.None);

        ExpectKeyDown(Keys.ShiftKey);
        ExpectKeyDownAndRelease(Keys.B);
        ExpectKeyUp(Keys.ShiftKey);

        keyboardSendKeys.SendWait("B");
    }

    [Test]
    public void SendWait_ShiftFormatted()
    {
        StubFormatter("+(ab)", "ab", "+", Keys.None);

        ExpectKeyDown(Keys.ShiftKey);
        ExpectKeyDownAndRelease(Keys.A);
        ExpectKeyDownAndRelease(Keys.B);
        ExpectKeyUp(Keys.ShiftKey);

        keyboardSendKeys.SendWait("+(ab)");
    }

    [Test]
    public void SendWait_ControlFormatted()
    {
        StubFormatter("^(ab)", "ab", "^", Keys.None);

        ExpectKeyDown(Keys.ControlKey);
        ExpectKeyDownAndRelease(Keys.A);
        ExpectKeyDownAndRelease(Keys.B);
        ExpectKeyUp(Keys.ControlKey);

        keyboardSendKeys.SendWait("^(ab)");
    }

    [Test]
    public void SendWait_AltFormatted()
    {
        StubFormatter("%(ab)", "ab", "%", Keys.None);

        ExpectKeyDown(Keys.Menu);
        ExpectKeyDownAndRelease(Keys.A);
        ExpectKeyDownAndRelease(Keys.B);
        ExpectKeyUp(Keys.Menu);

        keyboardSendKeys.SendWait("%(ab)");
    }

    [Test]
    public void SendWait_AltShiftControlFormatted()
    {
        StubFormatter("%+^(ab)", "ab", "%+^", Keys.None);

        ExpectKeyDown(Keys.Menu);
        ExpectKeyDown(Keys.ControlKey);
        ExpectKeyDown(Keys.ShiftKey);

        ExpectKeyDownAndRelease(Keys.A);
        ExpectKeyDownAndRelease(Keys.B);

        ExpectKeyUp(Keys.ShiftKey);
        ExpectKeyUp(Keys.ControlKey);
        ExpectKeyUp(Keys.Menu);

        keyboardSendKeys.SendWait("%+^(ab)");
    }

    [Test]
    [Ignore("This test is keyboard layout dependent.")]
    public void SendWait_SimpleText()
    {
        StubFormatter("aA {{}1.", "aA {1.", "", Keys.None);

        ExpectKeyDownAndRelease(Keys.A);

        ExpectKeyDown(Keys.ShiftKey);
        ExpectKeyDownAndRelease(Keys.A);
        ExpectKeyUp(Keys.ShiftKey);

        ExpectKeyDownAndRelease(Keys.Space);

        // This will be keyboard layout dependent
        ExpectKeyDown(Keys.ShiftKey);
        ExpectKeyDownAndRelease(Keys.Oem4); // '[' key shifted to give '{'
        ExpectKeyUp(Keys.ShiftKey);

        ExpectKeyDownAndRelease(Keys.NumPad1);
        ExpectKeyDownAndRelease(Keys.OemPeriod);

        keyboardSendKeys.SendWait("aA {{}1.");
    }

    private void ExpectKeyDown(Keys keyCode)
    {
        A.CallTo(() => keyboardInput.SendInput(window, keyCode, SendInputFlags.KeyDown)).MustHaveHappenedOnceExactly();
    }

    private void ExpectKeyUp(Keys keyCode)
    {
        A.CallTo(() => keyboardInput.SendInput(window, keyCode, SendInputFlags.KeyUp)).MustHaveHappenedOnceExactly();
    }

    private void ExpectKeyDownAndRelease(Keys keyCode)
    {
        A.CallTo(() => keyboardInput.SendInput(window, keyCode, SendInputFlags.KeyDown)).MustHaveHappenedOnceExactly();
        A.CallTo(() => keyboardInput.SendInput(window, keyCode, SendInputFlags.KeyUp)).MustHaveHappenedOnceExactly();
    }

    private void StubFormatter(string rawText, string body, string modifiers, Keys escapedKeys)
    {
        A.CallTo(() => parserFactory.Create(null!)).WithAnyArguments().Returns(parser);

        var group = NewMock<ISendKeysParserGroup>();
        A.CallTo(() => group.ModifierCharacters).Returns(modifiers);
        A.CallTo(() => group.EscapedKey).Returns(escapedKeys);
        A.CallTo(() => group.Body).Returns(body);

        A.CallTo(() => parser.Groups).Returns(new[] { group });
    }
}