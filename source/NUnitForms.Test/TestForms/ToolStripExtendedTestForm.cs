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

public class ToolStripExtendedTestForm : Form
{
    private ToolStripContainer myContainer = null!;
    private ToolStrip myToolStrip = null!;
    private ToolStripLabel myLabelItem = null!;
    private ToolStripProgressBar myProgressItem = null!;
    private ToolStripSeparator mySeparatorItem = null!;
    private ToolStripDropDownButton myDropDownButton = null!;
    private ToolStripMenuItem mySubItem = null!;
    private MenuStrip myMenuStrip = null!;
    private ToolStripMenuItem myFileMenu = null!;
    private ContextMenuStrip myContextMenu = null!;
    private ToolStripMenuItem myContextItem = null!;
    private StatusStrip myStatusStrip = null!;
    private ToolStripStatusLabel myStatusLabel = null!;
    private Label myOutputLabel = null!;

    public ToolStripExtendedTestForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        myContainer = new ToolStripContainer();
        myToolStrip = new ToolStrip();
        myLabelItem = new ToolStripLabel();
        myProgressItem = new ToolStripProgressBar();
        mySeparatorItem = new ToolStripSeparator();
        myDropDownButton = new ToolStripDropDownButton();
        mySubItem = new ToolStripMenuItem();
        myMenuStrip = new MenuStrip();
        myFileMenu = new ToolStripMenuItem();
        myContextMenu = new ContextMenuStrip();
        myContextItem = new ToolStripMenuItem();
        myStatusStrip = new StatusStrip();
        myStatusLabel = new ToolStripStatusLabel();
        myOutputLabel = new Label();
        SuspendLayout();

        myContainer.Dock = DockStyle.Fill;
        myContainer.Name = "myContainer";

        myToolStrip.Name = "myToolStrip";
        myLabelItem.Name = "myLabelItem";
        myLabelItem.Text = "ToolLabel";
        myProgressItem.Name = "myProgressItem";
        myProgressItem.Value = 30;
        mySeparatorItem.Name = "mySeparatorItem";
        myDropDownButton.Name = "myDropDownButton";
        myDropDownButton.Text = "DropDown";
        mySubItem.Name = "mySubItem";
        mySubItem.Text = "SubItem";
        mySubItem.Click += (s, e) => myOutputLabel.Text = "SubItemClicked";
        myDropDownButton.DropDownItems.Add(mySubItem);

        myToolStrip.Items.Add(myLabelItem);
        myToolStrip.Items.Add(myProgressItem);
        myToolStrip.Items.Add(mySeparatorItem);
        myToolStrip.Items.Add(myDropDownButton);

        myMenuStrip.Name = "myMenuStrip";
        myFileMenu.Name = "myFileMenu";
        myFileMenu.Text = "File";
        myFileMenu.Click += (s, e) => myOutputLabel.Text = "FileMenuClicked";
        myMenuStrip.Items.Add(myFileMenu);

        myContextMenu.Name = "myContextMenu";
        myContextItem.Name = "myContextItem";
        myContextItem.Text = "ContextAction";
        myContextItem.Click += (s, e) => myOutputLabel.Text = "ContextActionClicked";
        myContextMenu.Items.Add(myContextItem);

        myStatusStrip.Name = "myStatusStrip";
        myStatusLabel.Name = "myStatusLabel";
        myStatusLabel.Text = "Ready";
        myStatusStrip.Items.Add(myStatusLabel);

        myOutputLabel.Location = new Point(10, 50);
        myOutputLabel.Name = "myOutputLabel";
        myOutputLabel.Text = "Initial";
        myOutputLabel.ContextMenuStrip = myContextMenu;

        myContainer.TopToolStripPanel.Controls.Add(myToolStrip);
        myContainer.BottomToolStripPanel.Controls.Add(myStatusStrip);
        myContainer.ContentPanel.Controls.Add(myOutputLabel);

        ClientSize = new Size(300, 200);
        Controls.Add(myContainer);
        Controls.Add(myMenuStrip);
        MainMenuStrip = myMenuStrip;
        Name = "ToolStripExtendedTestForm";
        Text = "ToolStripExtendedTestForm";
        ResumeLayout(false);
    }
}
