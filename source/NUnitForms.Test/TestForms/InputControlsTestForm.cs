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

public class InputControlsTestForm : Form
{
    private MaskedTextBox myMasked = null!;
    private MonthCalendar myCalendar = null!;
    private PictureBox myPicture = null!;
    private PropertyGrid myPropertyGrid = null!;
    private PrintPreviewControl myPrintPreview = null!;
    private Label myLabel = null!;

    public InputControlsTestForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        myMasked = new MaskedTextBox();
        myCalendar = new MonthCalendar();
        myPicture = new PictureBox();
        myPropertyGrid = new PropertyGrid();
        myPrintPreview = new PrintPreviewControl();
        myLabel = new Label();
        SuspendLayout();

        myMasked.Location = new Point(10, 10);
        myMasked.Name = "myMasked";
        myMasked.Culture = System.Globalization.CultureInfo.InvariantCulture;
        myMasked.Mask = "00/00/0000";
        myMasked.Text = "11192026";
        myMasked.TextChanged += (s, e) => myLabel.Text = $"Masked:{myMasked.Text}";

        myCalendar.Location = new Point(10, 40);
        myCalendar.Name = "myCalendar";
        myCalendar.SetDate(new DateTime(2026, 11, 19));
        myCalendar.DateChanged += (s, e) => myLabel.Text = $"Calendar:{myCalendar.SelectionStart.ToShortDateString()}";

        myPicture.Location = new Point(220, 10);
        myPicture.Name = "myPicture";
        myPicture.Size = new Size(50, 50);
        myPicture.Click += (s, e) => myLabel.Text = "PictureClicked";

        myPropertyGrid.Location = new Point(220, 70);
        myPropertyGrid.Name = "myPropertyGrid";
        myPropertyGrid.Size = new Size(100, 100);

        myPrintPreview.Location = new Point(10, 210);
        myPrintPreview.Name = "myPrintPreview";
        myPrintPreview.Size = new Size(100, 50);

        myLabel.Location = new Point(120, 210);
        myLabel.Name = "myLabel";
        myLabel.Text = "Initial";

        ClientSize = new Size(340, 280);
        Controls.Add(myMasked);
        Controls.Add(myCalendar);
        Controls.Add(myPicture);
        Controls.Add(myPropertyGrid);
        Controls.Add(myPrintPreview);
        Controls.Add(myLabel);
        Name = "InputControlsTestForm";
        Text = "InputControlsTestForm";
        ResumeLayout(false);
    }
}
