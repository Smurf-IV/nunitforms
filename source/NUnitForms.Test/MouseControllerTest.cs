#region Copyright (c) 2003-2005, Luke T. Maxon : (Richard Schneider is the original author) : 2026-2026 Smurf.IV

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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using NUnit.Framework;
using NUnit.WinForms.Exceptions;
using NUnit.WinForms.Test.TestForms;
using NUnit.WinForms.Testers;

using TextBoxTester = NUnit.WinForms.Testers.TextBoxTester;

namespace NUnit.WinForms.Test;

/// <summary>
///   Tests the <see cref="MouseController"/>.
/// </summary>
[TestFixture]
[Apartment(ApartmentState.STA)]
[NonParallelizable]
[Category("DisplayHidden")]
[Category("ControlsMouse")]
[Explicit("Requires a stable interactive desktop; exercises global mouse and keyboard state.")]
public class MouseControllerTest : NUnitFormTest
{
    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int virtualKey);

    private Form CurrentForm;

    public override void Setup()
    {
        CurrentForm = new TextBoxTestForm();
        CurrentForm.Show();
    }

    [TearDown]
    public override void TearDown()
    {
        Mouse.Dispose();
        CurrentForm?.Dispose();
        base.TearDown();
    }

    public override bool DisplayHidden => true;

    private int enter;

    private int leave;

    private int move;

    private void OnMouseEnter(object sender, EventArgs e)
    {
        ++enter;
    }

    private void OnMouseLeave(object sender, EventArgs e)
    {
        ++leave;
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        ++move;
    }

    private int hover;


    private int up;

    private int down;

    private void OnMouseDown(object sender, MouseEventArgs e)
    {
        ++down;
    }

    private void OnMouseUp(object sender, MouseEventArgs e)
    {
        ++up;
    }

    private int click;

    private void OnClick(object sender, EventArgs e)
    {
        ++click;
    }

    private int doubleClick;

    private void OnDoubleClick(object sender, EventArgs e)
    {
        ++doubleClick;
    }

    private int drag;

    private int lastX;

    private int lastY;

    private void OnDrag(object sender, MouseEventArgs e)
    {
        if (lastX != e.X || lastY != e.Y)
        {
            if (lastX >= 0 && lastY >= 0)
            {
                Assert.AreEqual(lastX + 1, e.X);
                Assert.AreEqual(lastY + 1, e.Y);
            }
            ++drag;
            lastX = e.X;
            lastY = e.Y;
        }
    }

    [Test]
    public void ButtonEvents()
    {
        using var formTester = new FormTester(CurrentForm.Name);
        Mouse.UseOn(formTester);
        CurrentForm.MouseDown += OnMouseDown;
        CurrentForm.MouseUp += OnMouseUp;

        down = 0;
        up = 0;
        Mouse.Press(MouseButtons.Left);
        Assert.AreEqual(1, down);
        Assert.AreEqual(0, up);

        Mouse.Release(MouseButtons.Left);
        Assert.AreEqual(1, down);
        Assert.AreEqual(1, up);
    }

    [Test]
    public void Buttons()
    {
        using var formTester = new FormTester(CurrentForm.Name);
        Mouse.UseOn(formTester);
        MouseButtons downButton = MouseButtons.None;
        MouseButtons upButton = MouseButtons.None;
        CurrentForm.MouseDown += (_, e) => downButton = e.Button;
        CurrentForm.MouseUp += (_, e) => upButton = e.Button;

        Mouse.Press(MouseButtons.Left);
        Assert.AreEqual(MouseButtons.Left, downButton);

        Mouse.Release(MouseButtons.Left);
        Assert.AreEqual(MouseButtons.Left, upButton);

        downButton = MouseButtons.None;
        Mouse.Press(MouseButtons.Right);
        if (downButton == MouseButtons.None)
        {
            Assert.Ignore("The test desktop does not deliver right-button input.");
        }
        Assert.AreEqual(MouseButtons.Right, downButton);

        upButton = MouseButtons.None;
        Mouse.Release(MouseButtons.Right);
        Assert.AreEqual(MouseButtons.Right, upButton);

        downButton = MouseButtons.None;
        Mouse.Press(MouseButtons.Middle);
        if (downButton == MouseButtons.None)
        {
            Assert.Ignore("The test desktop does not deliver middle-button input.");
        }
        Assert.AreEqual(MouseButtons.Middle, downButton);

        upButton = MouseButtons.None;
        Mouse.Release(MouseButtons.Middle);
        Assert.AreEqual(MouseButtons.Middle, upButton);
    }

    [Test]
    public void Clicking()
    {
        var textBox = new TextBoxTester("myTextBox", CurrentForm);
        textBox.Properties.Click += OnClick;
        textBox.Properties.MouseDown += OnMouseDown;
        textBox.Properties.MouseUp += OnMouseUp;
        Mouse.UseOn(textBox);
        down = 0;
        up = 0;
        click = 0;
        Mouse.Click(3, 1);
        Assert.AreEqual(1, down);
        Assert.AreEqual(1, up);
        Assert.AreEqual(1, click);
    }

    [Test]
    public void Disposing()
    {
        Point originalPosition = Control.MousePosition;

        var textBox = new TextBoxTester("myTextBox", CurrentForm);
        Mouse.UseOn(textBox);
        try
        {
            Mouse.Position = new PointF(10, 10);
            Mouse.Press(MouseButtons.Middle, 10, 10);
            Mouse.Press(Keys.Shift);

            Assert.AreEqual(textBox.Properties.PointToScreen(new Point(10, 10)), Control.MousePosition);
            Assert.AreEqual(MouseButtons.Middle, Control.MouseButtons);
            Assert.AreEqual(Keys.Shift, ModifierKeysDown());
        }
        finally
        {
            Mouse.Dispose();
        }

        Assert.AreEqual(originalPosition, Control.MousePosition);
        Assert.AreEqual(MouseButtons.None, Control.MouseButtons);
        Assert.AreEqual(Keys.None, ModifierKeysDown());
    }

    [Test]
    public void DoubleClicking()
    {
        var textBox = new TextBoxTester("myTextBox", CurrentForm);
        textBox.Properties.DoubleClick += OnDoubleClick;
        doubleClick = 0;
        Mouse.UseOn(textBox);
        Mouse.DoubleClick(1, 3);

        Assert.AreEqual(1, doubleClick);
    }

    [Test]
    public void DragEmpty()
    {
        using var formTester = new FormTester(CurrentForm.Name);
        Assert.Throws<ArgumentException>(() => Mouse.Drag(new PointF(0, 0)));
    }

    [Test]
    public void Dragging()
    {
        using var formTester = new FormTester(CurrentForm.Name);
        CurrentForm.MouseMove += OnDrag;
        Mouse.UseOn(formTester);

        drag = 0;
        lastX = -1;
        lastY = -1;
        Mouse.Drag(new PointF(1, 1), new PointF(1, 1), new PointF(2, 2));
        Assert.AreEqual(3, drag);

        drag = 0;
        lastX = -1;
        lastY = -1;
        Mouse.Drag(0, 0, 1, 1, 2, 2);
        Assert.AreEqual(3, drag);
    }

    [Test]
    public void DragNull()
    {
        using var formTester = new FormTester(CurrentForm.Name);
        Assert.Throws<ArgumentNullException>(() => Mouse.Drag(new PointF(0, 0), null));
    }

    [Test]
    public void DragOdd()
    {
        using var formTester = new FormTester(CurrentForm.Name);
        Assert.Throws<ArgumentException>(() => Mouse.Drag(0, 0, 100, 100, 200));
    }

    [Test]
    public void Hovering()
    {
        var textBox = new TextBoxTester("myTextBox", CurrentForm);
        Mouse.UseOn(textBox);
        textBox.Properties.MouseHover += delegate { ++hover; };
        hover = 0;

        Mouse.Hover();
        Assert.AreEqual(1, hover);

        Mouse.Hover();
        Assert.AreEqual(2, hover);

        Mouse.Hover(1, 2);
        Assert.AreEqual(3, hover);
        Assert.AreEqual(new Point(1, 2), textBox.Properties.PointToClient(Control.MousePosition));

        Mouse.Hover(new PointF(1, 3));
        Assert.AreEqual(4, hover);
        Assert.AreEqual(new Point(1, 3), textBox.Properties.PointToClient(Control.MousePosition));
    }

    [Test]
    public void MissingControl()
    {
        Assert.Throws<NoSuchControlException>(() => Mouse.UseOn(new ControlTester("unknownTextBox")));
    }

    [Test]
    public void Modifiers()
    {
        using var formTester = new FormTester(CurrentForm.Name);

        try
        {
            Mouse.Press(Keys.Alt);
            Assert.AreEqual(Keys.Alt, ModifierKeysDown());
            Mouse.Release(Keys.Alt);
            Assert.AreEqual(Keys.None, ModifierKeysDown());

            Mouse.Press(Keys.Shift);
            Assert.AreEqual(Keys.Shift, ModifierKeysDown());
            Mouse.Release(Keys.Shift);
            Assert.AreEqual(Keys.None, ModifierKeysDown());

            Mouse.Press(Keys.Control);
            Assert.AreEqual(Keys.Control, ModifierKeysDown());
            Mouse.Release(Keys.Control);
            Assert.AreEqual(Keys.None, ModifierKeysDown());

            Mouse.Press(Keys.Control | Keys.Alt | Keys.Shift);
            Assert.AreEqual(Keys.Control | Keys.Alt | Keys.Shift, ModifierKeysDown());
            Mouse.Release(Keys.Control | Keys.Alt | Keys.Shift);
            Assert.AreEqual(Keys.None, ModifierKeysDown());
        }
        finally
        {
            Keys modifiers = ModifierKeysDown();
            if (modifiers != Keys.None)
            {
                Mouse.Release(modifiers);
            }
        }
    }

    private static Keys ModifierKeysDown()
    {
        Keys modifiers = Keys.None;
        if (GetAsyncKeyState((int)Keys.Menu) < 0)
        {
            modifiers |= Keys.Alt;
        }
        if (GetAsyncKeyState((int)Keys.ShiftKey) < 0)
        {
            modifiers |= Keys.Shift;
        }
        if (GetAsyncKeyState((int)Keys.ControlKey) < 0)
        {
            modifiers |= Keys.Control;
        }
        return modifiers;
    }

    [Test]
    public void ModifiersInvalid1()
    {
        using var formTester = new FormTester(CurrentForm.Name);
        Assert.Throws<ArgumentOutOfRangeException>(() => Mouse.Press(Keys.A));
    }

    [Test]
    public void ModifiersInvalid2()
    {
        using var formTester = new FormTester(CurrentForm.Name);
        Assert.Throws<ArgumentOutOfRangeException>(() => Mouse.Release(Keys.A));
    }

    [Test]
    public void PositioningEvents()
    {
        var textBox = new TextBoxTester("myTextBox", CurrentForm);
        Mouse.UseOn(textBox);
        textBox.Properties.MouseEnter += OnMouseEnter;
        textBox.Properties.MouseLeave += OnMouseLeave;
        textBox.Properties.MouseMove += OnMouseMove;

        enter = 0;
        leave = 0;
        move = 0;

        Mouse.Position = new PointF(1, 1);
        Assert.AreEqual(1, enter);
        Assert.AreEqual(0, leave);
        Assert.AreEqual(1, move);

        Mouse.Position = new PointF(1, 2);
        Assert.AreEqual(1, enter);
        Assert.AreEqual(0, leave);
        Assert.AreEqual(2, move);

        Mouse.Position = (Point)textBox.Properties.Size + new Size(1, 1);
        Assert.AreEqual(1, enter);
        Assert.AreEqual(1, leave);
        Assert.AreEqual(2, move);
    }

    [Test]
    public void PositioningInControl()
    {
        var textBox = new TextBoxTester("myTextBox", CurrentForm);
        Mouse.UseOn(textBox);

        Mouse.Position = new PointF(1, 1);
        Assert.AreEqual(new PointF(1, 1), Mouse.Position);

        Assert.AreEqual(textBox.Properties.PointToScreen(new Point(1, 1)), Control.MousePosition);
    }

    [Test]
    public void PositioningInForm()
    {
        using var formTester = new FormTester(CurrentForm.Name);
        Mouse.UseOn(formTester);
        Mouse.Position = new PointF(10, 10);
        Assert.AreEqual(new PointF(10, 10), Mouse.Position);
        Assert.AreEqual(CurrentForm.PointToScreen(new Point(10, 10)), Control.MousePosition);
    }

    [Test]
    public void PositioningUnits()
    {
        using var formTester = new FormTester(CurrentForm.Name);
        Mouse.UseOn(formTester);
        int dpiX, dpiY;
        using (Graphics graphics = CurrentForm.CreateGraphics())
        {
            dpiX = (int)graphics.DpiX;
            dpiY = (int)graphics.DpiY;
        }
        Point targetPoint = CurrentForm.PointToScreen(new Point(dpiX, 2 * dpiY));

        Mouse.PositionUnit = GraphicsUnit.Pixel;
        Mouse.Position = new PointF(10, 10);
        Assert.AreEqual(new PointF(10, 10), Mouse.Position);
        Assert.AreEqual(CurrentForm.PointToScreen(new Point(10, 10)), Control.MousePosition);

        Mouse.PositionUnit = GraphicsUnit.Display;
        Mouse.Position = new PointF(75, 150);
        Assert.AreEqual(new PointF(75, 150), Mouse.Position);
        Assert.AreEqual(targetPoint, Control.MousePosition);

        Mouse.PositionUnit = GraphicsUnit.Document;
        Mouse.Position = new PointF(300, 600);
        Assert.AreEqual(new PointF(300, 600), Mouse.Position);
        Assert.AreEqual(targetPoint, Control.MousePosition);

        Mouse.PositionUnit = GraphicsUnit.Inch;
        Mouse.Position = new PointF(1, 2);
        Assert.AreEqual(new PointF(1, 2), Mouse.Position);
        Assert.AreEqual(targetPoint, Control.MousePosition);

        Mouse.PositionUnit = GraphicsUnit.Point;
        Mouse.Position = new PointF(72, 144);
        Assert.AreEqual(new PointF(72, 144), Mouse.Position);
        Assert.AreEqual(targetPoint, Control.MousePosition);

        Mouse.PositionUnit = GraphicsUnit.Millimeter;
        Mouse.Position = new PointF(25.40f, 50.80F);
        Assert.AreEqual(new PointF(25.40f, 50.80F), Mouse.Position);
        Assert.AreEqual(targetPoint, Control.MousePosition);
    }

    [Test]
    public void PositioningUnitsWorld()
    {
        using var formTester = new FormTester(CurrentForm.Name);
        Mouse.UseOn(formTester);
        Assert.Throws<NotSupportedException>(() => Mouse.PositionUnit = GraphicsUnit.World);
    }

    [Test]
    public void XButtonEvents()
    {
        using var formTester = new FormTester(CurrentForm.Name);
        Mouse.UseOn(formTester);
        CurrentForm.MouseDown += OnMouseDown;
        CurrentForm.MouseUp += OnMouseUp;
        down = 0;
        up = 0;
        if (SystemInformation.MouseButtons > 3)
        {
            Mouse.Press(MouseButtons.XButton1);
            Assert.AreEqual(1, down);
            Assert.AreEqual(0, up);
            Mouse.Release(MouseButtons.XButton1);
            Assert.AreEqual(1, down);
            Assert.AreEqual(1, up);
        }

        if (SystemInformation.MouseButtons > 4)
        {
            down = 0;
            up = 0;
            Mouse.Press(MouseButtons.XButton2);
            if (down == 0)
            {
                Assert.Ignore("The test desktop does not deliver XButton2 input.");
            }
            Assert.AreEqual(1, down);
            Assert.AreEqual(0, up);
            Mouse.Release(MouseButtons.XButton2);
            Assert.AreEqual(1, down);
            Assert.AreEqual(1, up);
        }
    }

    [Test]
    public void XButtons()
    {
        using var formTester = new FormTester(CurrentForm.Name);
        Mouse.UseOn(formTester);
        if (SystemInformation.MouseButtons > 3)
        {
            Mouse.Press(MouseButtons.XButton1);
            Assert.AreEqual(MouseButtons.XButton1, Control.MouseButtons);
        }
        if (SystemInformation.MouseButtons > 4)
        {
            Mouse.Press(MouseButtons.XButton2);
            if ((Control.MouseButtons & MouseButtons.XButton2) == 0)
            {
                Assert.Ignore("The test desktop does not deliver XButton2 input.");
            }
            Assert.AreEqual(MouseButtons.XButton1 | MouseButtons.XButton2, Control.MouseButtons);
            Mouse.Release(MouseButtons.XButton2);
            Assert.AreEqual(MouseButtons.XButton1, Control.MouseButtons);
        }
        if (SystemInformation.MouseButtons > 3)
        {
            Mouse.Release(MouseButtons.XButton1);
            Assert.AreEqual(MouseButtons.None, Control.MouseButtons);
        }
    }
}