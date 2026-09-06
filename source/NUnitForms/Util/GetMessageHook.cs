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
using System.Collections.Generic;
using System.Windows.Forms;

namespace NUnit.Extensions.Forms.Util;

/// <summary>
/// Installs a GetMessageHook, and maintains a list of user supplied callbacks.
/// Every time the hook is run, all the callbacks are run, and those that returned true are discarded.
/// When a callback is recorded, or when the list of callbacks is not empy after a run, a message is
/// posted to the current thread to ensure that the recorded callbacks will be run. Of course one should
/// be wary of infinite loops.
/// 
/// Due to the static design, this class will not work in the case of multiple UI threads,
/// each operating a message pump. If this is a concern, it should be feasible to modify it.
/// </summary>
public class GetMessageHook
{
    /// <summary>
    /// Represents a callback that can be executed by the message hook.
    /// </summary>
    /// <returns>true if the callback has been handled and can be discarded; false otherwise.</returns>
    public delegate bool Callback();

    private static int s_msgId;
    private static Win32.CBTCallback? s_callback;
    private static IntPtr s_handleToHook = IntPtr.Zero;
    private static List<Callback> s_callbacks = [];

    /// <summary>
    /// Installs the message hook.
    /// </summary>
    public static void InstallHook()
    {
        s_msgId = Win32.RegisterWindowMessage("NUnitForms Callback");
        s_callbacks = [];
        s_callback = ProcessCallbacks;
        s_handleToHook = Win32.SetWindowsHookEx(3, s_callback, IntPtr.Zero, Win32.GetCurrentThreadId());
    }

    /// <summary>
    /// Removes the message hook.
    /// </summary>
    public static void RemoveHook()
    {
        if (s_handleToHook != IntPtr.Zero)
        {
            Win32.UnhookWindowsHookEx(s_handleToHook);
            s_handleToHook = IntPtr.Zero;
            s_callback = null;
        }
    }

    /// <summary>
    /// Records a callback to be executed by the message hook.
    /// </summary>
    /// <param name="c">The callback to record.</param>
    public static void Record(Callback c)
    {
        bool wasEmpty = s_callbacks.Count == 0;
        s_callbacks.Add(c);
        if (wasEmpty)
        {
            Post();
        }
    }

    /// <summary>
    /// Posts a message to trigger callbacks.
    /// </summary>
    private static void Post()
    {
        Win32.PostThreadMessage(Win32.GetCurrentThreadId(), (uint)s_msgId, UIntPtr.Zero, IntPtr.Zero);
    }

    private static IntPtr ProcessCallbacks(int code, IntPtr wParam, IntPtr lParam)
    {
        if (code >= 0 && lParam != IntPtr.Zero)
        {
            unsafe
            {
                Win32.MSG* msg = (Win32.MSG*)lParam;
                if (msg->message == (uint)s_msgId)
                {
                    List<Callback> tmp = s_callbacks;
                    s_callbacks = [];
                    foreach (Callback c in tmp)
                    {
                        if (!c())
                        {
                            s_callbacks.Add(c);
                        }
                    }
                    if (s_callbacks.Count > 0)
                    {
                        Post();
                    }
                }
            }
        }
        return Win32.CallNextHookEx(s_handleToHook, code, wParam, lParam);
    }
}
