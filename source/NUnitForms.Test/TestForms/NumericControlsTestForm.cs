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

public class NumericControlsTestForm : Form
{
    private NumericUpDown myNumeric = null!;
    private DomainUpDown myDomain = null!;
    private ProgressBar myProgress = null!;
    private TrackBar myTrack = null!;
    private HScrollBar myHScroll = null!;
    private VScrollBar myVScroll = null!;
    private Label myLabel = null!;

    public NumericControlsTestForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        myNumeric = new NumericUpDown();
        myDomain = new DomainUpDown();
        myProgress = new ProgressBar();
        myTrack = new TrackBar();
        myHScroll = new HScrollBar();
        myVScroll = new VScrollBar();
        myLabel = new Label();
        SuspendLayout();

        myNumeric.Location = new Point(10, 10);
        myNumeric.Name = "myNumeric";
        myNumeric.Value = 10;
        myNumeric.ValueChanged += (s, e) => myLabel.Text = $"Numeric:{myNumeric.Value}";

        myDomain.Location = new Point(10, 40);
        myDomain.Name = "myDomain";
        myDomain.Items.Add("Alpha");
        myDomain.Items.Add("Beta");
        myDomain.SelectedIndex = 0;
        myDomain.SelectedItemChanged += (s, e) => myLabel.Text = $"Domain:{myDomain.SelectedItem}";

        myProgress.Location = new Point(10, 70);
        myProgress.Name = "myProgress";
        myProgress.Value = 50;

        myTrack.Location = new Point(10, 100);
        myTrack.Name = "myTrack";
        myTrack.Value = 5;
        myTrack.Scroll += (s, e) => myLabel.Text = $"Track:{myTrack.Value}";

        myHScroll.Location = new Point(10, 150);
        myHScroll.Name = "myHScroll";
        myHScroll.Value = 20;
        myHScroll.Scroll += (s, e) => myLabel.Text = $"HScroll:{myHScroll.Value}";

        myVScroll.Location = new Point(180, 10);
        myVScroll.Name = "myVScroll";
        myVScroll.Value = 30;
        myVScroll.Scroll += (s, e) => myLabel.Text = $"VScroll:{myVScroll.Value}";

        myLabel.Location = new Point(10, 180);
        myLabel.Name = "myLabel";
        myLabel.Text = "Initial";

        ClientSize = new Size(220, 220);
        Controls.Add(myNumeric);
        Controls.Add(myDomain);
        Controls.Add(myProgress);
        Controls.Add(myTrack);
        Controls.Add(myHScroll);
        Controls.Add(myVScroll);
        Controls.Add(myLabel);
        Name = "NumericControlsTestForm";
        Text = "NumericControlsTestForm";
        ResumeLayout(false);
    }
}
