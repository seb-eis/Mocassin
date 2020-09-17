using System.Collections.Generic;
using System.Runtime.Serialization;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    ///     The simulation model data object that stores model objects and parameters affiliated with simulation package
    ///     building
    /// </summary>
    [DataContract]
    public class SimulationModelData : ModelData<ISimulationDataPort>
    {
        /// <summary>
        ///     List of all indexed single kinetic simulation model objects
        /// </summary>
        [DataMember, IndexedModelData(typeof(IKineticSimulation))]
        public List<KineticSimulation> KineticSimulations { get; set; }

        /// <summary>
        ///     List of all indexed single metropolis simulation model objects
        /// </summary>
        [DataMember, IndexedModelData(typeof(IMetropolisSimulation))]
        public List<MetropolisSimulation> MetropolisSimulations { get; set; }

        /// <inheritdoc />
        public override ISimulationDataPort AsReadOnly() => new SimulationDataManager(this);

        /// <inheritdoc />
        public override void ResetToDefault()
        {
            ResetAllIndexedData();
        }

        /// <summary>
        ///     Creates a new default simulation model data object
        /// </summary>
        /// <returns></returns>
        public static SimulationModelData CreateNew() => CreateDefault<SimulationModelData>();
    }
}