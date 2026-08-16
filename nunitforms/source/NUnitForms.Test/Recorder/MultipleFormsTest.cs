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

using NUnit.Extensions.Forms.Exceptions;
using NUnit.Extensions.Forms.TestApplications.TestForms;
using NUnit.Extensions.Forms.Testers;
using NUnit.Framework;

using NUnitForms.Recorder;


namespace NUnit.Extensions.Forms.TestApplications.Recorder;

[TestFixture]
[Category("Recorder")]
public class MultipleFormsTest : NUnitFormTest
{
    [Test]
    public void EventCausesAnother()
    {
        var form = new MultiForm();
        form.Show();
        var writer = new TestWriter(form);
        var button = new ButtonTester("btnClose");
        button.Click();
        try
        {
            button.Click();
            Assert.Fail("Window did not close.");
        }
        catch (NoSuchControlException)
        {
            //window is closed.. good.
        }

        Assert.AreEqual(
            @"[Test]
public void Test()
{

	ButtonTester btnClose = new ButtonTester(""btnClose"");

	btnClose.Click();

}",
            writer.Test);
    }

    [Test]
    public void FormClose()
    {
        var form = new MultiForm();
        form.Show();
        var writer = new TestWriter(form);
        var button = new ButtonTester("myButton");
        button.Click();
        using var form0 = new FormTester("Form-0");
        form0.Close();

        Assert.AreEqual(
            $@"[Test]
public void Test()
{{

	ButtonTester myButton = new ButtonTester(""myButton"");
	FormTester Form-0 = new FormTester(""Form-0"");

	myButton.Click();
	Form-0.Close();

}}",
            writer.Test);
    }

    [Test]
    public void MultipleForms()
    {
        var form = new MultiForm();
        form.Show();
        var writer = new TestWriter(form);
        Assert.AreEqual("", writer.Test);

        var button = new ButtonTester("myButton");
        button.Click();
        var button2 = new ButtonTester("myButton", "Form-0");
        button2.Click();

        Assert.AreEqual(
            @"[Test]
public void Test()
{

	ButtonTester myButton = new ButtonTester(""myButton"");
	ButtonTester Form-0_myButton = new ButtonTester(""myButton"", ""Form-0"");

	myButton.Click();
	Form-0_myButton.Click();

}",
            writer.Test);
    }

    [Test]
    public void NamesShouldAdapt()
    {
        var form = new MultiForm();
        form.Show();
        var writer = new TestWriter(form);
        Assert.AreEqual("", writer.Test);

        var nothingButton = new ButtonTester("nothingButton");
        nothingButton.Click();

        //------------------------------------------------------

        Assert.AreEqual(
            @"[Test]
public void Test()
{

	ButtonTester nothingButton = new ButtonTester(""nothingButton"");

	nothingButton.Click();

}",
            writer.Test);

        //------------------------------------------------------

        var myButton = new ButtonTester("myButton");
        myButton.Click();

        //------------------------------------------------------

        Assert.AreEqual(
            @"[Test]
public void Test()
{

	ButtonTester nothingButton = new ButtonTester(""nothingButton"");
	ButtonTester myButton = new ButtonTester(""myButton"");

	nothingButton.Click();
	myButton.Click();

}",
            writer.Test);

        //------------------------------------------------------

        var nothingButton2 = new ButtonTester("nothingButton", "Form-0");
        nothingButton2.Click();
        var nothingButton3 = new ButtonTester("nothingButton", "Form");
        nothingButton3.Click();

        //------------------------------------------------------

        Assert.AreEqual(
            @"[Test]
public void Test()
{

	ButtonTester Form_nothingButton = new ButtonTester(""nothingButton"", ""Form"");
	ButtonTester myButton = new ButtonTester(""myButton"");
	ButtonTester Form-0_nothingButton = new ButtonTester(""nothingButton"", ""Form-0"");

	Form_nothingButton.Click();
	myButton.Click();
	Form-0_nothingButton.Click();
	Form_nothingButton.Click();

}",
            writer.Test);
        //------------------------------------------------------
    }
}