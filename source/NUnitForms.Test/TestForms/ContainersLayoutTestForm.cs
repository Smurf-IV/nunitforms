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

using System.Drawing;
using System.Windows.Forms;

namespace NUnit.WinForms.Test.TestForms;

public class ContainersLayoutTestForm : Form
{
    private SplitContainer mySplitContainer = null!;
    private Splitter mySplitter = null!;
    private FlowLayoutPanel myFlow = null!;
    private TableLayoutPanel myTable = null!;
    private GroupBox myGroup = null!;
    private TabControl myTabs = null!;
    private TabPage myTabPage = null!;
    private UserControl myUserControl = null!;

    public ContainersLayoutTestForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        mySplitContainer = new SplitContainer();
        mySplitter = new Splitter();
        myFlow = new FlowLayoutPanel();
        myTable = new TableLayoutPanel();
        myGroup = new GroupBox();
        myTabs = new TabControl();
        myTabPage = new TabPage();
        myUserControl = new UserControl();
        SuspendLayout();

        mySplitContainer.Location = new Point(10, 10);
        mySplitContainer.Name = "mySplitContainer";
        mySplitContainer.Size = new Size(150, 50);

        mySplitter.Location = new Point(10, 65);
        mySplitter.Name = "mySplitter";

        myFlow.Location = new Point(10, 80);
        myFlow.Name = "myFlow";
        myFlow.Size = new Size(100, 40);

        myTable.Location = new Point(120, 80);
        myTable.Name = "myTable";
        myTable.Size = new Size(100, 40);

        myGroup.Location = new Point(10, 130);
        myGroup.Name = "myGroup";
        myGroup.Text = "Group Heading";
        myGroup.Size = new Size(100, 50);

        myTabs.Location = new Point(120, 130);
        myTabs.Name = "myTabs";
        myTabPage.Name = "myTabPage";
        myTabPage.Text = "Page 1";
        myTabs.Controls.Add(myTabPage);

        myUserControl.Location = new Point(10, 190);
        myUserControl.Name = "myUserControl";

        ClientSize = new Size(250, 250);
        Controls.Add(mySplitContainer);
        Controls.Add(mySplitter);
        Controls.Add(myFlow);
        Controls.Add(myTable);
        Controls.Add(myGroup);
        Controls.Add(myTabs);
        Controls.Add(myUserControl);
        Name = "ContainersLayoutTestForm";
        Text = "ContainersLayoutTestForm";
        ResumeLayout(false);
    }
}
