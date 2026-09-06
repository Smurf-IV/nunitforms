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

using NUnit.Extensions.Forms.Exceptions;
using NUnit.Extensions.Forms.Util;


namespace NUnit.Extensions.Forms.Finders;

/// <summary>
/// A class to help find a form according to its name.  NUnitForms users should not need to use
/// this class.  Consider it as internal.  
/// </summary>
/// <remarks>
/// It is also used by the recorder application.</remarks>
public class FormFinder
{
    private List<Form>? _forms;

    private string? _name;

    private int FindMatchingForms(IntPtr hwnd, IntPtr lParam)
    {
        if (Form.FromHandle(hwnd) is Form theForm 
            && (_name == null || theForm.Name == _name))
        {
            _forms.Add(theForm);
        }
        return 1;
    }

    /// <summary>
    /// Finds all of the forms with a specified name and returns them in a FormCollection.
    /// </summary>
    /// <param name="formName">The name of the form to search for.</param>
    /// <returns>the FormCollection of all found forms.</returns>
    public List<Form> FindAll(string? formName)
    {
        lock (this)
        {
            _forms = [];
            _name = formName;
            IntPtr desktop = Win32.GetDesktopWindow();
            Win32.EnumChildWindows(desktop, FindMatchingForms, IntPtr.Zero);
            return _forms;
        }
    }

    /// <summary>
    /// Finds one form with the specified name.
    /// </summary>
    /// <param name="formName">The name of the form to search for.</param>
    /// <returns>The form it finds.</returns>
    /// <exception cref="NoSuchControlException">
    /// Thrown if there are no forms with the specified name.
    /// </exception>
    /// <exception cref="AmbiguousNameException">
    /// Thrown if there is more than one form with the specified name.</exception>
    public Form Find(string? formName)
    {
        List<Form> list = FindAll(formName);
        return list.Count switch
        {
            0 => ThrowHelper.ThrowNoSuchControlException<Form>($"Could not find form with name '{formName}'"),
            > 1 => ThrowHelper.ThrowAmbiguousNameException<Form>($"Found too many forms with the name '{formName}'"),
            _ => list[0]
        };
    }

    /// <summary>
    /// Finds all the forms, regardless of name.
    /// </summary>
    public List<Form> FindAll()
        => FindAll(null);
}