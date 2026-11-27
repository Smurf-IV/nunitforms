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

using System;
using System.Drawing;
using System.Windows.Forms;

namespace NUnit.WinForms.Test.TestForms;

public class DateTimePickerTestForm : Form
{
    private DateTimePicker myDatePicker = null!;
    private Label myLabel = null!;

    public DateTimePickerTestForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        myDatePicker = new DateTimePicker();
        myLabel = new Label();
        SuspendLayout();

        myDatePicker.Location = new Point(10, 10);
        myDatePicker.Name = "myDatePicker";
        myDatePicker.Value = new DateTime(2026, 1, 1);
        myDatePicker.ValueChanged += MyDatePicker_ValueChanged;

        myLabel.Location = new Point(10, 40);
        myLabel.Name = "myLabel";
        myLabel.Text = myDatePicker.Value.ToShortDateString();

        ClientSize = new Size(250, 80);
        Controls.Add(myDatePicker);
        Controls.Add(myLabel);
        Name = "DateTimePickerTestForm";
        Text = "DateTimePickerTestForm";
        ResumeLayout(false);
    }

    private void MyDatePicker_ValueChanged(object? sender, EventArgs e)
    {
        myLabel.Text = myDatePicker.Value.ToShortDateString();
    }
}
