using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Transitions.ConflictHandling
{
    /// <summary>
    /// Object handler that handles internal data changes of the transition manager system required after a new metropolis transition input
    /// </summary>
    public class MetropolisTransitionAddedHandler : MetropolisTransitionHandlerBase
    {

        /// <summary>
        /// Create new metropolis transition added handler with the provided data accessor and project services
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <param name="modelProject"></param>
        public MetropolisTransitionAddedHandler(IDataAccessor<TransitionModelData> dataAccess, IModelProject modelProject)
            : base(dataAccess, modelProject)
        {
        }

        /// <summary>
        /// Determine the conflicts induced by the new metropolis transition and update the transition model data structure with the changes
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dataAccess"></param>
        /// <param name="projectServices"></param>
        /// <returns></returns>
        public override ConflictReport HandleConflicts(MetropolisTransition obj)
        {
            var report = new ConflictReport();
            CreateNewRulesandUpdateModelData(obj, report);
            return report;
        }

        /// <summary>
        /// Creates new rule set and links them to the kinetic parent transition. Additionally adds the new rule set to the model data object with a new indexing
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="report"></param>
        protected void CreateNewRulesandUpdateModelData(MetropolisTransition transition, ConflictReport report)
        {
            transition.TransitionRules = CreateTransitionRules(transition).ToList();
            DataAccess.Query(data => data.MetropolisRules.AddRange(transition.TransitionRules));

            var detail0 = $"Automatically added number of new metropolis model rules is ({transition.TransitionRules.Count})";
            report.AddWarning(ModelMessageSource.CreateConflictHandlingWarning(this, detail0));
        }
    }
}
