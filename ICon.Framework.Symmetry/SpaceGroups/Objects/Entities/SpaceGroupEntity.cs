using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICon.Symmetry.SpaceGroups
{
    /// <summary>
    /// Class for a space group entity that provides the crystal symmetry information and can be stored in a database
    /// </summary>
    [Serializable]
    public class SpaceGroupEntity : ISpaceGroup, IEquatable<SpaceGroupEntity>
    {
        /// <summary>
        /// The literal name of the space group
        /// </summary>
        [XmlAttribute("Literal")]
        [Column("Literal")]
        public string Literal { get; set; }

        /// <summary>
        /// The additional specifier in cases where specializations of a group exist
        /// </summary>
        [XmlAttribute("Specifier")]
        [Column("Specifier")]
        public string Specifier { get; set; }

        /// <summary>
        /// The space group number according to the international space group tables
        /// </summary>
        [XmlAttribute("GroupID")]
        [Column("GroupID")]
        public int Index { get; set; }

        /// <summary>
        /// Subnumber for cases where multiple specializations exist
        /// </summary>
        [XmlAttribute("SpecifierID")]
        [Column("SpecifierID")]
        public int SpecifierIndex { get; set; }

        /// <summary>
        /// The crystal system ID affiliated with the spacegroup (0 - 6)
        /// </summary>
        [XmlAttribute("SystemID")]
        [Column("SystemID")]
        public int CrystalSystemIndex { get; set; }

        /// <summary>
        /// The context ID for database storage and retrival
        /// </summary>
        [XmlIgnore]
        [Key]
        [Column("ContextID")]
        public int ContextID { get; set; }

        /// <summary>
        /// The list of matrix symmetry operations for Wyckoff-1 positions
        /// </summary>
        [XmlArray("Operations")]
        [XmlArrayItem("Operation")]
        public List<SymmetryOperationEntity> BaseSymmetryOperations { get; set; }

        /// <summary>
        /// Creates new empty space group
        /// </summary>
        public SpaceGroupEntity()
        {
            Literal = "";
            Specifier = "None";
            BaseSymmetryOperations = new List<SymmetryOperationEntity>();
        }

        /// <summary>
        /// Checks if the symmetry operation array is empty
        /// </summary>
        /// <returns></returns>
        public bool IsReady()
        {
            return BaseSymmetryOperations.Count != 0;
        }

        /// <summary>
        /// Cehcks if two space groups are equal without considering the list of matric operations
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(SpaceGroupEntity other)
        {
            return Literal == other.Literal 
                && Specifier == other.Specifier 
                && Index == other.Index 
                && SpecifierIndex == other.SpecifierIndex 
                && CrystalSystemIndex == other.CrystalSystemIndex;
        }

        /// <summary>
        /// Uses the group information to create a space group entryy object
        /// </summary>
        /// <returns></returns>
        public SpaceGroupEntry GetGroupEntry()
        {
            return new SpaceGroupEntry(Index, Literal, Specifier);
        }

        /// <summary>
        /// Compares to other space group interface by index and specifier index
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ISpaceGroup other)
        {
            int indexCompare = Index.CompareTo(other.Index);
            if (indexCompare == 0)
            {
                return SpecifierIndex.CompareTo(other.SpecifierIndex);
            }
            return indexCompare;
        }
    }
}
