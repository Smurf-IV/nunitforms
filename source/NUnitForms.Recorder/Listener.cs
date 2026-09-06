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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

using NUnit.Extensions.Forms.Finders;


namespace NUnitForms.Recorder;

public delegate void EventHappened(Type testerType, object control, Action action);

///<summary>
/// A <see cref="Listener"/> is used by a <see cref="Recorder"/>
/// to manage events happening on a target control.
///</summary>
public class Listener
{
    private string actionInProcess;
    private object controlInProcess;
    private readonly List<Form> formsIAmListeningTo = [];
    private bool inProcess;
    private readonly SupportedEventsRegistry registry;
    private Type testerTypeInProcess;

    ///<summary>
    /// Constructs a new <see cref="Listener"/>.
    ///</summary>
    public Listener()
    {
        registry = new SupportedEventsRegistry(this);
    }

    public event EventHappened Event;

    public void ListenTo(Control control)
    {
        if (control is Form form)
        {
            formsIAmListeningTo.Add(form);
        }

        // Todo: This is problably not the correct way of solving the 
        // problem but it solves the problem with ToolStripComboBox,
        // ToolStripTextBox and ToolStripProgressBar
        // We here test if the item parent is a Toolbar,context menu or menu. If so and is 
        // not a ToolStripItem we do no add an eventlistener.
        /*
  if (!(control is ToolStripItem) && (control.Parent != null) && (control.Parent.GetType() == typeof(ToolStrip) ||
    control.Parent.GetType() == typeof(MenuStrip) || control.Parent.GetType() == typeof(ContextMenuStrip)))
  {
    return;
  }
  */

        if (!string.IsNullOrEmpty(control.Name))
        {
            AddEventListeners(control);
        }
#if NETFRAMEWORK  // https://github.com/dotnet/designs/blob/main/accepted/2020/net5/net5.md#preprocessor-symbols
        ListenTo(control.ContextMenu);
#endif
        if (control is ToolStrip toolstrip)
        {
            if (toolstrip.ContextMenuStrip != null)
            {
                ListenTo(toolstrip.ContextMenuStrip);
            }

            foreach (ToolStripItem item in toolstrip.Items)
            {
                if (item is ToolStripControlHost host)
                {
                    ListenTo(host.Control);
                }

                AddEventListeners(item);

                if (item is ToolStripDropDownItem dropdown)
                {
                    ListenTo(dropdown.DropDownItems);
                }
            }
        }

        //#if NETFRAMEWORK  // https://github.com/dotnet/designs/blob/main/accepted/2020/net5/net5.md#preprocessor-symbols
        if (!string.IsNullOrEmpty(control.Name))
        {
            AddPropertyAssertListeners(control);
        }
        //#endif
        control.ControlAdded += ControlAdded;


        foreach (Control c in control.Controls)
        {
            ListenTo(c);
        }
    }

    private void ListenTo(ToolStripItemCollection? collection)
    {
        if (collection != null)
        {
            foreach (ToolStripItem item in collection)
            {
                AddEventListeners(item);
                if (item is ToolStripControlHost host)
                {
                    ListenTo(host.Control);
                }
                if (item is ToolStripDropDownItem dropdown)
                {
                    ListenTo(dropdown.DropDownItems);
                }
            }
        }
    }

    private void AddPropertyAssertListeners(Control control)
    {
        PropertyInfo[] properties = control.GetType().GetProperties();
        foreach (PropertyInfo propertyInfo in properties)
        {
            AddAssertMenuItem(propertyInfo, control);
        }
    }

#if NETFRAMEWORK  // https://github.com/dotnet/designs/blob/main/accepted/2020/net5/net5.md#preprocessor-symbols
    private void ListenTo(Menu? menu)
    {
        if (menu == null)
        {
            return;
        }
        AddEventListeners(menu);
        foreach (MenuItem item in menu.MenuItems)
        {
            ListenTo(item);
        }
    }
#endif

