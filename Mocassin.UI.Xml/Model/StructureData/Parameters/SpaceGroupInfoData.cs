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
    [XmlRoot]
    public class SpaceGroupInfoData : ModelParameterObject
    {
        private string literal;
        private int number;
        private string specifier;

        /// <summary>
        ///     Get or set the number of the space group
        /// </summary>
        [XmlAttribute]
        public int Number
        {
            get => number;
            set => SetProperty(ref number, value);
        }

        /// <summary>
        ///     Get or set the literal name of the space group
        /// </summary>
        [XmlAttribute]
        public string Literal
        {
            get => literal;
            set => SetProperty(ref literal, value);
        }

        /// <summary>
        ///     Get or set the literal specifier of the space group
        /// </summary>
        [XmlAttribute]
        public string Specifier
        {
            get => specifier;
            set => SetProperty(ref specifier, value);
        }

        /// <summary>
        ///     Create default space group info object
        /// </summary>
        public SpaceGroupInfoData()
        {
            Number = 1;
            Specifier = "None";
            Literal = "P1";
        }

        /// <inheritdoc />
        protected override ModelParameter GetModelObjectInternal()
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
        public SpaceGroupEntry GetSpaceGroupEntry()
        {
            try
            {
                return new SpaceGroupEntry(Number, Literal ?? "", Specifier ?? "None");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return SpaceGroupEntry.CreateDefault();
            }
        }

        /// <summary>
        ///     Populates the <see cref="SpaceGroupInfoData" /> from the passed <see cref="SpaceGroupEntry" />
        /// </summary>
        /// <param name="entry"></param>
        public void PopulateFrom(SpaceGroupEntry entry)
        {
            entry ??= SpaceGroupEntry.CreateDefault();
            Number = entry.Index;
            Literal = entry.Literal;
            Specifier = entry.Specifier;
        }
    }
}