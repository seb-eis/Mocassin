﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Energies;

namespace Mocassin.UI.Xml.CustomizationData
{
    /// <summary>
    ///     Serializable base data object for <see cref="Mocassin.Model.Energies.IGroupInteraction" /> model object parameter
    ///     customization
    /// </summary>
    [XmlRoot]
    public class XmlGroupEnergyParameterSet : IComparable<XmlGroupEnergyParameterSet>
    {
        /// <summary>
        ///     Get or set the key of the group interaction
        /// </summary>
        [XmlAttribute("Group")]
        public string GroupInteractionKey { get; set; }

        /// <summary>
        ///     Get or set the key of the center unit cell position
        /// </summary>
        [XmlAttribute("Around")]
        public string CenterUnitCellPositionKey { get; set; }

        /// <summary>
        ///     Get or set the list of affiliated energy entries
        /// </summary>
        [XmlArray("Permutations")]
        [XmlArrayItem("Permutation")]
        public List<XmlGroupEnergyEntry> EnergyEntries { get; set; }


        /// <summary>
        ///     Creates a new serializable <see cref="XmlGroupEnergyParameterSet" /> by pulling all data defined in the passed
        ///     <see cref="IGroupEnergySetter" /> context
        /// </summary>
        /// <param name="energySetter"></param>
        /// <returns></returns>
        public static XmlGroupEnergyParameterSet Create(IGroupEnergySetter energySetter)
        {
            if (energySetter == null)
                throw new ArgumentNullException(nameof(energySetter));

            var obj = new XmlGroupEnergyParameterSet
            {
                GroupInteractionKey = energySetter.GroupInteraction.Key,
                CenterUnitCellPositionKey = energySetter.GroupInteraction.CenterUnitCellPosition.Key,
                EnergyEntries = energySetter.EnergyEntries.Select(x => XmlGroupEnergyEntry.Create(x)).ToList()
            };

            obj.EnergyEntries.Sort();
            return obj;
        }

        /// <inheritdoc />
        public int CompareTo(XmlGroupEnergyParameterSet other)
        {
            if (ReferenceEquals(this, other))
                return 0;

            if (other is null)
                return 1;

            var groupInteractionKeyComparison = string.Compare(GroupInteractionKey, other.GroupInteractionKey, StringComparison.Ordinal);
            return groupInteractionKeyComparison != 0
                ? groupInteractionKeyComparison
                : string.Compare(CenterUnitCellPositionKey, other.CenterUnitCellPositionKey, StringComparison.Ordinal);
        }
    }
}