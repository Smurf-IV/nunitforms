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
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using NUnit.Extensions.Forms.Exceptions;
using NUnit.Extensions.Forms.Generic_Testers;
using NUnit.Extensions.Forms.Testers;
using NUnit.Extensions.Forms.Util;


namespace NUnit.Extensions.Forms;

/// <summary>
///   Provides testing control over the mouse.
/// </summary>
/// <remarks>
///  <b>MouseController</b> allows you to control the mouse position 
///  and buttons and simulate dragging and clicking during a test.  It can be
///  used to test the behavior of a control.  For testing the behavior of an
///  application in reponse to a control's events, it is not necessary to use
///  the MouseControl.  I suggest you directly fire the appropriate events instead.
///  (You probably aren't using this class unless you are writing some custom
///  control and need to see its response to actual low level mouse movement, etc..)
///  <para>
///  The mouse Position can be relative to a <see cref="Form"/> or a <see cref="Control"/> 
///  on a <b>Form</b>.  The <b>Position</b> can be specified in pixels, inches or millimetres
///  </para>
///  <para>
///  The mouse buttons are controlled with the PressAndRelease and Release methods.  Both
///  methods allow a bitwise combination of the <see cref="MouseButtons"/>, for example 
///  <c>PressAndRelease(MouseButtons.Left | MouseButtons.Center)</c>.
///  Button modifiers, the Alt, Shift and Ctrl keys, are also controlled with the two methods.
///  </para>
///  <para>
///  The <b>XButtons</b> (<see cref="MouseButtons.XButton1"/> and <see cref="MouseButtons.XButton2"/>)
///  can only be simulated when a mouse with <see cref="SystemInformation.MouseButtons">5 buttons</see> is installed. 
///  The <b>PressAndRelease</b> and <b>Release</b> methods will throw <see cref="System.NotSupportedException"/> if the XButtons are used 
///  when a mouse does not have 4 or 5 button support.
///  </para>
/// </remarks>
/// <example>
/// <code>
///[TestFixture] public class ATest : NUnitFormTest
///{
///  // Gets the Form used for testing.
///  public override Type FormType
///  {
///    get {return typeof(MyTestForm);}
///  }
///  
///  [Test] public void Selecting()
///  {
///    ControlTester myControl = new ControlTester("myControl", CurrentForm);
///    using (MouseController mouse = myControl.MouseController())
///    {
///      mouse.Drag (10,10, 20,20);
///      AssertEquals (1, myControl.Properties.SelectedObjects.Count);
///    }
///  }
/// </code>
/// </example>
public class MouseController : IDisposable
{
    private static readonly int s_hoverTime;
    private MouseControl? _mouseControl;

    private Win32.Point _originalPosition;

    private PointF _scale;

    #region Contstructors

    static MouseController()
    {
        Win32.SystemParametersInfo(Win32.SPI_GETMOUSEHOVERTIME, 0, out s_hoverTime, 0);
        s_hoverTime += s_hoverTime / 2;
    }

    /// <summary>
    ///   Creates and initialises a new instance of the class. 
    /// </summary>
    internal MouseController()
    {
    }

    /// <summary>
    ///   Creates and initialises a new instance of the class for the specified tester.
    /// </summary>
    /// <remarks>
    ///   The <see cref="Position">mouse position</see> is relative to the <see cref="Control"/> managed by
    ///   the <paramref name="controlTester"/>.
    ///   <para>
    ///   While the <b>MouseController</b> is active, user keyboard and mouse input is disabled.  For this
    ///   reason the <b>MouseController</b> should be disposed of when the testing is concluded.
    ///   </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// using (MouseController mouse = new MouseController(myTestControl))
    /// {
    ///   mouse.Position = new PointF(1,1);
    ///   mouse.PressAndRelease(MouseButtons.Middle);
    /// }
    /// </code>
    /// </example>
    public MouseController(ReflectionTester controlTester)
    {
        UseOn(controlTester);
    }

    /// <summary>
    /// Specify which control or form we should use for the relative position.
    /// </summary>
    public void UseOn(ReflectionTester control)
    {
        if (_mouseControl == null)
        {
            Win32.GetCursorPos(out _originalPosition);
        }

        ThrowHelper.ThrowIfNull(control, nameof(control));

        _mouseControl = new MouseControl(control);

        PositionUnit = GraphicsUnit.Pixel;

        // Do not block user input globally; it can interfere with activation/click delivery between sequential tests
        // and after modal dialogs. Rely on targeted positioning and focus instead.
    }

    /// <summary>
    /// Overloaded.  Specifies control by name.
    /// </summary>
    /// <param name="name">The name of the control</param>
    public void UseOn(string name)
    {
        UseOn(new ControlTester(name));
    }

