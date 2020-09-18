using System;
using System.Diagnostics;

namespace Mocassin.Symmetry.CrystalSystems
{
    /// <summary>
    ///     Defines a clear identification for a crystal system by <see cref="CrystalSystemType" /> and
    ///     <see cref="CrystalSystemVariation" />
    /// </summary>
    [DebuggerDisplay("{CrystalType} & {CrystalVariation}")]
    public readonly struct CrystalSystemIdentification : IEquatable<CrystalSystemIdentification>
    {
        /// <summary>
        ///     Get the default triclinic <see cref="CrystalSystemIdentification" />
        /// </summary>
        public static CrystalSystemIdentification Triclinic { get; } =
            new CrystalSystemIdentification(CrystalSystemType.Triclinic, CrystalSystemVariation.NoneOrOriginChoice);

        /// <summary>
        ///     Get the <see cref="CrystalSystemType" />
        /// </summary>
        public CrystalSystemType CrystalType { get; }

        /// <summary>
        ///     Get the <see cref="CrystalSystemVariation" />
        /// </summary>
        public CrystalSystemVariation CrystalVariation { get; }

        /// <summary>
        ///     Creates a new crystal identifier by <see cref="CrystalSystemType" /> and <see cref="CrystalSystemVariation" />
        /// </summary>
        /// <param name="crystalType"></param>
        /// <param name="crystalVariation"></param>
        public CrystalSystemIdentification(CrystalSystemType crystalType, CrystalSystemVariation crystalVariation)
        {
            CrystalType = crystalType;
            CrystalVariation = crystalVariation;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is CrystalSystemIdentification identification && Equals(identification);

        /// <inheritdoc />
        public bool Equals(CrystalSystemIdentification other) =>
            CrystalType == other.CrystalType &&
            CrystalVariation == other.CrystalVariation;

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(CrystalType, CrystalVariation);
    }
}