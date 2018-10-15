using System;
using System.Collections.Generic;
using System.Text;
using ICon.Mathematics.Permutation;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Abstract base permuter for simulation series that permutes the parameters and procudes translatable simulation objects
    /// </summary>
    public abstract class SimulationSeriesPermuter
    {
        /// <summary>
        /// The list of value setter actions for the properties
        /// </summary>
        public IList<Action<object>> ValueSetter { get; protected set; }

        /// <summary>
        /// The list of permutable sets of values
        /// </summary>
        public IList<IEnumerable<object>> PermutableSets { get; protected set; }

        /// <summary>
        /// Initializes the base permutation sets of the unspecified simulation series
        /// </summary>
        /// <param name="baseSeries"></param>
        protected void InitializeBaseInformation(ISimulationSeries baseSeries)
        {

        }
    }
}
