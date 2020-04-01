using System.Globalization;
using System.IO;
using System.Text;
using Mocassin.Framework.Extensions;
using Mocassin.Tools.Evaluation.PlotData;

namespace Mocassin.Tools.Evaluation.Extensions
{
    /// <summary>
    ///     Provides static helper methods for dealing with DAT file creation
    /// </summary>
    public static class DatFileWritableExtensions
    {
        /// <summary>
        ///     Writes a <see cref="IDatFileWritable{T}" /> to a file at the specified path
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="filePath"></param>
        /// <param name="isOverwrite"></param>
        public static void WriteDatToFile<T>(this IDatFileWritable<T> value, string filePath, bool isOverwrite = true) where T : IDatLineWritable
        {
            using var streamWriter = isOverwrite ? File.CreateText(filePath) : File.AppendText(filePath);
            value.WriteDatToStream(streamWriter);
        }

        /// <summary>
        ///     Writes a <see cref="IDatFileWritable{T}" /> to a <see cref="StreamWriter" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="streamWriter"></param>
        public static void WriteDatToStream<T>(this IDatFileWritable<T> value, StreamWriter streamWriter) where T : IDatLineWritable
        {
            var format = value.DefaultDatLineFormat();
            var header = value.GetDatHeader(format);
            if (header != null) streamWriter.WriteLine(header);
            foreach (var line in value.GetDatLines()) streamWriter.WriteLine(line.ToDatLine(format));
        }

        /// <summary>
        ///     Get the default DAT file format for a <see cref="IDatFileWritable{T}" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DefaultDatLineFormat<T>(this IDatFileWritable<T> value) where T : IDatLineWritable
        {
            var count = value.GetDatEntriesPerLine();
            var builder = new StringBuilder(count * 10);
            for (var i = 0; i < count; i++) builder.Append($"{{{i}}}\t");
            builder.PopBack(1);
            return builder.ToString();
        }
    }
}