namespace ICon.Model.Basic
{
    /// <summary>
    ///     Generic interface for all reader providers that supply safe disposable data readers for model data objects
    /// </summary>
    /// <typeparam name="TPort"></typeparam>
    public interface IDataReaderSource<out TPort> where TPort : class, IModelDataPort
    {
        /// <summary>
        ///     Creates a new disposable model data reader interface
        /// </summary>
        /// <returns></returns>
        IDataReader<TPort> CreateInterface();
    }
}