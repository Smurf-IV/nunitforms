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

using NUnit.Extensions.Forms.Exceptions;

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
    private readonly List<Form>? forms;
    private readonly string name;

    public Finder()
    {
    }

    /// <summary>
    /// Creates a Finder that will find things on a specific Form according to their name.
    /// </summary>
    /// <param name="name">The name of the Control to find.</param>
    /// <param name="form">The form to search for the control.</param>
    public Finder(string name, Form? form)
    {
        this.name = name;
        if (form != null)
        {
            forms = [form];
        }
    }

    /// <summary>
    /// Creates a Finder that will find things according to their name.  
    /// </summary>
    /// <param name="name">The name of the thing to find.</param>
    public Finder(string name)
    {
        this.name = name;
    }

    public int Count => FindAll(typeof(T)).Count;

    private List<Form> FormCollection
    {
        get
        {
            if (forms == null)
            {
                return new FormFinder().FindAll();
            }
            return forms;
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
        foreach (var form in FormCollection)
        {
            found.AddRange(Find(name, form, null));
        }
        return found;
    }

    private List<object> FindAll(Type type)
    {
        var found = new List<object>();
        var allFound = FindAll();
        foreach (var o in allFound)
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
        var found = FindAll(type);
        if (index < 0)
        {
            return found.Count switch
            {
                1 => found[0],
                0 => throw new NoSuchControlException(name),
                _ => throw new AmbiguousNameException(name)
            };
        }

        if (found.Count > index)
        {
            return found[index];
        }

        throw new NoSuchControlException(name + "[" + index + "]");
    }

    private List<object> Find(string name, object obj, object src)
    {
        var results = new List<object>();

        if (Matches(name, obj, src))
        {
            results.Add(obj);
        }

        if (obj is Form form)
        {
            var f = form;
#if NETFRAMEWORK  // https://github.com/dotnet/designs/blob/main/accepted/2020/net5/net5.md#preprocessor-symbols
            if (f.Menu != null)
            {
                results.AddRange(Find(name, f.Menu, f));
            }
#else
            throw new NotImplementedException();
#endif
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
                results.AddRange(Find(name, c2, null));
            }
#if NETFRAMEWORK  // https://github.com/dotnet/designs/blob/main/accepted/2020/net5/net5.md#preprocessor-symbols
            if (control.ContextMenu != null)
            {
                results.AddRange(Find(name, control.ContextMenu, control));
            }
#else
            throw new NotImplementedException();
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
        throw new NotImplementedException();
#endif
        return results;
    }

    private bool Matches(string name, object control, object src)
    {
        var c = control;
        var names = name.Split('.');
        for (var i = names.Length - 1; i >= 0; i--)
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

    public object? Parent(object o)
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

    public string Name(object o)
    {
        return o switch
        {
            ToolStripControlHost host => host.Name,
            ToolStripItem item => item.Name,
            Control control => control.Name,
#if NETFRAMEWORK  // https://github.com/dotnet/designs/blob/main/accepted/2020/net5/net5.md#preprocessor-symbols
            MenuItem menuItem => menuItem.Text.Replace("&", string.Empty).Replace(".", string.Empty),
            MainMenu => "MainMenu",
            ContextMenu => "ContextMenu",
#endif
            Component component => component.Site.Name,
            _ => throw new Exception("Object name not defined")
        };
    }
}