using System;
using System.Runtime.Serialization;
using Mocassin.Framework.Constraints;

namespace Mocassin.Symmetry.SpaceGroups
{
    /// <summary>
    ///     Space group entry class that contains all information required to fully define a space group
    /// </summary>
    [DataContract(Name = "SpaceGroupEntry")]
    public class SpaceGroupEntry : IComparable<SpaceGroupEntry>, IEquatable<SpaceGroupEntry>
    {
        /// <summary>
        ///     The group number constraint from 1 to 230
        /// </summary>
        private static readonly ValueConstraint<int> IndexConstraint = new ValueConstraint<int>(true, 1, 230, true);

        /// <summary>
        ///     The group number
        /// </summary>
        [DataMember]
        public int Index { get; }

        /// <summary>
        ///     The name of the space group
        /// </summary>
        [DataMember]
        public string Literal { get; }

        /// <summary>
        ///     The specifier name, does only exist for groups with multiple specified versions
        /// </summary>
        [DataMember]
        public string Specifier { get; }

        /// <summary>
        ///     Default constructor for a space group entry (Should be used for serialization only)
        /// </summary>
        public SpaceGroupEntry()
        {
        }

        /// <summary>
        ///     Create from values
        /// </summary>
        /// <param name="index"></param>
        /// <param name="literal"></param>
        /// <param name="specifier"></param>
        public SpaceGroupEntry(int index, string literal, string specifier)
        {
            if (IndexConstraint.IsValid(index) == false) 
                throw new ArgumentOutOfRangeException(nameof(index));

            Index = index;
            Literal = literal ?? throw new ArgumentNullException(nameof(literal));
            Specifier = specifier ?? throw new ArgumentNullException(nameof(specifier));
        }

        /// <summary>
        ///     Create directly from a space group object
        /// </summary>
        /// <param name="group"></param>
        public SpaceGroupEntry(ISpaceGroup group)
            : this(group.InternationalIndex, group.MauguinNotation, group.VariationName)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));
        }

        /// <summary>
        ///     Compares the identifier
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(SpaceGroupEntry other)
        {
            var numberCompare = Index.CompareTo(other.Index);
            if (numberCompare != 0)
                return numberCompare;

            var nameCompare = string.Compare(Literal, other.Literal, StringComparison.Ordinal);
            return nameCompare == 0 
                ? string.Compare(Specifier, other.Specifier, StringComparison.Ordinal)
                : nameCompare;
        }

        /// <summary>
        ///     Creates the default space group entry (P1 group)
        /// </summary>
        /// <returns></returns>
        public static SpaceGroupEntry CreateDefault()
        {
            return new SpaceGroupEntry(1, "P1", "None");
        }

        /// <inheritdoc />
        public bool Equals(SpaceGroupEntry other)
        {
            return CompareTo(other) == 0;
        }
    }
}