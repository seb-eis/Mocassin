using System;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Transitions
{
    /// <inheritdoc cref="Mocassin.Model.Transitions.ITransitionManager" />
    internal class TransitionManager :
        ModelManager<TransitionModelData, TransitionModelCache, TransitionDataManager, TransitionCacheManager, TransitionInputManager,
            TransitionQueryManager, TransitionEventManager, TransitionUpdateManager>, ITransitionManager
    {
        /// <inheritdoc />
        public new ITransitionInputPort InputAccess => InputManager;

        /// <inheritdoc />
        public ITransitionQueryPort DataAccess => QueryManager;

        /// <inheritdoc />
        public new ITransitionEventPort EventAccess => EventManager;

        /// <inheritdoc />
        public TransitionManager(IModelProject modelProject, TransitionModelData data)
            : base(modelProject, data)
        {
        }

        /// <inheritdoc />
        public override Type GetManagerInterfaceType()
        {
            return typeof(ITransitionManager);
        }

        /// <inheritdoc />
        public override IValidationService CreateValidationService(ProjectSettings settings)
        {
            if (!settings.TryGetModuleSettings(out MocassinTransitionSettings moduleSettings))
                throw new InvalidOperationException("Settings object for the transition module is missing");

            return new TransitionValidationService(moduleSettings, ModelProject);
        }
    }
}