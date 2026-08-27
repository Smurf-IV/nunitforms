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

using System;
using System.Windows.Forms;
using NUnit.Extensions.Forms.Util;

namespace NUnit.Extensions.Forms.Win32Interop;

public class SendKeyboardInput : ISendKeyboardInput
{
    // Track simple modifier state so we can synthesize correct character input
    // when using targeted key messages (PostMessage) that do not affect thread keyboard state.
    private bool _shiftActive;
    private bool _ctrlActive;
    private bool _altActive;

    public void SendInput(IntPtr window, Keys keys, SendInputFlags flags)
    {
        // Targeted alternative to deprecated/global keybd_event: send to the provided hwnd
        var vk = (uint)((int)keys & 0xFF);

        // Compute scan code for lParam construction
        var hkl = Win32.GetKeyboardLayout(Win32.GetCurrentThreadId());
        uint scanCode = Win32.MapVirtualKeyEx(vk, 0, hkl) & 0xFF;

        // Build lParam per MSDN: repeat(0-15)=1, scan(16-23), extended(24)=0, context(29)=0,
        // previous state(30) and transition(31) depend on up/down
        uint lParam = 1u | (scanCode << 16);
        if (flags == SendInputFlags.KeyUp)
        {
            lParam |= (1u << 30) | (1u << 31);
        }

        // Choose message
        // - Use SYSKEY for Alt/Menu to match system semantics
        // - When ONLY Shift is active and a letter key is pressed, also use SYSKEY variants to avoid
        //   TranslateMessage generating a second (lowercase) WM_CHAR from our posted KEYDOWN.
        bool onlyShiftActive = _shiftActive && !_ctrlActive && !_altActive;
        bool isLetterKey = vk >= 'A' && vk <= 'Z';
        bool useSysKey = ((keys & Keys.Alt) == Keys.Alt) || keys == Keys.Menu || (onlyShiftActive && isLetterKey);
        uint msg = flags == SendInputFlags.KeyDown
            ? (useSysKey ? Win32.WM_SYSKEYDOWN : Win32.WM_KEYDOWN)
            : (useSysKey ? Win32.WM_SYSKEYUP : Win32.WM_KEYUP);

        // Post to the specific window/control
        Win32.PostMessage(window, msg, (IntPtr)vk, (IntPtr)lParam);

        // Maintain simple modifier state based on the virtual key pressed/released
        if (keys == Keys.ShiftKey)
        {
            _shiftActive = flags == SendInputFlags.KeyDown;
        }
        else if (keys == Keys.ControlKey)
        {
            _ctrlActive = flags == SendInputFlags.KeyDown;
        }
        else if (keys == Keys.Menu)
        {
            _altActive = flags == SendInputFlags.KeyDown;
        }

        // When ONLY Shift is active, emulate translated character generation for letters
        // because PostMessage of key events does not update keyboard state used by TranslateMessage.
        // Emit a corresponding WM_CHAR for letter keydowns so text controls receive uppercased input.
        if (flags == SendInputFlags.KeyDown
            && _shiftActive && !_ctrlActive && !_altActive
            && vk >= 'A' && vk <= 'Z')
        {
            // Uppercase letter as character
            Win32.PostMessage(window, Win32.WM_CHAR, (IntPtr)vk, (IntPtr)1);
        }

        Application.DoEvents();
    }

    public void SendChar(IntPtr window, char ch)
    {
        // Send a character directly to the target control; this bypasses the need for TranslateMessage
        // wParam = UTF-16 code unit of the character; lParam repeat count = 1
        Win32.PostMessage(window, Win32.WM_CHAR, (IntPtr)ch, (IntPtr)1);
        Application.DoEvents();
    }
}