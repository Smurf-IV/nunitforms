#if !NETFRAMEWORK
using System.Windows.Forms;
using NUnit.Extensions.Forms.Generic_Testers;

#pragma warning disable IDE0130
namespace NUnit.Extensions.Forms.Testers;

public partial class ToolBarTester : ControlTester<ToolStrip, ToolBarTester>
{
    public ToolBarTester()
    {
    }

    public ToolBarTester(string name, Form form)
        : base(name, form)
    {
    }

    public ToolBarTester(string name, string formName)
        : base(name, formName)
    {
    }

    public ToolBarTester(string name)
        : base(name)
    {
    }

    public ToolBarTester(ToolBarTester tester, int index)
        : base(tester, index)
    {
    }

    public ToolBarButtonTester GetButton(string name)
    {
        return new ToolBarButtonTester(name, Properties.FindForm());
    }

    public ToolBarButtonTester GetButton(int index)
    {
        if (index < 0 || index >= Properties.Items.Count)
        {
            throw new System.ArgumentOutOfRangeException(nameof(index));
        }
        var item = Properties.Items[index];
        var itemName = string.IsNullOrEmpty(item.Name) ? item.Text : item.Name;
        return new ToolBarButtonTester(itemName, Properties.FindForm());
    }
}
#endif