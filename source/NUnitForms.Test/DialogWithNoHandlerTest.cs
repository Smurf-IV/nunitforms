using System.Windows.Forms;
using NUnit.WinForms.Test.TestForms;
using NUnit.Framework;
using NUnit.WinForms.Testers;

namespace NUnit.WinForms.Test;

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
        using var form = new DialogWithNoHandlersForm();
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

        using var form = new DialogWithNoHandlersForm();
        DialogResult result = form.ShowDialog();
        Assert.AreEqual(DialogResult.Cancel, result, "Wrong dialog result.");
        Assert.IsFalse(form.Visible, "Form was still visible.");
        form.Close();
    }
}