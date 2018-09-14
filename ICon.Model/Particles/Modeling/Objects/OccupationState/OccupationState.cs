using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Collections;
using ICon.Framework.Extensions;

namespace ICon.Model.Particles
{
    /// <summary>
    /// Represents an occupation state for a sequnce of positions that are occupied by a series of particle objects
    /// </summary>
    [DataContract]
    public class OccupationState : IOccupationState
    {
        /// <summary>
        /// The length of the occupation state
        /// </summary>
        [IgnoreDataMember]
        public int StateLength => Particles.Count;

        /// <summary>
        /// The particle array that describes the occupation state of a sequence of positions
        /// </summary>
        [DataMember]
        public List<IParticle> Particles { get; set; }

        /// <summary>
        /// Read only interface access to the set of particles
        /// </summary>
        [IgnoreDataMember]
        IReadOnlyList<IParticle> IOccupationState.Particles => Particles;

        /// <summary>
        /// Index based access on the particle array of the occupation state
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IParticle this[int index]
        {
            get => Particles[index];
            set => Particles[index] = value;
        }

        /// <summary>
        /// Default construct an empty occupation state
        /// </summary>
        public OccupationState()
        {
        }

        /// <summary>
        /// Construct a new occupation state from an occupation state interface
        /// </summary>
        /// <param name="state"></param>
        public OccupationState(IOccupationState state)
        {
            Particles = state.ToList();
        }

        /// <summary>
        /// Creates an index occupation code from the particle seqeunce of the occupation state
        /// </summary>
        /// <returns></returns>
        public OccupationCode AsCode()
        {
            return new OccupationCode() { CodeValues = Particles.Select(a => a.Index).ToArray() };
        }

        /// <summary>
        /// Creates a deep copy of the occupation state
        /// </summary>
        /// <returns></returns>
        public OccupationState DeepCopy()
        {
            var state = new OccupationState() { Particles = new List<IParticle>(Particles.Count) };
            state.Particles.AddRange(Particles);
            return state;
        }

        /// <summary>
        /// Get the enumerator for the particle array
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IParticle> GetEnumerator()
        {
            return Particles.AsEnumerable().GetEnumerator();
        }

        /// <summary>
        /// Get the enumerator for the particle array
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Checks for exact equality to other occupation state
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IOccupationState other)
        {
            if (StateLength != other.StateLength)
            {
                return false;
            }
            for (int i = 0; i < Particles.Count; i++)
            {
                if (!Particles[i].Equals(other.Particles[i]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Get the hash code of the particle set
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Particles.Sum(value => value.GetHashCode());
        }

        /// <summary>
        /// Compares to other occupation state
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(IOccupationState other)
        {
            var comparer = Comparer<IParticle>.Create((a, b) => a.Index.CompareTo(b.Index));
            return Particles.LexicographicCompare(other.Particles, comparer);
        }
    }
}
