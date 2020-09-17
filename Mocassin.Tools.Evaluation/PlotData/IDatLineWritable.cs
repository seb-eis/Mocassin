namespace Mocassin.Tools.Evaluation.PlotData
{
    /// <summary>
    ///     Represents a data item that can be formatted as a DAT file line
    /// </summary>
    public interface IDatLineWritable
    {
        /// <summary>
        ///     Get a new <see cref="string" /> formatted as a DAT file line
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        string ToDatLine(string format);
    }
}