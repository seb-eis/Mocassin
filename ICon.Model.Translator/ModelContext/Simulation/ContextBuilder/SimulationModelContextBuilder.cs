using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Builder for simulation model context objects
    /// </summary>
    public class SimulationModelContextBuilder : ModelContextBuilderBase<ISimulationModelContext>
    {
        /// <summary>
        /// Create nw context builder that is linked to the provided main context builder
        /// </summary>
        /// <param name="projectModelContextBuilder"></param>
        public SimulationModelContextBuilder(IProjectModelContextBuilder projectModelContextBuilder) : base(projectModelContextBuilder)
        {
        }

        protected override void PopulateContext()
        {
            throw new NotImplementedException();
        }
    }
}
