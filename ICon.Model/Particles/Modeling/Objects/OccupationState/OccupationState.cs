using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;

namespace Mocassin.Model.Particles
{
    /// <inheritdoc cref="IOccupationState" />
    public class OccupationState : IEquatable<OccupationState>, IOccupationState
    {
        /// <inheritdoc />
        public int StateLength => Particles.Count;

        /// <summary>
        ///     The particle array that describes the occupation state of a sequence of positions
        /// </summary>
        public List<IParticle> Particles { get; set; }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public bool Equals(OccupationState other) => Equals(other as IOccupationState);

        /// <inheritdoc />
        public IEnumerator<IParticle> GetEnumerator() => Particles.AsEnumerable().GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public bool Equals(IOccupationState other)
        {
            if (other == null) return false;
            if (StateLength != other.StateLength) return false;

            return !Particles
                    .Where((t, i) => !t.Equals(other.Particles[i]))
                    .Any();
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

        /// <summary>
        ///     Get the hash code of the particle set
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Particles.Sum(value => value.GetHashCode());
        }
    }
}