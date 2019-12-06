using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HelixToolkit.Wpf;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.Loading;
using Mocassin.UI.GUI.Controls.Base.IO;

namespace Mocassin.UI.GUI.Controls.Visualizer.Commands
{
    /// <summary>
    ///     A <see cref="Command{T}" /> implementation to export a <see cref="Viewport3D" /> to a file
    /// </summary>
    public class ExportViewportCommand : Command<HelixViewport3D>
    {
        /// <summary>
        ///     Get the <see cref="UserFileSelectionSource" /> for the export file name
        /// </summary>
        private UserFileSelectionSource FileSelectionSource { get; }

        /// <summary>
        ///     Get the <see cref="Func{TResult}"/> that provides the render size
        /// </summary>
        private Func<(int Width, int Height)> ExportSizeProvider { get; }

        /// <summary>
        ///     Creates a new <see cref="ExportViewportCommand"/> with the provided provider functions
        /// </summary>
        /// <param name="exportSizeProvider"></param>
        public ExportViewportCommand(Func<(int Width, int Height)> exportSizeProvider)
        {
            if (exportSizeProvider != null) ExportSizeProvider = exportSizeProvider;
            FileSelectionSource = new UserFileSelectionSource(true, GetSupportedFiles());
        }

        /// <inheritdoc />
        public override bool CanExecute(HelixViewport3D parameter)
        {
            return parameter != null && base.CanExecute(parameter);
        }

        /// <inheritdoc />
        public override void Execute(HelixViewport3D parameter)
        {
            var fileName = FileSelectionSource.GetFileSelection();
            if (string.IsNullOrWhiteSpace(fileName)) return;
            var window = new ExportWindow();
            try
            {
                window.Show();
                switch (Path.GetExtension(fileName).ToLower())
                {
                    case ".bmp":
                        ExportAsBitmap(parameter, fileName, new BmpBitmapEncoder());
                        break;
                    case ".jpg":
                        ExportAsBitmap(parameter, fileName, new JpegBitmapEncoder());
                        break;
                    case ".png":
                        ExportAsBitmap(parameter, fileName, new PngBitmapEncoder());
                        break;
                    case ".tiff" :
                        ExportAsBitmap(parameter, fileName, new TiffBitmapEncoder());
                        break;
                    case ".wdp":
                        ExportAsBitmap(parameter, fileName, new WmpBitmapEncoder());
                        break;
                    default:
                        parameter.Export(fileName);
                        break;
                }
            }
            catch (Exception e)
            {
                window.Close();
                MessageBox.Show($"Export error:\n{e.Message}", "Export", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            window.Close();
        }

        /// <summary>
        ///     Gets the set of supported file types with name and extension
        /// </summary>
        /// <returns></returns>
        public static (string Name, string FileExtension)[] GetSupportedFiles()
        {
            return new[]
            {
                ("JPEG", "jpg"),
                ("Portable network graphic", "png"),
                ("Tagged image file format", "tiff"),
                ("Bitmap file", "bmp"),
                ("Windows media photo", "wdp"),
                //("Collada file", "dae"),
                //("Object file", "obj"),
                ("Standard triangulation language", "stl"),
                //("Extensible 3D", "x3d"),
                //("Extensible application markup language", "xaml"),
                //("Kerkythea file", "xml")
            };
        }

        /// <summary>
        ///     Exports a bitmap to a file using the provided <see cref="BitmapEncoder"/> and background <see cref="Brush"/> (Defaults to white background)
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="path"></param>
        /// <param name="encoder"></param>
        /// <param name="backgroundBrush"></param>
        public void ExportAsBitmap(HelixViewport3D parameter, string path, BitmapEncoder encoder, Brush backgroundBrush = null)
        {
            if (encoder == null) throw new ArgumentNullException(nameof(encoder));

            backgroundBrush = backgroundBrush ?? parameter.Background ?? new SolidColorBrush(Colors.White);
            var (width, height) = ExportSizeProvider.Invoke();
            var bitmapSource = parameter.Viewport.RenderBitmap(width, height, backgroundBrush);
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(fileStream);
            }

        }
    }
}