using System;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Structures
{
    /// <summary>
    ///     Basic structure manager implementation that handles building of unit cells
    /// </summary>
    internal class StructureManager :
        ModelManager<StructureModelData, StructureModelCache, StructureDataManager, StructureCacheManager, StructureInputManager,
            StructureQueryManager, StructureEventManager, StructureUpdateManager>, IStructureManager
    {
        /// <inheritdoc />
        public new IStructureInputPort InputAccess => InputManager;

        /// <inheritdoc />
        public IStructureQueryPort DataAccess => QueryManager;

        /// <inheritdoc />
        public new IStructureEventPort EventAccess => EventManager;

        /// <inheritdoc />
        public StructureManager(IModelProject modelProject, StructureModelData data)
            : base(modelProject, data)
        {
        }

        /// <inheritdoc />
        public override IValidationService CreateValidationService(ProjectSettings settings)
        {
            if (!settings.TryGetModuleSettings(out MocassinStructureSettings moduleSettings))
                throw new InvalidOperationException("Settings object for the structure module is missing");

            return new StructureValidationService(moduleSettings, ModelProject);
        }

        /// <inheritdoc />
        public override Type GetManagerInterfaceType() => typeof(IStructureManager);
    }
}