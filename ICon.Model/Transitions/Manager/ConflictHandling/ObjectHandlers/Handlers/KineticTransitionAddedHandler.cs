using System;
using System.Collections.Generic;
using System.Linq;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Transitions.ConflictHandling
{
    /// <summary>
    /// Object handler that handles internal data changes of the transition manager system required after a new kinetic transition input
    /// </summary>
    public class KineticTransitionAddedHandler : KineticTransitionHandlerBase
    {
        /// <summary>
        /// Create new kinetic transition added handler with the provided project services and data accessor
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <param name="projectServices"></param>
        public KineticTransitionAddedHandler(IDataAccessor<TransitionModelData> dataAccess, IProjectServices projectServices)
            : base(dataAccess, projectServices)
        {
        }

        /// <summary>
        /// Determine the conflicts induced by the new kinetic transition and update the transition model data structure with the changes
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dataAccess"></param>
        /// <param name="projectServices"></param>
        /// <returns></returns>
        public override ConflictReport HandleConflicts(KineticTransition obj)
        {
            var report = new ConflictReport();
            CreateNewRulesAndUpdateModelData(obj, report);
            return report;
        }

        /// <summary>
        /// Creates new rule set and links them to the kinetic parent transition. Additionally adds the new rule set to the model data object with a new indexing
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="report"></param>
        protected void CreateNewRulesAndUpdateModelData(KineticTransition transition, ConflictReport report)
        {
            transition.TransitionRules = CreateTransitionRules(transition).ToList();
            IndexAndAddToModelData(transition.TransitionRules);

            var detail0 = $"Automatically added number of new kinetic model rules is ({transition.TransitionRules.Count})";
            report.AddWarning(ModelMessages.CreateConflictHandlingWarning(this, detail0));
        }
    }
}
