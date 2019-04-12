using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Energies;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Model;

namespace Mocassin.UI.Xml.Customization
{
    /// <summary>
    ///     Serializable base data object for <see cref="Mocassin.Model.Energies.IPairInteraction" /> model object parameter
    ///     customization
    /// </summary>
    [XmlRoot]
    public class PairEnergySetGraph : ProjectObjectGraph, IComparable<PairEnergySetGraph>
    {
        /// <summary>
        ///     Get or set the <see cref="ModelObjectReferenceGraph{T}" /> that targets the center wyckoff position
        /// </summary>
        [XmlElement("CenterPosition")]
        public ModelObjectReferenceGraph<UnitCellPosition> CenterPosition { get; set; }

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReferenceGraph{T}" /> that targets the partner wyckoff position
        /// </summary>
        [XmlElement("PartnerPosition")]
        public ModelObjectReferenceGraph<UnitCellPosition> PartnerPosition { get; set; }

        /// <summary>
        ///     Get or set the distance in [Ang]
        /// </summary>
        [XmlAttribute("Distance")]
        public double Distance { get; set; }

        /// <summary>
        ///     Get or set the 3D vector of the center
        /// </summary>
        [XmlElement("Center")]
        public VectorGraph3D StartVector { get; set; }

        /// <summary>
        ///     Get or set the 3D vector of the partner
        /// </summary>
        [XmlElement("Partner")]
        public VectorGraph3D EndVector { get; set; }

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
        public List<PairEnergyGraph> PairEnergyEntries { get; set; }

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
        ///     Creates a new serializable <see cref="PairEnergySetGraph" /> by pulling all data defined in the passed
        ///     <see cref="IPairEnergySetter" /> context and <see cref="ProjectModelGraph" /> parent
        /// </summary>
        /// <param name="energySetter"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static PairEnergySetGraph Create(IPairEnergySetter energySetter, ProjectModelGraph parent)
        {
            if (energySetter == null) throw new ArgumentNullException(nameof(energySetter));
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            var centerPosition =
                parent.StructureModelGraph.UnitCellPositions.SingleOrDefault(x => x.Key == energySetter.PairInteraction.Position0.Key)
                ?? throw new InvalidOperationException("Parent does not contain requested center wyckoff position");

            var partnerPosition =
                parent.StructureModelGraph.UnitCellPositions.SingleOrDefault(x => x.Key == energySetter.PairInteraction.Position1.Key)
                ?? throw new InvalidOperationException("Parent does not contain requested partner wyckoff position");

            var obj = new PairEnergySetGraph
            {
                Name = $"Pair.Collection.{energySetter.PairInteraction.Index}",
                PairInteractionIndex = energySetter.PairInteraction.Index,
                CenterPosition = new ModelObjectReferenceGraph<UnitCellPosition>(centerPosition),
                PartnerPosition = new ModelObjectReferenceGraph<UnitCellPosition>(partnerPosition),
                Distance = energySetter.PairInteraction.Distance,
                StartVector = VectorGraph3D.Create(energySetter.PairInteraction.Position0.Vector),
                EndVector = VectorGraph3D.Create(energySetter.PairInteraction.GetSecondPositionVector()),
                PairEnergyEntries = energySetter.EnergyEntries.Select(x => PairEnergyGraph.Create(x, parent)).ToList()
            };

            obj.PairEnergyEntries.Sort();

            return obj;
        }

        /// <inheritdoc />
        public int CompareTo(PairEnergySetGraph other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;

            var distanceComparison = Distance.CompareTo(other.Distance);
            if (distanceComparison != 0) return distanceComparison;

            var centerUnitCellPositionKeyComparison =
                string.Compare(CenterPosition.Key, other.CenterPosition.Key, StringComparison.Ordinal);

            if (centerUnitCellPositionKeyComparison != 0) return centerUnitCellPositionKeyComparison;

            var partnerUnitCellPositionKeyComparison =
                string.Compare(PartnerPosition.Key, other.PartnerPosition.Key, StringComparison.Ordinal);

            return partnerUnitCellPositionKeyComparison != 0
                ? distanceComparison
                : PairInteractionIndex.CompareTo(other.PairInteractionIndex);
        }
    }
}