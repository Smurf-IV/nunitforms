/*----------------------------------------------------------------------------------------

    A-Soft Ingenieurbüro

    Copyright © 1994 - 2007. All Rights reserved.

    Related Copyrights :

            Microsoft .NET Windows Forms V2.0 library.
            Copyright (C) 2004...2006 Microsoft Corporation,
            All rights reserved.


    FILE		:	ScreenCapture.cs

    PROJECT		:	A-Soft Library
    SUB			:	Standard Library

    SYSTEM		:	Windows-XP, (Windows 2000), C# (.NET 2.0, Visual Studio.NET 2005)

    AUTHOR		:	Joachim Holzhauer

    DESCRIPTION	:	This class implements the capturing of a screen (form, complete desktop etc.)

    VERSION		:	1.0 - 2006.01.31

----------------------------------------------------------------------------------------*/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;

namespace NUnit.Extensions.Forms.ScreenCapture;

///<summary>
/// This delegate defines a method that takes a window handle
/// and generates a bitmap.
///</summary>
///<param name="handle">The window handle of the window to capture.</param>
public delegate Bitmap CaptureHandleDelegateHandler(IntPtr handle);

/// <summary>
/// This class implements the capturing of a screen (form, complete desktop etc.).
/// The captured image(s) can be saved into a file in different formats, 
/// and it can be printed.
/// </summary>
public class ScreenCapture
{
    #region CaptureType enum

    /// <summary>
    /// Define the type of screen capture.
    /// </summary>
    public enum CaptureType
    {
        /// <summary>
        /// Capture the complete virtual screen (on multi monitor applications all screens).
        /// </summary>
        VirtualScreen,

        /// <summary>
        /// Capture the complete primary screen, including the taskbar.
        /// </summary>
        PrimaryScreen,

        /// <summary>
        /// Capture only the working area of the primary screen, this excludes the taskbar.
        /// </summary>
        WorkingArea,

        /// <summary>
        /// On a multi monitor system capture all screens in different images.
        /// </summary>
        AllScreens
    }

    #endregion

    /// <summary>
    /// Used for printing the captured object
    /// </summary>
    private readonly PrintDocument doc = new PrintDocument();

    /// <summary>
    /// Handler for the different graphic formats
    /// </summary>
    private ImageFormatHandler? formatHandler;

    /// <summary>
    /// The actual image used in printing
    /// </summary>
    private Bitmap image;

    /// <summary>
    /// These are all captured images.
    /// </summary>
    private Bitmap[]? images;

    /// <summary>
    /// Creator
    /// </summary>
    public ScreenCapture()
    {
        doc.PrintPage += printPage;
        formatHandler = new ImageFormatHandler();
    }

    /// <summary>
    /// Creator, set format handler
    /// </summary>
    /// <param name="formatHandler">The format handler instance</param>
    public ScreenCapture(ImageFormatHandler formatHandler)
    {
        doc.PrintPage += printPage;

        this.formatHandler = formatHandler;
    }

    /// <summary>
    /// Define a format handler
    /// </summary>
    public ImageFormatHandler FormatHandler
    {
        set => formatHandler = value;
    }

