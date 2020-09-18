using Mocassin.Model.Basic;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Basic implementation of the transition query manager that handles safe data queries and service requests to the
    ///     transition manager from outside sources
    /// </summary>
    internal class TransitionQueryManager :
        ModelQueryManager<TransitionModelData, ITransitionDataPort, TransitionModelCache, ITransitionCachePort>, ITransitionQueryPort
    {
        /// <inheritdoc />
        public TransitionQueryManager(TransitionModelData modelData, TransitionModelCache modelCache, AccessLockSource lockSource)
            : base(modelData, modelCache, lockSource)
        {
        }
    }
}