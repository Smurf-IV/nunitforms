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
/// Summary description for MultiForm.
/// </summary>
public class ModalMultiForm : Form
{
    private Button btnClose;

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private readonly Container? components = null;

    private Button myButton;
    private Label myLabel;
    private Button nothingButton;

    public ModalMultiForm()
    {
        //
        // Required for Windows Form Designer support
        //
        InitializeComponent();

        //
        // TODO: Add any constructor code after InitializeComponent call
        //
        Name = "Form";
        Text = "Form";
    }

    ~ModalMultiForm()
    {
        Dispose(false);
    }


    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        lock (this)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            ////if (Handle != IntPtr.Zero)
            ////{
            //bool closeHandle = Win32.CloseHandle(Handle);
            //System.Console.WriteLine("disposing ModalMultiForm : " + closeHandle);
            ////}
            base.Dispose(disposing);
        }
    }

    private void button1_Click(object sender, EventArgs e)
    {
        int i = int.Parse(myLabel.Text) + 1;
        myLabel.Text = i.ToString();

        var newForm = new ModalMultiForm();
        newForm.Name = $"{Name}-{(i - 1)}";
        newForm.Text = newForm.Name;
        newForm.ShowDialog();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
        Close();
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        myButton = new Button();
        myLabel = new Label();
        nothingButton = new Button();
        btnClose = new Button();
        SuspendLayout();
        // 
        // myButton
        // 
        myButton.Location = new Point(104, 88);
        myButton.Name = "myButton";
        myButton.TabIndex = 0;
        myButton.Text = "Click me";
        myButton.Click += new EventHandler(button1_Click);
        // 
        // myLabel
        // 
        myLabel.Location = new Point(112, 144);
        myLabel.Name = "myLabel";
        myLabel.TabIndex = 1;
        myLabel.Text = "0";
        // 
        // nothingButton
        // 
        nothingButton.Location = new Point(104, 208);
        nothingButton.Name = "nothingButton";
        nothingButton.TabIndex = 2;
        nothingButton.Text = "Nothing";
        // 
        // btnClose
        // 
        btnClose.Location = new Point(200, 208);
        btnClose.Name = "btnClose";
        btnClose.TabIndex = 3;
        btnClose.Text = "Close";
        btnClose.Click += new EventHandler(btnClose_Click);
        // 
        // MultiForm
        // 
        AutoScaleDimensions = new SizeF(5, 13);
        ClientSize = new Size(292, 273);
        Controls.Add(btnClose);
        Controls.Add(nothingButton);
        Controls.Add(myLabel);
        Controls.Add(myButton);
        Name = "MultiForm";
        Text = "MultiForm1";
        ResumeLayout(false);
    }

    #endregion
}