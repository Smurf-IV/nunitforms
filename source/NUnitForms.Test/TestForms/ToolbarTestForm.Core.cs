#if !NETFRAMEWORK
using System;
using System.Windows.Forms;

namespace NUnit.Extensions.Forms.TestApplications.TestForms;

public class ToolbarTestForm : Form
{
    private ToolStrip toolStripTest;
    private ToolStripLabel labelToolbarSelection;

    public ToolbarTestForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        toolStripTest = new ToolStrip();
        labelToolbarSelection = new ToolStripLabel("labelToolbarSelection") { Name = "labelToolbarSelection" };

        // Create buttons similar to the old ToolBar test form
        var btnOpen = new ToolStripButton("Open") { Name = "Open" };
        var btnClose = new ToolStripButton("Close") { Name = "Close" };
        var btnNext = new ToolStripButton("Next") { Name = "Next" };
        var btnPrevious = new ToolStripButton("Previous") { Name = "Previous" };
        var sep = new ToolStripSeparator() { Name = "Seperator" };

        // Drop-down button for Color
        var colorDrop = new ToolStripDropDownButton("Color") { Name = "Color" };
        var red = new ToolStripMenuItem("Red");
        var orange = new ToolStripMenuItem("Orange");
        var yellow = new ToolStripMenuItem("Yellow");
        var green = new ToolStripMenuItem("Green");
        var blue = new ToolStripMenuItem("Blue");
        var indigo = new ToolStripMenuItem("Indigo");
        var violet = new ToolStripMenuItem("Violet");
        colorDrop.DropDownItems.AddRange(new ToolStripItem[] { red, orange, yellow, green, blue, indigo, violet });

        // Toggle button (uses Checked property)
        var toggle = new ToolStripButton("Toggle") { Name = "Toggle", CheckOnClick = true };

        // Wire events so tests can observe text changes like the original form
        toolStripTest.ItemClicked += ToolStripTest_ItemClicked;
        red.Click += MenuItem_Click;
        orange.Click += MenuItem_Click;
        yellow.Click += MenuItem_Click;
        green.Click += MenuItem_Click;
        blue.Click += MenuItem_Click;
        indigo.Click += MenuItem_Click;
        violet.Click += MenuItem_Click;

        toolStripTest.Items.AddRange(new ToolStripItem[] { btnOpen, btnClose, btnNext, btnPrevious, sep, colorDrop, toggle });

        // Place a Label on the form to mirror original test expectations
        var label = new Label { Name = "labelToolbarSelection", Text = "label1", Location = new System.Drawing.Point(80, 80) };

        Controls.Add(toolStripTest);
        Controls.Add(label);

        toolStripTest.Name = "toolBarTest"; // preserve the original tested name

        Text = "ToolbarTestForm";
        ClientSize = new System.Drawing.Size(352, 272);
    }

    private void MenuItem_Click(object sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem m && Controls.Find("labelToolbarSelection", true)[0] is Label label)
        {
            label.Text = m.Text;
        }
    }

    private void ToolStripTest_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
        if (Controls.Find("labelToolbarSelection", true)[0] is Label label)
        {
            label.Text = e.ClickedItem.Text;
        }
    }
}
#endif