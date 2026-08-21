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
/// Summary description for RadioButtonTestForm.
/// </summary>
public class RadioButtonTestForm : Form
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private readonly Container? components = null;

    private GroupBox grpColors;

    private Label lblSelectedColor;

    private RadioButton rbBlue;
    private RadioButton rbGreen;

    private RadioButton rbIndigo;
    private RadioButton rbOrange;
    private RadioButton rbRed;

    private RadioButton rbViolet;
    private RadioButton rbYellow;

    public RadioButtonTestForm()
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

    private void rb_CheckedChanged(object sender, EventArgs e)
    {
        var rb = (RadioButton)sender;
        lblSelectedColor.Text = (string)rb.Tag;
    }

    private void RadioButtonTestForm_Load(object sender, EventArgs e)
    {
        rbRed.Tag = "Red";
        rbOrange.Tag = "Orange";
        rbGreen.Tag = "Green";
        rbYellow.Tag = "Yellow";
        rbBlue.Tag = "Blue";
        rbIndigo.Tag = "Indigo";
        rbViolet.Tag = "Violet";
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        grpColors = new GroupBox();
        rbViolet = new RadioButton();
        rbIndigo = new RadioButton();
        rbBlue = new RadioButton();
        rbYellow = new RadioButton();
        rbGreen = new RadioButton();
        rbOrange = new RadioButton();
        rbRed = new RadioButton();
        lblSelectedColor = new Label();
        grpColors.SuspendLayout();
        SuspendLayout();
        // 
        // grpColors
        // 
        grpColors.Controls.Add(rbViolet);
        grpColors.Controls.Add(rbIndigo);
        grpColors.Controls.Add(rbBlue);
        grpColors.Controls.Add(rbYellow);
        grpColors.Controls.Add(rbGreen);
        grpColors.Controls.Add(rbOrange);
        grpColors.Controls.Add(rbRed);
        grpColors.Location = new Point(8, 16);
        grpColors.Name = "grpColors";
        grpColors.Size = new Size(272, 248);
        grpColors.TabIndex = 0;
        grpColors.TabStop = false;
        grpColors.Text = "Colors";
        // 
        // rbViolet
        // 
        rbViolet.Location = new Point(8, 208);
        rbViolet.Name = "rbViolet";
        rbViolet.TabIndex = 6;
        rbViolet.Text = "Violet";
        rbViolet.Click += new EventHandler(rb_CheckedChanged);
        // 
        // rbIndigo
        // 
        rbIndigo.Location = new Point(8, 176);
        rbIndigo.Name = "rbIndigo";
        rbIndigo.TabIndex = 5;
        rbIndigo.Text = "Indigo";
        rbIndigo.Click += new EventHandler(rb_CheckedChanged);
        // 
        // rbBlue
        // 
        rbBlue.Location = new Point(8, 144);
        rbBlue.Name = "rbBlue";
        rbBlue.TabIndex = 4;
        rbBlue.Text = "Blue";
        rbBlue.Click += new EventHandler(rb_CheckedChanged);
        // 
        // rbYellow
        // 
        rbYellow.Location = new Point(8, 112);
        rbYellow.Name = "rbYellow";
        rbYellow.TabIndex = 3;
        rbYellow.Text = "Yellow";
        rbYellow.Click += new EventHandler(rb_CheckedChanged);
        // 
        // rbGreen
        // 
        rbGreen.Location = new Point(8, 80);
        rbGreen.Name = "rbGreen";
        rbGreen.TabIndex = 2;
        rbGreen.Text = "Green";
        rbGreen.Click += new EventHandler(rb_CheckedChanged);
        // 
        // rbOrange
        // 
        rbOrange.Location = new Point(8, 48);
        rbOrange.Name = "rbOrange";
        rbOrange.TabIndex = 1;
        rbOrange.Text = "Orange";
        rbOrange.Click += new EventHandler(rb_CheckedChanged);
        // 
        // rbRed
        // 
        rbRed.Location = new Point(8, 16);
        rbRed.Name = "rbRed";
        rbRed.TabIndex = 0;
        rbRed.Text = "Red";
        rbRed.CheckedChanged += new EventHandler(rb_CheckedChanged);
        // 
        // lblSelectedColor
        // 
        lblSelectedColor.Location = new Point(8, 288);
        lblSelectedColor.Name = "lblSelectedColor";
        lblSelectedColor.TabIndex = 1;
        // 
        // RadioButtonTestForm
        // 
        AutoScaleDimensions = new SizeF(6, 15);
        ClientSize = new Size(292, 336);
        Controls.Add(lblSelectedColor);
        Controls.Add(grpColors);
        Name = "RadioButtonTestForm";
        Text = "RadioButtonTestForm";
        Load += new EventHandler(RadioButtonTestForm_Load);
        grpColors.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion
}