    /// <summary>
    /// Overloaded.  Specifies control by name and form name
    /// </summary>
    /// <param name="name">The name of the control.</param>
    /// <param name="formName">The name of the form.</param>
    public void UseOn(string name, string formName)
    {
        UseOn(new ControlTester(name, formName));
    }

    /// <summary>
    /// Overloaded.  Specifies control by name and form instance.
    /// </summary>
    /// <param name="name">The name of the control.</param>
    /// <param name="form">The form instance.</param>
    public void UseOn(string name, Form form)
    {
        UseOn(new ControlTester(name, form));
    }

    #endregion

    #region IDisposable Members

    /// <summary>
    ///   Releases the resources used by the <see cref="MouseController"/>.
    /// </summary>
    /// <remarks>
    ///   <b>Dispose</b> releases any pressed mouse keys, restores the
    ///   mouse <see cref="Position"/> and enables user input.
    /// </remarks>
    public void Dispose()
    {
        if (_mouseControl != null)
        {
            // If we do not have a control, then an exception will be thrown.
            try
            {
                // Release any pressed mouse buttons.
                MouseButtons pressedButtons = Control.MouseButtons;
                if (pressedButtons != MouseButtons.None)
                {
                    Release(pressedButtons);
                }

                // Release any modifier keys.
                Keys keys = Control.ModifierKeys;
                if (keys != Keys.None)
                {
                    Release(keys);
                }
            }
            catch (AmbiguousNameException)
            {
            }
            catch (NoSuchControlException)
            {
            }
            finally
            {
                // Restore the mouse position
                Win32.SetCursorPos(_originalPosition.x, _originalPosition.y);

                // No global input blocking to undo; leave system input state unchanged
            }
        }
    }

    #region Properties

    /// <summary>
    ///   Gets or sets the mouse position.
    /// </summary>
    /// <value>
    ///   A <see cref="PointF"/> representing the mouse position in the <see cref="PositionUnit"/> space.
    /// </value>
    /// <remarks>
    ///   <b>Position</b> is the position of the mouse, relative to the control under test
    ///   and specified in <see cref="PositionUnit">position units</see>.
    /// </remarks>
    public PointF Position
    {
        get
        {
            Win32.GetCursorPos(out Win32.Point p);
            return _mouseControl.Convert(p, _scale);
        }
        set
        {
            _mouseControl.Focus();
            Win32.Point p = _mouseControl.Convert(value, _scale);
            Win32.SetCursorPos(p.x, p.y);
            Application.DoEvents();
        }
    }

    /// <summary>
    ///   Gets or sets the unit of measure used for mouse coordinates.
    /// </summary>
    /// <value>
    ///   A member of the <see cref="GraphicsUnit"/> enumeration.  The default
    ///   is <b>GraphicsUnit.Pixel</b>.
    /// </value>
    /// <remarks>
    ///   <b>PositionUnit</b> specifies how the mouse <see cref="Position"/> coordinates
    ///   are interpreted.
    /// </remarks>
    /// <exception cref="NotSupportedException">
    ///   When value is <see cref="GraphicsUnit">GraphicsUnit.World</see>.
    /// </exception>
    /// <exception cref="InvalidEnumArgumentException">
    ///   When value is not a valid enumeration value.
    /// </exception>
    public GraphicsUnit PositionUnit
    {
        get;
        set
        {
            if (value == GraphicsUnit.Pixel)
            {
                _scale = new PointF(1, 1);
            }
            else
            {
                PointF resolution = _mouseControl.Resolution;

                _scale = value switch
                {
                    GraphicsUnit.Inch => new PointF(resolution.X, resolution.Y),
                    GraphicsUnit.Point => new PointF(resolution.X / 72, resolution.Y / 72),
                    GraphicsUnit.Display => new PointF(resolution.X / 75, resolution.Y / 75),
                    GraphicsUnit.Document => new PointF(resolution.X / 300, resolution.Y / 300),
                    GraphicsUnit.Millimeter => new PointF(resolution.X / 25.40F, resolution.Y / 25.40F),
                    GraphicsUnit.World => ThrowHelper.ThrowNotSupportedException<PointF>("World units not supported."),
                    _ => ThrowHelper.ThrowInvalidEnumArgumentException<PointF>(nameof(value), (int)value, typeof(GraphicsUnit))
                };
            }

            field = value;
        }
    }

    #endregion

    #region Public methods

    /// <summary>
    ///   Simulate hovering over the control under test.
    /// </summary>
    /// <remarks>
    ///   <b>Hover</b> positions the mouse over the control under test for the
    ///   system defined mouse hover time and then <see cref="Application.DoEvents">processes</see> the events.
    /// </remarks>
    public void Hover()
    {
        Hover(PointF.Empty);
    }

