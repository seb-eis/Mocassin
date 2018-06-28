using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ICon.Model.Basic;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// The simulation model data object that stores model objects and parameters affiliated with simulation package building
    /// </summary>
    [DataContract]
    public class SimulationModelData : ModelData<ISimulationDataPort>
    {
        /// <summary>
        /// Creates an adapter interface that provides read only access to the model data object
        /// </summary>
        /// <returns></returns>
        public override ISimulationDataPort AsReadOnly()
        {
            return new SimulationDataManager(this);
        }

        /// <summary>
        /// Reset the model data object to default conditions
        /// </summary>
        public override void ResetToDefault()
        {
            ResetAllIndexedData();
        }
    }
}
