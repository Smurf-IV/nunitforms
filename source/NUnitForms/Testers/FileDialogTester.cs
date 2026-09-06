#region Copyright (c) 2006-2007, Luke T. Maxon : (Authored by Anders Lillrank) : 2026-2026 Smurf.IV

/********************************************************************************************************************
'
' Copyright (c) 2006-2007, Luke T. Maxon
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
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using NUnit.Extensions.Forms.Exceptions;
using NUnit.Extensions.Forms.Util;


namespace NUnit.Extensions.Forms.Testers;

///<summary>
/// A form tester for the <see cref="FileDialog"/>.
///</summary>
public class FileDialogTester
{
    /// <summary>
    /// Control ID for the Cancel button.
    /// </summary>
    protected const int CancelButton = 2;

    /// <summary>
    /// Control ID for the file name checkbox.
    /// </summary>
    protected const int FileNameCheckBox = 1148;

    /// <summary>
    /// Control ID for the Open or Save button.
    /// </summary>
    protected const int OpenButton = 1;

    private IntPtr _hWnd;
    public IntPtr Handle => _hWnd;


    public FileDialogTester(IntPtr hWnd)
    {
        _hWnd = hWnd;
    }


    /// <summary>
    /// Clicks the cancel button of a OpenFileDialog.
    /// </summary>
    public void ClickCancel()
    {
        if (_hWnd == IntPtr.Zero)
        {
            _hWnd = FindFileDialog();
        }

        GetMessageHook.Record(ClickCancelCB);
    }

    private bool ClickCancelCB()
    {
        IntPtr cancel_btn = Win32.GetDlgItem(_hWnd, CancelButton);
        Win32.PostMessage(cancel_btn, Win32.BM_CLICK, IntPtr.Zero, IntPtr.Zero);
        return true;
    }

    /// <summary>
    /// Click the first button, usually "Open" or "Save".
    /// </summary>
    protected void ClickOpenSaveButton()
    {
        if (_hWnd == IntPtr.Zero)
        {
            _hWnd = FindFileDialog();
        }

        GetMessageHook.Record(ClickOpenSaveButtonCB);
    }

    private bool ClickOpenSaveButtonCB()
    {
        IntPtr open_btn = Win32.GetDlgItem(_hWnd, OpenButton);
        Win32.PostMessage(open_btn, Win32.BM_CLICK, IntPtr.Zero, IntPtr.Zero);
        return true;
    }


    /// <summary>
    /// Sets the filename in the filename ComboBox and presses the OpenSave button.
    /// </summary>
    protected void SetFileName(string file)
    {
        if (_hWnd == IntPtr.Zero)
        {
            _hWnd = FindFileDialog();
        }

        GetMessageHook.Record(() => SetFileNameCB(file));
    }

    private bool SetFileNameCB(string file)
    {
        if (!Win32.IsWindowVisible(_hWnd))
        {
            return false;
        }

        IntPtr fnh = Win32.GetDlgItem(_hWnd, FileNameCheckBox);
        if (fnh == IntPtr.Zero)
        {
            // On Vista 64, it seems the combo box does not have an id. However, it contains a control with id 1001.
            Win32.EnumChildWindows(_hWnd,
                delegate (IntPtr wnd, IntPtr lparam)
                {
                    if (Win32.GetDlgItem(wnd, 1001) == IntPtr.Zero)
                    {
                        return 1;
                    }

                    fnh = wnd;
                    return 0;
                }, IntPtr.Zero);

            if (fnh == IntPtr.Zero)
            {
                ThrowHelper.ThrowNoSuchControlException("NUnitForms fatal error: cannot find filename box");
            }

            GetMessageHook.Record(delegate
            {
                Win32.SetWindowText(fnh, file);
                var sb = new StringBuilder(file.Length + 1);
                Win32.GetWindowText(fnh, sb, file.Length + 1);
                if (sb.ToString().ToLowerInvariant() != file.ToLowerInvariant())
                {
                    return false;
                }

                IntPtr open_btn = Win32.GetDlgItem(_hWnd, OpenButton);
                Win32.PostMessage(open_btn, Win32.BM_CLICK, IntPtr.Zero, IntPtr.Zero);

                return true;
            });
        }
        else
        {
            Win32.SetDlgItemText(_hWnd, FileNameCheckBox, file);

            var sb = new StringBuilder(file.Length + 1);
            Win32.GetDlgItemText(_hWnd, FileNameCheckBox, sb, file.Length + 1);

            if (sb.ToString().ToLowerInvariant() != file.ToLowerInvariant())
            {
                return false;
            }

            IntPtr open_btn = Win32.GetDlgItem(_hWnd, OpenButton);
            Win32.PostMessage(open_btn, Win32.BM_CLICK, IntPtr.Zero, IntPtr.Zero);
            return true;
        }

        return true;
    }

    // Old interface

    /// <summary>
    /// Determines the initial name of the file dialog boxes, based on the locale.
    /// Currently works for english, german and french only.
    /// </summary>
    static FileDialogTester()
    {
        // The initial name for the file dialogs depend on the locale.
        // Add a case for your own system if you really want to use the obsoleted
        // ExpectFileDialog functions
        if (new Regex("french|france", RegexOptions.IgnoreCase)
            .IsMatch(CultureInfo.CurrentCulture.EnglishName))
        {
            InitialFileDialogName = @"Ouvrir";
        }
        else if (new Regex("german", RegexOptions.IgnoreCase)
                 .IsMatch(CultureInfo.CurrentCulture.EnglishName))
        {
            InitialFileDialogName = @"Öffnen";
        }
        else
        {
            InitialFileDialogName = @"Open";
        }
    }
    /// <summary>
    /// Initial name of the file dialog boxes. They seem to change name after their creation.
    /// </summary>
    public static string InitialFileDialogName
    {
        get;
        private set;
    }


    [Obsolete]
    public FileDialogTester(string title)
    {
        // disregard the given title, since at the time it is called, it will always be InitialFileDialogName
        _hWnd = IntPtr.Zero;
    }


    /// <summary>
    /// Finds the OpenFileDialog.
    /// </summary>
    protected static IntPtr FindFileDialog()
    {
        IntPtr desktop = Win32.GetDesktopWindow();
        IntPtr res = IntPtr.Zero;
        Win32.EnumChildWindows(desktop,
            delegate (IntPtr hwnd, IntPtr lParam)
            {
                if (WindowHandle.IsDialog(hwnd))
                {
                    string name = WindowHandle.GetCaption(hwnd);
                    if (name == InitialFileDialogName
                        // Vista 64 hack
                        || name == @"Save as"
                        || name == @"Enregistrer sous"
                        || name == @"Speichern unter")
                    {
                        res = hwnd;
                    }
                }
                return 1;
            },
            IntPtr.Zero);
        if (res == IntPtr.Zero)
        {
            ThrowHelper.ThrowControlNotVisibleException("Open File Dialog is not visible");
        }
        return res;
    }
}