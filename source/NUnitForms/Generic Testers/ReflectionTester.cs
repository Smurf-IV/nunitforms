#region Copyright (c) 2003-2007, Luke T. Maxon : 2026-2026 Smurf.IV

/********************************************************************************************************************
'
' Copyright (c) 2003-2007, Luke T. Maxon
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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

using NUnit.Extensions.Forms.Util;


namespace NUnit.Extensions.Forms.Generic_Testers;

///<summary>
/// Abstract base class for control and component testers that
/// handles getting and setting properties and fields via reflection.
///</summary>
public abstract class ReflectionTester
{
    private static readonly ConditionalWeakTable<Control, object> _readiedControls = new();

    /// <summary>
    /// Derived testers must override this method to provide the object being tested.
    /// </summary>
    public abstract object? TheObject { get; }

    /// <summary>
    /// Convenience accessor / mutator for any nonsupported property on a control
    /// to test.
    /// </summary>
    /// <example>
    /// ControlTester t = new ControlTester("t");
    /// t["Text"] = "a";
    /// AssertEqual("a", t["Text"]);
    /// </example>
    /// 
    public object? this[string propertyName]
    {
        get
        {
            PropertyInfo? prop = GetPropertyInfo(propertyName);
            if (prop != null)
            {
                return prop.GetValue(TheObject, null);
            }

            FieldInfo? field = GetFieldInfo(propertyName);
            return field != null
                ? field.GetValue(TheObject)
                : null;
        }
        set
        {
            PropertyInfo? prop = GetPropertyInfo(propertyName);
            if (prop != null)
            {
                prop.SetValue(TheObject, value, null);
                DoAfterSetProperty(propertyName);
            }
            else
            {
                FieldInfo? field = GetFieldInfo(propertyName);
                if (field != null)
                {
                    field.SetValue(TheObject, value);
                }
            }
        }
    }

    /// <summary>
    /// Called after this[string] is used to set a property value.
    /// Typically, calls "EndCurrentEdit" on the object's data binding binding for that property.
    /// </summary>
    protected virtual void DoAfterSetProperty(string propertyName)
    {
    }

    private FieldInfo? GetFieldInfo(string fieldName)
    {
        return TheObject.GetType().GetField(fieldName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    }

    private PropertyInfo? GetPropertyInfo(string propertyName)
    {
        return TheObject.GetType().GetProperty(propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    }

    /// <summary>
    /// Simulates firing of an event by the control being tested.
    /// </summary>
    /// <param name="eventName">The name of the event to fire.</param>
    /// <param name="args">The optional arguments required to construct the EventArgs for the specified event.</param>
    public void FireEvent(string eventName, params object[] args)
    {
        EventHelper.RaiseEvent(TheObject, eventName, args);
    }

    /// <summary>
    /// Simulates firing of an event by the control being tested.
    /// </summary>
    /// <param name="eventName">The name of the event to fire.</param>
    /// <param name="arg">The EventArgs object to pass as a parameter on the event.</param>
    public void FireEvent(string eventName, EventArgs arg)
    {
        EventHelper.RaiseEvent(TheObject, eventName, arg);
    }

    /// <summary>
    /// Simulates firing of an event by the control being tested.
    /// </summary>
    /// <param name="eventName">The name of the event to fire.</param>
    public void FireEvent(string eventName)
    {
        EventHelper.RaiseEvent(TheObject, eventName);
    }

    /// <summary>
    /// Convenience method invoker for any nonsupported method on a control to test
    /// </summary>
    /// <param name="methodName">the name of the method to invoke</param>
    /// <param name="args">the arguments to pass into the method</param>
    public object Invoke(string methodName, params object[] args)
    {
        return EventHelper.Call(TheObject, methodName, args);
    }

    /// <summary>
    /// Synchronously blocks until the control's Win32 handle and layouts are fully initialized,
    /// preventing missing events or layout race conditions across .NET 4.x and .NET 6+.
    /// </summary>
    /// <param name="timeoutMilliseconds">Max time to wait before forcing a fallback creation.</param>
    public void EnsureHandleReady(int timeoutMilliseconds = 1000)
    {
        var control = TheObject as Control;
        if (control == null || control.IsDisposed)
        {
            return;
        }

        if (_readiedControls.TryGetValue(control, out _))
        {
            return;
        }

        // 1. Ensure the handle exists first
        if (!control.IsHandleCreated)
        {
            IntPtr forceHandle = control.Handle;
        }

        var watch = System.Diagnostics.Stopwatch.StartNew();
        // 2. CRITICAL FOR .NET 6+: Execute multiple message loops to clear 
        // structural painting notifications (like theme painting and DPI adjustments)
        for (int i = 0; i < 3; i++)
        {
            if (control.IsDisposed)
            {
                return;
            }

            Application.DoEvents();
            System.Threading.Thread.Sleep(10); // Give the OS time to dispatch background paints
        }
        // 3. Fallback strategy: Force-pump the Win32 message loop until the handle is established
        while (!control.IsHandleCreated && watch.ElapsedMilliseconds < timeoutMilliseconds)
        {
            if (control.IsDisposed)
            {
                return;
            }

            // Process underlying OS layout messages safely
            Application.DoEvents();

            // Give the CPU a tiny break between message cycles
            System.Threading.Thread.Sleep(1);
        }

        try
        {
            _readiedControls.Add(control, new object());
        }
        catch (ArgumentException)
        {
            // Already added by another thread/tester
        }
    }
}