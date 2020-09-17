using System.Linq;
using System.Text;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Transitions.ConflictHandling
{
    /// <summary>
    ///     Object handler that handles internal data changes of the transition manager system required after a kinetic
    ///     transition is removed/deprecated
    /// </summary>
    public class MetropolisTransitionRemovedHandler : MetropolisTransitionHandlerBase
    {
        /// <inheritdoc />
        public MetropolisTransitionRemovedHandler(IDataAccessor<TransitionModelData> dataAccessor, IModelProject modelProject)
            : base(dataAccessor, modelProject)
        {
        }

        /// <inheritdoc />
        public override ConflictReport HandleConflicts(MetropolisTransition obj)
        {
            var report = new ConflictReport();
            DeprecateRules(obj, report);
            return report;
        }

        /// <summary>
        ///     Deprecates all rules of the transition in the model data and writes the information to the report
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="report"></param>
        protected void DeprecateRules(MetropolisTransition transition, ConflictReport report)
        {
            var indexManager = new IndexedDataManager<MetropolisRule>();
            var builder = new StringBuilder(transition.TransitionRules.Count * 2);
            builder.BuildCommaSeparatedValueString(indexManager.DeprecateAll(transition.TransitionRules, a => true).ToArray());

            var detail0 = $"Metropolis rules at indices ({builder}) are no longer valid and where marked as deprecated";
            report.AddWarning(ModelMessageSource.CreateConflictHandlingWarning(this, detail0));
        }
    }
}