    private void AddAssertMenuItem(PropertyInfo propertyInfo, Control control)
    {
        if (propertyInfo.PropertyType.IsPrimitive || propertyInfo.PropertyType.Equals(typeof(string)))
        {
            string propertyName = propertyInfo.Name;
            EventHandler recorder = registry.PropertyAssertHandler(control.GetType());
            if (recorder != null)
            {
#if NETFRAMEWORK  // https://github.com/dotnet/designs/blob/main/accepted/2020/net5/net5.md#preprocessor-symbols
                AddAssertMenuItem(control, propertyName, recorder);
#else
                throw new NotSupportedException("AddAssertMenuItem is not supported in this framework.");
#endif
            }
        }
    }
    private void AddEventListeners(object control)
    {
        EventInfo[] events = control.GetType().GetEvents();

        foreach (EventInfo eventInfo in events)
        {
            string eventName = eventInfo.Name;
            MulticastDelegate recorder = registry.EventHandler(control.GetType(), eventName);
            if (recorder != null)
            {
                // TODO: Work out why adding it twice works ??
                eventInfo.AddEventHandler(control, recorder);
                /*
                 Your Code  ──>  EventInfo.AddEventHandler()
                              │
                              ▼
                     WinForms Control
                              │
               ┌──────────────┴──────────────┐
               ▼                             ▼
        .NET Framework 4.x                .NET 6+
        Legacy EventHandlerList       Modernised EventHandlerList
        (LIFO Traversal)              (FIFO Traversal)
                 */
                AddEventHandlerAtStartOfChain(control, eventName, recorder);
            }
        }
    }

