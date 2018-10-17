using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Structures
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
        /// <param name="modelProject"></param>
        public StructureManager(IModelProject modelProject, StructureModelData data) : base(modelProject, data)
        {

        }

        /// <summary>
        /// Creates new validation service for this manager
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public override IValidationService CreateValidationService(ProjectSettings settings)
        {
            if (!settings.TryGetModuleSettings(out MocassinStructureSettings moduleSettings))
                throw new InvalidOperationException("Settings object for the structure module is missing");

            return new StructureValidationService(moduleSettings, ModelProject);
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
