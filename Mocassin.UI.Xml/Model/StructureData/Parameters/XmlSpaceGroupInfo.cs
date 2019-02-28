using System;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Structures;
using Mocassin.Symmetry.SpaceGroups;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.StructureModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Structures.ISpaceGroupInfo" /> model parameter creation
    /// </summary>
    [XmlRoot("SpaceGroup")]
    public class XmlSpaceGroupInfo : XmlModelParameter
    {
        /// <summary>
        ///     Get or set the number of the space group
        /// </summary>
        [XmlAttribute("Number")]
        public int Number { get; set; }

        /// <summary>
        ///     Get or set the literal name of the space group
        /// </summary>
        [XmlAttribute("Name")]
        public string Literal { get; set; }

        /// <summary>
        ///     Get or set the literal specifier of the space group
        /// </summary>
        [XmlAttribute("Specifier")]
        public string Specifier { get; set; }

        /// <summary>
        ///     Create default space group info object
        /// </summary>
        public XmlSpaceGroupInfo()
        {
            Number = 1;
        }

        /// <inheritdoc />
        protected override ModelParameter GetPreparedModelObject()
        {
            return new SpaceGroupInfo
            {
                GroupEntry = GetSpaceGroupEntry()
            };
        }

        /// <summary>
        ///     Get the space group entry object for the set properties
        /// </summary>
        /// <returns>Space group entry for the current object state or P1 default object if the group number is out of range</returns>
        private SpaceGroupEntry GetSpaceGroupEntry()
        {
            try
            {
                return new SpaceGroupEntry(Number, Literal ?? "", Specifier ?? "None");
            }
            catch (Exception e)
            {
                return SpaceGroupEntry.CreateDefault();
            }
        }
    }
}