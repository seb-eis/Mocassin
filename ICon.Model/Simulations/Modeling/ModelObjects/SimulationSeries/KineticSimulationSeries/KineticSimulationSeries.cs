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
    public class KineticSimulationSeries : SimulationSeriesBase, IKineticSimulationSeries
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
        [DataMember]
        public new IKineticSimulation BaseSimulation { get; set; }

        /// <summary>
        /// Get a string representing the model object name
        /// </summary>
        /// <returns></returns>
        public override string GetModelObjectName()
        {
            return "'Kinetic Simulation Series'";
        }

        /// <summary>
        /// Populate this object from a model object intreface and return this. Retruns null if the population failed
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (CastWithDepricatedCheck<IKineticSimulationSeries>(base.PopulateFrom(obj)) is IKineticSimulationSeries series)
            {
                ElectricFieldSeries = series.ElectricFieldSeries;
                NormalizationProbabilitySeries = series.NormalizationProbabilitySeries;
                return this;
            }
            return null;
        }
    }
}
