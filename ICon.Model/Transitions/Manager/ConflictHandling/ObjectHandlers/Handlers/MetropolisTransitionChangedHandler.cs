using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Transitions.ConflictHandling
{
    /// <summary>
    ///     Object handler that handles internal data changes of the transition manager system required after a metropolis
    ///     transition data change
    /// </summary>
    public class MetropolisTransitionChangedHandler : MetropolisTransitionHandlerBase
    {
        /// <inheritdoc />
        public MetropolisTransitionChangedHandler(IDataAccessor<TransitionModelData> dataAccessor, IModelProject modelProject)
            : base(dataAccessor, modelProject)
        {
        }

        /// <inheritdoc />
        public override ConflictReport HandleConflicts(MetropolisTransition obj)
        {
            return new ConflictReport();
        }
    }
}