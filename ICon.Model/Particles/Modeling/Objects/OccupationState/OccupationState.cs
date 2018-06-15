using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Collections;

namespace ICon.Model.Particles
{
    /// <summary>
    /// Represents an occupation state for a sequnce of positions that are occupied by a series of particle objects
    /// </summary>
    [DataContract]
    public class OccupationState : IEnumerable<IParticle>
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
        /// Index based access on the particle array of the occupation state
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IParticle this[int index]
        {
            get { return Particles[index]; }
            set { Particles[index] = value; }
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
    }
}
