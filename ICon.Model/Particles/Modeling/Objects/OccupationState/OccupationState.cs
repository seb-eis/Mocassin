using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Mocassin.Framework.Extensions;

namespace Mocassin.Model.Particles
{
    /// <inheritdoc />
    [DataContract]
    public class OccupationState : IOccupationState
    {
        /// <inheritdoc />
        [IgnoreDataMember]
        public int StateLength => Particles.Count;

        /// <summary>
        ///     The particle array that describes the occupation state of a sequence of positions
        /// </summary>
        [DataMember]
        public List<IParticle> Particles { get; set; }

        /// <inheritdoc />
        [IgnoreDataMember]
        IReadOnlyList<IParticle> IOccupationState.Particles => Particles;

        /// <inheritdoc />
        public IParticle this[int index]
        {
            get => Particles[index];
            set => Particles[index] = value;
        }

        /// <summary>
        ///     Default construct an empty occupation state
        /// </summary>
        public OccupationState()
        {
        }

        /// <summary>
        ///     Construct a new occupation state from an occupation state interface
        /// </summary>
        /// <param name="state"></param>
        public OccupationState(IOccupationState state)
        {
            Particles = state.ToList();
        }

        /// <summary>
        ///     Creates an index occupation code from the particle sequence of the occupation state
        /// </summary>
        /// <returns></returns>
        public OccupationCode AsCode()
        {
            return new OccupationCode {CodeValues = Particles.Select(a => a.Index).ToArray()};
        }

        /// <summary>
        ///     Creates a deep copy of the occupation state
        /// </summary>
        /// <returns></returns>
        public OccupationState DeepCopy()
        {
            var state = new OccupationState {Particles = new List<IParticle>(Particles.Count)};
            state.Particles.AddRange(Particles);
            return state;
        }

        /// <inheritdoc />
        public IEnumerator<IParticle> GetEnumerator()
        {
            return Particles.AsEnumerable().GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public bool Equals(IOccupationState other)
        {
            if (other == null)
                return false;

            if (StateLength != other.StateLength) 
                return false;

            return !Particles
                .Where((t, i) => !t.Equals(other.Particles[i]))
                .Any();
        }

        /// <summary>
        ///     Get the hash code of the particle set
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Particles.Sum(value => value.GetHashCode());
        }

        /// <summary>
        ///     Compares to other occupation state
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