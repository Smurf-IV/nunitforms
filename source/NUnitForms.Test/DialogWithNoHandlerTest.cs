using System.Windows.Forms;

using NUnit.Extensions.Forms.Testers;
using NUnit.Framework;


namespace NUnit.Extensions.Forms.TestApplications;

[TestFixture]
public class DialogWithNoHandlerTest : NUnitFormTest
{
    private ButtonTester acceptButton;
    private ButtonTester rejectButton;


    [Test]
    public void TestAcceptButton()
    {
        ModalFormHandler =
            delegate
            {
                acceptButton = new ButtonTester("button1");
                acceptButton.Click();
            };
        var form = new TestForms.DialogWithNoHandlersForm();
        DialogResult result = form.ShowDialog();
        Assert.AreEqual(DialogResult.OK, result, "Wrong dialog result.");
        Assert.IsFalse(form.Visible, "Form was still visible.");
        form.Close();
    }

    [Test]
    public void TestRejectButton()
    {
        ModalFormHandler =
            delegate
            {
                rejectButton = new ButtonTester("button2");
                rejectButton.Click();
            };

        var form = new TestForms.DialogWithNoHandlersForm();
        DialogResult result = form.ShowDialog();
        Assert.AreEqual(DialogResult.Cancel, result, "Wrong dialog result.");
        Assert.IsFalse(form.Visible, "Form was still visible.");
        form.Close();
    }
}