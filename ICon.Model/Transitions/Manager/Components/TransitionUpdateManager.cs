using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;
using Mocassin.Model.Transitions.Handler;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Basic update manager for the transition module that handles pushed information on data changes in required modules
    /// </summary>
    internal class TransitionUpdateManager : ModelUpdateManager<TransitionModelData, TransitionEventManager>, ITransitionUpdatePort
    {
        /// <summary>
        ///     Pipeline based event handler for added model objects events in the connected structure manager
        /// </summary>
        [UpdateHandler(typeof(IStructureEventPort))]
        protected StructureObjectAddedEventHandler StructureObjectAddedEventHandler { get; set; }

        /// <summary>
        ///     Pipeline based event handler for changed model objects events in the connected structure manager
        /// </summary>
        [UpdateHandler(typeof(IStructureEventPort))]
        protected StructureObjectChangedEventHandler StructureObjectChangedEventHandler { get; set; }

        /// <summary>
        ///     Pipeline based event handler for removed model objects events in the connected structure manager
        /// </summary>
        [UpdateHandler(typeof(IStructureEventPort))]
        protected StructureObjectRemovedEventHandler StructureObjectRemovedEventHandler { get; set; }

        /// <summary>
        ///     Pipeline based event handler for model object list reindexing events in the connected structure manager
        /// </summary>
        [UpdateHandler(typeof(IStructureEventPort))]
        protected StructureObjectIndexingChangedEventHandler StructureObjectIndexingChangedEventHandler { get; set; }

        /// <summary>
        ///     Pipeline based event handler for changed model parameter events in the connected structure manager
        /// </summary>
        [UpdateHandler(typeof(IStructureEventPort))]
        protected StructureParameterChangedEventHandler StructureParameterChangedEventHandler { get; set; }

        /// <inheritdoc />
        public TransitionUpdateManager(TransitionModelData modelData, TransitionEventManager eventManager, IModelProject modelProject)
            : base(modelData, eventManager, modelProject)
        {
        }
    }
}