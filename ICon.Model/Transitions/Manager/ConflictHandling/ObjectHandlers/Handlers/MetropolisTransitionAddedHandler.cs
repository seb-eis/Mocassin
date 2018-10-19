using System.Linq;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Transitions.ConflictHandling
{
    /// <summary>
    ///     Object handler that handles internal data changes of the transition manager system required after a new metropolis
    ///     transition input
    /// </summary>
    public class MetropolisTransitionAddedHandler : MetropolisTransitionHandlerBase
    {
        /// <inheritdoc />
        public MetropolisTransitionAddedHandler(IDataAccessor<TransitionModelData> dataAccessor, IModelProject modelProject)
            : base(dataAccessor, modelProject)
        {
        }

        /// <inheritdoc />
        public override ConflictReport HandleConflicts(MetropolisTransition obj)
        {
            var report = new ConflictReport();
            CreateNewRulesAndUpdateModelData(obj, report);
            return report;
        }

        /// <summary>
        ///     Creates new rule set and links them to the kinetic parent transition. Additionally adds the new rule set to the
        ///     model data object with a new indexing
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="report"></param>
        protected void CreateNewRulesAndUpdateModelData(MetropolisTransition transition, ConflictReport report)
        {
            transition.TransitionRules = CreateTransitionRules(transition).ToList();
            DataAccess.Query(data => data.MetropolisRules.AddRange(transition.TransitionRules));

            var detail0 = $"Automatically added number of new metropolis model rules is ({transition.TransitionRules.Count})";
            report.AddWarning(ModelMessageSource.CreateConflictHandlingWarning(this, detail0));
        }
    }
}