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
        UseStaticTracking = 0b1, UseSingleAtomTracking = 0b10, UseProbabilityTracking = 0b100
    }

    /// <summary>
    /// Implementation of the specialized monte carlo simulation that contains all refernce data for a kinetic monte carlo simulation
    /// </summary>
    [DataContract]
    public class KineticSimulation : CustomSimulation, IKineticSimulation
    {
        /// <summary>
        /// Read only interface access to the electric field vector inf fractional coordinates
        /// </summary>
        Fractional3D IKineticSimulation.ElectricFieldVector => ElectricFieldVector.AsFractional();

        /// <summary>
        /// The probability value used for dynamic normalization of all jump attempts
        /// </summary>
        [DataMember]
        public double NormalizationProbability { get; set; }

        /// <summary>
        /// The set of transitions attached to the simulation
        /// </summary>
        [DataMember]
        [LinkableByIndex]
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
        /// Get all transitions attached to this simulation
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IKineticTransition> GetTransitions()
        {
            return Transitions.AsEnumerable();
        }

        /// <summary>
        /// Populates this object from a model object interface and returns it as a basic model object. Retruns null if population fails
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateObject(IModelObject obj)
        {
            if (CastWithDepricatedCheck<IKineticSimulation>(base.PopulateObject(obj)) is IKineticSimulation simulation)
            {
                NormalizationProbability = simulation.NormalizationProbability;
                KineticFlags = simulation.KineticFlags;
                Transitions = simulation.GetTransitions().ToList();
                return this;
            }
            return null;
        }
    }
}
