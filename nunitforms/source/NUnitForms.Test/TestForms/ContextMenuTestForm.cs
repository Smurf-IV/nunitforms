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

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NUnit.Extensions.Forms.TestApplications.TestForms;

/// <summary>
/// Summary description for ContextMenuTestForm.
/// </summary>
public class ContextMenuTestForm : Form
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private readonly Container? components = null;

    private MenuItem menuItem1;

    private MenuItem menuItem2;

    private MenuItem menuItem3;

    private MenuItem menuItem4;

    private MenuItem menuItem5;

    private MenuItem menuItem6;

    private MenuItem menuItem7;

    private MenuItem menuItem8;
    private Label myCounterLabel;
    private MenuItem myFirstMenuItem;
    private Label myLabel;
    private ContextMenu myLabelContextMenu;
    private ContextMenu secondMenu;

    public ContextMenuTestForm()
    {
        //
        // Required for Windows Form Designer support
        //
        InitializeComponent();

        //
        // TODO: Add any constructor code after InitializeComponent call
        //
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (components != null)
            {
                components.Dispose();
            }
        }
        base.Dispose(disposing);
    }

    private void myFirstMenuItem_Click(object sender, EventArgs e)
    {
        int i = int.Parse(myCounterLabel.Text) + 1;
        myCounterLabel.Text = i.ToString();
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        myLabel = new Label();
        myLabelContextMenu = new ContextMenu();
        myFirstMenuItem = new MenuItem();
        menuItem1 = new MenuItem();
        menuItem2 = new MenuItem();
        menuItem3 = new MenuItem();
        menuItem5 = new MenuItem();
        menuItem4 = new MenuItem();
        menuItem6 = new MenuItem();
        myCounterLabel = new Label();
        secondMenu = new ContextMenu();
        menuItem7 = new MenuItem();
        menuItem8 = new MenuItem();
        SuspendLayout();
        // 
        // myLabel
        // 
        myLabel.ContextMenu = myLabelContextMenu;
        myLabel.Location = new Point(32, 40);
        myLabel.Name = "myLabel";
        myLabel.Size = new Size(100, 48);
        myLabel.TabIndex = 0;
        myLabel.Text = "Right click me for a context menu";
        // 
        // myLabelContextMenu
        // 
        myLabelContextMenu.MenuItems.AddRange(
            new MenuItem[]
                {myFirstMenuItem, menuItem1, menuItem2, menuItem3, menuItem4});
        // 
        // myFirstMenuItem
        // 
        myFirstMenuItem.Index = 0;
        myFirstMenuItem.Text = "Click To Count";
        myFirstMenuItem.Click += new EventHandler(myFirstMenuItem_Click);
        // 
        // menuItem1
        // 
        menuItem1.Index = 1;
        menuItem1.Text = "Ambiguous";
        // 
        // menuItem2
        // 
        menuItem2.Index = 2;
        menuItem2.Text = "Ambiguous";
        // 
        // menuItem3
        // 
        menuItem3.Index = 3;
        menuItem3.MenuItems.AddRange(new MenuItem[] {menuItem5});
        menuItem3.Text = "Test 1";
        // 
        // menuItem5
        // 
        menuItem5.Index = 0;
        menuItem5.Text = "Not Ambiguous";
        // 
        // menuItem4
        // 
        menuItem4.Index = 4;
        menuItem4.MenuItems.AddRange(new MenuItem[] {menuItem6});
        menuItem4.Text = "Test 2";
        // 
        // menuItem6
        // 
        menuItem6.Index = 0;
        menuItem6.Text = "Not Ambiguous";
        // 
        // myCounterLabel
        // 
        myCounterLabel.ContextMenu = secondMenu;
        myCounterLabel.Location = new Point(184, 48);
        myCounterLabel.Name = "myCounterLabel";
        myCounterLabel.TabIndex = 1;
        myCounterLabel.Text = "0";
        // 
        // secondMenu
        // 
        secondMenu.MenuItems.AddRange(new MenuItem[] {menuItem7});
        // 
        // menuItem7
        // 
        menuItem7.Index = 0;
        menuItem7.MenuItems.AddRange(new MenuItem[] {menuItem8});
        menuItem7.Text = "Test 2";
        // 
        // menuItem8
        // 
        menuItem8.Index = 0;
        menuItem8.Text = "Not Ambiguous";
        // 
        // ContextMenuTestForm
        // 
        AutoScaleDimensions = new SizeF(5, 13);
        ClientSize = new Size(336, 149);
        Controls.Add(myCounterLabel);
        Controls.Add(myLabel);
        Name = "ContextMenuTestForm";
        Text = "ContextMenuTestForm";
        ResumeLayout(false);
    }

    #endregion
}