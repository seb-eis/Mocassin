using System.Collections.Generic;
using System.Xml.Serialization;

namespace Mocassin.Symmetry.SpaceGroups
{
    /// <summary>
    ///     XML Serializable container for space groups
    /// </summary>
    [XmlRoot]
    public class SpaceGroupSet
    {
        /// <summary>
        ///     The space group list
        /// </summary>
        [XmlArray("Groups")]
        [XmlArrayItem("Group")]
        public List<SpaceGroupEntity> SpaceGroups { get; set; }

        /// <summary>
        ///     Create new space group set
        /// </summary>
        public SpaceGroupSet()
        {
            SpaceGroups = new List<SpaceGroupEntity>();
        }
    }
}