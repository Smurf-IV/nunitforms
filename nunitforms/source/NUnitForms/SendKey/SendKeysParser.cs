#region Copyright (c) 2003-2005, Luke T. Maxon : 2026-2026 Smurf.IV

/********************************************************************************************************************
'
' Copyright (c) 2003-2005, Luke T. Maxon
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

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NUnit.Extensions.Forms.SendKey;

public class SendKeysParser : ISendKeysParser
{
    private readonly List<SendKeysParserGroup> groups = [];

    private readonly List<string> groupModifiers = [];
    private readonly List<Keys> escapedKeyCodes = [];
    private readonly List<string> bodyTexts = [];

    private const string groupsPattern = @"(?<group> ([\(\{+^%~\[] .+? [\)\}]) | ([^\(\{+^%~\[\)\}]+) | ([+^%~].) )";
    private const string modifiersPattern = @"^(?<modifier> [+^%~] + )? (?<escapedKey>\{ [^\}]+? \})? ([\(\{\[]? (?<body> .*? )? [\)\}\]]?) $";
    private readonly Dictionary<string, Keys> keyValueMap = new Dictionary<string, Keys>();

    public SendKeysParser(string sendKeysFormattedText)
    {
        InitialiseKeyValueMap();

        var regex = new Regex(groupsPattern, RegexOptions.IgnorePatternWhitespace);
        var matches = regex.Matches(sendKeysFormattedText);
        var groupsList = matches.Cast<Match>().Select(match => match.Value).ToArray();

        foreach (var group in groupsList)
        {
            ParseGroupElements(group);
        }
    }

    private void InitialiseKeyValueMap()
    {
        keyValueMap.Add("{BACKSPACE}", Keys.Back);
        keyValueMap.Add("{BS}", Keys.Back);
        keyValueMap.Add("{BKSP}", Keys.Back);

        keyValueMap.Add("{DELETE}", Keys.Delete);
        keyValueMap.Add("{DEL}", Keys.Delete);
        keyValueMap.Add("{DOWN}", Keys.Down);
        keyValueMap.Add("{END}", Keys.End);
        keyValueMap.Add("{ENTER}", Keys.Enter);
        keyValueMap.Add("{ESC}", Keys.Escape);
        keyValueMap.Add("{HELP}", Keys.Help);
        keyValueMap.Add("{HOME}", Keys.Home);
        keyValueMap.Add("{INSERT}", Keys.Insert);
        keyValueMap.Add("{INS}", Keys.Insert);
        keyValueMap.Add("{NUMLOCK}", Keys.NumLock);

        keyValueMap.Add("{F1}", Keys.F1);
        keyValueMap.Add("{F2}", Keys.F2);
        keyValueMap.Add("{F3}", Keys.F3);
        keyValueMap.Add("{F4}", Keys.F4);
        keyValueMap.Add("{F5}", Keys.F5);
        keyValueMap.Add("{F6}", Keys.F6);
        keyValueMap.Add("{F7}", Keys.F7);
        keyValueMap.Add("{F8}", Keys.F8);
        keyValueMap.Add("{F9}", Keys.F9);
        keyValueMap.Add("{F10}", Keys.F10);
        keyValueMap.Add("{F11}", Keys.F11);
        keyValueMap.Add("{F12}", Keys.F12);
        keyValueMap.Add("{F13}", Keys.F13);
        keyValueMap.Add("{F14}", Keys.F14);
        keyValueMap.Add("{F15}", Keys.F15);
        keyValueMap.Add("{F16}", Keys.F16);

        keyValueMap.Add("{LEFT}", Keys.Left);
        keyValueMap.Add("{RIGHT}", Keys.Right);
        keyValueMap.Add("{CAPSLOCK}", Keys.Capital);
        keyValueMap.Add("{CAPS}", Keys.Capital);
        keyValueMap.Add("{SPACE}", Keys.Space);
        keyValueMap.Add("{TAB}", Keys.Tab);
        keyValueMap.Add("{UP}", Keys.Up);
    }

    private void ParseGroupElements(string group)
    {
        var regex = new Regex(modifiersPattern, RegexOptions.IgnorePatternWhitespace);

        var modifierCharacters = string.Empty;
        var keyCode = Keys.None; 
        var bodyText = string.Empty;

        var match = regex.Matches(group);
        if (match.Count == 1)
        {
            var modiferGroup = match[0].Groups["modifier"];
            if (modiferGroup.Success)
            {
                modifierCharacters = modiferGroup.Value;
            }

            var escapedKeyGroup = match[0].Groups["escapedKey"];
            if (escapedKeyGroup.Success)
            {
                if (!keyValueMap.TryGetValue(escapedKeyGroup.Value, out keyCode))
                {
                    keyCode = Keys.None;
                    if (escapedKeyGroup.Value.Length == 3)
                    {
                        // escaped character
                        bodyText = escapedKeyGroup.Value.Substring(1,1);
                    }
                }
            }

            if (bodyText == string.Empty)
            {
                var bodyGroup = match[0].Groups["body"];
                if (bodyGroup.Success)
                {
                    bodyText = bodyGroup.Value;
                }
            }
        }

        groupModifiers.Add(modifierCharacters);
        escapedKeyCodes.Add(keyCode);
        bodyTexts.Add(bodyText);

        groups.Add(new SendKeysParserGroup(modifierCharacters, bodyText, keyCode));
    }

    public int GroupCount => bodyTexts.Count;

    public string[] Modifiers => [.. groupModifiers];

    public Keys[] EscapedKeys => [.. escapedKeyCodes];

    public string[] Text => [.. bodyTexts];

    public ISendKeysParserGroup[] Groups => [.. groups];
}