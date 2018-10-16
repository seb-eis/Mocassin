﻿using System;
using System.Runtime.Serialization;

namespace ICon.Model.Energies
{
    /// <summary>
    ///     Represents an symmetric pair of particles to identify pair interactions where the order is not relevant
    /// </summary>
    [DataContract]
    public class SymmetricParticlePair : ParticlePair, IEquatable<SymmetricParticlePair>
    {
        /// <inheritdoc />
        public bool Equals(SymmetricParticlePair other)
        {
            if (other != null && Particle0.Index != other.Particle0.Index)
                return Particle0.Index == other.Particle1.Index && Particle1.Index == other.Particle0.Index;

            return Particle1.Index == other.Particle1.Index;
        }

        /// <inheritdoc />
        public override bool Equals(ParticlePair other)
        {
            if (other is SymmetricParticlePair pair) 
                return Equals(pair);

            return false;
        }
    }
}