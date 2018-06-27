using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Particles
{
    /// <summary>
    /// Represents an occupation state that fully describes the occupation of a set of positions as a set of particles
    /// </summary>
    public interface IOccupationState : IEnumerable<IParticle>, IEquatable<IOccupationState>
    {
        /// <summary>
        /// The number of positions within the occupation state
        /// </summary>
        int StateLength { get; }

        /// <summary>
        /// Read only list of particles that describe the occupation
        /// </summary>
        IReadOnlyList<IParticle> Particles { get; }

        /// <summary>
        /// Direct index access to the particle at the provided index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IParticle this[int index] { get; }
    }
}
