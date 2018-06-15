using ICon.Model.Basic;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Represents a query port for query based access to the reference and extended transition data
    /// </summary>
    public interface ITransitionQueryPort : IModelQueryPort<ITransitionDataPort>, IModelQueryPort<ITransitionCachePort>
    {

    }
}
