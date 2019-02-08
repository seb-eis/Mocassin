using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace Mocassin.Symmetry.SpaceGroups
{
    /// <summary>
    ///     Class for a space group entity that provides the crystal symmetry information and can be stored in a database
    /// </summary>
    [XmlRoot]
    public class SpaceGroupEntity : ISpaceGroup, IEquatable<SpaceGroupEntity>
    {
        /// <inheritdoc />
        [XmlAttribute("Literal")]
        [Column("Literal")]
        public string Literal { get; set; }

        /// <inheritdoc />
        [XmlAttribute("Specifier")]
        [Column("Specifier")]
        public string Specifier { get; set; }

        /// <inheritdoc />
        [XmlAttribute("GroupID")]
        [Column("GroupID")]
        public int Index { get; set; }

        /// <inheritdoc />
        [XmlAttribute("SpecifierID")]
        [Column("SpecifierID")]
        public int SpecifierIndex { get; set; }

        /// <inheritdoc />
        [XmlAttribute("SystemId")]
        [Column("SystemId")]
        public int CrystalSystemIndex { get; set; }

        /// <summary>
        ///     The context ID for database storage and retrieval
        /// </summary>
        [XmlIgnore]
        [Key]
        [Column("ContextID")]
        public int Id { get; set; }

        /// <summary>
        ///     The list of matrix symmetry operations for Wyckoff-1 positions
        /// </summary>
        [XmlArray("Operations")]
        [XmlArrayItem("Operation")]
        public List<SymmetryOperationEntity> BaseSymmetryOperations { get; set; }

        /// <summary>
        ///     Creates new empty space group
        /// </summary>
        public SpaceGroupEntity()
        {
            Literal = "";
            Specifier = "None";
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
                 && (Literal == other.Literal
                 && Specifier == other.Specifier
                 && Index == other.Index
                 && SpecifierIndex == other.SpecifierIndex
                 && CrystalSystemIndex == other.CrystalSystemIndex);
        }

        /// <inheritdoc />
        public SpaceGroupEntry GetGroupEntry()
        {
            return new SpaceGroupEntry(Index, Literal, Specifier);
        }

        /// <summary>
        ///     Compares to other space group interface by index and specifier index
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ISpaceGroup other)
        {
            var indexCompare = Index.CompareTo(other.Index);
            return indexCompare == 0 
                ? SpecifierIndex.CompareTo(other.SpecifierIndex)
                : indexCompare;
        }
    }
}