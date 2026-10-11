#if !NETFRAMEWORK
using NUnit.Extensions.Forms.TestApplications.TestForms;
using NUnit.Extensions.Forms.Testers;
using NUnit.Framework;

using ToolBarTester = NUnit.Extensions.Forms.Testers.ToolBarTester;

namespace NUnit.Extensions.Forms.TestApplications;
n[TestFixture]
public class ToolbarTest : NUnitFormTest
{
    private LabelTester labelToolbarSelection;
    private ToolBarTester toolbarTest;
n    public override void Setup()
    {
        new ToolbarTestForm().Show();
        labelToolbarSelection = new LabelTester("labelToolbarSelection");
        toolbarTest = new ToolBarTester("toolBarTest");
    }
n    [Test]
    public void ClickToolbarButton()
    {
        ToolBarButtonTester button = toolbarTest.GetButton("Open");
        button.Click();
        Assert.AreEqual("Open", labelToolbarSelection.Text);
n        button = toolbarTest.GetButton("Previous");
        button.Click();
        Assert.AreEqual("Previous", labelToolbarSelection.Text);
    }
n    [Test]
    public void DropDownButtonTest()
    {
        ToolBarButtonTester button = toolbarTest.GetButton("Color");
        button.ClickDropDownMenuItem("Red");
        Assert.AreEqual("Red", labelToolbarSelection.Text);
n        button = toolbarTest.GetButton("Color");
        button.ClickDropDownMenuItem("Violet");
        Assert.AreEqual("Violet", labelToolbarSelection.Text);
    }
n    [Test]
    public void GetToolbar()
    {
        Assert.IsTrue(toolbarTest.Properties.Visible);
    }
n    [Test]
    public void ToggleButtonTest()
    {
        ToolBarButtonTester button = toolbarTest.GetButton(6); // toggle is at index 6 in Core form
        button.Push();
        Assert.IsTrue(button.Pushed);
n        button.PartialPush();
        Assert.IsTrue(button.PartialPushed);
        button.PartialPush();
        Assert.IsFalse(button.PartialPushed);
    }
}
#endif