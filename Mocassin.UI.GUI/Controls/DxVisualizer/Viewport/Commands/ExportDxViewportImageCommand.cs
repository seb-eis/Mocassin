using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using HelixToolkit.Wpf.SharpDX;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Controls.Base.IO;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Commands
{
    /// <summary>
    ///     The <see cref="Command{T}" /> to open the export dialog for a <see cref="Viewport3DX" />
    /// </summary>
    public class ExportDxViewportImageCommand : Command<Viewport3DX>
    {
        /// <summary>
        ///     Get or set the callback <see cref="Func{TResult}" /> that the command uses to get the size information
        /// </summary>
        public Func<(int Width, int Height)> ExportSizeCallback { get; set; }

        /// <inheritdoc />
        public ExportDxViewportImageCommand()
        {
        }

        /// <summary>
        ///     Creates new <see cref="ExportDxViewportImageCommand" /> using the provided export size callback
        /// </summary>
        /// <param name="exportSizeCallback"></param>
        public ExportDxViewportImageCommand(Func<(int Width, int Height)> exportSizeCallback)
        {
            ExportSizeCallback = exportSizeCallback;
        }

        /// <inheritdoc />
        public override void Execute(Viewport3DX parameter)
        {
            var path = new UserFileSelectionSource(true, EnumerateSupportedImageFormats().ToArray()).RequestFileSelection();
            if (string.IsNullOrWhiteSpace(path)) return;
            var (width, height) = GetOutputSize(parameter);
            TrySaveImage(parameter, path, GetImageFormatForPath(path), width, height);
        }

        /// <inheritdoc />
        public override bool CanExecute(Viewport3DX parameter) => parameter != null && base.CanExecute(parameter);

        /// <summary>
        ///     Gets the output width and height for the image
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        private (int Width, int Height) GetOutputSize(Viewport3DX view)
        {
            var (width, height) = ExportSizeCallback?.Invoke() ?? (0, 0);
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
        protected virtual void TrySaveImage(Viewport3DX view, string path, Direct2DImageFormat format, int width, int height)
        {
            var (oldWith, oldHeight) = ((int) view.ActualWidth, (int) view.ActualHeight);

            void OnRendered(object sender, EventArgs args)
            {
                view.OnRendered -= OnRendered;
                view.SaveScreen(path, format);
                ResizeViewport(view, oldWith, oldHeight);
            }

            view.OnRendered += OnRendered;
            try
            {
                if (ResizeViewport(view, width, height)) return;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                MessageBox.Show($"Cannot apply {width}x{height} pixels.", "Image - Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ResizeViewport(view, oldWith, oldHeight);
                return;
            }

            view.OnRendered -= OnRendered;
            view.SaveScreen(path, format);
        }

        /// <summary>
        ///     Resizes the <see cref="Viewport3DX" /> if the provided values differ from the current size. Returns true if the
        ///     view was resized
        /// </summary>
        /// <param name="view"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected virtual bool ResizeViewport(Viewport3DX view, int width, int height)
        {
            if (((int) view.ActualWidth, (int) view.ActualHeight) == (width, height)) return false;
            view.ResizeAndArrange(width, height);
            return true;
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