using System;
using System.Threading;
using System.Windows.Forms;

using NUnit.Extensions.Forms.Exceptions;
using NUnit.Extensions.Forms.Testers;
using NUnit.Framework;


namespace NUnit.Extensions.Forms.TestApplications;

[TestFixture]
public class OuterTestTest
{
    [Test, STAThread]
    [Explicit("Hangs for some reason - Does not show the first dialog")]
    public void RunTwiceWithoutFailure()
    {
        using (var nuf = new OuterTest())
        {
            using (var dlg = new OpenFileDialog())
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
            using (var dlg = new OpenFileDialog())
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