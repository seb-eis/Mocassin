using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HelixToolkit.Wpf.SharpDX;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Controls.Base.IO;

namespace Mocassin.UI.GUI.Controls.VisualizerDX.Viewport.Commands
{
    /// <summary>
    ///     The <see cref="Command{T}" /> to open the export dialog for a <see cref="Viewport3DX" />
    /// </summary>
    public class ExportViewportImageCommand : Command<Viewport3DX>
    {
        /// <summary>
        ///     Get or set the callback <see cref="Func{TResult}"/> that the command uses to get the size information
        /// </summary>
        public Func<(int Width, int Height)> ExportSizeCallback { get; set; }

        /// <inheritdoc />
        public ExportViewportImageCommand()
        {
        }

        /// <summary>
        ///     Creates new <see cref="ExportViewportImageCommand"/> using the provided export size callback
        /// </summary>
        /// <param name="exportSizeCallback"></param>
        public ExportViewportImageCommand(Func<(int Width, int Height)> exportSizeCallback)
        {
            ExportSizeCallback = exportSizeCallback;
        }

        /// <inheritdoc />
        public override void Execute(Viewport3DX parameter)
        {
            var path = new UserFileSelectionSource(EnumerateSupportedImageFormats().ToArray()).GetFileSelection();
            if (path == null) return;
            var (width, height) = GetOutputSize(parameter);
            SaveImage(parameter, path, GetImageFormatForPath(path), width, height);
        }

        /// <inheritdoc />
        public override bool CanExecute(Viewport3DX parameter)
        {
            return parameter != null && base.CanExecute(parameter);
        }

        /// <summary>
        ///     Gets the output width and height for the image
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        private (int Width, int Height) GetOutputSize(Viewport3DX view)
        {
            var (width, height) = ExportSizeCallback?.Invoke() ?? (0,0);
            width = width == 0 ? (int) view.ActualWidth : width;
            height = height == 0 ? (int) view.ActualHeight : height;
            return (width, height);
        }

        /// <summary>
        ///     Saves the <see cref="Viewport3DX" /> to an image using the provided settings
        /// </summary>
        /// <param name="view"></param>
        /// <param name="path"></param>
        /// <param name="format"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected virtual void SaveImage(Viewport3DX view, string path, Direct2DImageFormat format, int width, int height)
        {
            var (oldWith, oldHeight) = ((int) view.ActualWidth, (int) view.ActualHeight);

            void OnRendered(object sender, EventArgs args)
            {
                view.OnRendered -= OnRendered;
                view.SaveScreen(path, format);
                view.ResizeAndArrange(oldWith, oldHeight);
            }

            view.OnRendered += OnRendered;
            view.ResizeAndArrange(width, height);
        }

        /// <summary>
        ///     Gets the <see cref="Direct2DImageFormat" /> for the file extension of the provided path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected virtual Direct2DImageFormat GetImageFormatForPath(string path)
        {
            var extension = Path.GetExtension(path)?.ToLower();
            return extension switch
            {
                ".bmp" => Direct2DImageFormat.Bmp,
                ".jpg" => Direct2DImageFormat.Jpeg,
                ".png" => Direct2DImageFormat.Png,
                ".tiff" => Direct2DImageFormat.Tiff,
                ".wdp" => Direct2DImageFormat.Wmp,
                _ => throw new InvalidOperationException("Invalid path extension or null path.")
            };
        }

        /// <summary>
        ///     Enumerates the supported image formats with full name and file extension
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<(string Name, string Extension)> EnumerateSupportedImageFormats()
        {
            yield return ("JPEG", "jpg");
            yield return ("Portable network graphic", "png");
            yield return ("Tagged image file format", "tiff");
            yield return ("Windows media photo", "wdp");
            yield return ("Bitmap file", "bmp");
        }
    }
}