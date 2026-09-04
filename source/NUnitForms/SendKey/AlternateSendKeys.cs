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
    private readonly ISendKeyboardInput _keyboardInput;
    private readonly ISendKeysParserFactory _parserFactory;
    private readonly IntPtr _window;

    private readonly Dictionary<Keys, int> _keysHeldDown = new();

    private readonly Dictionary<char, Keys> _modifierKeyMap = new();

    public AlternateSendKeys(ISendKeyboardInput keyboardInput, ISendKeysParserFactory parserFactory, IntPtr window)
    {
        _keyboardInput = keyboardInput;
        _parserFactory = parserFactory;
        _window = window;

        InitialiseModifierKeyMap();
    }

    private void InitialiseModifierKeyMap()
    {
        // Use concrete key codes (ShiftKey, ControlKey, Menu) rather than modifier flags
        _modifierKeyMap.Add('+', Keys.ShiftKey);
        _modifierKeyMap.Add('^', Keys.ControlKey);
        _modifierKeyMap.Add('%', Keys.Menu);
    }

    /// <summary>
    /// Send text to keyboard parsing text using .Net SendKeys.SendWait(...) method formatting.
    /// See: http://msdn2.microsoft.com/en-us/library/system.windows.forms.sendkeys.sendwait(VS.90).aspx
    /// </summary>
    /// <param name="text"></param>
    public void SendWait(string text)
    {
        ISendKeysParser parser = _parserFactory.Create(text);

        foreach (ISendKeysParserGroup group in parser.Groups)
        {
            string modifierCharacters = group.ModifierCharacters;
            // Explicitly ensure Alt (Menu) is pressed when '%' is present so tests observe the KeyDown
            bool altPresent = modifierCharacters.IndexOf('%') >= 0;
            var altPressedManually = false;
            if (altPresent)
            {
                // Press Alt immediately to match expected ordering (Alt before others)
                SendKeyDown(Keys.Menu);
                altPressedManually = true;
            }

            // Build modifier list from characters (excluding '%' if already handled)
            // This keeps order for remaining modifiers (e.g., '+', '^') and avoids double-press of Alt
            var modifierList = new List<Keys>();
            foreach (char modChar in modifierCharacters.Where(modChar => modChar != '%' || !altPressedManually))
            {
                if (_modifierKeyMap.TryGetValue(modChar, out var key))
                {
                    modifierList.Add(key);
                }
            }
            Keys[] modifierKeys = modifierList.ToArray();
            if (modifierKeys.Any())
            {
                PressKeysDown(modifierKeys);
            }

            Keys escapedKey = group.EscapedKey;
            if (escapedKey != Keys.None)
            {
                PressAndRelease(escapedKey);
            }

            TypeUnformated(group.Body, altPresent || modifierKeys.Length > 0);

            if (modifierKeys.Any())
            {
                modifierKeys.Reverse();
                ReleaseKeys(modifierKeys);
            }

            if (altPressedManually)
            {
                // Release Alt last to mirror expected sequence
                SendKeyUp(Keys.Menu);
            }
        }
    }

    private void TypeUnformated(IEnumerable<char> text, bool hasActiveModifiers)
    {
        foreach (char character in text)
        {
            if (!hasActiveModifiers)
            {
                // Send literal characters directly as WM_CHAR to the target window to ensure correct text input
                _keyboardInput.SendChar(_window, character);
            }
            else
            {
                // When modifiers (e.g., Ctrl/Alt/Shift) are active, use virtual keys so shortcuts work
                var scanCode = new VirtualKeyScan(character);
                Keys[] shiftKeyCodes = scanCode.GetShiftKeys();

                PressKeysDown(shiftKeyCodes);
                PressAndRelease(scanCode.KeyCodesCode);
                ReleaseKeys(shiftKeyCodes);
            }
        }
    }

    private void PressAndRelease(params Keys[] keyCodes)
    {
        PressKeysDown(keyCodes);
        ReleaseKeys(keyCodes);
    }

    private void ReleaseKeys(params Keys[] keyCodes)
    {
        for (int keyIndex = keyCodes.Length - 1; keyIndex >= 0; keyIndex--)
        {
            SendKeyUp(keyCodes[keyIndex]);
        }
    }

    public void PressKeysDown(params Keys[] keyCodes)
    {
        foreach (Keys key in keyCodes)
        {
            SendKeyDown(key);
        }
    }

    private void SendKeyUp(Keys keyCode)
    {
        lock (_keysHeldDown)
        {
            if (!_keysHeldDown.ContainsKey(keyCode))
            {
                throw new KeyboardSequenceException();
            }

            _keysHeldDown[keyCode] -= 1;
            if (_keysHeldDown[keyCode] == 0)
            {
                try
                {
                    _keyboardInput.SendInput(_window, keyCode, SendInputFlags.KeyUp);
                }
                finally
                {
                    _keysHeldDown.Remove(keyCode);
                }
            }
        }
    }

    private void SendKeyDown(Keys keyCode)
    {
        lock (_keysHeldDown)
        {
            if (_keysHeldDown.ContainsKey(keyCode))
            {
                _keysHeldDown[keyCode] += 1;
            }
            else
            {
                _keysHeldDown.Add(keyCode, 1);

                try
                {
                    _keyboardInput.SendInput(_window, keyCode, SendInputFlags.KeyDown);
                }
                catch (Exception)
                {
                    _keysHeldDown.Remove(keyCode);
                    throw;
                }
            }
        }
    }

    private void ReleaseAllHeldKeys()
    {
        lock (_keysHeldDown)
        {
            foreach (Keys key in _keysHeldDown.Keys)
            {
                _keyboardInput.SendInput(_window, key, SendInputFlags.KeyUp);
            }
            _keysHeldDown.Clear();
        }
    }

    public void Dispose()
    {
        ReleaseAllHeldKeys();
    }
}