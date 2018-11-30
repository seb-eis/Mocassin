namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     Represents a data adapter that provides functionality for extracting and injection data from a handler object
    /// </summary>
    public interface IDataAdapter<TData, in THandler> : IDataExtractor<THandler, TData>, IDataInjector<TData, THandler>
    {
    }
}