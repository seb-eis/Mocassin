using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ICon.Framework.Constraints;
using ICon.Model.Basic;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Implementation of a specialized simulation series for sets of kinetic monte carlo simulations
    /// </summary>
    [DataContract]
    public class KineticSimulationSeries : SimulationSeries, IKineticSimulationSeries
    {
        /// <summary>
        /// The value series for the simulation electric field magnitude
        /// </summary>
        [DataMember]
        public IValueSeries ElectricFieldSeries { get; set; }

        /// <summary>
        /// The value series for the simulation normlization values
        /// </summary>
        [DataMember]
        public IValueSeries NormalizationProbabilitySeries { get; set; }

        /// <summary>
        /// Interface access to the kinetic base simulation
        /// </summary>
        IKineticSimulation IKineticSimulationSeries.BaseSimulation => (IKineticSimulation)BaseSimulation;

        public override string GetModelObjectName()
        {
            throw new NotImplementedException();
        }

        public override ModelObject PopulateObject(IModelObject obj)
        {
            return base.PopulateObject(obj);
        }
    }
}
