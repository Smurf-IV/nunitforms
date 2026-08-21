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
/// Summary description for TabControlForm.
/// </summary>
public class TabControlTestForm : Form
{
    private Button button1;

    private Button button2;

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private readonly Container? components = null;

    private Label label1;
    private Label label2;
    private TabControl myTabs;

    private TabPage page1;

    private TabPage page2;

    public TabControlTestForm()
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

    private void button2_Click(object sender, EventArgs e)
    {
        IncrementLabel(label2);
    }

    private void button1_Click(object sender, EventArgs e)
    {
        IncrementLabel(label1);
    }

    private void IncrementLabel(Label label)
    {
        int i = int.Parse(label.Text) + 1;
        label.Text = i.ToString();
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        myTabs = new TabControl();
        page1 = new TabPage();
        page2 = new TabPage();
        button1 = new Button();
        label1 = new Label();
        button2 = new Button();
        label2 = new Label();
        myTabs.SuspendLayout();
        page1.SuspendLayout();
        page2.SuspendLayout();
        SuspendLayout();
        // 
        // myTabs
        // 
        myTabs.Controls.Add(page1);
        myTabs.Controls.Add(page2);
        myTabs.Location = new Point(16, 16);
        myTabs.Name = "myTabs";
        myTabs.SelectedIndex = 0;
        myTabs.Size = new Size(232, 96);
        myTabs.TabIndex = 0;
        // 
        // page1
        // 
        page1.Controls.Add(label1);
        page1.Controls.Add(button1);
        page1.Location = new Point(4, 22);
        page1.Name = "page1";
        page1.Size = new Size(224, 70);
        page1.TabIndex = 0;
        page1.Text = "page1";
        // 
        // page2
        // 
        page2.Controls.Add(label2);
        page2.Controls.Add(button2);
        page2.Location = new Point(4, 22);
        page2.Name = "page2";
        page2.Size = new Size(224, 70);
        page2.TabIndex = 1;
        page2.Text = "page2";
        // 
        // button1
        // 
        button1.Location = new Point(8, 24);
        button1.Name = "button1";
        button1.TabIndex = 0;
        button1.Text = "button1";
        button1.Click += new EventHandler(button1_Click);
        // 
        // label1
        // 
        label1.Location = new Point(96, 24);
        label1.Name = "label1";
        label1.TabIndex = 1;
        label1.Text = "0";
        // 
        // button2
        // 
        button2.Location = new Point(8, 24);
        button2.Name = "button2";
        button2.TabIndex = 0;
        button2.Text = "button2";
        button2.Click += new EventHandler(button2_Click);
        // 
        // label2
        // 
        label2.Location = new Point(104, 24);
        label2.Name = "label2";
        label2.TabIndex = 1;
        label2.Text = "0";
        // 
        // TabControlForm
        // 
        AutoScaleDimensions = new SizeF(5, 13);
        ClientSize = new Size(264, 133);
        Controls.Add(myTabs);
        Name = "TabControlForm";
        Text = "TabControlForm";
        myTabs.ResumeLayout(false);
        page1.ResumeLayout(false);
        page2.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion
}