    /// <summary>
    ///   Simulate hovering over the control under test at the specified x and y-coordinate.
    /// </summary>
    /// <param name="x">
    ///   The <see cref="float"/> x-coordinate,
    ///   relative to the control under test,
    ///   to <see cref="Position">move</see> the mouse to.
    /// </param>
    /// <param name="y">
    ///   The <see cref="float"/> y-coordinate,
    ///   relative to the control under test,
    ///   to <see cref="Position">move</see> the mouse to.
    /// </param>
    /// <remarks>
    ///   <b>Hover</b> positions the mouse over the control under test 
    ///   at the specified point (<paramref name="x"/>, <paramref name="y"/>) for the system defined mouse hover 
    ///   time and then <see cref="Application.DoEvents">processes</see> the events.
    /// </remarks>
    public void Hover(float x, float y)
    {
        Hover(new PointF(x, y));
    }

    /// <summary>
    ///   Simulate hovering over the control under test at the specified <see cref="PointF"/>.
    /// </summary>
    /// <param name="point">
    ///   A <see cref="PointF"/>, relative to the control under test,
    ///    to <see cref="Position">move</see> the mouse to.
    /// </param>
    /// <remarks>
    ///   <b>Hover</b> positions the mouse over the control under test 
    ///   at the specified <paramref name="point"/> for the system defined mouse hover 
    ///   time and then <see cref="Application.DoEvents">processes</see> the events.
    /// </remarks>
    public void Hover(PointF point)
    {
        // A Hover event is only reported when the position changes.
        Position = new PointF(-100, -100); // TODO: outside of hover rect
        Application.DoEvents(); // Process the hover event.

        Position = point; // Enter the control
        Application.DoEvents(); // Process the hover event.
        Thread.Sleep(s_hoverTime); // Sleep for the hover time.
        Application.DoEvents(); // Process the hover event.
    }

    /// <summary>
    ///   Simulate clicking on the control under test.
    /// </summary>
    /// <remarks>
    ///   <b>Click</b> positions the mouse over the control under test 
    ///   and then presses and releases the left mouse button.
    /// </remarks>
    public void Click(MouseButtons buttons = MouseButtons.Left)
    {
        Click(PointF.Empty, buttons);
    }

    /// <summary>
    ///   Simulate clicking at the specified x and y-coordinate.
    /// </summary>
    /// <param name="x">
    ///     The <see cref="float"/> x-coordinate,
    ///     relative to the control under test,
    ///     to <see cref="Position">move</see> the mouse to.
    /// </param>
    /// <param name="y">
    ///     The <see cref="float"/> y-coordinate,
    ///     relative to the control under test,
    ///     to <see cref="Position">move</see> the mouse to.
    /// </param>
    /// <param name="buttons"></param>
    /// <remarks>
    ///   <b>Click</b> positions the mouse over the control under test 
    ///   at the specified point (<paramref name="x"/>, <paramref name="y"/>)
    ///   and then presses and releases the left mouse button.
    /// </remarks>
    public void Click(float x, float y, MouseButtons buttons = MouseButtons.Left)
    {
        Click(new PointF(x, y), buttons);
    }

    /// <summary>
    ///   Simulate clicking at the specified <see cref="PointF"/>.
    /// </summary>
    /// <param name="point">
    ///     A <see cref="PointF"/>, relative to the control under test,
    ///     to <see cref="Position">move</see> the mouse to.
    /// </param>
    /// <param name="buttons"></param>
    /// <remarks>
    ///   <b>Click</b> positions the mouse over the control under test 
    ///   at the specified <paramref name="point"/>
    ///   and then presses and releases the left mouse button.
    /// </remarks>
    public void Click(PointF point, MouseButtons buttons = MouseButtons.Left)
    {
        Press(buttons, point);
        Release(buttons, point);
    }

    /// <summary>
    ///   Simulate double clicking on the control under test.
    /// </summary>
    /// <remarks>
    ///   <b>Click</b> positions the mouse over the control under test 
    ///   and then presses and releases the left mouse button twice.
    /// </remarks>
    public void DoubleClick(MouseButtons buttons = MouseButtons.Left)
    {
        DoubleClick(PointF.Empty, buttons);
    }

