#if !NETFRAMEWORK
using NUnit.WinForms.Test.TestForms;
using NUnit.WinForms.Testers;
using NUnit.Framework;

using ToolBarTester = NUnit.WinForms.Testers.ToolBarTester;

namespace NUnit.WinForms.Test;

[TestFixture]
public class ToolbarTest : NUnitFormTest
{
    private LabelTester labelToolbarSelection;
    private ToolBarTester toolbarTest;

    public override void Setup()
    {
        new ToolbarTestForm().Show();
        labelToolbarSelection = new LabelTester("labelToolbarSelection");
        toolbarTest = new ToolBarTester("toolBarTest");
    }

    [Test]
    public void ClickToolbarButton()
    {
        ToolBarButtonTester button = toolbarTest.GetButton("Open");
        button.Click();
        Assert.AreEqual("Open", labelToolbarSelection.Text);

        button = toolbarTest.GetButton("Previous");
        button.Click();
        Assert.AreEqual("Previous", labelToolbarSelection.Text);
    }

    [Test]
    public void DropDownButtonTest()
    {
        ToolBarButtonTester button = toolbarTest.GetButton("Color");
        button.ClickDropDownMenuItem("Red");
        Assert.AreEqual("Red", labelToolbarSelection.Text);

        button = toolbarTest.GetButton("Color");
        button.ClickDropDownMenuItem("Violet");
        Assert.AreEqual("Violet", labelToolbarSelection.Text);
    }

    [Test]
    public void GetToolbar()
    {
        Assert.IsTrue(toolbarTest.Properties.Visible);
    }

    [Test]
    public void ToggleButtonTest()
    {
        ToolBarButtonTester button = toolbarTest.GetButton(6); // toggle is at index 6 in Core form
        button.Push();
        Assert.IsTrue(button.Pushed);

        button.PartialPush();
        Assert.IsTrue(button.PartialPushed);
        button.PartialPush();
        Assert.IsFalse(button.PartialPushed);
    }
}
#endif