using Mocassin.Framework.Operations;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     Represents a generic data injector that handles data injection and conversion from a data source into an injection
    ///     target
    /// </summary>
    public interface IDataInjector<in TData, in THandler>
    {
        /// <summary>
        ///     Pushes the data on the passed data source and creates an operation report informing about potential errors
        /// </summary>
        /// <param name="data"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        IOperationReport InjectData(TData data, THandler handler);
    }
}