    /// <summary>
    ///   Simulate double clicking at the specified x and y-coordinate.
    /// </summary>
    /// <param name="x">
    ///   The <see cref="float"/> x-coordinate,
    ///   relative to the control under test,
    ///   to <see cref="Position">move</see> the mouse to.
    /// </param>
    /// <param name="y">
    ///   The <see cref="float"/> y-coordinate,
    ///   relative to the control under test,
    ///   to <see cref="Position">move</see> the mouse to.
    /// </param>
    /// <remarks>
    ///   <b>Click</b> positions the mouse over the control under test 
    ///   at the specified point (<paramref name="x"/>, <paramref name="y"/>)
    ///   and then presses and releases the left mouse button twice.
    /// </remarks>
    public void DoubleClick(float x, float y, MouseButtons buttons = MouseButtons.Left)
    {
        DoubleClick(new PointF(x, y), buttons);
    }

    /// <summary>
    ///   Simulate double clicking at the specified <see cref="PointF"/>.
    /// </summary>
    /// <param name="point">
    ///   A <see cref="PointF"/>, relative to the control under test,
    ///   to <see cref="Position">move</see> the mouse to.
    /// </param>
    /// <remarks>
    ///   <b>Click</b> positions the mouse over the control under test 
    ///   at the specified <paramref name="point"/>
    ///   and then presses and releases the left mouse button twice.
    /// </remarks>
    public void DoubleClick(PointF point, MouseButtons buttons = MouseButtons.Left)
    {
        Click(point, buttons);
        Click(point, buttons);
    }

    /// <summary>
    ///   Simulate pressing the mouse button(s).
    /// </summary>
    /// <param name="buttons">
    ///   A bitwise combination of the <see cref="MouseButtons"/> enumeration values. 
    /// </param>
    /// <remarks>
    ///   <b>PressAndRelease</b> positions the mouse over the control under test 
    ///   and then simulates pressing the specified <paramref name="buttons"/>.
    /// </remarks>
    /// <exception cref="NotSupportedException">
    ///   When <paramref name="buttons"/> contains <see cref="MouseButtons">MouseButtons.XButton1</see>
    ///   or <see cref="MouseButtons">MouseButtons.XButton2</see> and the installed mouse does have 4
    ///   or 5 buttons, respectively.
    /// </exception>
    public void Press(MouseButtons buttons)
    {
        Press(buttons, PointF.Empty);
        //Press(buttons, new PointF(1, 1)); 
    }

    /// <summary>
    ///   Simulate pressing the mouse button(s) at the specified  x and y-coordinate.
    /// </summary>
    /// <param name="buttons">
    ///   A bitwise combination of the <see cref="MouseButtons"/> enumeration values. 
    /// </param>
    /// <param name="x">
    ///   The <see cref="float"/> x-coordinate,
    ///   relative to the control under test,
    ///   to <see cref="Position">move</see> the mouse to.
    /// </param>
    /// <param name="y">
    ///   The <see cref="float"/> y-coordinate,
    ///   relative to the control under test,
    ///   to <see cref="Position">move</see> the mouse to.
    /// </param>
    /// <remarks>
    ///   <b>PressAndRelease</b> positions the mouse over the control under test 
    ///   at the specified point (<paramref name="x"/>, <paramref name="y"/>)
    ///   and then simulates pressing the specified <paramref name="buttons"/>.
    /// </remarks>
    /// <exception cref="NotSupportedException">
    ///   When <paramref name="buttons"/> contains <see cref="MouseButtons">MouseButtons.XButton1</see>
    ///   or <see cref="MouseButtons">MouseButtons.XButton2</see> and the installed mouse does have 4
    ///   or 5 buttons, respectively.
    /// </exception>
    public void Press(MouseButtons buttons, float x, float y)
    {
        Press(buttons, new PointF(x, y));
    }

