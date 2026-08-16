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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NUnit.Extensions.Forms.Exceptions;
using NUnit.Extensions.Forms.Win32Interop;

namespace NUnit.Extensions.Forms.SendKey;

/// <summary>
/// Alternative to the dot Net SendKeys class.
/// 
/// SendWait method emulates the dot Net class method.
/// </summary>
public class AlternateSendKeys : ISendKeys, IDisposable
{
    private readonly ISendKeyboardInput keyboardInput;
    private readonly ISendKeysParserFactory parserFactory;
    private readonly IntPtr window;

    private readonly Dictionary<Keys, int> keysHeldDown = new ();

    private readonly Dictionary<char, Keys> modifierKeyMap = new ();

    public AlternateSendKeys(ISendKeyboardInput keyboardInput, ISendKeysParserFactory parserFactory, IntPtr window)
    {
        this.keyboardInput = keyboardInput;
        this.parserFactory = parserFactory;
        this.window = window;

        InitialiseModifierKeyMap();
    }

    private void InitialiseModifierKeyMap()
    {
        modifierKeyMap.Add('+', Keys.Shift);
        modifierKeyMap.Add('^', Keys.Control);
        modifierKeyMap.Add('%', Keys.Alt);
    }

    /// <summary>
    /// Send text to keyboard parsing text using .Net SendKeys.SendWait(...) method formatting.
    /// See: http://msdn2.microsoft.com/en-us/library/system.windows.forms.sendkeys.sendwait(VS.90).aspx
    /// </summary>
    /// <param name="text"></param>
    public void SendWait(string text)
    {
        var parser = parserFactory.Create(text);

        foreach (var group in parser.Groups)
        {
            var modifierCharacters = group.ModifierCharacters;
            //TODO examine other uses of modifierKeys, can I use .ToArray instead and avoid the copies?
            var modifierKeys = modifierCharacters.Select(modChar => modifierKeyMap[modChar]).ToList();

            PressKeysDown([.. modifierKeys]);

            var escapedKey = group.EscapedKey;
            if (escapedKey != Keys.None)
            {
                PressAndRelease(escapedKey);
            }

            TypeUnformated(group.Body);

            modifierKeys.Reverse();
            ReleaseKeys([.. modifierKeys]);
        }
    }

    private void TypeUnformated(IEnumerable<char> text)
    {
        foreach (var character in text)
        {
            var scanCode = new VirtualKeyScan(character);
            var shiftKeyCodeses = scanCode.GetShiftKeys();

            PressKeysDown(shiftKeyCodeses);
            PressAndRelease(scanCode.KeyCodesCode);
            ReleaseKeys(shiftKeyCodeses);
        }
    }

    private void PressAndRelease(params Keys[] keyCodeses)
    {
        PressKeysDown(keyCodeses);
        ReleaseKeys(keyCodeses);
    }

    private void ReleaseKeys(params Keys[] keyCodeses)
    {
        for (var keyIndex = keyCodeses.Length - 1; keyIndex >= 0; keyIndex--)
        {
            SendKeyUp(keyCodeses[keyIndex]);
        }
    }

    public void PressKeysDown(params Keys[] keyCodeses)
    {
        foreach (var key in keyCodeses)
        {
            SendKeyDown(key);
        }
    }

    private void SendKeyUp(Keys keyCodes)
    {
        lock (keysHeldDown)
        {
            if (!keysHeldDown.ContainsKey(keyCodes))
            {
                throw new KeyboardSequenceException();
            }

            keysHeldDown[keyCodes] -= 1;
            if (keysHeldDown[keyCodes] == 0)
            {
                try
                {
                    keyboardInput.SendInput(window, keyCodes, SendInputFlags.KeyUp);
                }
                finally
                {
                    keysHeldDown.Remove(keyCodes);
                }
            }
        }
    }

    private void SendKeyDown(Keys keyCodes)
    {
        lock (keysHeldDown)
        {
            if (keysHeldDown.ContainsKey(keyCodes))
            {
                keysHeldDown[keyCodes] += 1;
            }
            else
            {
                keysHeldDown.Add(keyCodes, 1);

                try
                {
                    keyboardInput.SendInput(window, keyCodes, SendInputFlags.KeyDown);
                }
                catch (Exception)
                {
                    keysHeldDown.Remove(keyCodes);
                    throw;
                }
            }
        }
    }

    private void ReleaseAllHeldKeys()
    {
        lock (keysHeldDown)
        {
            foreach (var key in keysHeldDown.Keys)
            {
                keyboardInput.SendInput(window, key, SendInputFlags.KeyUp);
            }
            keysHeldDown.Clear();
        }
    }

    public void Dispose()
    {
        ReleaseAllHeldKeys();
    }
}