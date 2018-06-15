using System;
using System.Collections.Generic;
using System.Text;

using ICon.Mathematics.Bitmasks;
using ICon.Model.Basic;

namespace ICon.Model.Particles
{
    /// <summary>
    /// Represents a set of model particles that describe multiple possible species for one position in a simulation project
    /// </summary>
    public interface IParticleSet : IModelObject
    {
        /// <summary>
        /// Get the number of particles in this particle set
        /// </summary>
        int ParticleCount { get; }

        /// <summary>
        /// Get the bitmask encoded version of the particle set
        /// </summary>
        Bitmask64 GetEncoded();

        /// <summary>
        /// Get the seqeunce of particles that belong to this particle set
        /// </summary>
        /// <returns></returns>
        IEnumerable<IParticle> GetParticles();

        /// <summary>
        /// Check if the particle set interface equals another in all model relevant properties
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool EqualsInModelProperties(IParticleSet other);

        /// <summary>
        /// Checks if the particle set occupation mask is 0 therefore carries no possible occupation other than void
        /// </summary>
        /// <returns></returns>
        bool IsEmpty();
    }
}
