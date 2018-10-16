using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Structures
{
    /// <summary>
    /// Basic structure manager implementation that handles building of unit cells
    /// </summary>
    internal class StructureManager : ModelManager<StructureModelData, StructureDataCache, StructureDataManager, StructureCacheManager, StructureInputManager, StructureQueryManager, StructureEventManager, StructureUpdateManager>, IStructureManager
    {
        /// <summary>
        /// Restricted port based access to the internal input manager
        /// </summary>
        public new IStructureInputPort InputPort => InputManager;

        /// <summary>
        /// Restricted port based access to the internal query manager
        /// </summary>
        public IStructureQueryPort QueryPort => QueryManager;

        /// <summary>
        /// Restricted port based access to the internal event manager
        /// </summary>
        public new IStructureEventPort EventPort => EventManager;

        /// <summary>
        /// Creates new structure manager from project services and base data object
        /// </summary>
        /// <param name="projectServices"></param>
        public StructureManager(IProjectServices projectServices, StructureModelData data) : base(projectServices, data)
        {

        }

        /// <summary>
        /// Creates new validation service for this manager
        /// </summary>
        /// <param name="settingsData"></param>
        /// <returns></returns>
        public override IValidationService CreateValidationService(ProjectSettingsData settingsData)
        {
            return new StructureValidationService(settingsData.StructureSettings, ProjectServices);
        }

        /// <summary>
        /// Get the manager interface type (IStructureManager)
        /// </summary>
        /// <returns></returns>
        public override Type GetManagerInterfaceType()
        {
            return typeof(IStructureManager);
        }
    }
}
