using ICon.Model.Basic;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Represents a manager for transitions and affiliated rules within the modeling process
    /// </summary>
    public interface ITransitionManager : IModelManager<ITransitionInputPort, ITransitionEventPort, ITransitionQueryPort>
    {

    }
}
