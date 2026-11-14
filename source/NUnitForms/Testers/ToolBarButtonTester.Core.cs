#if !NETFRAMEWORK
using System.Windows.Forms;
using NUnit.Extensions.Forms.Testers;

#pragma warning disable IDE0130
namespace NUnit.Extensions.Forms.Testers;

/// <summary>
/// Compatibility shim for the old ToolBarButtonTester API on non-NETFRAMEWORK targets.
/// Internally uses ToolStripItemTester / ToolStrip items.
/// </summary>
public class ToolBarButtonTester
{
    private readonly ToolStripItemTester _inner;

    public ToolBarButtonTester()
    {
    }

    public ToolBarButtonTester(string name, Form form)
    {
        _inner = new ToolStripItemTester(name, form);
    }

    public ToolBarButtonTester(string name, string formName)
    {
        _inner = new ToolStripItemTester(name, formName);
    }

    public ToolBarButtonTester(string name)
    {
        _inner = new ToolStripItemTester(name);
    }


    public ToolStripItem Button => _inner.Properties;

    public ToolStrip Bar => (ToolStrip)Button.Owner;

    public bool Pushed
    {
        get => (Button as ToolStripButton)?.Checked ?? false;
        set
        {
            if (Button is ToolStripButton b)
            {
                b.Checked = value;
            }
        }
    }

    // ToolStrip has no "partial push" semantics; store a boolean in Tag to emulate it for tests.
    public bool PartialPushed
    {
        get => Button.Tag is bool b && b;
        set => Button.Tag = value;
    }

    public void Click()
    {
        Button.PerformClick();
    }

    public void ClickDropDownMenuItem(string menuText)
    {
        if (Button is ToolStripDropDownItem dd)
        {
            foreach (ToolStripItem it in dd.DropDownItems)
            {
                if (it.Text == menuText)
                {
                    it.PerformClick();
                    return;
                }
            }
        }

        // If not a dropdown, do nothing (keeps behavior safe for tests).
    }

    public void Push()
    {
        Pushed = !Pushed;
    }

    public void PartialPush()
    {
        PartialPushed = !PartialPushed;
    }
}
#endif