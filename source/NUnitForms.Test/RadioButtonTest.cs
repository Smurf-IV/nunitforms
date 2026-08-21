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

using NUnit.Extensions.Forms.Testers;
using NUnit.Extensions.Forms.TestApplications.TestForms;
using NUnit.Framework;


namespace NUnit.Extensions.Forms.TestApplications;

[TestFixture]
public class RadioButtonTest : NUnitFormTest
{
    public override void Setup()
    {
        new RadioButtonTestForm().Show();
    }

    [Test]
    public void SelectOption()
    {
        var rbRed = new RadioButtonTester("rbRed");
        var lblSelectedColor = new LabelTester("lblSelectedColor");
        var rbOrange = new RadioButtonTester("rbOrange");
        var rbGreen = new RadioButtonTester("rbGreen");
        var rbYellow = new RadioButtonTester("rbYellow");
        var rbBlue = new RadioButtonTester("rbBlue");
        var rbIndigo = new RadioButtonTester("rbIndigo");
        var rbViolet = new RadioButtonTester("rbViolet");

        rbRed.Click();
        Assert.AreEqual("Red", lblSelectedColor.Properties.Text);
        Assert.AreEqual(true, rbRed.Properties.Checked);
        Assert.AreEqual(false, rbOrange.Properties.Checked);
        Assert.AreEqual("Red", rbRed.Text);
        Assert.AreEqual("Red", rbRed.Properties.Text);

        rbOrange.Click();
        Assert.AreEqual("Orange", lblSelectedColor.Properties.Text);
        Assert.AreEqual(false, rbRed.Properties.Checked);
        Assert.AreEqual(true, rbOrange.Properties.Checked);
        Assert.AreEqual("Orange", rbOrange.Text);
        Assert.AreEqual("Orange", rbOrange.Properties.Text);
    }
}