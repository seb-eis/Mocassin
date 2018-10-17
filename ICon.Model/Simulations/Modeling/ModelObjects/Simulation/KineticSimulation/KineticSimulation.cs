using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    /// Enum for the specific kinetic simulation flags that extend the simulation base flags
    /// </summary>
    [Flags]
    public enum KineticSimulationFlags
    {
        UseStaticTrackers = 0b1, UseDynamicTrackers = 0b10
    }

    /// <inheritdoc cref="IKineticSimulation"/>
    [DataContract]
    public class KineticSimulation : SimulationBase, IKineticSimulation
    {
        /// <inheritdoc />
        [IgnoreDataMember]
        Fractional3D IKineticSimulation.ElectricFieldVector => ElectricFieldVector.AsFractional();

        /// <inheritdoc />
        [IgnoreDataMember]
        IReadOnlyList<IKineticTransition> IKineticSimulation.Transitions => Transitions;

        /// <inheritdoc />
        [DataMember]
        public double NormalizationProbability { get; set; }

        /// <inheritdoc />
        [DataMember]
        public double ElectricFieldMagnitude { get; set; }

        /// <summary>
        /// The set of transitions attached to the simulation
        /// </summary>
        [DataMember]
        [IndexResolved]
        public List<IKineticTransition> Transitions { get; set; }

        /// <inheritdoc />
        [DataMember]
        public KineticSimulationFlags KineticFlags { get; set; }

        /// <summary>
        /// The electric field vector in fractional coordinates
        /// </summary>
        [DataMember]
        public DataVector3D ElectricFieldVector { get; set; }

        /// <inheritdoc />
        public override string GetObjectName()
        {
            return "'Kinetic Simulation'";
        }

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IKineticSimulation>(obj) is IKineticSimulation simulation))
                return null;

            base.PopulateFrom(obj);
            NormalizationProbability = simulation.NormalizationProbability;
            ElectricFieldVector = new DataVector3D(simulation.ElectricFieldVector);
            KineticFlags = simulation.KineticFlags;
            Transitions = (simulation.Transitions ?? new List<IKineticTransition>()).ToList();
            return this;
        }
    }
}
