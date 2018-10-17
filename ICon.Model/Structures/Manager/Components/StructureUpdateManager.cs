using Mocassin.Model.Basic;
using Mocassin.Model.Particles;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures.Handler;

namespace Mocassin.Model.Structures
{
    /// <summary>
    /// Basic update manager for the structure module that handles pushed information on data changes in required modules
    /// </summary>
    internal class StructureUpdateManager : ModelUpdateManager<StructureModelData, StructureEventManager>, IStructureUpdatePort
    {
        /// <summary>
        /// Pipeline based handler for added model objects in the connected particle manager
        /// </summary>
        [UpdateHandler(typeof(IParticleEventPort))]
        protected NewParticleObjectsHandler NewParticleObjectsHandler { get; set; }

        /// <summary>
        /// Pipeline based handler for changed model objects in the connected particle manager
        /// </summary>
        [UpdateHandler(typeof(IParticleEventPort))]
        protected ObjectChangedParticleHandler ObjectChangedParticleHandler { get; set; }

        /// <summary>
        /// Pipeline based handler for removed model objects in the connected particle manager
        /// </summary>
        [UpdateHandler(typeof(IParticleEventPort))]
        protected ObjectRemovedParticleHandler ObjectRemovedParticleHandler { get; set; }

        /// <summary>
        /// Pipeline based handler for reindexed model object lists in the connected particle manager
        /// </summary>
        [UpdateHandler(typeof(IParticleEventPort))]
        protected ReindexedParticleObjectsHandler ReindexedParticleObjectsHandler { get; set; }

        /// <summary>
        /// Creates new structure update manager from base data, extended data, event manager and project services
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="extendedData"></param>
        /// <param name="eventManager"></param>
        /// <param name="modelProject"></param>
        public StructureUpdateManager(StructureModelData baseData, StructureEventManager eventManager, IModelProject modelProject)
            : base(baseData, eventManager, modelProject)
        {

        }
    }
}
