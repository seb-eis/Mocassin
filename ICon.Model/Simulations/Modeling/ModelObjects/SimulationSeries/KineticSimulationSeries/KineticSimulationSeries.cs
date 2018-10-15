using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ICon.Framework.Constraints;
using ICon.Model.Basic;

namespace ICon.Model.Simulations
{
    /// <inheritdoc cref="ICon.Model.Simulations.IKineticSimulationSeries"/>
    [DataContract]
    public class KineticSimulationSeries : SimulationSeriesBase, IKineticSimulationSeries
    {
        /// <inheritdoc />
        [DataMember]
        public IValueSeries ElectricFieldSeries { get; set; }

        /// <inheritdoc />
        [DataMember]
        public IValueSeries NormalizationProbabilitySeries { get; set; }

        /// <inheritdoc />
        [DataMember]
        public new IKineticSimulation BaseSimulation { get; set; }

        /// <inheritdoc />
        public override string GetObjectName()
        {
            return "'Kinetic Simulation Series'";
        }

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IKineticSimulationSeries>(obj) is IKineticSimulationSeries series))
                return null;

            base.PopulateFrom(obj);
            ElectricFieldSeries = series.ElectricFieldSeries;
            NormalizationProbabilitySeries = series.NormalizationProbabilitySeries;
            return this;
        }
    }
}
