using System.Collections.Generic;
using System.Linq;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Simulations
{
    /// <inheritdoc cref="IKineticSimulation" />
    public class KineticSimulation : SimulationBase, IKineticSimulation
    {
        /// <inheritdoc />
        IReadOnlyList<IKineticTransition> IKineticSimulation.Transitions => Transitions;

        /// <inheritdoc />
        public int PreRunMcsp { get; set; }

        /// <inheritdoc />
        public double NormalizationEnergy { get; set; }

        /// <inheritdoc />
        public double ElectricFieldMagnitude { get; set; }

        /// <summary>
        ///     The set of transitions attached to the simulation
        /// </summary>
        [UseTrackedData]
        public List<IKineticTransition> Transitions { get; set; }

        /// <summary>
        ///     The electric field vector in fractional coordinates
        /// </summary>
        public Fractional3D ElectricFieldVector { get; set; }

        /// <inheritdoc />
        public override string ObjectName => "Kinetic Simulation";

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IKineticSimulation>(obj) is { } simulation))
                return null;

            base.PopulateFrom(obj);
            NormalizationEnergy = simulation.NormalizationEnergy;
            ElectricFieldMagnitude = simulation.ElectricFieldMagnitude;
            ElectricFieldVector = simulation.ElectricFieldVector;
            Transitions = (simulation.Transitions ?? new List<IKineticTransition>()).ToList();
            PreRunMcsp = simulation.PreRunMcsp;
            return this;
        }
    }
}