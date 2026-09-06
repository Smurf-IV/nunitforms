#region Copyright (c) 2003-2005, Luke T. Maxon : 2026-2026 Smurf.IV

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

#if NETFRAMEWORK  // https://github.com/dotnet/designs/blob/main/accepted/2020/net5/net5.md#preprocessor-symbols

using System;
using System.Reflection;
using System.Windows.Forms;

using NUnit.Extensions.Forms.Finders;


namespace NUnit.Extensions.Forms.Testers;


/// <summary>
/// A ControlTester for MenuItems.
/// </summary>
/// <remarks>
/// It does not extend ControlTester because MenuItems are not Controls.  (sadly)</remarks>
public class MenuItemTester
{
    private readonly Form? _form;
    private readonly string? _formName;

    protected string Name;

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuItemTester"/> class with the specified menu item name and form.
    /// </summary>
    /// <param name="name">The name of the menu item.</param>
    /// <param name="form">The form containing the menu item.</param>
    public MenuItemTester(string name, Form form)
    {
        this._form = form;
        this.Name = name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuItemTester"/> class with the specified menu item name and form name.
    /// </summary>
    /// <param name="name">The name of the menu item.</param>
    /// <param name="formName">The name of the form containing the menu item.</param>
    public MenuItemTester(string name, string formName)
    {
        this._formName = formName;
        this.Name = name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MenuItemTester"/> class with the specified menu item name.
    /// </summary>
    /// <param name="name">The name of the menu item.</param>
    public MenuItemTester(string name)
    {
        this.Name = name;
    }

    protected MenuItem MenuItem
    {
        get
        {
            if (_form != null)
            {
                //may have dynamically added controls.  I am not saving this.
                return new Finder<MenuItem>(Name, _form).Find();
            }

            if (_formName != null)
            {
                return new Finder<MenuItem>(Name, new FormFinder().Find(_formName)).Find();
            }

            return new Finder<MenuItem>(Name).Find();
        }
    }

    /// <summary>
    /// Gets the text of this MenuItem.
    /// </summary>
    public string Text => MenuItem.Text;

    /// <summary>
    /// Gets the MenuItem property accessor. Allows you to access any properties of this MenuItem.
    /// </summary>
    public MenuItem Properties => MenuItem;

    #region EventFiring

    /// <summary>
    /// Fires an event on the menu item.
    /// </summary>
    /// <param name="eventName">The name of the event to fire.</param>
    protected void FireEvent(string eventName)
    {
        MethodInfo? mInfo = MenuItem.GetType().GetMethod($"On{eventName}",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        ParameterInfo[] param = mInfo.GetParameters();
        Type parameterType = param[0].ParameterType;
            mInfo.Invoke(MenuItem, [Activator.CreateInstance(parameterType)]);
    }

    #endregion

    /// <summary>
    /// Clicks the MenuItem (activates it)
    /// </summary>
    public virtual void Click()
    {
        FireEvent("Click");
    }

    /// <summary>
    /// Pops up a menu.
    /// </summary>
    public virtual void Popup()
    {
        FireEvent("Popup");
    }
}
#endif