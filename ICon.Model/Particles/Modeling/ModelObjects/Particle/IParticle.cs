using System;
using System.Collections.Generic;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Particles
{
    /// <summary>
    ///     Represents a single model species in a simulation project
    /// </summary>
    public interface IParticle : IModelObject, IComparable<IParticle>, IEquatable<IParticle>
    {
        /// <summary>
        ///     The charge value in electron volts
        /// </summary>
        double Charge { get; }

        /// <summary>
        ///     The short description symbol
        /// </summary>
        string Symbol { get; }

        /// <summary>
        ///     Flag that indicates that the particle supports usage as a vacancy
        /// </summary>
        bool IsVacancy { get; }

        /// <summary>
        ///     Flag that marks the particle as the 'Null-Particle' representing the nothing state
        /// </summary>
        bool IsVoid { get; }

        /// <summary>
        ///     Compares to another particle interface for equality in model relevant properties using the provided double comparer
        ///     for the charge values
        /// </summary>
        /// <param name="other"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        bool EqualsInModelProperties(IParticle other, IComparer<double> comparer);

        /// <summary>
        ///     Get the <see cref="IParticle" /> as a typical ion description <see cref="string" /> used in chemistry
        /// </summary>
        /// <returns></returns>
        string GetIonString();
    }
}