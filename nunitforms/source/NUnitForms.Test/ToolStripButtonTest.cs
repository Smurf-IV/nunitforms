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

using NUnit.Extensions.Forms.Testers;
using NUnit.Framework;


namespace NUnit.Extensions.Forms.TestApplications;

[TestFixture]
public class ToolStripButtonTest : NUnitFormTest
{
    private readonly LabelTester label1 = new LabelTester("label1");

    public override void Setup()
    {
        new TestForms.ToolStripButtonTestForm().Show();
    }

    [Test]
    public void BottomContainerButton()
    {
        var tester = new ToolStripButtonTester("toolStripButtonBottomInContainer");
        tester.Click();
        Assert.AreEqual(label1.Text, "toolStripButtonBottomInContainer clicked");
    }

    [Test]
    public void CenterContainerButton()
    {
        var tester = new ToolStripButtonTester("toolStripButtonCenterInContainer");
        tester.Click();
        Assert.AreEqual(label1.Text, "toolStripButtonCenterInContainer clicked");
    }

    [Test]
    public void LeftContainerButton()
    {
        var tester = new ToolStripButtonTester("toolStripButtonLeftInContainer");
        tester.Click();
        Assert.AreEqual(label1.Text, "toolStripButtonLeftInContainer clicked");
    }

    [Test]
    public void MainMenuButton()
    {
        var tester = new ToolStripButtonTester("toolStripButtonOnMain");
        tester.Click();
        Assert.AreEqual(label1.Text, "toolStripButtonOnMain clicked");
    }

    [Test]
    public void PanelButton()
    {
        var tester = new ToolStripButtonTester("toolStripButtonInPanel");
        tester.Click();
        Assert.AreEqual(label1.Text, "toolStripButtonInPanel clicked");
    }

    [Test]
    public void RightContainerButton()
    {
        var tester = new ToolStripButtonTester("toolStripButtonRightInContainer");
        tester.Click();
        Assert.AreEqual(label1.Text, "toolStripButtonRightInContainer clicked");
    }

    [Test]
    public void StatusBarDropDownButton()
    {
        var tester =
            new ToolStripDropDownButtonTester("toolStripDropDownButtonOnStatusStrip");
        tester.Click();
        Assert.AreEqual(label1.Text, "toolStripDropDownButtonOnStatusStrip clicked");
    }

    [Test]
    public void TopContainerButton()
    {
        var tester = new ToolStripButtonTester("toolStripButtonTopInContainer");
        tester.Click();
        Assert.AreEqual(label1.Text, "toolStripButtonTopInContainer clicked");
    }
}