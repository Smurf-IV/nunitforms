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

using NUnit.WinForms.Test.TestForms;
using NUnit.Framework;
using NUnit.WinForms.Testers;
using ContextMenuStripTester = NUnit.WinForms.Testers.ContextMenuStripTester;
using MenuStripTester = NUnit.WinForms.Testers.MenuStripTester;
using StatusStripTester = NUnit.WinForms.Testers.StatusStripTester;
using ToolStripTester = NUnit.WinForms.Testers.ToolStripTester;

namespace NUnit.WinForms.Test;

[TestFixture]
public class ToolStripExtendedTest : NUnitFormTest
{
    private ToolStripTester toolStrip = null!;
    private MenuStripTester menuStrip = null!;
    private ContextMenuStripTester contextMenu = null!;
    private StatusStripTester statusStrip = null!;
    private ToolStripLabelTester toolLabel = null!;
    private ToolStripProgressBarTester toolProgress = null!;
    private ToolStripSeparatorTester toolSeparator = null!;
    private ToolStripStatusLabelTester statusLabel = null!;
    private ToolStripDropDownItemTester dropDownItem = null!;
    private ToolStripMenuItemTester subItem = null!;
    private ToolStripContainerTester container = null!;
    private LabelTester outputLabel = null!;

    public override void Setup()
    {
        new ToolStripExtendedTestForm().Show();
        toolStrip = new ToolStripTester("myToolStrip");
        menuStrip = new MenuStripTester("myMenuStrip");
        contextMenu = new ContextMenuStripTester("myContextMenu");
        statusStrip = new StatusStripTester("myStatusStrip");
        toolLabel = new ToolStripLabelTester("myLabelItem");
        toolProgress = new ToolStripProgressBarTester("myProgressItem");
        toolSeparator = new ToolStripSeparatorTester("mySeparatorItem");
        statusLabel = new ToolStripStatusLabelTester("myStatusLabel");
        dropDownItem = new ToolStripDropDownItemTester("myDropDownButton");
        subItem = new ToolStripMenuItemTester("mySubItem");
        container = new ToolStripContainerTester("myContainer");
        outputLabel = new LabelTester("myOutputLabel");
    }

    [Test]
    public void ToolStripAndItemsTest()
    {
        Assert.AreEqual("ToolLabel", toolLabel.Text);
        Assert.AreEqual(30, toolProgress.Properties.Value);
        Assert.IsNotNull(toolSeparator.Properties);
        Assert.AreEqual("Ready", statusLabel.Text);
        Assert.AreEqual("DropDown", dropDownItem.Text);
    }

    [Test]
    public void MenuStripAndSubItemClick()
    {
        Assert.AreEqual("Initial", outputLabel.Text);
        ToolStripMenuItemTester fileItem = menuStrip.GetMenuItem("myFileMenu");
        fileItem.Click();
        Assert.AreEqual("FileMenuClicked", outputLabel.Text);

        subItem.Click();
        Assert.AreEqual("SubItemClicked", outputLabel.Text);
    }

    [Test]
    public void ContextMenuAndStatusStripItem()
    {
        Assert.AreEqual("Initial", outputLabel.Text);
        ToolStripMenuItemTester contextItem = contextMenu.GetMenuItem("myContextItem");
        contextItem.Click();
        Assert.AreEqual("ContextActionClicked", outputLabel.Text);

        ToolStripItemTester item = statusStrip.GetItem("myStatusLabel");
        Assert.AreEqual("Ready", item.Text);
    }

    [Test]
    public void ToolStripContainerPanels()
    {
        Assert.IsNotNull(container.Properties.TopToolStripPanel);
        Assert.IsNotNull(container.Properties.BottomToolStripPanel);
        Assert.IsNotNull(container.Properties.ContentPanel);
    }
}
