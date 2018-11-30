using System.Runtime.Serialization;
using Mocassin.Framework.Constraints;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Simulations
{
    /// <inheritdoc cref="IKineticSimulationSeries" />
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
            return "Kinetic Simulation Series";
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