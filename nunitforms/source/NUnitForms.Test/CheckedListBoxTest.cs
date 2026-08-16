#region Copyright (c) 2003-2005, Luke T. Maxon : (Contributed by Ian Cooper) : 2026-2026 Smurf.IV

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
using System.Collections;
using NUnit.Extensions.Forms.TestApplications.TestForms;
using NUnit.Extensions.Forms.Testers;
using NUnit.Framework;

namespace NUnit.Extensions.Forms.TestApplications;

[TestFixture]
public class CheckedListBoxTest : NUnitFormTest
{
    private CheckedListBoxTestForm? checkedListForm;

    private CheckedListBoxTester? checkedListBox;

    public override void Setup()
    {
        checkedListForm = new CheckedListBoxTestForm();
        checkedListBox = new CheckedListBoxTester("checkedListBox", checkedListForm);
    }

    [TearDown]
    public override void TearDown()
    {
        checkedListForm?.Dispose();
        base.TearDown();
    }

    private void FillListBox()
    {
        checkedListForm.Show();
        checkedListBox.Items.AddRange(["Red", "Orange", "Yellow", "Green", "Blue", "Indigo", "Violet"]);
    }

    [Test]
    public void CheckItem()
    {
        checkedListForm.Show();
        var ex = Assert.Throws<IndexOutOfRangeException>(() => checkedListBox.CheckItem("Ultra-Violet"));
        Assert.That(ex.Message, Does.Contain("Ultra-Violet not in list"));
    }

    [Test]
    public void CheckItems()
    {
        checkedListForm.Show();
        FillListBox();
        checkedListBox.CheckItem("Red");
        checkedListBox.CheckItem("Orange");
        checkedListBox.CheckItem("Indigo");
        checkedListBox.CheckItem("Violet");

        checkedListBox.CheckSelectedItems(new ArrayList(new[] {"Red", "Orange", "Indigo", "Violet"}));
    }

    [Test]
    public void ClearItems()
    {
        checkedListForm.Show();
        FillListBox();
        checkedListBox.CheckItem("Red");
        checkedListBox.CheckItem("Orange");
        checkedListBox.CheckItem("Indigo");
        checkedListBox.CheckItem("Violet");

        checkedListBox.ClearItem("Orange");

        checkedListBox.CheckSelectedItems(new ArrayList(new[] {"Red", "Indigo", "Violet"}));
    }

    [Test]
    public void HookupTestForm()
    {
        checkedListForm.Show();
        Assert.IsTrue(checkedListBox.Properties.Visible);
    }
}