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

using System.Windows.Forms;

using NUnit.Extensions.Forms.Exceptions;
using NUnit.Extensions.Forms.Testers;
using NUnit.Extensions.Forms.TestApplications.TestForms;
using NUnit.Framework;


using TabControlTester = NUnit.Extensions.Forms.Testers.TabControlTester;


namespace NUnit.Extensions.Forms.TestApplications;

[TestFixture]
public class MultipleFormsTest : NUnitFormTest
{
    private void ShowForm(Form f)
    {
        f.Show();
    }

    [Test]
    public void AmbiguousNameWithMultipleForms()
    {
        ShowForm(new ButtonTestForm());
        ShowForm(new ButtonTestForm());

        var myButton = new ButtonTester("myButton");
        Assert.Throws<AmbiguousNameException>(myButton.Click);
    }

    [Test]
    public void DontNeedToSpecifyForm()
    {
        ShowForm(new ButtonTestForm());
        ShowForm(new TabControlTestForm());

        var myButton = new ButtonTester("myButton");
        var myLabel = new LabelTester("myLabel");
        var myTabs = new TabControlTester("myTabs");
        var tabButton = new ButtonTester("button2");
        var tabLabel = new LabelTester("label2");

        myTabs.SelectTab(1);
        Assert.AreEqual("0", tabLabel.Text);

        Assert.AreEqual("0", myLabel.Text);
        myButton.Click();
        Assert.AreEqual("1", myLabel.Text);

        tabButton.Click();
        Assert.AreEqual("1", tabLabel.Text);
    }

    [Test]
    public void TestMultipleForms()
    {
        var form = new MultiForm();
        form.Show();

        var buttonOne = new ButtonTester("myButton", "Form");
        var buttonTwo = new ButtonTester("myButton", "Form-0");
        var buttonThree = new ButtonTester("myButton", "Form-0-0");
        var buttonFour = new ButtonTester("myButton", "Form-1");

        buttonOne.Click();
        buttonTwo.Click();
        buttonThree.Click();
        buttonOne.Click();
        buttonFour.Click();
    }

    [Test]
    public void TestMultipleFormsShouldNotFindLastButton()
    {
        var form = new MultiForm();
        form.Show();

        var buttonOne = new ButtonTester("myButton", "Form");
        var buttonTwo = new ButtonTester("myButton", "Form-0");
        var buttonThree = new ButtonTester("myButton", "Form-0-0");
        var buttonFour = new ButtonTester("myButton", "Form-1");

        buttonOne.Click();
        buttonTwo.Click();
        buttonThree.Click();
        var ex = Assert.Throws<NoSuchControlException>(buttonFour.Click);
        Assert.That(ex.Message, Does.Contain("Could not find form with name 'Form-1'"));
    }
}