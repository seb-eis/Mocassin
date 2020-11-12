using System;
using System.Collections.Generic;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Particles
{
    /// <inheritdoc cref="IParticle" />
    public class Particle : ModelObject, IParticle
    {
        /// <summary>
        ///     The const index that identifies the void particle
        /// </summary>
        public const int VoidIndex = 0;

        /// <inheritdoc />
        public double Charge { get; set; }

        /// <inheritdoc />
        public string Symbol { get; set; }

        /// <inheritdoc />
        public bool IsVacancy { get; set; }

        /// <inheritdoc />
        public bool IsVoid => Index == VoidIndex;

        /// <inheritdoc />
        public override string ObjectName => "Particle";

        /// <summary>
        ///     Compares particle by name, symbol and then charge (Charge is not compared with tolerance)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(IParticle other)
        {
            var nameCompare = string.Compare(Name, other.Name, StringComparison.Ordinal);
            if (nameCompare != 0)
                return nameCompare;

            var symbolCompare = string.Compare(Symbol, other.Symbol, StringComparison.Ordinal);

            return symbolCompare != 0 ? symbolCompare : Charge.CompareTo(other.Charge);
        }

        /// <inheritdoc />
        public bool EqualsInModelProperties(IParticle other, IComparer<double> comparer) =>
            Name == other.Name && Symbol == other.Symbol && comparer.Compare(Charge, other.Charge) == 0;

        /// <inheritdoc />
        public string GetIonString() => Math.Abs(Charge) < 1e-10 ? Symbol : $"{Symbol}{(Charge < 0 ? $"{Math.Abs(Charge):#.-}" : $"{Charge:#.+}")}";

        /// <inheritdoc />
        public bool Equals(IParticle other) => other != null && Index == other.Index;

        /// <summary>
        ///     Creates a void particle, this particle represents an active but context unavailable particle and should always have
        ///     the index 0 in a particle manager
        /// </summary>
        /// <returns></returns>
        public static Particle CreateVoid() => new Particle
            {Name = "Void", Symbol = "Void", Key = "Particle.Void", Charge = 0.0, Index = VoidIndex};


        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IParticle>(obj) is { } particle)) return null;
            if (particle.IsVoid) throw new ArgumentException("Empty particle object interface reached consume function");

            Name = particle.Name;
            Symbol = particle.Symbol;
            Charge = particle.Charge;
            IsVacancy = particle.IsVacancy;
            return this;
        }

        /// <summary>
        ///     Get the hash code of the particle based upon the particle index
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => 1 << Index;
    }
}