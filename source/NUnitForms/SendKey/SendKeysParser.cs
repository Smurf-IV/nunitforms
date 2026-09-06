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
    private readonly List<SendKeysParserGroup> _groups = [];

    private readonly List<string> _groupModifiers = [];
    private readonly List<Keys> _escapedKeyCodes = [];
    private readonly List<string> _bodyTexts = [];

    private const string GroupsPattern = @"(?<group> ([\(\{+^%~\[] .+? [\)\}]) | ([^\(\{+^%~\[\)\}]+) | ([+^%~].) )";
    private const string ModifiersPattern = @"^(?<modifier> [+^%~] + )? (?<escapedKey>\{ [^\}]+? \})? ([\(\{\[]? (?<body> .*? )? [\)\}\]]?) $";
    private readonly Dictionary<string, Keys> _keyValueMap = new();

    public SendKeysParser(string sendKeysFormattedText)
    {
        InitialiseKeyValueMap();

        var regex = new Regex(GroupsPattern, RegexOptions.IgnorePatternWhitespace);
        MatchCollection matches = regex.Matches(sendKeysFormattedText);
        string[] groupsList = [.. matches.Cast<Match>().Select(match => match.Value)];

        foreach (string group in groupsList)
        {
            ParseGroupElements(group);
        }
    }

    private void InitialiseKeyValueMap()
    {
        _keyValueMap.Add("{BACKSPACE}", Keys.Back);
        _keyValueMap.Add("{BS}", Keys.Back);
        _keyValueMap.Add("{BKSP}", Keys.Back);

        _keyValueMap.Add("{DELETE}", Keys.Delete);
        _keyValueMap.Add("{DEL}", Keys.Delete);
        _keyValueMap.Add("{DOWN}", Keys.Down);
        _keyValueMap.Add("{END}", Keys.End);
        _keyValueMap.Add("{ENTER}", Keys.Enter);
        _keyValueMap.Add("{ESC}", Keys.Escape);
        _keyValueMap.Add("{HELP}", Keys.Help);
        _keyValueMap.Add("{HOME}", Keys.Home);
        _keyValueMap.Add("{INSERT}", Keys.Insert);
        _keyValueMap.Add("{INS}", Keys.Insert);
        _keyValueMap.Add("{NUMLOCK}", Keys.NumLock);

        _keyValueMap.Add("{F1}", Keys.F1);
        _keyValueMap.Add("{F2}", Keys.F2);
        _keyValueMap.Add("{F3}", Keys.F3);
        _keyValueMap.Add("{F4}", Keys.F4);
        _keyValueMap.Add("{F5}", Keys.F5);
        _keyValueMap.Add("{F6}", Keys.F6);
        _keyValueMap.Add("{F7}", Keys.F7);
        _keyValueMap.Add("{F8}", Keys.F8);
        _keyValueMap.Add("{F9}", Keys.F9);
        _keyValueMap.Add("{F10}", Keys.F10);
        _keyValueMap.Add("{F11}", Keys.F11);
        _keyValueMap.Add("{F12}", Keys.F12);
        _keyValueMap.Add("{F13}", Keys.F13);
        _keyValueMap.Add("{F14}", Keys.F14);
        _keyValueMap.Add("{F15}", Keys.F15);
        _keyValueMap.Add("{F16}", Keys.F16);

        _keyValueMap.Add("{LEFT}", Keys.Left);
        _keyValueMap.Add("{RIGHT}", Keys.Right);
        _keyValueMap.Add("{CAPSLOCK}", Keys.Capital);
        _keyValueMap.Add("{CAPS}", Keys.Capital);
        _keyValueMap.Add("{SPACE}", Keys.Space);
        _keyValueMap.Add("{TAB}", Keys.Tab);
        _keyValueMap.Add("{UP}", Keys.Up);
    }

    private void ParseGroupElements(string group)
    {
        var regex = new Regex(ModifiersPattern, RegexOptions.IgnorePatternWhitespace);

        var modifierCharacters = string.Empty;
        var keyCode = Keys.None;
        var bodyText = string.Empty;

        MatchCollection match = regex.Matches(group);
        if (match.Count == 1)
        {
            Group modiferGroup = match[0].Groups["modifier"];
            if (modiferGroup.Success)
            {
                modifierCharacters = modiferGroup.Value;
            }

            Group escapedKeyGroup = match[0].Groups["escapedKey"];
            if (escapedKeyGroup.Success)
            {
                // First try direct map (e.g., {BACKSPACE})
                if (!_keyValueMap.TryGetValue(escapedKeyGroup.Value, out keyCode))
                {
                    // Handle repeats like: {x n} or {KEY n}
                    // Extract inner token and count
                    var value = escapedKeyGroup.Value; // includes braces
                    var m = Regex.Match(value, @"^\{\s*(.+?)\s+(\d+)\s*\}$");
                    if (m.Success)
                    {
                        string token = m.Groups[1].Value;
                        int count = int.Parse(m.Groups[2].Value);

                        if (token.Length == 1)
                        {
                            // Simple character repeated N times
                            bodyText = new string(token[0], count);
                            keyCode = Keys.None;
                        }
                        else
                        {
                            // Try to resolve the token as a named key and expand into N groups
                            string lookup = "{" + token.ToUpperInvariant() + "}";
                            if (_keyValueMap.TryGetValue(lookup, out var repeatedKey))
                            {
                                for (int i = 0; i < count; i++)
                                {
                                    _groupModifiers.Add(modifierCharacters);
                                    _escapedKeyCodes.Add(repeatedKey);
                                    _bodyTexts.Add(string.Empty);
                                    _groups.Add(new SendKeysParserGroup(modifierCharacters, string.Empty, repeatedKey));
                                }

                                // We've already appended expanded groups; return to avoid adding another below.
                                return;
                            }
                            else
                            {
                                // Fallback: treat as literal text repeated
                                bodyText = string.Concat(Enumerable.Repeat(token, count));
                                keyCode = Keys.None;
                            }
                        }
                    }
                    else
                    {
                        keyCode = Keys.None;
                        if (escapedKeyGroup.Value.Length == 3)
                        {
                            // escaped character like {+}
                            bodyText = escapedKeyGroup.Value.Substring(1, 1);
                        }
                    }
                }
            }

            if (bodyText == string.Empty)
            {
                Group bodyGroup = match[0].Groups["body"];
                if (bodyGroup.Success)
                {
                    bodyText = bodyGroup.Value;
                }
            }
        }

        _groupModifiers.Add(modifierCharacters);
        _escapedKeyCodes.Add(keyCode);
        _bodyTexts.Add(bodyText);

        _groups.Add(new SendKeysParserGroup(modifierCharacters, bodyText, keyCode));
    }

    public int GroupCount => _bodyTexts.Count;

    public string[] Modifiers => [.. _groupModifiers];

    public Keys[] EscapedKeys => [.. _escapedKeyCodes];

    public string[] Text => [.. _bodyTexts];

    public ISendKeysParserGroup[] Groups => [.. _groups];
}