    /// <summary>
    /// Gets the file path of the last captured screen shot.
    /// </summary>
    /// <value>
    /// The path and file name of the last captured screen shot.
    /// </value>
    /// <exception cref=" ArgumentException">
    /// This exception is thrown if the value is not effective.
    /// </exception>
    public string LastCapture
    {
        get;
        protected
        set
        {
            if (value != null)
            {
                field = value;
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }

    /// <summary>
    /// Capture a screen shot of a <see cref="Form"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="Bitmap"/> screen shot of <paramref name="form"/>
    /// </returns>        
    public Bitmap Capture(Form form, string screenShotPath)
    {
        if (!Directory.Exists(Path.GetDirectoryName(screenShotPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(screenShotPath));
        }
        return
            Capture(form, GenerateUniqueName(form.Name, screenShotPath), ImageFormatHandler.ImageFormatTypes.imgPNG);
    }

    /// <summary>
    /// Generate a unique name for a <c>PNG</c> file.
    /// </summary>
    /// <param name="formName">
    /// The name of the form which is captured.
    /// </param>
    /// <param name="path">
    /// The path where the capture of the form will be stored.
    /// </param>
    /// <returns>
    /// A unique <c>PNG</c> file name : <c>path + formName + number + .png</c>
    /// </returns>
    private string GenerateUniqueName(string formName, string path)
    {
        var counter = 1;
        while (File.Exists(path + formName + "_" + counter + ".png"))
        {
            counter++;
        }
        LastCapture = path + formName + "_" + counter + ".png";
        return LastCapture;
    }

    /// <summary>
    /// Capture a specific form and save it into a file.
    /// </summary>
    /// <param name="window">This is the desired window which should be captured.</param>
    /// <param name="filename">The name of the target file. The extension in there is ignored, 
    /// it will replaced by an extension derived from the desired file format.</param>
    /// <param name="format">The format of the file.</param>
    /// <returns>The image which has been captured.</returns>
    public virtual Bitmap Capture(Form window, string filename, ImageFormatHandler.ImageFormatTypes format)
    {
        window.Focus();
        return Capture(window, filename, format, false);
    }

    /// <summary>
    /// Capture a specific form and save it into a file.
    /// </summary>
    /// <param name="window">This is the desired window which should be captured.</param>
    /// <param name="filename">The name of the target file. The extension in there is ignored, 
    /// it will replaced by an extension derived from the desired file format.</param>
    /// <param name="format">The format of the file.</param>
    /// <param name="onlyClient">When set to 'true' then only the client area of the form is captured,
    /// otherwise the complete window with title bar, frame etc. is captured.</param>
    /// <returns>The image which has been captured.</returns>
    public virtual Bitmap Capture(Form window, string filename, ImageFormatHandler.ImageFormatTypes format,
        bool onlyClient)
    {
        Capture(window, onlyClient);
        Save(filename, format);
        return images[0];
    }

    /// <summary>
    /// Execute the capturing of window specified by it's windows handle.
    /// </summary>
    /// <param name="handle">The handle of the window to capture</param>
    /// <param name="filename">The name of the target file. The extension in there is ignored, 
    /// it will replaced by an extension derived from the desired file format.</param>
    /// <param name="format">The format of the file.</param>
    /// <returns>The image which has been captured.</returns>
    /// <remarks>
    /// This call uses the <i>Win32 API</i> and should therefore not be used in your
    /// code if you don't want to depend on <i>Win32</i>. <c>internal</c> Takes care
    /// of this issue.
    /// </remarks>
    internal Bitmap Capture(IntPtr handle, string filename, ImageFormatHandler.ImageFormatTypes format)
    {
        Capture(handle);
        Save(filename, format);
        return images[0];
    }

    /// <summary>
    /// Capture a specific control in the client area of a form.
    /// </summary>
    /// <param name="window">This is a control which should be captured.</param>
    /// <param name="filename">The name of the target file. The extension in there is ignored, 
    /// it will replaced by an extension derived from the desired file format.</param>
    /// <param name="format">The format of the file.</param>
    /// <returns>The image which has been captured.</returns>
    public virtual Bitmap CaptureControl(Control window, string filename, ImageFormatHandler.ImageFormatTypes format)
    {
        CaptureControl(window);
        Save(filename, format);
        return images[0];
    }

    /// <summary>
    /// Capture a specific control in the client area of a form.
    /// </summary>
    /// <param name="window">This is a control which should be captured.</param>
    /// <returns>The image which has been captured.</returns>
    public virtual Bitmap CaptureControl(Control window)
    {
        var rc = window.RectangleToScreen(window.DisplayRectangle);
        window.Update();
        return capture(window, rc);
    }

    /// <summary>
    /// Capture a specific form.
    /// </summary>
    /// <param name="window">This is the desired window which should be captured.</param>
    /// <param name="onlyClient">When set to 'true' then only the client area of the form is captured,
    /// otherwise the complete window with title bar, frame etc. is captured.</param>
    /// <returns>The image which has been captured.</returns>
    public virtual Bitmap Capture(Form window, bool onlyClient)
    {
        if (!onlyClient)
        {
            return Capture(window);
        }

        var rc = window.RectangleToScreen(window.ClientRectangle);
        return capture(window, rc);
    }

    /// <summary>
    /// Capture a specific form.
    /// </summary>
    /// <param name="window">This is the desired window which should be captured.</param>
    /// <returns>The image which has been captured.</returns>
    public virtual Bitmap Capture(Form window)
    {
        var rc = new Rectangle(window.Location, window.Size);
        return capture(window, rc);
    }

    /// <summary>
    /// Execute the capturing of a specified rectangle in a given window.
    /// </summary>
    /// <param name="window">The window to capture</param>
    /// <param name="rc">The rectangle used for capturing</param>
    /// <returns>The image which has been captured.</returns>
    private Bitmap capture(Control window, Rectangle rc)
    {
        Bitmap? memoryImage = null;
        images = new Bitmap[1];

        try
        {
            // Create new graphics object using handle to window.
            using var graphics = window.CreateGraphics();
            memoryImage = new Bitmap(rc.Width, rc.Height, graphics);

            using var memoryGrahics = Graphics.FromImage(memoryImage);
            memoryGrahics.CopyFromScreen(rc.X, rc.Y, 0, 0, rc.Size, CopyPixelOperation.SourceCopy);
        }
        catch (ObjectDisposedException)
        {
            MessageBox.Show("Please re-open your form.", "Capture failed", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Capture failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        images[0] = memoryImage;
        return memoryImage;
    }

    /// <summary>
    /// Execute the capturing of a window specified by it's windows handle.
    /// The image which has been captured is saved to the 'images[0]' attribute in this class.
    /// This method uses old API calls !!!!!!!
    /// </summary>
    /// <param name="handle">The handle of the window to capture</param>
    /// <returns>The image which has been captured.</returns>
    /// <remarks>
    /// This call uses the <i>Win32 API</i> and should therefore not be used in your
    /// code if you don't want to depend on <i>Win32</i>. <c>internal</c> Takes care
    /// of this issue.
    /// </remarks>
    internal virtual Bitmap Capture(IntPtr handle)
    {
        //	Move the window to capture to the top of the Z order.
        NativeMethods.BringWindowToTop(handle);

        CaptureHandleDelegateHandler dlg = CaptureHandle;

        //	Do an asynchronous call of the capturing method, this is necessary to allow the captured
        //	window to come up in front of the Z-order of the displayed screens.
        var result = dlg.BeginInvoke(handle, null, null);
        return dlg.EndInvoke(result);
    }


    /// <summary>
    /// Execute the capturing of a window specified by it's windows handle.
    /// This method uses old API calls !!!!!!!
    /// </summary>
    /// <param name="handle">The handle of the window to capture</param>
    /// <returns>The image which has been captured.</returns>
    protected virtual Bitmap CaptureHandle(IntPtr handle)
    {
        Bitmap? memoryImage = null;
        images = new Bitmap[1];
        try
        {
            // Create new graphics object using handle to window.
            using var graphics = Graphics.FromHwnd(handle);
            var rc = NativeMethods.GetWindowRect(handle);

            if ((int)graphics.VisibleClipBounds.Width > 0 && (int)graphics.VisibleClipBounds.Height > 0)
            {
                memoryImage = new Bitmap(rc.Width, rc.Height, graphics);

                using var memoryGrahics = Graphics.FromImage(memoryImage);
                memoryGrahics.CopyFromScreen(rc.X, rc.Y, 0, 0, rc.Size, CopyPixelOperation.SourceCopy);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Capture failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        images[0] = memoryImage;
        return memoryImage;
    }

    /// <summary>
    /// Capture the screen and save it into a file, which portion of the screen is captured
    /// is defined by <paramref name="typeOfCapture"/>.
    /// </summary>
    /// <param name="typeOfCapture">Selects, what is actually captured, see <see cref="CaptureType"/>.</param>
    /// <param name="filename">The name of the target file. The extension in there is ignored, 
    /// it will replaced by an extension derived from the desired file format.</param>
    /// <param name="format">The format of the file.</param>
    /// <returns>An array of images captured.</returns>
    public virtual Bitmap[] Capture(CaptureType typeOfCapture, string filename,
        ImageFormatHandler.ImageFormatTypes format)
    {
        Capture(typeOfCapture);
        Save(filename, format);
        return images;
    }

    /// <summary>
    /// Capture the screen, which portion of the screen is captured
    /// is defined by <paramref name="typeOfCapture"/>.
    /// </summary>
    /// <param name="typeOfCapture">Selects, what is actually captured, see <see cref="CaptureType"/>.</param>
    /// <returns>An array of images captured.</returns>
    public virtual Bitmap[] Capture(CaptureType typeOfCapture)
    {
        var count = 1;

        try
        {
            var screens = Screen.AllScreens;
            Rectangle rc;
            switch (typeOfCapture)
            {
                case CaptureType.PrimaryScreen:
                    rc = Screen.PrimaryScreen.Bounds;
                    break;
                case CaptureType.VirtualScreen:
                    rc = SystemInformation.VirtualScreen;
                    break;
                case CaptureType.WorkingArea:
                    rc = Screen.PrimaryScreen.WorkingArea;
                    break;
                case CaptureType.AllScreens:
                    count = screens.Length;
                    rc = screens[0].WorkingArea;
                    break;
                default:
                    rc = SystemInformation.VirtualScreen;
                    break;
            }
            images = new Bitmap[count];

            for (var index = 0; index < count; index++)
            {
                if (index > 0)
                {
                    rc = screens[index].WorkingArea;
                }

                var memoryImage = new Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb);

                using (var memoryGrahics = Graphics.FromImage(memoryImage))
                {
                    memoryGrahics.CopyFromScreen(rc.X, rc.Y, 0, 0, rc.Size, CopyPixelOperation.SourceCopy);
                }
                images[index] = memoryImage;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Capture failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        return images;
    }

    /// <summary>
    /// Print all captured screens.
    /// </summary>
    public virtual void Print()
    {
        if (images != null)
        {
            try
            {
                for (var i = 0; i < images.Length; i++)
                {
                    image = images[i];
                    doc.DefaultPageSettings.Landscape = (image.Width > image.Height);
                    doc.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Capture failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    /// <summary>
    /// Event handler called from printing.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void printPage(object sender, PrintPageEventArgs e)
    {
        RectangleF rc = doc.DefaultPageSettings.Bounds;
        var ratio = image.Height / (float)(image.Width != 0 ? image.Width : 1);

        rc.Height = rc.Height - doc.DefaultPageSettings.Margins.Top - doc.DefaultPageSettings.Margins.Bottom;
        rc.Y = rc.Y + doc.DefaultPageSettings.Margins.Top;

        rc.Width = rc.Width - doc.DefaultPageSettings.Margins.Left - doc.DefaultPageSettings.Margins.Right;
        rc.X = rc.X + doc.DefaultPageSettings.Margins.Left;

        if (rc.Height / rc.Width > ratio)
        {
            rc.Height = rc.Width * ratio;
        }
        else
        {
            rc.Width = rc.Height / (ratio != 0 ? ratio : 1);
        }

        e.Graphics.DrawImage(image, rc);
    }

    /// <summary>
    /// Save all captured screens into a file.
    /// </summary>
    /// <param name="filename">The name of the target file. The extension in there is ignored, 
    /// it will replaced by an extension derived from the desired file format.</param>
    /// <param name="format">The format of the file.</param>
    /// <returns>An array of images captured.</returns>
    public virtual void Save(string filename, ImageFormatHandler.ImageFormatTypes format)
    {
        var directory = Path.GetDirectoryName(filename);
        var name = Path.GetFileNameWithoutExtension(filename);
        var ext = formatHandler.GetDefaultFilenameExtension(format);

        if (ext.Length == 0)
        {
            format = ImageFormatHandler.ImageFormatTypes.imgPNG;
            ext = "png";
        }

        try
        {
            var parameters = formatHandler.GetEncoderParameters(format, out var info);

            for (var i = 0; i < images.Length; i++)
            {
                if (images.Length > 1)
                {
                    filename = $"{directory}\\{name}.{i + 1:D2}.{ext}";
                }
                else
                {
                    filename = $"{directory}\\{name}.{ext}";
                }
                image = images[i];

                if (parameters != null)
                {
                    image.Save(filename, info, parameters);
                }
                else
                {
                    image.Save(filename, ImageFormatHandler.GetImageFormat(format));
                }
            }
        }
        catch (Exception ex)
        {
            var s =
                $"Saving image to [{filename}] in format [{format}].\n{ex}";
            MessageBox.Show(s, "Capture failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}