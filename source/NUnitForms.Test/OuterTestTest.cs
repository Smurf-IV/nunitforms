using System;
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;
using NUnit.WinForms.Exceptions;
using NUnit.WinForms.Testers;


namespace NUnit.WinForms.Test;

[TestFixture]
[Apartment(ApartmentState.STA)]
public class OuterTestTest
{
    [Test]
    public void RunTwiceWithoutFailure()
    {
        using (var nuf = new OuterTest())
        {
            using (var dlg = new OpenFileDialog { AutoUpgradeEnabled = false })
            {
                dlg.CheckFileExists = false;

                nuf.ModalFormHandler = delegate (string name, IntPtr hWnd, Form form)
                {
                    new OpenFileDialogTester(hWnd).ClickCancel();
                };

                dlg.ShowDialog();
            }
        }
        using (var nuf = new OuterTest())
        {
            using (var dlg = new OpenFileDialog { AutoUpgradeEnabled = false })
            {
                dlg.CheckFileExists = false;

                nuf.ModalFormHandler = delegate (string name, IntPtr hWnd, Form form)
                {
                    new OpenFileDialogTester(hWnd).ClickCancel();
                };

                dlg.ShowDialog();
            }
        }
        Assert.Pass();
    }

    // Put this here, because ExpectedException does not work when the exn is thrown in teardown
    [Test]
    public void DanglingWindowMessage()
    {
        var ex = Assert.Throws<FormsTestAssertionException>(() =>
        {
            using var nuf = new OuterTest();
            var f = new Form();
            f.Show();
            var w =
                new EventWaitHandle(false, EventResetMode.AutoReset);
            ThreadPool.QueueUserWorkItem(delegate
            {
                f.BeginInvoke(new MethodInvoker(delegate { MessageBox.Show("", "Blah"); }));
                w.Set();
            });
            w.WaitOne();
        });
        Assert.That(ex.Message, Does.Contain("Blah"));
    }

}