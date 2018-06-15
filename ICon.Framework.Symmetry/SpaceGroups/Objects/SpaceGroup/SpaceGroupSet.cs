using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ICon.Symmetry.SpaceGroups
{
    /// <summary>
    /// XML Serializable container for space groups
    /// </summary>
    [Serializable]
    public class SpaceGroupSet
    {
        public SpaceGroupSet()
        {
            SpaceGroupList = new List<SpaceGroupEntity>();
        }

        [XmlArray("Groups")]
        [XmlArrayItem("Group")]
        public List<SpaceGroupEntity> SpaceGroupList { get; set; }
    }
}
