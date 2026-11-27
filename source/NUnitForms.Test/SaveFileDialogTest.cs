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
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;
using NUnit.WinForms.Testers;


namespace NUnit.WinForms.Test;

[TestFixture]
[Apartment(ApartmentState.STA)]
public class SaveFileDialogTest : NUnitFormTest
{
    private readonly LabelTester label1 = new("lblFileName");
    private string _fileName = "";
    private string? _dialogFileName;
    private TestForms.SaveFileDialogTestForm form;

    public override void Setup()
    {
        base.Setup();

        form = new TestForms.SaveFileDialogTestForm();
        form.Show();
    }

    [TearDown]
    public override void TearDown()
    {
        form.Close();
        form.Dispose();
        base.TearDown();
    }


    private void ClickSaveButton()
    {
        var save_btn = new ButtonTester("btSave", form);
        save_btn.Click();
    }

    public void SaveFileHandler(string name, IntPtr hWnd, Form form)
    {
        var dlg_tester = new SaveFileDialogTester(hWnd);
        dlg_tester.SaveFile(_fileName);
    }

    public void CancelFileHandler(string name, IntPtr hWnd, Form form)
    {
        var dlg_tester = new SaveFileDialogTester(hWnd);

        dlg_tester.ClickCancel();

    }

    public void SaveDefaultFileHandler(string name, IntPtr hWnd, Form form)
    {
        var dlg_tester = new SaveFileDialogTester(hWnd);
        int attempts = 0;
        var timer = new System.Windows.Forms.Timer { Interval = 50 };
        timer.Tick += (_, _) =>
        {
            var fileName = new StringBuilder(512);
            NUnit.WinForms.Util.Win32.GetDlgItemText(hWnd, 1148, fileName, fileName.Capacity);
            _dialogFileName = fileName.ToString();
            bool isDefaultFileName = IsExpectedDefaultFileName(_dialogFileName);
            if (isDefaultFileName || ++attempts == 20)
            {
                timer.Stop();
                if (isDefaultFileName)
                {
                    dlg_tester.SaveFile(_fileName);
                }
                else
                {
                    dlg_tester.ClickCancel();
                }
                timer.Dispose();
            }
        };
        timer.Start();
    }

    private bool IsExpectedDefaultFileName(string? dialogFileName)
    {
        if (string.IsNullOrEmpty(dialogFileName))
        {
            return false;
        }

        string expectedFileName = Path.GetFileName(_fileName);
        return dialogFileName.Equals(expectedFileName, StringComparison.OrdinalIgnoreCase)
               || dialogFileName.Equals(Path.GetFileNameWithoutExtension(expectedFileName), StringComparison.OrdinalIgnoreCase);
    }

    [Test]
    public void CancelTest()
    {
        ModalFormHandler = CancelFileHandler;
        ClickSaveButton();
        Assert.AreEqual(label1.Text, "cancel pressed");
    }

    [Test]
    public void SaveTest()
    {
        ModalFormHandler = SaveFileHandler;

        // Generate a temporary file
        _fileName = Path.Combine(Path.GetTempPath(), $"NUnitFormsTestFile_{Guid.NewGuid():N}.tmp");

        ClickSaveButton();
        Assert.AreEqual(label1.Text.ToLowerInvariant(), _fileName.ToLowerInvariant());
    }


    [Test]
    public void SaveWithDefaultFile()
    {
        ModalFormHandler = SaveDefaultFileHandler;

        _fileName = Path.Combine(Path.GetTempPath(), $"NUnitFormsDefaultTestFile_{Guid.NewGuid():N}.tmp");

        form.SetDefaultTestFileName(_fileName);
        ClickSaveButton();
        Assert.That(IsExpectedDefaultFileName(_dialogFileName), Is.True,
            $"Unexpected default filename displayed by the save dialog: '{_dialogFileName}'.");
        Assert.AreEqual(label1.Text.ToLowerInvariant(), _fileName.ToLowerInvariant());
    }
}