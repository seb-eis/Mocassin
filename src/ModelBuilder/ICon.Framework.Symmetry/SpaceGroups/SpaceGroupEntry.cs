using System;
using Mocassin.Framework.Constraints;
using Mocassin.Symmetry.CrystalSystems;

namespace Mocassin.Symmetry.SpaceGroups
{
    /// <summary>
    ///     Space group entry class that contains all information required to fully define a space group
    /// </summary>
    public class SpaceGroupEntry : IComparable<SpaceGroupEntry>, IEquatable<SpaceGroupEntry>
    {
        /// <summary>
        ///     The group number constraint from 1 to 230
        /// </summary>
        private static ValueConstraint<int> GroupNumberConstraint { get; } = new ValueConstraint<int>(true, 1, 230, true);

        /// <summary>
        ///     The group number
        /// </summary>
        public int GroupNumber { get; }

        /// <summary>
        ///     The name of the space group
        /// </summary>
        public string Literal { get; }

        /// <summary>
        ///     The name of the variation
        /// </summary>
        public string VariationName { get; }

        /// <summary>
        ///     The <see cref="CrystalSystemVariation" /> that identifies special versions of a group
        /// </summary>
        public CrystalSystemVariation CrystalVariation { get; }

        /// <summary>
        ///     Default constructor for a space group entry (Should be used for serialization only)
        /// </summary>
        public SpaceGroupEntry()
        {
        }

        /// <summary>
        ///     Creates a new <see cref="SpaceGroupEntry" /> from group number, literal and variation
        /// </summary>
        /// <param name="groupNumber"></param>
        /// <param name="literal"></param>
        /// <param name="variationName"></param>
        /// <param name="crystalVariation"></param>
        public SpaceGroupEntry(int groupNumber, string literal, string variationName, CrystalSystemVariation crystalVariation)
        {
            if (GroupNumberConstraint.IsValid(groupNumber) == false)
                throw new ArgumentOutOfRangeException(nameof(groupNumber));

            GroupNumber = groupNumber;
            Literal = literal ?? throw new ArgumentNullException(nameof(literal));
            VariationName = variationName ?? throw new ArgumentNullException(nameof(variationName));
            CrystalVariation = crystalVariation;
        }

        /// <summary>
        ///     Create directly from a space group object
        /// </summary>
        /// <param name="group"></param>
        public SpaceGroupEntry(ISpaceGroup group)
            : this(group.InternationalIndex, group.MauguinNotation, group.VariationName, group.CrystalVariation)
        {
            if (group == null) throw new ArgumentNullException(nameof(group));
        }

        /// <summary>
        ///     Compares the identifier
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(SpaceGroupEntry other)
        {
            var numberCompare = GroupNumber.CompareTo(other.GroupNumber);
            if (numberCompare != 0) return numberCompare;

            var variationComp = string.Compare(VariationName, other.VariationName, StringComparison.Ordinal);
            if (variationComp != 0) return variationComp;

            var nameCompare = string.Compare(Literal, other.Literal, StringComparison.Ordinal);
            return nameCompare == 0
                ? CrystalVariation.CompareTo(other.CrystalVariation)
                : nameCompare;
        }

        /// <inheritdoc />
        public bool Equals(SpaceGroupEntry other) => CompareTo(other) == 0;

        /// <summary>
        ///     Creates the default space group entry (P1 group)
        /// </summary>
        /// <returns></returns>
        public static SpaceGroupEntry CreateDefault() => new SpaceGroupEntry(1, "P1", "None", CrystalSystemVariation.NoneOrOriginChoice);
    }
}