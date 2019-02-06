using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Energies;
using Mocassin.Model.ModelProject;
using Mocassin.UI.Xml.BaseData;

namespace Mocassin.UI.Xml.CustomizationData
{
    /// <summary>
    ///     Serializable base data object for <see cref="Mocassin.Model.Energies.IPairInteraction" /> model object parameter
    ///     customization
    /// </summary>
    [XmlRoot]
    public class XmlPairEnergyParameterSet : IComparable<XmlPairEnergyParameterSet>
    {
        /// <summary>
        ///     Get or set the center wyckoff key of the interaction
        /// </summary>
        [XmlAttribute("From")]
        public string CenterUnitCellPositionKey { get; set; }

        /// <summary>
        ///     Get or set the partner wyckoff key of the interaction
        /// </summary>
        [XmlAttribute("To")]
        public string PartnerUnitCellPositionKey { get; set; }

        /// <summary>
        ///     Get or set the distance in [Ang]
        /// </summary>
        [XmlAttribute("Distance")]
        public double Distance { get; set; }

        /// <summary>
        ///     Get or set the 3D vector of the center
        /// </summary>
        [XmlElement("Center")]
        public XmlVector3D StartVector { get; set; }

        /// <summary>
        ///     Get or set the 3D vector of the partner
        /// </summary>
        [XmlElement("Partner")]
        public XmlVector3D EndVector { get; set; }

        /// <summary>
        ///     Get or set the index of the pair interaction the data is valid for
        /// </summary>
        [XmlAttribute("AutoId")]
        public int PairInteractionIndex { get; set; }

        /// <summary>
        ///     Get or set the pair energy entry permutations for this pair interaction
        /// </summary>
        [XmlArray("Permutations")]
        [XmlArrayItem("Permutation")]
        public List<XmlPairEnergyEntry> PairEnergyEntries { get; set; }

        /// <summary>
        ///     Set all data on the passed <see cref="IPairEnergySetter" /> and push the values to the affiliated
        ///     <see cref="Mocassin.Model.ModelProject.IModelProject" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="energySetter"></param>
        public void PushToModel(IModelProject modelProject, IPairEnergySetter energySetter)
        {
            energySetter.SetEnergyValues(PairEnergyEntries.Select(x => x.ToInternal(modelProject, energySetter.PairInteraction)));
            energySetter.PushData();
        }

        /// <summary>
        ///     Creates a new serializable <see cref="XmlPairEnergyParameterSet" /> by pulling all data defined in the passed
        ///     <see cref="IPairEnergySetter" /> context
        /// </summary>
        /// <param name="energySetter"></param>
        /// <returns></returns>
        public static XmlPairEnergyParameterSet Create(IPairEnergySetter energySetter)
        {
            if (energySetter == null) throw new ArgumentNullException(nameof(energySetter));

            var obj = new XmlPairEnergyParameterSet
            {
                PairInteractionIndex = energySetter.PairInteraction.Index,
                CenterUnitCellPositionKey = energySetter.PairInteraction.Position0.Key,
                PartnerUnitCellPositionKey = energySetter.PairInteraction.Position1.Key,
                Distance = energySetter.PairInteraction.Distance,
                StartVector = XmlVector3D.Create(energySetter.PairInteraction.Position0.Vector),
                EndVector = XmlVector3D.Create(energySetter.PairInteraction.GetSecondPositionVector()),
                PairEnergyEntries = energySetter.EnergyEntries.Select(x => XmlPairEnergyEntry.Create(x)).ToList()
            };

            obj.PairEnergyEntries.Sort();

            return obj;
        }

        /// <inheritdoc />
        public int CompareTo(XmlPairEnergyParameterSet other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;

            var distanceComparison = Distance.CompareTo(other.Distance);
            if (distanceComparison != 0) return distanceComparison;

            var centerUnitCellPositionKeyComparison =
                string.Compare(CenterUnitCellPositionKey, other.CenterUnitCellPositionKey, StringComparison.Ordinal);
            if (centerUnitCellPositionKeyComparison != 0) return centerUnitCellPositionKeyComparison;

            var partnerUnitCellPositionKeyComparison =
                string.Compare(PartnerUnitCellPositionKey, other.PartnerUnitCellPositionKey, StringComparison.Ordinal);
            return partnerUnitCellPositionKeyComparison != 0
                ? distanceComparison
                : PairInteractionIndex.CompareTo(other.PairInteractionIndex);
        }
    }
}