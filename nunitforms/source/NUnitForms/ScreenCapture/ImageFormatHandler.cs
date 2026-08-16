/*----------------------------------------------------------------------------------------

    A-Soft Ingenieurbüro

    Copyright © 1994 - 2007. All Rights reserved.

    Related Copyrights :

            Microsoft .NET Windows Forms V2.0 library.
            Copyright (C) 2004...2006 Microsoft Corporation,
            All rights reserved.


    FILE		:	ImageFormatHandler.cs

    PROJECT		:	A-Soft Library
    SUB			:	Standard Library

    SYSTEM		:	Windows-XP, (Windows 2000), C# (.NET 2.0, Visual Studio.NET 2005)

    AUTHOR		:	Joachim Holzhauer

    DESCRIPTION	:	Supplies some helpers methods used for image processing (file
                    selection dialog, image format options selection etc.).

    VERSION		:	1.0 - 2006.01.31

----------------------------------------------------------------------------------------*/

using System.Drawing.Imaging;


namespace NUnit.Extensions.Forms.ScreenCapture;

/// <summary>
/// Supplies some helpers methods used for image processing (file selection dialog, 
/// image format options selection etc.).
/// Here, this is just a fragment of the actual implementation !!!
/// 
/// </summary>
public class ImageFormatHandler
{
    #region ImageFormatTypes enum

    /// <summary>
    /// An enum which represents all supported graphic formats. These enum names are also used
    /// to get a description from the resources.
    /// </summary>
    public enum ImageFormatTypes
    {
        /// <summary>
        /// Undefined format
        /// </summary>
        imgNone,
        /// <summary>
        /// Windows bitmap
        /// </summary>
        imgBMP,
        /// <summary>
        /// Enhanced Windows metafile
        /// </summary>
        imgEMF,
        /// <summary>
        /// Exchangeable Image File
        /// </summary>
        imgEXIF,
        /// <summary>
        /// Graphics Interchange Format
        /// </summary>
        imgGIF,
        /// <summary>
        /// Windows icon
        /// </summary>
        imgICON,
        /// <summary>
        /// Joint Photographic Experts Group
        /// </summary>
        imgJPEG,
        /// <summary>
        /// Portable Network Graphics
        /// </summary>
        imgPNG,
        /// <summary>
        /// Tag Image File
        /// </summary>
        imgTIFF,
        /// <summary>
        /// Windows metafile
        /// </summary>
        imgWMF
    }

    #endregion

    /// <summary>
    /// All image decoders available.
    /// </summary>
    private readonly ImageCodecInfo[] availableDecoders;

    /// <summary>
    /// All image encoders available.
    /// </summary>
    private readonly ImageCodecInfo[] availableEncoders;

    /// <summary>
    /// The color depth for TIFF
    /// </summary>
    private readonly long encodingColorDepth = 24;

    /// <summary>
    /// The TIFF compression type
    /// </summary>
    private readonly EncoderValue encodingCompression = EncoderValue.CompressionLZW;

    /// <summary>
    /// The quality for JPEG compresseion (0...100)
    /// </summary>
    private readonly long encodingQuality = 50;

    /// <summary>
    /// The rendering method for ????
    /// </summary>
    private readonly EncoderValue encodingRenderMethod = EncoderValue.RenderProgressive;

    /// <summary>
    /// The scanning method for GIF
    /// </summary>
    private readonly EncoderValue encodingScanMethod = EncoderValue.ScanMethodInterlaced;

    /// <summary>
    /// Creator
    /// </summary>
    public ImageFormatHandler()
    {
        availableEncoders = ImageCodecInfo.GetImageEncoders();
        availableDecoders = ImageCodecInfo.GetImageDecoders();
    }

    /// <summary>
    /// Get/Set the default image format used
    /// </summary>
    public ImageFormatTypes DefaultFormat
    {
        get;
        set => field = value;
    }

    /// <summary>
    /// Get the image format associated with that enum, if it does not exist, then return a 'null'.
    /// </summary>
    /// <param name="type">The image format enum type</param>
    /// <returns>The windows image format type</returns>
    public static ImageFormat? GetImageFormat(ImageFormatTypes type)
    {
        return type switch
        {
            ImageFormatTypes.imgBMP => ImageFormat.Bmp,
            ImageFormatTypes.imgEMF => ImageFormat.Emf,
            ImageFormatTypes.imgEXIF => ImageFormat.Exif,
            ImageFormatTypes.imgGIF => ImageFormat.Gif,
            ImageFormatTypes.imgICON => ImageFormat.Icon,
            ImageFormatTypes.imgJPEG => ImageFormat.Jpeg,
            ImageFormatTypes.imgPNG => ImageFormat.Png,
            ImageFormatTypes.imgTIFF => ImageFormat.Tiff,
            ImageFormatTypes.imgWMF => ImageFormat.Wmf,
            _ => null
        };
    }

