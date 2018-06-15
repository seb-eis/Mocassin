using System;
using System.Collections.Generic;

using ICon.Model.Basic;

namespace ICon.Model.Particles
{
    /// <summary>
    /// Represents a single model species in a simulation project
    /// </summary>
    public interface IParticle : IModelObject, IComparable<IParticle>
    {
        /// <summary>
        /// The charge value in electron volts
        /// </summary>
        Double Charge { get; }

        /// <summary>
        /// The name of the particle
        /// </summary>
        String Name { get; }

        /// <summary>
        /// The short description symbol
        /// </summary>
        String Symbol { get; }

        /// <summary>
        /// Flag that indicates that the particle supports usage as a vacancy
        /// </summary>
        Boolean IsVacancy { get; }

        /// <summary>
        /// Flag that marks the particle as the 'Null-Particle' (Particle exists but cannot be accessed, only relevant for disabled periodic boundaries)
        /// </summary>
        Boolean IsEmpty { get; }

        /// <summary>
        /// Compares to another particle interface for equality in model relevant properties using the provided double comparer for the charge values
        /// </summary>
        /// <param name="other"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        Boolean EqualsInModelProperties(IParticle other, IComparer<Double> comparer);
    }
}