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

using NUnit.WinForms.Test.TestForms;
using NUnit.Framework;
using NUnit.WinForms.Testers;

namespace NUnit.WinForms.Test;

[TestFixture]
public class NumericControlsTest : NUnitFormTest
{
    private NumericUpDownTester numeric = null!;
    private DomainUpDownTester domain = null!;
    private ProgressBarTester progress = null!;
    private TrackBarTester track = null!;
    private HScrollBarTester hScroll = null!;
    private VScrollBarTester vScroll = null!;
    private ScrollBarTester genericScroll = null!;
    private LabelTester label = null!;

    public override void Setup()
    {
        new NumericControlsTestForm().Show();
        numeric = new NumericUpDownTester("myNumeric");
        domain = new DomainUpDownTester("myDomain");
        progress = new ProgressBarTester("myProgress");
        track = new TrackBarTester("myTrack");
        hScroll = new HScrollBarTester("myHScroll");
        vScroll = new VScrollBarTester("myVScroll");
        genericScroll = new ScrollBarTester("myHScroll");
        label = new LabelTester("myLabel");
    }

    [Test]
    public void NumericUpDownTest()
    {
        Assert.AreEqual(10, numeric.Properties.Value);
        numeric.Properties.Value = 25;
        numeric.FireEvent("ValueChanged");
        Assert.AreEqual("Numeric:25", label.Text);
    }

    [Test]
    public void DomainUpDownTest()
    {
        Assert.AreEqual("Alpha", domain.Properties.SelectedItem);
        domain.Properties.SelectedIndex = 1;
        Assert.AreEqual("Domain:Beta", label.Text);
    }

    [Test]
    public void ProgressBarTest()
    {
        Assert.AreEqual(50, progress.Properties.Value);
        progress.Properties.Value = 75;
        Assert.AreEqual(75, progress.Properties.Value);
    }

    [Test]
    public void TrackBarTest()
    {
        Assert.AreEqual(5, track.Properties.Value);
        track.Properties.Value = 8;
        track.FireEvent("Scroll", System.EventArgs.Empty);
        Assert.AreEqual("Track:8", label.Text);
    }

    [Test]
    public void ScrollBarsTest()
    {
        Assert.AreEqual(20, hScroll.Properties.Value);
        hScroll.Properties.Value = 40;
        hScroll.FireEvent("Scroll", new System.Windows.Forms.ScrollEventArgs(System.Windows.Forms.ScrollEventType.ThumbPosition, 40));
        Assert.AreEqual("HScroll:40", label.Text);

        Assert.AreEqual(30, vScroll.Properties.Value);
        vScroll.Properties.Value = 60;
        vScroll.FireEvent("Scroll", new System.Windows.Forms.ScrollEventArgs(System.Windows.Forms.ScrollEventType.ThumbPosition, 60));
        Assert.AreEqual("VScroll:60", label.Text);

        Assert.AreEqual(40, genericScroll.Properties.Value);
    }
}
