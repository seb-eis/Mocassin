using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using ICon.Mathematics.ValueTypes;
using ICon.Model.Basic;
using ICon.Model.Transitions;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Enum for the specific kinetic simulation flags that extend the simulation base flags
    /// </summary>
    public enum KineticSimulationFlags
    {
        UseStaticTrackers = 0b1, UseDynamicTrackers = 0b10
    }

    /// <summary>
    /// Implementation of the specialized monte carlo simulation that contains all refernce data for a kinetic monte carlo simulation
    /// </summary>
    [DataContract]
    public class KineticSimulation : SimulationBase, IKineticSimulation
    {
        /// <summary>
        /// Read only interface access to the electric field vector inf fractional coordinates
        /// </summary>
        Fractional3D IKineticSimulation.ElectricFieldVector => ElectricFieldVector.AsFractional();

        /// <summary>
        /// Get a read only interface access to the transitions of this simulation
        /// </summary>
        IReadOnlyList<IKineticTransition> IKineticSimulation.Transitions => Transitions;

        /// <summary>
        /// The probability value used for dynamic normalization of all jump attempts
        /// </summary>
        [DataMember]
        public double NormalizationProbability { get; set; }

        /// <summary>
        /// Get the magnitude of the electric field in [V/m]
        /// </summary>
        [DataMember]
        public double ElectricFieldMagnitude { get; set; }

        /// <summary>
        /// The set of transitions attached to the simulation
        /// </summary>
        [DataMember]
        [IndexResolved]
        public List<IKineticTransition> Transitions { get; set; }

        /// <summary>
        /// Specific simulation flags for kinetic simulation settings
        /// </summary>
        [DataMember]
        public KineticSimulationFlags KineticFlags { get; set; }

        /// <summary>
        /// The electric field vector in fractional coodrinates
        /// </summary>
        [DataMember]
        public DataVector3D ElectricFieldVector { get; set; }

        /// <summary>
        /// Returns a string representing the model object name
        /// </summary>
        /// <returns></returns>
        public override string GetModelObjectName()
        {
            return "'Kinetic Simulation'";
        }

        /// <summary>
        /// Populates this object from a model object interface and returns it as a basic model object. Retruns null if population fails
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (CastWithDepricatedCheck<IKineticSimulation>(base.PopulateFrom(obj)) is IKineticSimulation simulation)
            {
                NormalizationProbability = simulation.NormalizationProbability;
                ElectricFieldVector = new DataVector3D(simulation.ElectricFieldVector);
                KineticFlags = simulation.KineticFlags;
                Transitions = (simulation.Transitions ?? new List<IKineticTransition>()).ToList();
                return this;
            }
            return null;
        }
    }
}
