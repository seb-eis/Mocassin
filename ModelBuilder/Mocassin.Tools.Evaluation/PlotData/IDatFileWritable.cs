using System.Collections.Generic;

namespace Mocassin.Tools.Evaluation.PlotData
{
    /// <summary>
    ///     Represents a data class that can be written to a DAT file format
    /// </summary>
    public interface IDatFileWritable<out T> where T : IDatLineWritable
    {
        /// <summary>
        ///     Get the number of DAT file entries per line
        /// </summary>
        /// <returns></returns>
        int GetDatEntriesPerLine();

        /// <summary>
        ///     Get the header string for the DAT file using the provided format
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        string GetDatHeader(string format);

        /// <summary>
        ///     Enumerates the <see cref="IDatLineWritable" /> data
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetDatLines();
    }
}