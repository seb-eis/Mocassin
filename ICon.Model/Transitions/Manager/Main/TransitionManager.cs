using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    /// BAsic implementation of a model transition manager that handles transitions and affiliated rules
    /// </summary>
    internal class TransitionManager : ModelManager<TransitionModelData, TransitionDataCache, TransitionDataManager, TransitionCacheManager, TransitionInputManager, TransitionQueryManager, TransitionEventManager, TransitionUpdateManager>, ITransitionManager
    {
        /// <summary>
        /// Transition manager input port for adding and manipulation of model data
        /// </summary>
        public new ITransitionInputPort InputPort => InputManager;

        /// <summary>
        /// Transition manager query port for access to refernce and cached data
        /// </summary>
        public ITransitionQueryPort QueryPort => QueryManager;

        /// <summary>
        /// Transition manager event port for access to the managers push notification systems
        /// </summary>
        public new ITransitionEventPort EventPort => EventManager;

        /// <summary>
        /// Creates new transition manager with the provided project services and model data object
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="data"></param>
        public TransitionManager(IModelProject modelProject, TransitionModelData data) : base(modelProject, data)
        {

        }

        /// <summary>
        /// Get the type of the interface used by the manager
        /// </summary>
        /// <returns></returns>
        public override Type GetManagerInterfaceType()
        {
            return typeof(ITransitionManager);
        }

        /// <summary>
        /// Create a validation service for transition data using the provided settings information
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public override IValidationService CreateValidationService(ProjectSettings settings)
        {
            if (!settings.TryGetModuleSettings(out MocassinTransitionSettings moduleSettings))
                throw new InvalidOperationException("Settings object for the transition module is missing");

            return new TransitionValidationService(moduleSettings, ModelProject);
        }
    }
}
