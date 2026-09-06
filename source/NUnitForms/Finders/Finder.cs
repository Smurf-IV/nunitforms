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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using NUnit.Extensions.Forms.Util;


namespace NUnit.Extensions.Forms.Finders;

/// <summary>
/// Internal use only.  Finds controls, components, and menuitems
/// according to their name property.
/// </summary>
/// <remarks>
/// It is also used by the recorder application which is why it is not
/// internal.
/// </remarks>
/// the recorder application.
public class Finder<T>
{
    private readonly List<Form>? _forms;
    private readonly string? _name;

    public Finder()
    {
    }

    /// <summary>
    /// Creates a Finder that will find things on a specific Form according to their name.
    /// </summary>
    /// <param name="name">The name of the Control to find.</param>
    /// <param name="form">The form to search for the control.</param>
    public Finder(string? name, Form? form)
    {
        this._name = name;
        if (form != null)
        {
            _forms = [form];
        }
    }

    /// <summary>
    /// Creates a Finder that will find things according to their name.  
    /// </summary>
    /// <param name="name">The name of the thing to find.</param>
    public Finder(string? name)
    {
        this._name = name;
    }

    public int Count => FindAll(typeof(T)).Count;

    private List<Form> FormCollection
    {
        get
        {
            if (_forms == null)
            {
                return new FormFinder().FindAll();
            }
            return _forms;
        }
    }

    /// <summary>
    /// Finds a control.  
    /// </summary>
    /// <exception>
    /// If there is more than one with the specified name, it will
    /// throw an AmbiguousNameException.  If the Control does not exist, it will throw
    /// a NoSuchControlException.
    /// </exception>
    /// <returns>The control if one is found.</returns>
    public T Find()
    {
        return Find(-1);
    }

    public T Find(int index)
    {
        return (T)Find(index, typeof(T));
    }

    private List<object> FindAll()
    {
        var found = new List<object>();
        foreach (Form form in FormCollection)
        {
            found.AddRange(Find(_name, form, null));
        }
        return found;
    }

    private List<object> FindAll(Type type)
    {
        var found = new List<object>();
        List<object> allFound = FindAll();
        foreach (object o in allFound)
        {
            if (type.IsAssignableFrom(o.GetType()))
            {
                found.Add(o);
            }
        }
        return found;
    }

    private object Find(int index, Type type)
    {
        List<object> found = FindAll(type);
        if (index < 0)
        {
            return found.Count switch
            {
                1 => found[0],
                0 => ThrowHelper.ThrowNoSuchControlException<object>(_name),
                _ => ThrowHelper.ThrowAmbiguousNameException<object>(_name)
            };
        }

        if (found.Count > index)
        {
            return found[index];
        }

        return ThrowHelper.ThrowNoSuchControlException<object>($"{_name}[{index}]");
    }

    private List<object> Find(string? name, object obj, object? src)
    {
        var results = new List<object>();

        if (Matches(name, obj, src))
        {
            results.Add(obj);
        }
        
        if (obj is Form form)
        {
            Form f = form;
#if NETFRAMEWORK  // https://github.com/dotnet/designs/blob/main/accepted/2020/net5/net5.md#preprocessor-symbols
            if (f.Menu != null)
            {
                results.AddRange(Find(name, f.Menu, f));
            }
#endif
            // .NET (Core/5+) replacement: use MainMenuStrip instead of the removed Form.Menu
            if (f.MainMenuStrip != null)
            {
                foreach (ToolStripItem item in f.MainMenuStrip.Items)
                {
                    results.AddRange(Find(name, item, f));
                }
            }
        }

        if (obj is ToolStrip strip)
        {
            foreach (ToolStripItem t2 in strip.Items)
            {
                results.AddRange(Find(name, t2, null));
            }
        }

        if (obj is ToolStripDropDownItem downItem)
        {
            foreach (ToolStripItem i2 in downItem.DropDownItems)
            {
                results.AddRange(Find(name, i2, null));
            }
        }

        if (obj is Control control)
        {
            foreach (Control c2 in control.Controls)
            {
                // Avoid enumerating the same main menu twice: it's already handled via Form.MainMenuStrip above
                if (control is Form ff && c2 is MenuStrip ms && ff.MainMenuStrip == ms)
                {
                    continue;
                }
                results.AddRange(Find(name, c2, null));
            }
#if NETFRAMEWORK  // https://github.com/dotnet/designs/blob/main/accepted/2020/net5/net5.md#preprocessor-symbols
            if (control.ContextMenu != null)
            {
                results.AddRange(Find(name, control.ContextMenu, control));
            }
#else
            // In .NET (Core/5+) use ContextMenuStrip instead. Handled below if present.
#endif
            if (control.ContextMenuStrip != null)
            {
                foreach (ToolStripItem item in control.ContextMenuStrip.Items)
                {
                    results.AddRange(Find(name, item, null));
                }
            }
        }

#if NETFRAMEWORK  // https://github.com/dotnet/designs/blob/main/accepted/2020/net5/net5.md#preprocessor-symbols
        if (obj is Menu menu)
        {
            foreach (MenuItem m2 in menu.MenuItems)
            {
                results.AddRange(Find(name, m2, src));
            }
        }
#else
        // Menu class is not available in .NET (Core/5+); menu traversal handled via ToolStrip items above.
#endif
        return results;
    }

    private bool Matches(string? name, object? control, object? src)
    {
        object? c = control;
        string[] names = name?.Split('.') ?? new string[0];
        for (int i = names.Length - 1; i >= 0; i--)
        {
            if (!names[i].Equals(Name(c)))
            {
                return false;
            }
            c = Parent(c);
            if (c == null && src != null)
            {
                c = src;
            }
        }
        return true;
    }

    public object? Parent(object? o)
    {
        return o switch
        {
            Control control => control.Parent,
#if NETFRAMEWORK  // https://github.com/dotnet/designs/blob/main/accepted/2020/net5/net5.md#preprocessor-symbols
            MenuItem item => item.Parent,
#endif
            Component component => component.Container,
            _ => null
        };
    }

    public string Name(object? o)
    {
        return o switch
        {
            ToolStripControlHost host => host.Name,
            ToolStripItem item => item.Name,
            Control control => control.Name,
#if NETFRAMEWORK  // https://github.com/dotnet/designs/blob/main/accepted/2020/net5/net5.md#preprocessor-symbols
            MenuItem menuItem => menuItem.Text.Replace("&", string.Empty).Replace(".", string.Empty),
            MainMenu => @"MainMenu",
            ContextMenu => @"ContextMenu",
#endif
            Component component => component.Site.Name,
            _ => ThrowHelper.KeyNotFoundException<string>("Object name not defined")
        };
    }
}