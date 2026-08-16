/*----------------------------------------------------------------------------------------

    A-Soft Ingenieurbüro

    Copyright © 1994 - 2007. All Rights reserved.

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
    private readonly ScreenCapture capture;

    /// <summary>
    /// Helper to handle the file formats
    /// </summary>
    private readonly ImageFormatHandler handlers;

    /// <summary>
    /// The path of the executable, used to save the captured images.
    /// </summary>
    private readonly string pathName;

    /// <summary>
    /// Do it....
    /// </summary>
    public MainForm()
    {
        InitializeComponent();
        pathName = $"{Path.GetDirectoryName(Application.ExecutablePath)}\\Data\\";
        if (! Directory.Exists(pathName))
        {
            Directory.CreateDirectory(pathName);
        }

        handlers = new ImageFormatHandler();
        capture = new ScreenCapture(handlers);
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        var processes = Process.GetProcesses();

        foreach (var process in processes)
        {
            if (!string.IsNullOrEmpty(process.ProcessName) && process.MainWindowHandle != null)
            {
                listBoxProcesses.Items.Add(process.ProcessName);
            }
        }
    }

    private void buttonPrimaryScreen_Click(object sender, EventArgs e)
    {
        var filename = $"{pathName}Captured PrimaryScreen.png";
        capture.Capture(ScreenCapture.CaptureType.PrimaryScreen, filename,
            ImageFormatHandler.ImageFormatTypes.imgPNG);
    }

    private void buttonWorkingArea_Click(object sender, EventArgs e)
    {
        var filename = $"{pathName}Captured WorkingArea.png";
        capture.Capture(ScreenCapture.CaptureType.WorkingArea, filename, ImageFormatHandler.ImageFormatTypes.imgPNG);
    }

    private void buttonVirtualScreen_Click(object sender, EventArgs e)
    {
        var filename = $"{pathName}Captured VirtualScreen.png";
        capture.Capture(ScreenCapture.CaptureType.VirtualScreen, filename,
            ImageFormatHandler.ImageFormatTypes.imgPNG);
    }

    private void buttonAllScreens_Click(object sender, EventArgs e)
    {
        var filename = $"{pathName}Captured All.png";
        capture.Capture(ScreenCapture.CaptureType.AllScreens, filename, ImageFormatHandler.ImageFormatTypes.imgPNG);
    }

    private void buttonAllFormats_Click(object sender, EventArgs e)
    {
        var filename = $"{pathName}Capture Formats.png";
        capture.Capture(ScreenCapture.CaptureType.VirtualScreen, filename,
            ImageFormatHandler.ImageFormatTypes.imgPNG);
        capture.Capture(ScreenCapture.CaptureType.VirtualScreen, filename,
            ImageFormatHandler.ImageFormatTypes.imgBMP);
        capture.Capture(ScreenCapture.CaptureType.VirtualScreen, filename,
            ImageFormatHandler.ImageFormatTypes.imgGIF);
        capture.Capture(ScreenCapture.CaptureType.VirtualScreen, filename,
            ImageFormatHandler.ImageFormatTypes.imgJPEG);
        capture.Capture(ScreenCapture.CaptureType.VirtualScreen, filename,
            ImageFormatHandler.ImageFormatTypes.imgTIFF);
    }

    private void buttonPrintScreen_Click(object sender, EventArgs e)
    {
        capture.Print();
    }

    private void buttonProcess_Click(object sender, EventArgs e)
    {
        try
        {
            var processes = Process.GetProcessesByName(listBoxProcesses.SelectedItem.ToString());

            var filename = $"{pathName}Captured Other Process.png";
            var bitmap =
                capture.Capture(processes[0].MainWindowHandle, filename, ImageFormatHandler.ImageFormatTypes.imgPNG);
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox1.Image = bitmap;
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message);
        }
    }
}