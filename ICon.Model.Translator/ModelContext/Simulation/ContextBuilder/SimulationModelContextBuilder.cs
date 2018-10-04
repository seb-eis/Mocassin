using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc cref="ICon.Model.Translator.ModelContext.ISimulationModelContextBuilder"/>
    public class SimulationModelContextBuilder : ModelContextBuilderBase<ISimulationModelContext>, ISimulationModelContextBuilder
    {
        /// <inheritdoc />
        public SimulationModelContextBuilder(IProjectModelContextBuilder projectModelContextBuilder) : base(projectModelContextBuilder)
        {
        }

        /// <inheritdoc />
        protected override ISimulationModelContext PopulateContext(ISimulationModelContext modelContext)
        {
            return modelContext;
        }

        /// <inheritdoc />
        protected override ISimulationModelContext GetEmptyDefaultContext()
        {
            return new SimulationModelContext();
        }
    }
}