    /// <summary>
    ///   Simulate pressing the mouse button(s) at the specified <see cref="PointF"/>.
    /// </summary>
    /// <param name="buttons">
    ///   A bitwise combination of the <see cref="MouseButtons"/> enumeration values. 
    /// </param>
    /// <param name="point">
    ///   A <see cref="PointF"/>, relative to the control under test,
    ///   to <see cref="Position">move</see> the mouse to.
    /// </param>
    /// <remarks>
    ///   <b>PressAndRelease</b> positions the mouse over the control under test 
    ///   at the specified <paramref name="point"/>
    ///   and then simulates pressing the specified <paramref name="buttons"/>.
    /// </remarks>
    /// <exception cref="NotSupportedException">
    ///   When <paramref name="buttons"/> contains <see cref="MouseButtons">MouseButtons.XButton1</see>
    ///   or <see cref="MouseButtons">MouseButtons.XButton2</see> and the installed mouse does have 4
    ///   or 5 buttons, respectively.
    /// </exception>
    public void Press(MouseButtons buttons, PointF point)
    {
        // Ensure no other window/control holds mouse capture (e.g., after a modal dialog)
        try { Win32.ReleaseCapture(); } catch { /* ignore */ }

        // Always ensure cursor is over the control
        Position = point;

        // Prefer targeted client messages for left-button to avoid foreground/focus issues after modals
        if ((buttons & MouseButtons.Left) != 0)
        {
            int lParam = _mouseControl.MakeLParam(point, _scale);
            // If the form isn't active, emulate mouse-activate handshake so the child can receive the click
            IntPtr formHandle = _mouseControl.FormHandle;
            try
            {
                // Best-effort: send WM_MOUSEACTIVATE to the top-level window
                if (formHandle != IntPtr.Zero)
                {
                    int actParam = ((int)Win32.WM_LBUTTONDOWN << 16) | (Win32.HTCLIENT & 0xFFFF);
                    Win32.SendMessage(formHandle, Win32.WM_MOUSEACTIVATE, _mouseControl.Handle, (IntPtr)actParam);
                    // Indicate activation due to mouse click
                    Win32.SendMessage(formHandle, Win32.WM_ACTIVATE, (IntPtr)Win32.WA_CLICKACTIVE, _mouseControl.Handle);
                    // Ensure the control has focus before clicking
                    Win32.SendMessage(_mouseControl.Handle, Win32.WM_SETFOCUS, IntPtr.Zero, IntPtr.Zero);
                }
            }
            catch { /* ignore */ }
            Win32.SendMessage(_mouseControl.Handle, Win32.WM_MOUSEMOVE, IntPtr.Zero, (IntPtr)lParam);
            Win32.SendMessage(_mouseControl.Handle, Win32.WM_LBUTTONDOWN, (IntPtr)Win32.MK_LBUTTON, (IntPtr)lParam);
            Application.DoEvents();
            return;
        }

        // Fallback to SendInput for other buttons
        var input = new Win32.MSINPUT(0)
        {
            mi =
            {
                dwFlags = 0
            }
        };
        if ((buttons & MouseButtons.Right) != 0)
        {
            input.mi.dwFlags |= Win32.MOUSEEVENTF_RIGHTDOWN;
        }
        if ((buttons & MouseButtons.Middle) != 0)
        {
            input.mi.dwFlags |= Win32.MOUSEEVENTF_MIDDLEDOWN;
        }
        if ((buttons & MouseButtons.XButton1) != 0)
        {
            if (SystemInformation.MouseButtons < 4)
            {
                ThrowHelper.ThrowNotSupportedException("A mouse with at least 4 buttons is required.");
            }
            input.mi.dwFlags |= Win32.MOUSEEVENTF_XDOWN;
            input.mi.mouseData |= Win32.XBUTTON1;
        }
        if ((buttons & MouseButtons.XButton2) != 0)
        {
            if (SystemInformation.MouseButtons < 5)
            {
                ThrowHelper.ThrowNotSupportedException("A mouse with at least 5 buttons is required.");
            }
            input.mi.dwFlags |= Win32.MOUSEEVENTF_XDOWN;
            input.mi.mouseData |= Win32.XBUTTON2;
        }

        if (input.mi.dwFlags != 0)
        {
            if (0 == Win32.SendMouseInput(1, ref input, Marshal.SizeOf(input)))
            {
                ThrowHelper.ThrowWin32Exception();
            }
            Application.DoEvents();
        }
    }

    /// <overloads>
    ///   Simulates pressing the <see cref="MouseButtons"/> or modifier <see cref="Keys"/>.
    /// </overloads>
    /// <summary>
    ///   Simulate pressing the mouse modifier key(s) (Alt, Shift and Control).
    /// </summary>
    /// <param name="keys">
    ///   A bitwise combination of the <see cref="Keys"/> enumeration values. Only <b>Alt</b>, <b>Shift</b>
    ///   and <b>Control</b> are allowed.
    /// </param>
    /// <remarks>
    ///   <b>PressAndRelease</b> simulates pressing the specified <paramref name="keys"/>.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   When <paramref name="keys"/> contains a value that is not 
    ///   <b>Alt</b>, <b>Shift</b> or <b>Control</b>.
    /// </exception>
    /// <example>
    ///   The following example performs a "shift drag" and verifies that
    ///   two objects are selected.
    /// <code>
    ///[TestFixture] public class ATest : NUnitFormTest
    ///{
    ///  // Gets the Form used for testing.
    ///  public override Type FormType
    ///  {
    ///    get {return typeof(MyTestForm);}
    ///  }
    ///  
    ///  [Test] public void Selecting()
    ///  {
    ///    ControlTester myControl = new ControlTester("myControl", CurrentForm);
    ///    using (MouseController mouse = myControl.MouseController())
    ///    {
    ///      mouse.Drag (10,10, 20,20);
    ///      AssertEquals (1, myControl.Properties.SelectedObjects.Count);
    ///      
    ///      mouse.PressAndRelease(Keys.Shift);
    ///      mouse.Drag(100,100, 200,200);
    ///      mouse.Release(Keys.Shift);
    ///      AssertEquals (2, myControl.Properties.SelectedObjects.Count);
    ///    }
    ///  }
    /// </code>
    /// </example>
    public void Press(Keys keys)
    {
        if ((keys & ~(Keys.Alt | Keys.Shift | Keys.Control)) != 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(keys), "Only Alt, Shift and Control are currently handled.");
        }
        var keyValue = (byte)keys;

