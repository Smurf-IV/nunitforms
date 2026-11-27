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

namespace NUnit.WinForms.Test;

[TestFixture]
public class ContainersLayoutTest : NUnitFormTest
{
    private SplitContainerTester splitContainer = null!;
    private SplitterTester splitter = null!;
    private FlowLayoutPanelTester flow = null!;
    private TableLayoutPanelTester table = null!;
    private GroupBoxTester group = null!;
    private TabPageTester tabPage = null!;
    private UserControlTester userControl = null!;
    private ContainerControlTester containerControl = null!;
    private ScrollableControlTester scrollableControl = null!;

    public override void Setup()
    {
        new ContainersLayoutTestForm().Show();
        splitContainer = new SplitContainerTester("mySplitContainer");
        splitter = new SplitterTester("mySplitter");
        flow = new FlowLayoutPanelTester("myFlow");
        table = new TableLayoutPanelTester("myTable");
        group = new GroupBoxTester("myGroup");
        tabPage = new TabPageTester("myTabPage");
        userControl = new UserControlTester("myUserControl");
        containerControl = new ContainerControlTester("myUserControl");
        scrollableControl = new ScrollableControlTester("myFlow");
    }

    [Test]
    public void ContainersAndLayoutProperties()
    {
        Assert.IsNotNull(splitContainer.Properties);
        Assert.IsNotNull(splitter.Properties);
        Assert.IsNotNull(flow.Properties);
        Assert.IsNotNull(table.Properties);
        Assert.AreEqual("Group Heading", group.Text);
        Assert.AreEqual("Page 1", tabPage.Text);
        Assert.IsNotNull(userControl.Properties);
        Assert.IsNotNull(containerControl.Properties);
        Assert.IsNotNull(scrollableControl.Properties);
    }
}
