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
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

using NUnit.Extensions.Forms.Util;


namespace NUnit.Extensions.Forms.Testers;

/// <summary>
/// A ControlTester for testing ComboBoxes.
/// </summary>
/// <remarks>
/// Has convenience methods for Selecting items and Entering text.
/// <para>
/// Fully supported by the recorder application
/// </para>
/// </remarks>
public partial class ComboBoxTester
{
    private bool checkedReady;
    /// <summary>
    /// Sets the text property of the ComboBox to the specified value.
    /// </summary>
    /// <remarks>
    /// Also calls EndCurrentEdit() so that databinding will happen.
    /// </remarks>
    /// <param name="text">The specified value for the text property.</param>
    public void Enter(string text)
    {
        if (!checkedReady)
        {
            checkedReady = true;
            EnsureHandleReady();
        }
        ComboBox comboBox = Properties;
        if (comboBox.Text == text)
        {
            // Debounce
            return;
        }

        comboBox.Text = text;
#if NETCOREAPP
        // If on .NET Core/.NET 6+, manually trigger the layout event to mirror .NET 4.8 behavior
        // In .NET 6+: The underlying event pipelines were heavily optimized to strictly adhere to official API design specs.
        // According to Microsoft's documentation guidelines, TextUpdate is only intended to occur when the control formats
        // text in response to direct user manipulation. Programmatic updates directly modify the data layer and skip the
        // interactive layout pass, meaning TextUpdate is no longer
            MethodInfo? onTextUpdateMethod = typeof(ComboBox).GetMethod("OnTextUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
            onTextUpdateMethod?.Invoke(comboBox, [EventArgs.Empty]);
#endif
        EndCurrentEdit("Text");
    }

#if NETCOREAPP
    private const int CB_SETCURSEL = 0x014E;
    private const int WM_COMMAND = 0x0111;
    private const int CBN_SELCHANGE = 1;
#endif
    /// <summary>
    /// Selects an entry in the ComboBox according to its index.
    /// </summary>
    /// <remarks>
    /// Sets the SelectedIndex property on the underlying control.
    /// </remarks>
    /// <param name="index">The index of the ComboBox entry to select.</param>
    public void Select(int index)
    {
        if (!checkedReady)
        {
            checkedReady = true;
            EnsureHandleReady();
        }

        ComboBox comboBox = Properties;
        if (comboBox.SelectedIndex == index)
        {
            // Debounce
            return;
        }

#if NETFRAMEWORK  // https://github.com/dotnet/designs/blob/main/accepted/2020/net5/net5.md#preprocessor-symbols
        comboBox.SelectedIndex = index;
#else
        // Setting SelectedItem triggers a completely alternative internal code path inside the .NET 6 runtime that

        // Ensure handle exists
        IntPtr handle = comboBox.Handle;

        // 1. EXTRACT THE WINFORMS EVENT LIST
        PropertyInfo eventsProp = typeof(Component).GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
        EventHandlerList eventHandlerList = (EventHandlerList)eventsProp.GetValue(comboBox);

        // 2. LOCATE THE TEXTUPDATE TRACKING KEY
        FieldInfo textUpdateField = typeof(ComboBox).GetField("s_textUpdateEvent", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                                    ?? typeof(ComboBox).GetField("EventTextUpdate", BindingFlags.Static | BindingFlags.NonPublic);

        object? textUpdateKey = textUpdateField?.GetValue(null);
        Delegate? savedTextUpdateHandlers = null;

        if (textUpdateKey != null)
        {
            // Backup existing TextUpdate handlers and temporarily remove them to stop the feedback loop
            savedTextUpdateHandlers = eventHandlerList[textUpdateKey];
            eventHandlerList[textUpdateKey] = null;
        }

        try
        {
            // 3. EXECUTE THE NATIVE EVENT SEQUENCE (Fires SelectedIndexChanged exactly when needed)
            Win32.SendMessage(handle, CB_SETCURSEL, (IntPtr)index, IntPtr.Zero);

            IntPtr wParam = (IntPtr)(((int)handle & 0xFFFF) | (CBN_SELCHANGE << 16));
            Win32.SendMessage(comboBox.Parent.Handle, WM_COMMAND, wParam, handle);

            // 4. SETTLE THE LAYOUT OVERAGES IMMEDIATELY BEFORE RESTORING
            // This flushes the layout-driven TextUpdate side-effects while the event is silenced
            Application.DoEvents();
        }
        finally
        {
            // 5. RESTORE THE RECORDER HOOKS
            if (textUpdateKey != null && savedTextUpdateHandlers != null)
            {
                eventHandlerList[textUpdateKey] = savedTextUpdateHandlers;
            }
        }
#endif
    }

    /// <summary>
    /// Selects an entry in the ComboBox according to its string value.
    /// </summary>
    /// <remarks>
    /// Sets the Selected Index property on the underlying control after calling
    /// FindStringExact
    /// </remarks>
    /// <param name="text">The string value of the entry to select.</param>
    public void Select(string text)
    {
        int index;
        if ((index = Properties.FindStringExact(text)) == -1)
        {
            ThrowHelper.ThrowFormsTestAssertionException($"Could not find text '{text}' in ComboBox '{Name}'");
        }
        Select(index);
    }
}