    /// <summary>
    /// Get the MIME name of the image format associated with that enum, 
    /// if it does not exist, then return a 'null'.
    /// </summary>
    /// <param name="type">The image format enum type</param>
    /// <returns>The MIME name of the desired image format type or 'null' when none exists.</returns>
    public static string GetMimeType(ImageFormatTypes type)
    {
        var s = type switch
        {
            ImageFormatTypes.imgBMP => "bmp",
            ImageFormatTypes.imgEMF => "x-emf",
            ImageFormatTypes.imgGIF => "gif",
            ImageFormatTypes.imgICON => "x-icon",
            ImageFormatTypes.imgJPEG => "jpeg",
            ImageFormatTypes.imgPNG => "png",
            ImageFormatTypes.imgTIFF => "tiff",
            ImageFormatTypes.imgWMF => "x-wmf",
            _ => null
        };
        if (!string.IsNullOrEmpty(s))
        {
            s = $"image/{s}";
        }

        return s;
    }

    /// <summary>
    /// Get the enum image format associated with that windows format, if it does not exist, then return a 'imgNone'.
    /// </summary>
    /// <param name="type">The windows image format type</param>
    /// <returns>The image format enum type</returns>
    public static ImageFormatTypes GetImageFormat(ImageFormat type)
    {
        if (type == ImageFormat.Bmp)
        {
            return ImageFormatTypes.imgBMP;
        }

        if (type == ImageFormat.Emf)
        {
            return ImageFormatTypes.imgEMF;
        }

        if (type == ImageFormat.Exif)
        {
            return ImageFormatTypes.imgEXIF;
        }

        if (type == ImageFormat.Gif)
        {
            return ImageFormatTypes.imgGIF;
        }

        if (type == ImageFormat.Icon)
        {
            return ImageFormatTypes.imgICON;
        }

        if (type == ImageFormat.Jpeg)
        {
            return ImageFormatTypes.imgJPEG;
        }

        if (type == ImageFormat.Png)
        {
            return ImageFormatTypes.imgPNG;
        }

        if (type == ImageFormat.Tiff)
        {
            return ImageFormatTypes.imgTIFF;
        }

        if (type == ImageFormat.Wmf)
        {
            return ImageFormatTypes.imgWMF;
        }

        return ImageFormatTypes.imgNone;
    }

    /// <summary>
    /// Search the codec for a given type, cycle through encoders and decoders until found. 
    /// If no match is found, return 'null'.
    /// </summary>
    /// <param name="type">The graphic format</param>
    /// <returns>The associated codec or 'null'.</returns>
    public virtual ImageCodecInfo? GetCodecInfo(ImageFormatTypes type)
    {
        var mimeType = GetMimeType(type);

        if (!string.IsNullOrEmpty(mimeType))
        {
            for (var i = 0; i < 2; i++)
            {
                var encoders = i == 0 ? availableEncoders : availableDecoders;

                foreach (var info in encoders)
                {
                    if (info.MimeType == mimeType)
                    {
                        return info;
                    }
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Get the encoder parameters used for saving a graphic into a file or stream.
    /// </summary>
    /// <param name="type">The desired format type.</param>
    /// <param name="info">Returns the codec info.</param>
    /// <returns>The encoder parameters or 'null' when not supported by the requested type.</returns>
    public virtual EncoderParameters GetEncoderParameters(ImageFormatTypes type, out ImageCodecInfo info)
    {
        EncoderParameters? parameters = null;
        info = GetCodecInfo(type);
        if (info != null)
        {
            switch (type)
            {
                case ImageFormatTypes.imgGIF:
                    parameters = new EncoderParameters(2);
                    parameters.Param[0] = new EncoderParameter(Encoder.Version, (long)EncoderValue.VersionGif89);
                    parameters.Param[1] = new EncoderParameter(Encoder.ScanMethod, (long)encodingScanMethod);
                    break;

                case ImageFormatTypes.imgJPEG:
                    parameters = new EncoderParameters(2);
                    parameters.Param[0] = new EncoderParameter(Encoder.RenderMethod, (long)encodingRenderMethod);
                    parameters.Param[1] = new EncoderParameter(Encoder.Quality, encodingQuality);
                    break;

                case ImageFormatTypes.imgPNG:
                    parameters = new EncoderParameters(2);
                    parameters.Param[0] = new EncoderParameter(Encoder.RenderMethod, (long)encodingRenderMethod);
                    parameters.Param[1] = new EncoderParameter(Encoder.ScanMethod, (long)encodingScanMethod);
                    break;

                case ImageFormatTypes.imgTIFF:
                    parameters = new EncoderParameters(2);
                    parameters.Param[0] = new EncoderParameter(Encoder.ColorDepth, encodingColorDepth);
                    parameters.Param[1] = new EncoderParameter(Encoder.Compression, (long)encodingCompression);
                    break;
            }
        }
        return parameters;
    }

    /// <summary>
    /// Get the default filename extension for a given type
    /// </summary>
    /// <param name="type">The image type</param>
    /// <returns>The extension string.</returns>
    public virtual string GetDefaultFilenameExtension(ImageFormatTypes type)
    {
        var ext = "";
        var info = GetCodecInfo(type);

        if (info != null)
        {
            var extensions = info.FilenameExtension.Split(';');
            ext = extensions[0];
            if (ext.StartsWith("*."))
            {
                ext = ext.Substring(2);
            }
        }
        return ext;
    }
}