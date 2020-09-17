using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Transitions.ConflictHandling
{
    /// <summary>
    ///     Object handler that handles internal data changes of the transition manager system required after a kinetic
    ///     transition data change
    /// </summary>
    public class KineticTransitionChangedHandler : KineticTransitionHandlerBase
    {
        /// <inheritdoc />
        public KineticTransitionChangedHandler(IDataAccessor<TransitionModelData> dataAccessor, IModelProject modelProject)
            : base(dataAccessor, modelProject)
        {
        }

        /// <inheritdoc />
        public override ConflictReport HandleConflicts(KineticTransition obj) => new ConflictReport();
    }
}