    private void AddEventHandlerAtStartOfChain(object control, string eventName, MulticastDelegate recorder)
    {
        if (control == null) throw new ArgumentNullException(nameof(control));
        if (recorder == null) throw new ArgumentNullException(nameof(recorder));

        Type baseType = control.GetType();

        // 1. Retrieve the official metadata for the specific target event
        EventInfo? eventInfo = baseType.GetEvent(eventName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (eventInfo == null)
        {
            throw new NotSupportedException($"The event '{eventName}' does not exist on type {baseType.FullName}");
        }

        // 2. Locate the private backing field if it's a standard C# class event (Pattern B)
        FieldInfo? backingField = null;
        Type? currentType = baseType;
        while (currentType != null)
        {
            backingField = currentType.GetField(eventName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (backingField != null) break;
            currentType = currentType.BaseType;
        }

        if (backingField != null)
        {
            // PATTERN B: Pure C# Backing Field Event (e.g., 'SuperClick')
            Delegate currentDelegate = (Delegate)backingField.GetValue(control);
            Delegate updatedDelegate = Delegate.Combine(recorder, currentDelegate);
            backingField.SetValue(control, updatedDelegate);
            return;
        }

        // 3. PATTERN A: Core WinForms Property Event (EventHandlerList shared slots)
        // To cleanly circumvent the InvalidCastException on shared slots like TabControl,
        // we use the official EventInfo to securely hook our delegate natively into the control first.
        Type requiredType = eventInfo.EventHandlerType ?? typeof(EventHandler);
        Delegate typedRecorder = recorder.GetType() == requiredType
            ? recorder
            : Delegate.CreateDelegate(requiredType, recorder.Target, recorder.Method);

        // Let the framework handle the subscription and type coercion into the shared slot natively
        eventInfo.AddEventHandler(control, typedRecorder);

        // 4. Force LIFO Ordering securely without corrupting the slot signature
        PropertyInfo eventsProp = typeof(Component).GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
        if (eventsProp != null)
        {
            var handlers = (EventHandlerList)eventsProp.GetValue(control, null);
            object? eventKey = null;

            // Trace the correct internal WinForms Key Token
            var typesToScan = new List<Type> { baseType };
            if (eventInfo.DeclaringType != null && !typesToScan.Contains(eventInfo.DeclaringType))
            {
                typesToScan.Add(eventInfo.DeclaringType);
            }

            string cleanTargetName = eventName.Replace("_", "").ToUpperInvariant();
            bool foundKey = false;

            foreach (Type scanType in typesToScan)
            {
                Type? targetType = scanType;
                while (targetType != null)
                {
                    FieldInfo[] staticFields = targetType.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                    foreach (FieldInfo field in staticFields)
                    {
                        string cleanFieldName = field.Name.Replace("_", "").ToUpperInvariant();
                        if (cleanFieldName.Equals($"EVENT{cleanTargetName}")
                            || cleanFieldName.Equals($"S{cleanTargetName}EVENT")
                            || cleanFieldName.Equals(cleanTargetName)
                            || (cleanFieldName.StartsWith("EVENT") && (cleanFieldName.Contains(cleanTargetName) || cleanTargetName.Contains(cleanFieldName.Replace("EVENT", ""))))
                            )
                        {
                            eventKey = field.GetValue(null);
                            if (eventKey != null) 
                            {
                                foundKey = true; 
                                break; 
                            }
                        }
                    }
                    if (foundKey) break;
                    targetType = targetType.BaseType;
                }
                if (foundKey) break;
            }

            // Specific absolute fallbacks for irregular controls if loops skip them
            if (eventKey == null && control is TabControl)
            {
                FieldInfo? tabKeyField = typeof(TabControl).GetField("EVENT_SELECTEDINDEX", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                if (tabKeyField != null) eventKey = tabKeyField.GetValue(null);
            }

            if (eventKey != null)
            {
                Delegate masterDelegate = handlers[eventKey];
                if (masterDelegate != null)
                {
                    Delegate[] invocationList = masterDelegate.GetInvocationList();

                    // Locate where the framework added our typedRecorder inside the collection chain
                    int registeredIndex = Array.IndexOf(invocationList, typedRecorder);

                    if (registeredIndex > 0)
                    {
                        // Reassemble the chain manually to move our recorder to index 0 (Head/LIFO priority)
                        Delegate? reorderedChain = typedRecorder;

                        for (int i = 0; i < invocationList.Length; i++)
                        {
                            if (i == registeredIndex) continue; // Skip since we placed it at the head
                            reorderedChain = Delegate.Combine(reorderedChain, invocationList[i]);
                        }

                        // Save the correctly-sorted, type-safe multicast array back to the slot
                        handlers[eventKey] = reorderedChain;
                    }
                }
            }
        }
    }

    private static void AddEventHandlerAtStartOfChain1(object control, EventInfo eventInfo, Delegate newHandler)
    {
        // --- STEP 1: Route Known Anomalous Composite Control Overrides First ---
        object? eventKey = null;

        // ComboBox uses a shared external reference key for TextChanged that breaks standard loop extraction
        if (control is ComboBox && string.Equals(eventInfo.Name, "TextChanged", StringComparison.OrdinalIgnoreCase))
        {
            FieldInfo ctrlTextChangedField = typeof(Control).GetField("s_textChangedEvent", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                                           ?? typeof(Control).GetField("EventTextChanged", BindingFlags.Static | BindingFlags.NonPublic);
            if (ctrlTextChangedField != null)
            {
                eventKey = ctrlTextChangedField.GetValue(null);
            }
        }

        // --- STEP 2: Standard Reflection Traversal (If not caught by anomalous check) ---
        if (eventKey == null)
        {
            Type currentType = control.GetType();
            while (currentType != null)
            {
                FieldInfo[] staticFields = currentType.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (var field in staticFields)
                {
                    if (string.Equals(field.Name, $"s_{eventInfo.Name}Event", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(field.Name, $"Event{eventInfo.Name}", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(field.Name, $"EVENT_{eventInfo.Name}", StringComparison.OrdinalIgnoreCase))
                    {
                        eventKey = field.GetValue(null);
                        break;
                    }
                }
                if (eventKey != null)
                {
                    break;
                }

                currentType = currentType.BaseType;
            }
        }

        // --- STEP 3: Fallbacks & Execution Application ---
        if (eventKey == null && string.Equals(eventInfo.Name, "Click", StringComparison.OrdinalIgnoreCase))
        {
            FieldInfo clickField = typeof(Control).GetField("s_clickEvent", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                                 ?? typeof(Control).GetField("EventClick", BindingFlags.Static | BindingFlags.NonPublic);
            if (clickField != null)
            {
                eventKey = clickField.GetValue(null);
            }
        }

        if (eventKey != null)
        {
            // PATTERN A: WinForms Control Property Event (Uses EventHandlerList)
            PropertyInfo eventsProp = typeof(Component).GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
            if (eventsProp == null)
            {
                throw new InvalidOperationException("Could not find internal 'Events' container.");
            }

            EventHandlerList eventHandlerList = (EventHandlerList)eventsProp.GetValue(control);
            Delegate currentDelegate = eventHandlerList[eventKey];


            // Modern .NET runs FIFO. We must prepend 'newHandler' to achieve LIFO.
            Delegate updatedDelegate = Delegate.Combine(newHandler, currentDelegate);

            // 5. Commit the re-ordered hook back to WinForms
            eventHandlerList[eventKey] = updatedDelegate;
        }
        else
        {
            // PATTERN B: Custom / Standard C# Backing Field Event (e.g., 'SuperClick')
            FieldInfo? backingField = null;
            Type currentType = control.GetType();

            while (currentType != null)
            {
                backingField = currentType.GetField(eventInfo.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (backingField != null)
                {
                    break;
                }

                currentType = currentType.BaseType;
            }

            if (backingField == null)
            {
                throw new InvalidOperationException($"Could not locate event tracking container nor instance backing field for event '{eventInfo.Name}' on type {control.GetType().FullName}.");
            }

            Delegate currentDelegate = (Delegate)backingField.GetValue(control);
            Delegate updatedDelegate = Delegate.Combine(newHandler, currentDelegate);

            backingField.SetValue(control, updatedDelegate);
        }
    }

#if NETFRAMEWORK  // https://github.com/dotnet/designs/blob/main/accepted/2020/net5/net5.md#preprocessor-symbols
    private void AddAssertMenuItem(Control control, string name, EventHandler handler)
    {
        ContextMenu menu = control.ContextMenu;
        if (menu == null)
        {
            menu = new ContextMenu();
            control.ContextMenu = menu;
        }
        menu.MenuItems.Add(new MenuItem(name, handler));
    }
#endif

    public void FireEvent(Type testerType, object control, string name, params object[] args)
    {
        if (!InProcess(testerType, control, name))
        {
            OnEvent(testerType, control, new EventAction(name, args));
        }
        CheckForNewForms();
    }

    public void FireEvent(Type testerType, object control, EventAction action)
    {
        if (!InProcess(testerType, control, action.MethodName))
        {
            OnEvent(testerType, control, action);
        }
        CheckForNewForms();
    }

    public void FireEvent(Type testerType, object control, PropertyAssertAction action)
    {
        OnEvent(testerType, control, action);
        CheckForNewForms();
    }

    private bool InProcess(Type testerType, object control, string action)
    {
        if (inProcess)
        {
            if ((testerType == testerTypeInProcess) && (control == controlInProcess) && (action == actionInProcess))
            {
                inProcess = false;
            }
            return true;
        }

        inProcess = true;
        testerTypeInProcess = testerType;
        controlInProcess = control;
        actionInProcess = action;
        return false;
    }

    protected void OnEvent(Type testerType, object control, Action action)
    {
        if (Event != null)
        {
            Event(testerType, control, action);
        }
    }

    private void ControlAdded(object sender, ControlEventArgs e)
    {
        ListenTo(e.Control);
    }

    private void CheckForNewForms()
    {
        List<Form> forms = new FormFinder().FindAll();
        foreach (Form form in forms)
        {
            if (!(form is AppForm))
            {
                if (!formsIAmListeningTo.Contains(form))
                {
                    ListenTo(form);
                }
            }
        }
    }
}