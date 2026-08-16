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

using NUnit.Extensions.Forms.TestApplications.TestForms;
using NUnit.Extensions.Forms.Testers;
using NUnit.Framework;

using TextBoxTester = NUnit.Extensions.Forms.Testers.TextBoxTester;


namespace NUnit.Extensions.Forms.TestApplications;

[TestFixture]
public class TextBoxTest : NUnitFormTest
{
    public void oldhandler(string name, IntPtr hWnd, Form form)
    {
        var mb = new MessageBoxTester(hWnd);
        Assert.AreEqual("Old", mb.Text);
        mb.ClickOk();
    }

    public void newhandler(string name, IntPtr hWnd, Form form)
    {
        var mb = new MessageBoxTester(hWnd);
        Assert.AreEqual("New", mb.Text);
        mb.ClickOk();
    }

    [Test]
    public void DataSetBinding()
    {
        var f = new TextBoxDataSetBindingTestForm();
        f.Show();

        ModalFormHandler = oldhandler;
        new ButtonTester("btnView").Click();

        new TextBoxTester("myTextBox").Enter("New");
        ModalFormHandler = newhandler;
        new ButtonTester("btnView").Click();

        f.Close();
    }

    [Test]
    public void DataSetBindingWithGenericPropertySetter()
    {
        var f = new TextBoxDataSetBindingTestForm();
        f.Show();

        ModalFormHandler = oldhandler;
        new ButtonTester("btnView").Click();

        new TextBoxTester("myTextBox")["Text"] = "New";

        ModalFormHandler = newhandler;
        new ButtonTester("btnView").Click();
        f.Close();
    }

    [Test]
    public void TextBox()
    {
        var f = new TextBoxTestForm();
        f.Show();
        var box = new TextBoxTester("myTextBox", f);
        Assert.AreEqual("default", box.Text);
        box.Enter("Text");
        Assert.AreEqual("Text", box.Text);
        f.Close();
    }
}