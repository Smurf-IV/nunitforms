using System;
using System.Windows.Forms;


namespace NUnit.WinForms.Test.TestForms;

public partial class SaveFileDialogTestForm : Form
{
    private string defaultFilename = string.Empty;

    public SaveFileDialogTestForm()
    {
        InitializeComponent();
    }

    public void SetDefaultTestFileName(string filename)
    {
        defaultFilename = filename;
    }

    private void btSave_Click(object sender, EventArgs e)
    {
        var save_dlg = new SaveFileDialog { AutoUpgradeEnabled = false };
        if (defaultFilename != string.Empty)
        {
            save_dlg.FileName = defaultFilename;
        }
        save_dlg.RestoreDirectory = true;

        if (save_dlg.ShowDialog() == DialogResult.OK)
        {
            lblFileName.Text = save_dlg.FileName;
        }
        else
        {
            lblFileName.Text = "cancel pressed";
        }
    }
}