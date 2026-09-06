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
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using NUnit.Extensions.Forms.Finders;
using NUnit.Extensions.Forms.Util;

namespace NUnit.Extensions.Forms.Generic_Testers;

public class Tester<T, TThis> : ReflectionTester, IEnumerable<TThis>
    where TThis : Tester<T, TThis>, new()
{
    private Form? _form;
    private string? _formName;
    private int _index = -1;

    /// <summary>
    /// The name of the underlying test object (sometimes a control).
    /// </summary>
    protected string? Name;

    public Tester(string name, string formName)
    {
        _formName = formName;
        Name = name;
    }

    public Tester(string name, Form form)
    {
        _form = form;
        Name = name;
    }

    public Tester(string? name)
    {
        Name = name;
    }

    public Tester(Tester<T, TThis> tester, int index)
    {
        InitFromTester(tester, index);
    }

    ///<summary>
    /// Default constructor for generic support.
    ///</summary>
    protected Tester()
    {
    }

    /// <summary>
    /// Synchronously blocks until the control's Win32 handle and layouts are fully initialized,
    /// preventing missing events or layout race conditions across .NET 4.x and .NET 6+.
    /// </summary>
    /// <param name="timeoutMilliseconds">Max time to wait before forcing a fallback creation.</param>
    public void EnsureHandleReady(int timeoutMilliseconds = 1000)
    {
        var control = TheObject as Control;
        ThrowHelper.ThrowIfNull(control, nameof(control));

        // 1. Ensure the handle exists first
        if (control is { IsHandleCreated: false, IsDisposed: false })
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
    }

    public int Count => GetFinder().Count;

    /// <summary>
    /// The underlying <see cref="Control"/> for this tester.
    /// </summary>
    public virtual T Properties => GetFinder().Find(_index);

    /// <summary>
    /// The Control being tested.
    /// </summary>
    public override object? TheObject => Properties;

    public virtual TThis this[int index]
    {
        get
        {
            var newTester = new TThis();
            newTester.InitFromTester(this, index);
            return newTester;
        }
    }

    #region IEnumerable<TThis> Members

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    ///<summary>
    ///Returns an enumerator that iterates through the collection.
    ///</summary>
    public IEnumerator<TThis> GetEnumerator()
    {
        var items = new List<TThis>();
        for (var i = 0; i < Count; i++)
        {
            items.Add(this[i]);
        }
        return items.GetEnumerator();
    }

    #endregion

    protected void InitFromTester(Tester<T, TThis> tester, int controlIndex)
    {
        if (controlIndex < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(controlIndex), controlIndex, @"Should not have index < 0");
        }

        _index = controlIndex;
        _form = tester._form;
        _formName = tester._formName;
        Name = tester.Name;
    }

    private Finder<T> GetFinder()
    {
        if (_form != null)
        {
            return new Finder<T>(Name, _form);
        }

        if (_formName != null)
        {
            return new Finder<T>(Name, new FormFinder().Find(_formName));
        }

        return new Finder<T>(Name);
    }
}