        Win32.KeyBdEvent(keyValue, 0, 0, UIntPtr.Zero); //key down

        //Win32.KBINPUT kbi = new Win32.KBINPUT();
        //kbi.ki.dwExtraInfo = Win32.GetMessageExtraInfo();

        //if ((keys & Keys.Alt) == Keys.Alt)
        //{
        //    kbi.ki.wVk = Win32.VK_MENU;
        //    if (0 == Win32.SendKeyboardInput(1, ref kbi, Marshal.SizeOf(kbi)))
        //    {
        //        ThrowHelper.ThrowWin32Exception();
        //    }
        //}
        //if ((keys & Keys.Control) == Keys.Control)
        //{
        //    kbi.ki.wVk = Win32.VK_CONTROL;
        //    if (0 == Win32.SendKeyboardInput(1, ref kbi, Marshal.SizeOf(kbi)))
        //    {
        //        ThrowHelper.ThrowWin32Exception();
        //    }
        //}
        //if ((keys & Keys.Shift) == Keys.Shift)
        //{
        //    kbi.ki.wVk = Win32.VK_SHIFT;
        //    if (0 == Win32.SendKeyboardInput(1, ref kbi, Marshal.SizeOf(kbi)))
        //    {
        //        ThrowHelper.ThrowWin32Exception();
        //    }
        //}

