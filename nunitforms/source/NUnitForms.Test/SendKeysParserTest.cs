#region Copyright (c) 2003-2007, Luke T. Maxon : 2026-2026 Smurf.IV

/********************************************************************************************************************
'
' Copyright (c) 2003-2007, Luke T. Maxon : 2026-2026 Smurf.IV
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

using System.Windows.Forms;

using NUnit.Extensions.Forms.SendKey;
using NUnit.Framework;

namespace NUnit.Extensions.Forms.TestApplications;

[TestFixture]
public class SendKeysParserTest
{
    [Test]
    public void GroupExtraction()
    {
        ISendKeysParser parser = new SendKeysParser("111+(aaa)22+^(bbb)33{{}4%(a)");

        Assert.AreEqual(8, parser.Groups.Length);

        var groupIndex = 0;
        AssertGroup(parser.Groups[groupIndex++], "", "111");
        AssertGroup(parser.Groups[groupIndex++], "+", "aaa");
        AssertGroup(parser.Groups[groupIndex++], "", "22");
        AssertGroup(parser.Groups[groupIndex++], "+^", "bbb");
        AssertGroup(parser.Groups[groupIndex++], "", "33");
        AssertGroup(parser.Groups[groupIndex++], "", "{");
        AssertGroup(parser.Groups[groupIndex++], "", "4");
        AssertGroup(parser.Groups[groupIndex], "%", "a");
    }

    [Test]
    public void ShiftModifier()
    {
        ISendKeysParser parser = new SendKeysParser("a+bc");

        Assert.AreEqual(3, parser.Groups.Length);

        var groupIndex = 0;
        AssertGroup(parser.Groups[groupIndex++], "", "a");
        AssertGroup(parser.Groups[groupIndex++], "+", "b");
        AssertGroup(parser.Groups[groupIndex], "", "c");
    }

    [Test]
    public void ControlModifier()
    {
        ISendKeysParser parser = new SendKeysParser("a^bc");

        Assert.AreEqual(3, parser.Groups.Length);

        var groupIndex = 0;
        AssertGroup(parser.Groups[groupIndex++], "", "a");
        AssertGroup(parser.Groups[groupIndex++], "^", "b");
        AssertGroup(parser.Groups[groupIndex], "", "c");
    }

    [Test]
    public void AltModifier()
    {
        ISendKeysParser parser = new SendKeysParser("a%bc");

        Assert.AreEqual(3, parser.Groups.Length);

        var groupIndex = 0;
        AssertGroup(parser.Groups[groupIndex++], "", "a");
        AssertGroup(parser.Groups[groupIndex++], "%", "b");
        AssertGroup(parser.Groups[groupIndex], "", "c");
    }

    [Test]
    public void Key_BACKSPACE()
    {
        AssertKeywordIsParsedAs("{BACKSPACE}", Keys.Back);
        AssertKeywordIsParsedAs("{BS}", Keys.Back);
        AssertKeywordIsParsedAs("{BKSP}", Keys.Back);
    }

    [Test]
    public void Key_BREAK()
    {
        // Need to confirm correct key code
        AssertKeywordIsParsedAs("{BREAK}", Keys.None);
    }

    [Test]
    [Ignore("Required but not implemented functionality")]
    public void Key_CAPSLOCK()
    {
        // Need to confirm correct key code
        AssertKeywordIsParsedAs("{CAPSLOCK}", Keys.Capital);
        AssertKeywordIsParsedAs("{CAP}", Keys.Capital);
    }

    [Test]
    public void Key_DELETE()
    {
        AssertKeywordIsParsedAs("{DELETE}", Keys.Delete);
        AssertKeywordIsParsedAs("{DEL}", Keys.Delete);
    }

    [Test]
    public void Key_DOWN()
    {
        AssertKeywordIsParsedAs("{DOWN}", Keys.Down);
    }

    [Test]
    public void Key_END()
    {
        AssertKeywordIsParsedAs("{END}", Keys.End);
    }

    [Test]
    public void Key_ENTER()
    {
        AssertKeywordIsParsedAs("{ENTER}", Keys.Enter);
    }

    [Test]
    public void Key_ESC()
    {
        AssertKeywordIsParsedAs("{ESC}", Keys.Escape);
    }

    [Test]
    public void Key_HELP()
    {
        AssertKeywordIsParsedAs("{HELP}", Keys.Help);
    }

    [Test]
    public void Key_HOME()
    {
        AssertKeywordIsParsedAs("{HOME}", Keys.Home);
    }

    [Test]
    public void Key_INSERT()
    {
        AssertKeywordIsParsedAs("{INSERT}", Keys.Insert);
        AssertKeywordIsParsedAs("{INS}", Keys.Insert);
    }

    [Test]
    public void Key_LEFT()
    {
        AssertKeywordIsParsedAs("{LEFT}", Keys.Left);
    }

    [Test]
    public void Key_NUMLOCK()
    {
        AssertKeywordIsParsedAs("{NUMLOCK}", Keys.NumLock);
    }

    [Test]
    public void Key_PGDN()
    {
        // Need to find the virtual key code
        AssertKeywordIsParsedAs("{PGDN}", Keys.None);
    }

    [Test]
    public void Key_PGUP()
    {
        // Need to find the virtual key code
        AssertKeywordIsParsedAs("{PGUP}", Keys.None);
    }

    [Test]
    public void Key_PRTSC()
    {
        // Need to find the virtual key code
        AssertKeywordIsParsedAs("{PRTSC}", Keys.None);
    }

    [Test]
    public void Key_RIGHT()
    {
        AssertKeywordIsParsedAs("{RIGHT}", Keys.Right);
    }

    [Test]
    public void Key_SCROLLLOCK()
    {
        // Need to find the virtual key code
        AssertKeywordIsParsedAs("{SCROLLLOCK}", Keys.None);
    }

    [Test]
    public void Key_SPACE()
    {
        AssertKeywordIsParsedAs("{SPACE}", Keys.Space);
    }

    [Test]
    public void Key_TAB()
    {
        AssertKeywordIsParsedAs("{TAB}", Keys.Tab);
    }

    [Test]
    public void Key_UP()
    {
        // Need to find the virtual key code
        AssertKeywordIsParsedAs("{UP}", Keys.Up);
    }

    private static void AssertKeywordIsParsedAs(string keyword, Keys expectedKey)
    {
        ISendKeysParser parser = new SendKeysParser(keyword);

        Assert.AreEqual(1, parser.Groups.Length);

        Assert.AreEqual("", parser.Groups[0].ModifierCharacters);
        Assert.AreEqual(expectedKey, parser.Groups[0].EscapedKey);
        Assert.AreEqual("", parser.Groups[0].Body);
    }

    [Test]
    public void Key_FunctionKeys()
    {
        ISendKeysParser parser = new SendKeysParser("{F1}{F2}{F3}{F4}{F5}{F6}{F7}{F8}{F9}{F10}{F11}{F12}{F13}{F14}{F15}{F16}");

        Assert.AreEqual(16, parser.Groups.Length);

        var groupIndex = 0;
        Assert.AreEqual(Keys.F1, parser.Groups[groupIndex++].EscapedKey);
        Assert.AreEqual(Keys.F2, parser.Groups[groupIndex++].EscapedKey);
        Assert.AreEqual(Keys.F3, parser.Groups[groupIndex++].EscapedKey);
        Assert.AreEqual(Keys.F4, parser.Groups[groupIndex++].EscapedKey);
        Assert.AreEqual(Keys.F5, parser.Groups[groupIndex++].EscapedKey);
        Assert.AreEqual(Keys.F6, parser.Groups[groupIndex++].EscapedKey);
        Assert.AreEqual(Keys.F7, parser.Groups[groupIndex++].EscapedKey);
        Assert.AreEqual(Keys.F8, parser.Groups[groupIndex++].EscapedKey);
        Assert.AreEqual(Keys.F9, parser.Groups[groupIndex++].EscapedKey);
        Assert.AreEqual(Keys.F10, parser.Groups[groupIndex++].EscapedKey);
        Assert.AreEqual(Keys.F11, parser.Groups[groupIndex++].EscapedKey);
        Assert.AreEqual(Keys.F12, parser.Groups[groupIndex++].EscapedKey);
        Assert.AreEqual(Keys.F13, parser.Groups[groupIndex++].EscapedKey);
        Assert.AreEqual(Keys.F14, parser.Groups[groupIndex++].EscapedKey);
        Assert.AreEqual(Keys.F15, parser.Groups[groupIndex++].EscapedKey);
        Assert.AreEqual(Keys.F16, parser.Groups[groupIndex].EscapedKey);

        foreach (SendKeysParserGroup group in parser.Groups)
        {
            Assert.AreEqual(string.Empty, group.ModifierCharacters);
            Assert.AreEqual(string.Empty, group.Body);
        }
    }

    private static void AssertGroup(ISendKeysParserGroup group, string modifierCharacters, string bodyText)
    {
        Assert.AreEqual(modifierCharacters, group.ModifierCharacters);
        Assert.AreEqual(bodyText, group.Body);
    }
}