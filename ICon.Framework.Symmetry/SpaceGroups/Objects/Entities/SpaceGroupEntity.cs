using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Symmetry.CrystalSystems;

namespace Mocassin.Symmetry.SpaceGroups
{
    /// <summary>
    ///     Class for a space group entity that provides the crystal symmetry information and can be stored in a database
    /// </summary>
    [XmlRoot]
    public class SpaceGroupEntity : ISpaceGroup, IEquatable<SpaceGroupEntity>
    {
        /// <summary>
        ///     Get the <see cref="IReadOnlyList{T}" /> of <see cref="ISymmetryOperation" /> where orientation flipping operations
        ///     are at the end
        /// </summary>
        private IReadOnlyList<ISymmetryOperation> OrderedOperations { get; set; }

        /// <inheritdoc />
        [Column("Literal")]
        public string MauguinNotation { get; set; }

        /// <inheritdoc />
        [Column("Specifier")]
        public string VariationName { get; set; }

        /// <inheritdoc />
        [Column("GroupID")]
        public int InternationalIndex { get; set; }

        /// <inheritdoc />
        [Column("SpecifierID")]
        public CrystalSystemVariation CrystalVariation { get; set; }

        /// <inheritdoc />
        [Column("SystemId")]
        public CrystalSystemType CrystalType { get; set; }

        /// <inheritdoc />
        public IReadOnlyList<ISymmetryOperation> Operations =>
            OrderedOperations ??= BaseSymmetryOperations.OrderBy(x => x.FlipsOrientation).ToList().AsReadOnly();

        /// <inheritdoc />
        public IEnumerable<string> OperationLiterals => BaseSymmetryOperations?.Select(x => x.Literal);

        /// <summary>
        ///     The context ID for database storage and retrieval
        /// </summary>
        [XmlIgnore]
        [Key]
        [Column("ContextID")]
        public int Id { get; set; }

        /// <summary>
        ///     The list of matrix symmetry operations of the group
        /// </summary>
        [XmlArray("Operations")]
        [XmlArrayItem("Operation")]
        public List<SymmetryOperationEntity> BaseSymmetryOperations { get; set; }

        /// <summary>
        ///     Creates new empty space group
        /// </summary>
        public SpaceGroupEntity()
        {
            MauguinNotation = "";
            VariationName = "None";
            BaseSymmetryOperations = new List<SymmetryOperationEntity>();
        }

        /// <summary>
        ///     Checks if the symmetry operation array is empty
        /// </summary>
        /// <returns></returns>
        public bool IsReady()
        {
            return BaseSymmetryOperations.Count != 0;
        }

        /// <inheritdoc />
        public bool Equals(SpaceGroupEntity other)
        {
            return other != null
                   && MauguinNotation == other.MauguinNotation
                   && VariationName == other.VariationName
                   && InternationalIndex == other.InternationalIndex
                   && CrystalVariation == other.CrystalVariation
                   && CrystalType == other.CrystalType;
        }

        /// <inheritdoc />
        public SpaceGroupEntry GetGroupEntry()
        {
            return new SpaceGroupEntry(InternationalIndex, MauguinNotation, VariationName);
        }

        /// <summary>
        ///     Compares to other space group interface by index and specifier index
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ISpaceGroup other)
        {
            var indexCompare = InternationalIndex.CompareTo(other.InternationalIndex);
            return indexCompare == 0 ? CrystalVariation.CompareTo(other.CrystalVariation) : indexCompare;
        }
    }
}