        Application.DoEvents();
    }

    /// <summary>
    ///   Simulate releasing the mouse button(s).
    /// </summary>
    /// <param name="buttons">
    ///   A bitwise combination of the <see cref="MouseButtons"/> enumeration values. 
    /// </param>
    /// <remarks>
    ///   <b>Release</b> positions the mouse over the control under test 
    ///   and then simulates releasing the specified <paramref name="buttons"/>.
    /// </remarks>
    /// <exception cref="NotSupportedException">
    ///   When <paramref name="buttons"/> contains <see cref="MouseButtons">MouseButtons.XButton1</see>
    ///   or <see cref="MouseButtons">MouseButtons.XButton2</see> and the installed mouse does have 4
    ///   or 5 buttons, respectively.
    /// </exception>
    public void Release(MouseButtons buttons)
    {
        Release(buttons, PointF.Empty);
    }

    /// <summary>
    ///   Simulate release the mouse button(s) at the specified  x and y-coordinate.
    /// </summary>
    /// <param name="buttons">
    ///   A bitwise combination of the <see cref="MouseButtons"/> enumeration values. 
    /// </param>
    /// <param name="x">
    ///   The <see cref="float"/> x-coordinate,
    ///   relative to the control under test,
    ///   to <see cref="Position">move</see> the mouse to.
    /// </param>
    /// <param name="y">
    ///   The <see cref="float"/> y-coordinate,
    ///   relative to the control under test,
    ///   to <see cref="Position">move</see> the mouse to.
    /// </param>
    /// <remarks>
    ///   <b>Release</b> positions the mouse over the control under test 
    ///   at the specified point (<paramref name="x"/>, <paramref name="y"/>)
    ///   and then simulates releasing the specified <paramref name="buttons"/>.
    /// </remarks>
    /// <exception cref="NotSupportedException">
    ///   When <paramref name="buttons"/> contains <see cref="MouseButtons">MouseButtons.XButton1</see>
    ///   or <see cref="MouseButtons">MouseButtons.XButton2</see> and the installed mouse does have 4
    ///   or 5 buttons, respectively.
    /// </exception>
    public void Release(MouseButtons buttons, float x, float y)
    {
        Release(buttons, new PointF(x, y));
    }

    /// <summary>
    ///   Simulate releasing the mouse button(s) at the specified <see cref="PointF"/>.
    /// </summary>
    /// <param name="buttons">
    ///   A bitwise combination of the <see cref="MouseButtons"/> enumeration values. 
    /// </param>
    /// <param name="point">
    ///   A <see cref="PointF"/>, relative to the control under test,
    ///   to <see cref="Position">move</see> the mouse to.
    /// </param>
    /// <remarks>
    ///   <b>Release</b> positions the mouse over the control under test 
    ///   at the specified <paramref name="point"/>
    ///   and then simulates releasing the specified <paramref name="buttons"/>.
    /// </remarks>
    /// <exception cref="NotSupportedException">
    ///   When <paramref name="buttons"/> contains <see cref="MouseButtons">MouseButtons.XButton1</see>
    ///   or <see cref="MouseButtons">MouseButtons.XButton2</see> and the installed mouse does have 4
    ///   or 5 buttons, respectively.
    /// </exception>
    public void Release(MouseButtons buttons, PointF point)
    {
        // Ensure cursor remains over the control
        Position = point;

        if ((buttons & MouseButtons.Left) != 0)
        {
            int lParam = _mouseControl.MakeLParam(point, _scale);
            Win32.SendMessage(_mouseControl.Handle, Win32.WM_LBUTTONUP, IntPtr.Zero, (IntPtr)lParam);
            Application.DoEvents();
            return;
        }

        var input = new Win32.MSINPUT(0);
        if ((buttons & MouseButtons.Right) != 0)
        {
            input.mi.dwFlags |= Win32.MOUSEEVENTF_RIGHTUP;
        }
        if ((buttons & MouseButtons.Middle) != 0)
        {
            input.mi.dwFlags |= Win32.MOUSEEVENTF_MIDDLEUP;
        }
        if ((buttons & MouseButtons.XButton1) != 0)
        {
            if (SystemInformation.MouseButtons < 4)
            {
                ThrowHelper.ThrowNotSupportedException("A mouse with at least 4 buttons is required.");
            }
            input.mi.dwFlags |= Win32.MOUSEEVENTF_XUP;
            input.mi.mouseData = Win32.XBUTTON1;
        }
        if ((buttons & MouseButtons.XButton2) != 0)
        {
            if (SystemInformation.MouseButtons < 5)
            {
                ThrowHelper.ThrowNotSupportedException("A mouse with at least 5 buttons is required.");
            }
            input.mi.dwFlags |= Win32.MOUSEEVENTF_XUP;
            input.mi.mouseData = Win32.XBUTTON2;
        }

        if (input.mi.dwFlags != 0)
        {
            if (0 == Win32.SendMouseInput(1, ref input, Marshal.SizeOf(input)))
            {
                ThrowHelper.ThrowWin32Exception();
            }
            Application.DoEvents();
        }
    }

    /// <overloads>
    ///   Simulates releasing the <see cref="MouseButtons"/> or modifier <see cref="Keys"/>.
    /// </overloads>
    /// <summary>
    ///   Simulate releasing the mouse modifier key(s) (Alt, Shift and Control).
    /// </summary>
    /// <param name="keys">
    ///   A bitwise combination of the <see cref="Keys"/> enumeration values. Only <b>Alt</b>, <b>Shift</b>
    ///   and <b>Control</b> are allowed.
    /// </param>
    /// <remarks>
    ///   <b>Release</b> simulates releasing the specified <paramref name="keys"/>.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   When <paramref name="keys"/> contains a value that is not 
    ///   <b>Alt</b>, <b>Shift</b> or <b>Control</b>.
    /// </exception>
    /// <example>
    ///   The following example performs a "shift drag" and verifies that
    ///   two objects are selected.
    /// <code>
    ///[TestFixture] public class ATest : NUnitFormTest
    ///{
    ///  // Gets the Form used for testing.
    ///  public override Type FormType
    ///  {
    ///    get {return typeof(MyTestForm);}
    ///  }
    ///  
    ///  [Test] public void Selecting()
    ///  {
    ///    ControlTester myControl = new ControlTester("myControl", CurrentForm);
    ///    using (MouseController mouse = myControl.MouseController())
    ///    {
    ///      mouse.Drag (10,10, 20,20);
    ///      AssertEquals (1, myControl.Properties.SelectedObjects.Count);
    ///      
    ///      mouse.PressAndRelease(Keys.Shift);
    ///      mouse.Drag(100,100, 200,200);
    ///      mouse.Release(Keys.Shift);
    ///      AssertEquals (2, myControl.Properties.SelectedObjects.Count);
    ///    }
    ///  }
    /// </code>
    /// </example>
    public void Release(Keys keys)
    {
        if ((keys & ~(Keys.Alt | Keys.Shift | Keys.Control)) != 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(keys), "Only Alt, Shift and Control is allowed.");
        }
        var keyValue = (byte)keys;
        Win32.KeyBdEvent(keyValue, 0, 0x02, UIntPtr.Zero); //key Up


        //Win32.KBINPUT kbi = new Win32.KBINPUT();
        //kbi.ki.dwExtraInfo = Win32.GetMessageExtraInfo();
        //kbi.ki.dwFlags = Win32.KEYEVENTF_KEYUP;

        //if ((keys & Keys.Alt) == Keys.Alt)
        //{
        //    kbi.ki.wVk = Win32.VK_MENU;
        //    if (0 == Win32.SendKeyboardInput(1, ref kbi, Marshal.SizeOf(kbi)))
        //    {
        //        ThrowHelper.ThrowWin32Exception();
        //    }
        //}
        //if ((keys & Keys.Control) == Keys.Control)
        //{
        //    kbi.ki.wVk = Win32.VK_CONTROL;
        //    if (0 == Win32.SendKeyboardInput(1, ref kbi, Marshal.SizeOf(kbi)))
        //    {
        //        ThrowHelper.ThrowWin32Exception();
        //    }
        //}
        //if ((keys & Keys.Shift) == Keys.Shift)
        //{
        //    kbi.ki.wVk = Win32.VK_SHIFT;
        //    if (0 == Win32.SendKeyboardInput(1, ref kbi, Marshal.SizeOf(kbi)))
        //    {
        //        ThrowHelper.ThrowWin32Exception();
        //    }
        //}

        Application.DoEvents();
    }

    /// <summary>
    ///   Simulate dragging the mouse.
    /// </summary>
    /// <param name="startPoint">
    ///   A <see cref="PointF"/> to start the drag operation at.
    /// </param>
    /// <param name="points">
    ///   Array of <see cref="PointF"/> structures that represent the points to <see cref="Position">move</see>>
    ///   the mouse to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   When <paramref name="points"/> is <b>null</b>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   When <paramref name="points"/> does not contain any elements.
    /// </exception>
    /// <remarks>
    ///   <b>Drag</b> positions the mouse over the control under test 
    ///   at the specified <paramref name="startPoint"/> and then presses the
    ///   left mouse button.  It then moves the mouse to each point specified in the <paramref name="points"/>
    ///   array.  Finally the left button is released.
    /// </remarks>
    /// <example>
    /// <code>
    /// using (MouseController mouse = new MouseControler(myTestControl))
    /// {
    ///   mouse.Drag(new PointF(0,0), new Point(100, 100));
    /// }
    /// </code>
    /// </example>
    public void Drag(PointF startPoint, params PointF[] points)
    {
        if (points == null!)
        {
            ThrowHelper.ThrowArgumentNullException(nameof(points));
        }
        if (points.Length < 1)
        {
            ThrowHelper.ThrowArgumentException("At lease one point must be specified.", nameof(points));
        }

        Press(MouseButtons.Left, startPoint);
        for (var i = 0; i < points.Length - 1; ++i)
        {
            Position = points[i];
        }
        Release(MouseButtons.Left, points[points.Length - 1]);
    }

    /// <summary>
    ///   Simulate dragging the mouse.
    /// </summary>
    /// <param name="startX">
    ///   A <see cref="float"/> x-coordinate to start the drag operation at.
    /// </param>
    /// <param name="startY">
    ///   A <see cref="float"/> y-coordinate to start the drag operation at.
    /// </param>
    /// <param name="points">
    ///   Array of <see cref="float"/> values that represent the x and y coordinates to <see cref="Position">move</see>>
    ///   the mouse to.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   When <paramref name="points"/> is <b>null</b>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   When <paramref name="points"/> does not contain at lease two values,
    ///   <br/><b>Or</b>, the number of elements is odd.
    /// </exception>
    /// <remarks>
    ///   <b>Drag</b> positions the mouse over the control under test 
    ///   at the specified <see cref="PointF"/>(<paramref name="startX"/>, <paramref name="startY"/>)
    ///   and then presses the  left mouse button.  It then moves the mouse
    ///   to each point specified in the <paramref name="points"/> array.  Finally the left button is released.
    /// </remarks>
    /// <example>
    /// <code>
    /// using (MouseController mouse = new MouseControler(myTestControl))
    /// {
    ///   mouse.Drag(0,0, 100,100);
    /// }
    /// </code>
    /// </example>
    public void Drag(float startX, float startY, params float[] points)
    {
        if (points == null!)
        {
            ThrowHelper.ThrowArgumentNullException(nameof(points));
        }
        if (points.Length < 2)
        {
            ThrowHelper.ThrowArgumentException("At lease one point must be specified.", nameof(points));
        }
        if ((points.Length & 1) != 0)
        {
            ThrowHelper.ThrowArgumentException("Missing the final y-coordinate.", nameof(points));
        }

        Press(MouseButtons.Left, new PointF(startX, startY));
        for (var i = 0; i < points.Length - 2; i += 2)
        {
            Position = new PointF(points[i], points[i + 1]);
        }
        Release(MouseButtons.Left, new PointF(points[points.Length - 2], points[points.Length - 1]));
    }

    #endregion

    #endregion
}