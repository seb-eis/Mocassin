namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     Represents a generic data extractor that handles data extraction and conversion from a source object type to a
    ///     target data object type
    /// </summary>
    public interface IDataExtractor<in THandler, out TData>
    {
        /// <summary>
        ///     Pulls the data from the passed input object and returns it as the required object type
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        TData ExtractData(THandler handler);
    }
}