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

#if NETFRAMEWORK  // https://github.com/dotnet/designs/blob/main/accepted/2020/net5/net5.md#preprocessor-symbols

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NUnit.Extensions.Forms.TestApplications.TestForms;

/// <summary>
/// Summary description for MainMenuTestForm.
/// </summary>
public class MainMenuTestForm : Form
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private readonly Container? components = null;

    private Label label;

    private MainMenu mainMenu1;

    private MenuItem menuItem1;

    private MenuItem menuItem2;

    private MenuItem menuItem3;

    private MenuItem menuItem4;

    private MenuItem menuItem5;

    private MenuItem menuItem6;

    public MainMenuTestForm()
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

    private void menuItem_Click(object sender, EventArgs e)
    {
        label.Text = "clicked";
    }

    private void menuItem_Popup(object sender, EventArgs e)
    {
        label.Text = "shown";
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        mainMenu1 = new MainMenu();
        menuItem1 = new MenuItem();
        menuItem6 = new MenuItem();
        menuItem2 = new MenuItem();
        menuItem3 = new MenuItem();
        menuItem4 = new MenuItem();
        menuItem5 = new MenuItem();
        label = new Label();
        SuspendLayout();
        // 
        // mainMenu1
        // 
        mainMenu1.MenuItems.AddRange(new MenuItem[] {menuItem1});
        // 
        // menuItem1
        // 
        menuItem1.Index = 0;
        menuItem1.MenuItems.AddRange(
            new MenuItem[] {menuItem6, menuItem2, menuItem4, menuItem5});
        menuItem1.Text = "Main";
        menuItem1.Popup += new EventHandler(menuItem_Popup);
        // 
        // menuItem6
        // 
        menuItem6.Index = 0;
        menuItem6.Text = "Item";
        menuItem6.Click += new EventHandler(menuItem_Click);
        // 
        // menuItem2
        // 
        menuItem2.Index = 1;
        menuItem2.MenuItems.AddRange(new MenuItem[] {menuItem3});
        menuItem2.Text = "Sub Menu";
        // 
        // menuItem3
        // 
        menuItem3.Index = 0;
        menuItem3.Text = "Sub Menu Item";
        menuItem3.Click += new EventHandler(menuItem_Click);
        // 
        // menuItem4
        // 
        menuItem4.Index = 2;
        menuItem4.Text = "With &Alt Key";
        menuItem4.Click += new EventHandler(menuItem_Click);
        // 
        // menuItem5
        // 
        menuItem5.Index = 3;
        menuItem5.Text = "With Dots...";
        menuItem5.Click += new EventHandler(menuItem_Click);
        // 
        // label
        // 
        label.Location = new Point(88, 40);
        label.Name = "label";
        label.Size = new Size(96, 16);
        label.TabIndex = 0;
        label.Text = "not clicked";
        // 
        // MainMenuTestForm
        // 
        AutoScaleDimensions = new SizeF(5, 13);
        ClientSize = new Size(292, 94);
        Controls.Add(label);
        Menu = mainMenu1;
        Name = "MainMenuTestForm";
        Text = "MainMenuTestForm";
        ResumeLayout(false);
    }

    #endregion
}
#endif 