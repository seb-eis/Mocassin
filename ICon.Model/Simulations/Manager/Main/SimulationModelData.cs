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
        /// List of all indexed single kinetic simulation model objects
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IKineticSimulation))]
        public List<KineticSimulation> KineticSimulations { get; set; }

        /// <summary>
        /// List of all indexed single metropolis simulation model objects
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IMetropolisSimulation))]
        public List<MetropolisSimulation> MetropolisSimulations { get; set; }

        /// <summary>
        /// List of all indexed kinetic simulation series model objects
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IKineticSimulationSeries))]
        public List<KineticSimulationSeries> KineticSeries { get; set; }

        /// <summary>
        /// List of all indexed metropolis simulation series model objects
        /// </summary>
        [DataMember]
        [IndexedModelData(typeof(IMetropolisSimulationSeries))]
        public List<MetropolisSimulationSeries> MetropolisSeries { get; set; }

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

        /// <summary>
        /// Creates a new default simulation model data object
        /// </summary>
        /// <returns></returns>
        public static SimulationModelData CreateNew()
        {
            return CreateDefault<SimulationModelData>();
        }
    }
}
