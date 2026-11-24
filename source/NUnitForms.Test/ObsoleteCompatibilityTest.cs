#region Copyright (c) 2006-2007, Luke T. Maxon : 2026-2026 Smurf.IV

/********************************************************************************************************************
'
' Copyright (c) 2006-2007, Luke T. Maxon
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
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using NUnit.Framework;
using NUnit.WinForms.Exceptions;
using NUnit.WinForms.Testers;

namespace NUnit.WinForms.Test;

[TestFixture]
[Apartment(ApartmentState.STA)]
public class ObsoleteCompatibilityTest : NUnitFormTest
{
    [Test]
    public void ParameterlessFileDialogLookupRemainsAvailableToDerivedTesters()
    {
        var method = typeof(FileDialogTester).GetMethod("FindFileDialog",
            BindingFlags.Static | BindingFlags.NonPublic, null, Type.EmptyTypes, null);

        Assert.That(method, Is.Not.Null);
        Assert.That(method!.IsFamily, Is.True);
        Assert.That(method.ReturnType, Is.EqualTo(typeof(IntPtr)));
    }

    [Test]
    public void LegacyHandlerCanInstallModernHandlerForNextModal()
    {
        using var tester = new ModalFormTester();
        using var form = new Form();
        int calls = 0;
        ModalFormHandler next = (_, _, _) => calls++;

#pragma warning disable CS0618
        tester.ExpectModal("Legacy", () => tester.FormHandler = next);
#pragma warning restore CS0618
        InvokeModal(tester, "Legacy", form);

        Assert.That(tester.FormHandler, Is.EqualTo(next));
        Assert.That(tester.Verify().AllModalsShown, Is.False);
        InvokeModal(tester, "Modern", form);
        Assert.That(calls, Is.EqualTo(1));
        Assert.That(tester.Verify().AllModalsShown, Is.True);
    }

    [Test]
    public void LegacyVerificationIncludesUnexpectedModernModals()
    {
        using var tester = new ModalFormTester();
        using var form = new Form();
#pragma warning disable CS0618
        tester.ExpectModal("Legacy", () => { });
#pragma warning restore CS0618
        InvokeModal(tester, "Legacy", form);
        tester.FormHandler = null;
        InvokeModal(tester, "Unexpected", form);

        var result = tester.Verify();
        Assert.That(result.AllModalsShown, Is.True);
        Assert.That(result.UnexpectedModals, Does.Contain("Unexpected"));
    }

    [Test]
    public void LegacyModalHandlerCanMatchManagedFormCaption()
    {
        using var tester = new ModalFormTester();
        using var form = new Form { Name = "FormName", Text = "FormCaption" };
        bool called = false;
#pragma warning disable CS0618
        tester.ExpectModal(form.Text, () => called = true);
#pragma warning restore CS0618

        InvokeModal(tester, form.Name, form);

        Assert.That(called, Is.True);
        Assert.That(tester.Verify().AllModalsShown, Is.True);
    }

    [Test]
    public void LegacyModalHandlerPrefersFormNameWhenBothMatch()
    {
        using var tester = new ModalFormTester();
        using var form = new Form { Name = "FormName", Text = "FormCaption" };
        string? matched = null;
#pragma warning disable CS0618
        tester.ExpectModal(form.Name, () => matched = "name");
        tester.ExpectModal(form.Text, () => matched = "caption", expected: false);
#pragma warning restore CS0618

        InvokeModal(tester, form.Name, form);

        Assert.That(matched, Is.EqualTo("name"));
        Assert.That(tester.Verify().AllModalsShown, Is.True);
    }

    [Test]
    public void VerificationDrainsMessagesWithLegacyHandlerRegistered()
    {
        using var control = new Control();
        _ = control.Handle;
        bool invoked = false;
#pragma warning disable CS0618
        ExpectModal("NotShown", () => { }, expected: false);
#pragma warning restore CS0618
        control.BeginInvoke((Action)(() => invoked = true));

        Verify();

        Assert.That(invoked, Is.True);
    }

    [Test]
    public void LegacyMessageBoxTesterFindsTheNamedDialog()
    {
        string title = $"NUnitFormsMessage_{Guid.NewGuid():N}";
        string? actualTitle = null;
        string? actualText = null;
        Exception? lookupException = null;
        DialogBoxHandler = (_, handle) =>
        {
            try
            {
#pragma warning disable CS0618
                var tester = new MessageBoxTester(title);
#pragma warning restore CS0618
                actualTitle = tester.Title;
                actualText = tester.Text;
                tester.ClickOk();
            }
            catch (Exception exception)
            {
                lookupException = exception;
                new MessageBoxTester(handle).ClickOk();
            }
        };

        Assert.That(MessageBox.Show("Message", title), Is.EqualTo(DialogResult.OK));
        Assert.That(lookupException, Is.Null);
        Assert.That(actualTitle, Is.EqualTo(title));
        Assert.That(actualText, Is.EqualTo("Message"));
    }

    public static IEnumerable<TestCaseData> FileDialogTesterTypes()
    {
        yield return new TestCaseData(typeof(FileDialogTester)).SetName("FileDialogTester_StringConstructorUsesTitle");
        yield return new TestCaseData(typeof(OpenFileDialogTester)).SetName("OpenFileDialogTester_StringConstructorUsesTitle");
        yield return new TestCaseData(typeof(SaveFileDialogTester)).SetName("SaveFileDialogTester_StringConstructorUsesTitle");
    }

    [Test]
    public void StringConstructorFindsDialogWithConfiguredTitle()
    {
        string title = $"NUnitFormsConfigured_{Guid.NewGuid():N}";
        Exception? lookupException = null;
        ModalFormHandler = (_, handle, _) =>
        {
            try
            {
#pragma warning disable CS0618
                new OpenFileDialogTester(title).ClickCancel();
#pragma warning restore CS0618
            }
            catch (Exception exception)
            {
                lookupException = exception;
                new OpenFileDialogTester(handle).ClickCancel();
            }
        };

        Assert.That(ShowOpenDialog(title), Is.EqualTo(DialogResult.Cancel));
        Assert.That(lookupException, Is.Null);
    }

    [TestCaseSource(nameof(FileDialogTesterTypes))]
    public void StringConstructorFindsDialogByTitle(Type testerType)
    {
        string title = $"NUnitFormsDialog_{Guid.NewGuid():N}";
        Exception? lookupException = null;
        bool titleWasSet = false;
        ModalFormHandler = (_, handle, _) =>
        {
            titleWasSet = SetWindowText(handle, title);
            try
            {
                var tester = (FileDialogTester)(Activator.CreateInstance(testerType, title)
                                                ?? throw new InvalidOperationException($"Could not create {testerType.Name}."));
                tester.ClickCancel();
            }
            catch (Exception exception)
            {
                lookupException = exception;
                new OpenFileDialogTester(handle).ClickCancel();
            }
        };

        Assert.That(ShowOpenDialog(title), Is.EqualTo(DialogResult.Cancel));
        Assert.That(titleWasSet, Is.True);
        Assert.That(lookupException, Is.Null, testerType.Name);
    }

    [Test]
    public void StringConstructorDoesNotSelectAnotherVisibleDialogWhenTitleIsMissing()
    {
        Exception? lookupException = null;
        ModalFormHandler = (_, handle, _) =>
        {
            try
            {
                new OpenFileDialogTester($"Missing_{Guid.NewGuid():N}").ClickCancel();
            }
            catch (Exception exception)
            {
                lookupException = exception;
            }

            new OpenFileDialogTester(handle).ClickCancel();
        };

        Assert.That(ShowOpenDialog(FileDialogTester.InitialFileDialogName), Is.EqualTo(DialogResult.Cancel));
        Assert.That(lookupException, Is.TypeOf<ControlNotVisibleException>());
    }

    [Test]
    public void HandleConstructorTargetsDialogWithCustomTitle()
    {
        string title = $"NUnitFormsHandle_{Guid.NewGuid():N}";
        Exception? lookupException = null;
        bool titleWasSet = false;
        ModalFormHandler = (_, handle, _) =>
        {
            titleWasSet = SetWindowText(handle, title);
            try
            {
                new OpenFileDialogTester(handle).ClickCancel();
            }
            catch (Exception exception)
            {
                lookupException = exception;
                new OpenFileDialogTester(handle).ClickCancel();
            }
        };

        Assert.That(ShowOpenDialog(title), Is.EqualTo(DialogResult.Cancel));
        Assert.That(titleWasSet, Is.True);
        Assert.That(lookupException, Is.Null);
    }

    [Test]
    public void ZeroHandleConstructorRetainsDefaultDialogDiscovery()
    {
        Exception? lookupException = null;
        ModalFormHandler = (_, handle, _) =>
        {
            try
            {
                new OpenFileDialogTester(IntPtr.Zero).ClickCancel();
            }
            catch (Exception exception)
            {
                lookupException = exception;
                new OpenFileDialogTester(handle).ClickCancel();
            }
        };

        Assert.That(ShowOpenDialog(FileDialogTester.InitialFileDialogName), Is.EqualTo(DialogResult.Cancel));
        Assert.That(lookupException, Is.Null);
    }

    [Test]
    public void LegacyNamedHandlersRemainRegisteredForSubsequentModals()
    {
        using var firstForm = CreateModalForm("LegacyFirst");
        using var secondForm = CreateModalForm("LegacySecond");
        int handlerCalls = 0;

#pragma warning disable CS0618
        ExpectModal(firstForm.Name, () =>
        {
            handlerCalls++;
            firstForm.Close();
        });
        ExpectModal(secondForm.Name, () =>
        {
            handlerCalls++;
            secondForm.Close();
        });
#pragma warning restore CS0618

        firstForm.ShowDialog();
        secondForm.ShowDialog();

        Assert.That(handlerCalls, Is.EqualTo(2));
        Assert.DoesNotThrow(Verify);
    }

    [Test]
    public void ModernOneShotHandlerCanRegisterTheNextModalHandler()
    {
        using var firstForm = CreateModalForm("ModernFirst");
        using var secondForm = CreateModalForm("ModernSecond");
        int firstHandlerCalls = 0;
        int secondHandlerCalls = 0;

        ModalFormHandler = (_, _, _) =>
        {
            firstHandlerCalls++;
            ModalFormHandler = (_, _, form) =>
            {
                secondHandlerCalls++;
                form.Close();
            };
            firstForm.Close();
        };

        firstForm.ShowDialog();
        secondForm.ShowDialog();

        Assert.That(firstHandlerCalls, Is.EqualTo(1));
        Assert.That(secondHandlerCalls, Is.EqualTo(1));
        Assert.DoesNotThrow(Verify);
    }

    [Test]
    public void LegacyFileDialogHandlerHonorsExpectedFalse()
    {
        string title = FileDialogTester.InitialFileDialogName;

#pragma warning disable CS0618
        ExpectFileDialog(() => new OpenFileDialogTester(title).ClickCancel(), expected: false);
#pragma warning restore CS0618

        Assert.That(ShowOpenDialog(title), Is.EqualTo(DialogResult.Cancel));
        FormsTestAssertionException exception = Assert.Throws<FormsTestAssertionException>(Verify)!;
        Assert.That(exception.Message, Does.Contain("Expected Modal Form did not show"));
    }

    private DialogResult ShowOpenDialog(string title)
    {
        using var dialog = new OpenFileDialog { Title = title, AutoUpgradeEnabled = false };
        return dialog.ShowDialog();
    }

    private static Form CreateModalForm(string name)
    {
        return new Form { Name = name, Text = name };
    }

    private static void InvokeModal(ModalFormTester tester, string name, Form form)
    {
        typeof(ModalFormTester).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(tester, [name, IntPtr.Zero, form]);
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool SetWindowText(IntPtr handle, string text);
}