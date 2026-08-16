using System;
using System.IO;
using System.Windows.Forms;


namespace NUnit.Extensions.Forms.TestApplications.TestForms;

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
        var save_dlg = new SaveFileDialog();
        if (defaultFilename != string.Empty)
        {
            save_dlg.InitialDirectory = Path.GetDirectoryName(defaultFilename);
            save_dlg.FileName = Path.GetFileName(defaultFilename);
        }

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