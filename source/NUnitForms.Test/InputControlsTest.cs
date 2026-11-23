#region Copyright (c) 2026-2026 Smurf.IV

/********************************************************************************************************************
'
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

using NUnit.WinForms.Test.TestForms;
using NUnit.Framework;
using NUnit.WinForms.Testers;

namespace NUnit.WinForms.Test;

[TestFixture]
public class InputControlsTest : NUnitFormTest
{
    private MaskedTextBoxTester masked = null!;
    private MonthCalendarTester calendar = null!;
    private PictureBoxTester picture = null!;
    private PropertyGridTester propertyGrid = null!;
    private PrintPreviewControlTester printPreview = null!;
    private LabelTester label = null!;

    public override void Setup()
    {
        new InputControlsTestForm().Show();
        masked = new MaskedTextBoxTester("myMasked");
        calendar = new MonthCalendarTester("myCalendar");
        picture = new PictureBoxTester("myPicture");
        propertyGrid = new PropertyGridTester("myPropertyGrid");
        printPreview = new PrintPreviewControlTester("myPrintPreview");
        label = new LabelTester("myLabel");
    }

    [Test]
    public void MaskedTextBoxTest()
    {
        Assert.AreEqual("11/19/2026", masked.Text);
        masked.Properties.Text = "12/25/2026";
        masked.FireEvent("TextChanged");
        Assert.AreEqual("Masked:12/25/2026", label.Text);
    }

    [Test]
    public void MonthCalendarTest()
    {
        var newDate = new DateTime(2026, 12, 1);
        calendar.Properties.SetDate(newDate);
        calendar.FireEvent("DateChanged", new System.Windows.Forms.DateRangeEventArgs(newDate, newDate));
        Assert.AreEqual($"Calendar:{newDate.ToShortDateString()}", label.Text);
    }

    [Test]
    public void PictureBoxTest()
    {
        Assert.AreEqual("Initial", label.Text);
        picture.Click();
        Assert.AreEqual("PictureClicked", label.Text);
    }

    [Test]
    public void OtherInputControlsProperties()
    {
        Assert.IsNotNull(propertyGrid.Properties);
        Assert.IsNotNull(printPreview.Properties);
        var webBrowserTester = new WebBrowserTester();
        Assert.IsNotNull(webBrowserTester);
    }
}
