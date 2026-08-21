/*----------------------------------------------------------------------------------------

    A-Soft Ingenieurbüro

    Copyright © 1994 - 2007. All Rights reserved.
    Modernisation 2026-2026 Smurf.IV

    Related Copyrights :

            Microsoft .NET Windows Forms V2.0 library.
            Copyright (C) 2004...2006 Microsoft Corporation,
            All rights reserved.


    FILE		:	MainForm.cs

    PROJECT		:	A-Soft Library
    SUB			:	Standard Library

    SYSTEM		:	Windows-XP, (Windows 2000), C# (.NET 2.0, Visual Studio.NET 2005)

    AUTHOR		:	Joachim Holzhauer

    DESCRIPTION	:	Main form of screen capture application

    VERSION		:	1.0 - 2006.01.31

----------------------------------------------------------------------------------------*/

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


namespace NUnit.Extensions.Forms.ScreenCapture;

/// <summary>
/// Main form of screen capture application
/// </summary>
public partial class MainForm : Form
{
    /// <summary>
    /// The class instance used to capture the screen
    /// </summary>
    private readonly ScreenCapture _capture;

    /// <summary>
    /// The path of the executable, used to save the captured images.
    /// </summary>
    private readonly string _pathName;

    /// <summary>
    /// Do it....
    /// </summary>
    public MainForm()
    {
        InitializeComponent();
        _pathName = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Data");
        if (!Directory.Exists(_pathName))
        {
            Directory.CreateDirectory(_pathName);
        }

        var handlers1 = new ImageFormatHandler();
        _capture = new ScreenCapture(handlers1);
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        Process[] processes = Process.GetProcesses();

        foreach (Process process in processes)
        {
            if (!string.IsNullOrEmpty(process.ProcessName)
                && process.MainWindowHandle != IntPtr.Zero)
            {
                listBoxProcesses.Items.Add(process.ProcessName);
            }
        }
    }

    private void buttonPrimaryScreen_Click(object sender, EventArgs e)
    {
        var filename = Path.Combine(_pathName, "Captured PrimaryScreen.png");
        _capture.Capture(ScreenCapture.CaptureType.PrimaryScreen, filename,
            ImageFormatHandler.ImageFormatTypes.imgPNG);
    }

    private void buttonWorkingArea_Click(object sender, EventArgs e)
    {
        var filename = Path.Combine(_pathName, "Captured WorkingArea.png");
        _capture.Capture(ScreenCapture.CaptureType.WorkingArea, filename, ImageFormatHandler.ImageFormatTypes.imgPNG);
    }

    private void buttonVirtualScreen_Click(object sender, EventArgs e)
    {
        var filename = Path.Combine(_pathName, "Captured VirtualScreen.png");
        _capture.Capture(ScreenCapture.CaptureType.VirtualScreen, filename,
            ImageFormatHandler.ImageFormatTypes.imgPNG);
    }

    private void buttonAllScreens_Click(object sender, EventArgs e)
    {
        var filename = Path.Combine(_pathName, "Captured All Screens.png");
        _capture.Capture(ScreenCapture.CaptureType.AllScreens, filename, ImageFormatHandler.ImageFormatTypes.imgPNG);
    }

    private void buttonAllFormats_Click(object sender, EventArgs e)
    {
        var filename = Path.Combine(_pathName, "Capture Formats.All");
        _capture.Capture(ScreenCapture.CaptureType.VirtualScreen, filename,
            ImageFormatHandler.ImageFormatTypes.imgPNG);
        _capture.Capture(ScreenCapture.CaptureType.VirtualScreen, filename,
            ImageFormatHandler.ImageFormatTypes.imgBMP);
        _capture.Capture(ScreenCapture.CaptureType.VirtualScreen, filename,
            ImageFormatHandler.ImageFormatTypes.imgGIF);
        _capture.Capture(ScreenCapture.CaptureType.VirtualScreen, filename,
            ImageFormatHandler.ImageFormatTypes.imgJPEG);
        _capture.Capture(ScreenCapture.CaptureType.VirtualScreen, filename,
            ImageFormatHandler.ImageFormatTypes.imgTIFF);
    }

    private void buttonPrintScreen_Click(object sender, EventArgs e)
    {
        _capture.Print();
    }

    private void buttonProcess_Click(object sender, EventArgs e)
    {
        try
        {
            Process[] processes = Process.GetProcessesByName(listBoxProcesses.SelectedItem.ToString());

            var filename = Path.Combine(_pathName, "Captured Other Process.png");
            Bitmap bitmap =
                _capture.Capture(processes[0].MainWindowHandle, filename, ImageFormatHandler.ImageFormatTypes.imgPNG);
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox1.Image = bitmap;
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message);
        }
    }
}