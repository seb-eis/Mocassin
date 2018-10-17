namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Common interface for all model update ports
    /// </summary>
    public interface IModelUpdatePort
    {
        /// <summary>
        ///     Disconnects from all known event ports ad returns number of disconnected ports
        /// </summary>
        void DisconnectAll();

        /// <summary>
        ///     Connects to an event port. Returns false if the connection failed due to an already existing connection
        /// </summary>
        /// <param name="eventPort"></param>
        /// <returns></returns>
        bool Connect(IModelEventPort eventPort);

        /// <summary>
        ///     Disconnects from event port if this specific port exists
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="eventPort"></param>
        void Disconnect(IModelEventPort eventPort);
    }
}