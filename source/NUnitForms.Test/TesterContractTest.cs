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
'  following disclaimer.
' 
' * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and
'  the following disclaimer in the documentation and/or other materials provided with the distribution.
' 
' * Neither the name of the author nor the names of its contributors may be used to endorse or 
'  promote products derived from this software without specific prior written permission.
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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Windows.Forms;

using NUnit.Framework;
using NUnit.WinForms.Exceptions;
using NUnit.WinForms.Testers;


namespace NUnit.WinForms.Test;

[TestFixture]
public class TesterContractTest : NUnitFormTest
{
    public static IEnumerable<TestCaseData> ConcreteTesterTypes()
    {
        return typeof(ControlTester).Assembly.GetTypes()
            .Where(IsTesterWithStandardLookup)
            .OrderBy(type => type.Name)
            .Select(testerType => new TestCaseData(testerType)
                .SetName($"CommonTesterContract_{testerType.Name}"));
    }

    [Test]
    [TestCaseSource(nameof(ConcreteTesterTypes))]
    [Apartment(ApartmentState.STA)]
    public void CommonTesterContract(Type testerType)
    {
        Type targetType = GetTesterBase(testerType)!.GetGenericArguments()[0];
        var formName = $"TesterContractForm_{Guid.NewGuid():N}";
        var targetName = $"TesterContractTarget_{Guid.NewGuid():N}";
        var form = new Form { Name = formName };
        object firstTarget = AddTarget(form, targetType, targetName);
        form.Show();

        object nameTester = CreateTester(testerType, targetName);
        object formTester = CreateTester(testerType, targetName, form);
        object formNameTester = CreateTester(testerType, targetName, formName);

        AssertTesterFinds(nameTester, firstTarget, testerType);
        AssertTesterFinds(formTester, firstTarget, testerType);
        AssertTesterFinds(formNameTester, firstTarget, testerType);

        object secondTarget = AddTarget(form, targetType, targetName);
        Assert.That(GetPropertyValue(formTester, "Count"), Is.EqualTo(2), testerType.Name);
        AssertTesterFinds(GetIndexedTester(formTester, 0), firstTarget, testerType);
        AssertTesterFinds(GetIndexedTester(formTester, 1), secondTarget, testerType);
        AssertTesterFinds(CreateTester(testerType, formTester, 1), secondTarget, testerType);

        object[] enumeratedTargets = ((IEnumerable)formTester)
            .Cast<object>()
            .Select(tester => ((ReflectionTester)tester).TheObject!)
            .ToArray();
        Assert.That(enumeratedTargets, Is.EqualTo(new[] { firstTarget, secondTarget }), testerType.Name);

        var missingName = $"Missing_{targetName}";
        object missingTester = CreateTester(testerType, missingName, form);
        Assert.That(GetPropertyValue(missingTester, "Count"), Is.Zero, testerType.Name);
        Assert.Throws<NoSuchControlException>(() => GetPropertyValue(missingTester, "Properties"), testerType.Name);
        object missingIndexedTester = GetIndexedTester(missingTester, 0);
        Assert.Throws<NoSuchControlException>(() => GetPropertyValue(missingIndexedTester, "Properties"), testerType.Name);
        Assert.Throws<ArgumentOutOfRangeException>(() => GetIndexedTester(formTester, -1), testerType.Name);
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateTester(testerType, formTester, -1), testerType.Name);
    }

    private static bool IsTesterWithStandardLookup(Type type)
    {
        if (!type.IsClass || type.IsAbstract || type.ContainsGenericParameters ||
            type.Namespace != typeof(ControlTester).Namespace ||
            type == typeof(FormTester) || type == typeof(MessageBoxTester))
        {
            return false;
        }

        Type? testerBase = GetTesterBase(type);
        if (testerBase == null)
        {
            return false;
        }

        Type[] arguments = testerBase.GetGenericArguments();
        Type targetType = arguments[0];
        return arguments[1] == type &&
               (typeof(Control).IsAssignableFrom(targetType) ||
                typeof(ToolStripItem).IsAssignableFrom(targetType) ||
                targetType == typeof(Component));
    }

    private static Type? GetTesterBase(Type testerType)
    {
        for (Type? baseType = testerType; baseType != null; baseType = baseType.BaseType)
        {
            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(Tester<,>))
            {
                return baseType;
            }
        }

        return null;
    }

    private static object AddTarget(Form form, Type targetType, string name)
    {
        Type concreteType = GetConcreteTargetType(targetType);
        object target = Activator.CreateInstance(concreteType)
                        ?? throw new InvalidOperationException($"Could not create {concreteType.Name}.");

        if (target is ContextMenuStrip contextMenu)
        {
            contextMenu.Name = name;
            var owner = new Panel { Name = $"ContextMenuOwner_{Guid.NewGuid():N}", ContextMenuStrip = contextMenu };
            form.Controls.Add(owner);
            return target;
        }

        if (target is TabPage tabPage)
        {
            tabPage.Name = name;
            var tabs = new TabControl();
            tabs.TabPages.Add(tabPage);
            form.Controls.Add(tabs);
            return target;
        }

        if (target is ToolStripDropDown dropDown)
        {
            dropDown.Name = name;
            var owner = new ToolStripMenuItem { Name = $"DropDownOwner_{Guid.NewGuid():N}" };
            ToolStrip strip = GetOrCreateToolStrip(form);
            strip.Items.Add(owner);
            owner.DropDown = dropDown;
            return target;
        }

        if (target is Control control)
        {
            control.Name = name;
            form.Controls.Add(control);
            return target;
        }

        if (target is ToolStripItem item)
        {
            item.Name = name;
            GetOrCreateToolStrip(form).Items.Add(item);
            return target;
        }

        throw new InvalidOperationException($"No form host is defined for {targetType.Name}.");
    }

    private static ToolStrip GetOrCreateToolStrip(Form form)
    {
        ToolStrip? strip = form.Controls.OfType<ToolStrip>().FirstOrDefault();
        if (strip != null)
        {
            return strip;
        }

        strip = new ToolStrip { Name = $"TesterContractHost_{Guid.NewGuid():N}" };
        form.Controls.Add(strip);
        return strip;
    }

    private static Type GetConcreteTargetType(Type targetType)
    {
        Type[] preferredTypes =
        [
            typeof(Panel),
            typeof(UserControl),
            typeof(HScrollBar),
            typeof(ToolStripButton),
            typeof(ToolStripMenuItem),
            typeof(ToolStripComboBox),
            typeof(ToolStripTextBox)
        ];

        Type? preferredType = preferredTypes.FirstOrDefault(targetType.IsAssignableFrom);
        if (preferredType != null)
        {
            return preferredType;
        }

        if (!targetType.IsAbstract)
        {
            return targetType;
        }

        Type? concreteType = typeof(Control).Assembly.GetTypes()
            .FirstOrDefault(type => targetType.IsAssignableFrom(type) &&
                                    !type.IsAbstract &&
                                    type.GetConstructor(Type.EmptyTypes) != null);
        return concreteType ?? throw new InvalidOperationException($"No concrete form object is available for {targetType.Name}.");
    }

    private static object CreateTester(Type testerType, params object[] arguments)
    {
        try
        {
            return Activator.CreateInstance(testerType, arguments)
                   ?? throw new InvalidOperationException($"Could not create {testerType.Name}.");
        }
        catch (TargetInvocationException exception) when (exception.InnerException != null)
        {
            ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
            throw;
        }
    }

    private static object GetIndexedTester(object tester, int index)
    {
        PropertyInfo indexer = tester.GetType().GetProperties()
            .Single(property =>
            {
                ParameterInfo[] parameters = property.GetIndexParameters();
                return property.Name == "Item" && parameters.Length == 1 && parameters[0].ParameterType == typeof(int);
            });
        return GetPropertyValue(tester, indexer, [index])!;
    }

    private static object? GetPropertyValue(object tester, string propertyName)
    {
        PropertyInfo property = tester.GetType().GetProperty(propertyName)
                                ?? throw new InvalidOperationException($"{tester.GetType().Name} has no {propertyName} property.");
        return GetPropertyValue(tester, property, null);
    }

    private static object? GetPropertyValue(object tester, PropertyInfo property, object[]? indexArguments)
    {
        try
        {
            return property.GetValue(tester, indexArguments);
        }
        catch (TargetInvocationException exception) when (exception.InnerException != null)
        {
            ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
            throw;
        }
    }

    private static void AssertTesterFinds(object tester, object expected, Type testerType)
    {
        Assert.That(((ReflectionTester)tester).TheObject, Is.SameAs(expected), testerType.Name);
        Assert.That(GetPropertyValue(tester, "Properties"), Is.SameAs(expected), testerType.Name);
    }
}