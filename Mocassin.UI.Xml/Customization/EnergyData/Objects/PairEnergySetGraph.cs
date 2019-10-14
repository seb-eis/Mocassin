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
    public class PairEnergySetGraph : ProjectObjectGraph, IComparable<PairEnergySetGraph>, IDuplicable<PairEnergySetGraph>
    {
        private ModelObjectReferenceGraph<UnitCellPosition> centerPosition;
        private ModelObjectReferenceGraph<UnitCellPosition> partnerPosition;
        private double distance;
        private VectorGraph3D startVector;
        private VectorGraph3D endVector;
        private int pairInteractionIndex;
        private List<PairEnergyGraph> pairEnergyEntries;

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReferenceGraph{T}" /> that targets the center wyckoff position
        /// </summary>
        [XmlElement("CenterPosition")]
        public ModelObjectReferenceGraph<UnitCellPosition> CenterPosition
        {
            get => centerPosition;
            set => SetProperty(ref centerPosition, value);
        }

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReferenceGraph{T}" /> that targets the partner wyckoff position
        /// </summary>
        [XmlElement("PartnerPosition")]
        public ModelObjectReferenceGraph<UnitCellPosition> PartnerPosition
        {
            get => partnerPosition;
            set => SetProperty(ref partnerPosition, value);
        }

        /// <summary>
        ///     Get or set the distance in [Ang]
        /// </summary>
        [XmlAttribute("Distance")]
        public double Distance
        {
            get => distance;
            set => SetProperty(ref distance, value);
        }

        /// <summary>
        ///     Get or set the 3D vector of the center
        /// </summary>
        [XmlElement("Center")]
        public VectorGraph3D StartVector
        {
            get => startVector;
            set => SetProperty(ref startVector, value);
        }

        /// <summary>
        ///     Get or set the 3D vector of the partner
        /// </summary>
        [XmlElement("Partner")]
        public VectorGraph3D EndVector
        {
            get => endVector;
            set => SetProperty(ref endVector, value);
        }

        /// <summary>
        ///     Get or set the index of the pair interaction the data is valid for
        /// </summary>
        [XmlAttribute("AutoId")]
        public int PairInteractionIndex
        {
            get => pairInteractionIndex;
            set => SetProperty(ref pairInteractionIndex, value);
        }

        /// <summary>
        ///     Get or set the pair energy entry permutations for this pair interaction
        /// </summary>
        [XmlArray("Permutations")]
        [XmlArrayItem("Permutation")]
        public List<PairEnergyGraph> PairEnergyEntries
        {
            get => pairEnergyEntries;
            set => SetProperty(ref pairEnergyEntries, value);
        }

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
                parent.StructureModelGraph.UnitCellPositions.Single(x => x.Key == energySetter.PairInteraction.Position0.Key);

            var partnerPosition =
                parent.StructureModelGraph.UnitCellPositions.Single(x => x.Key == energySetter.PairInteraction.Position1.Key);

            var obj = new PairEnergySetGraph
            {
                Name = $"Pair.Energy.Set.{energySetter.PairInteraction.Index}",
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

        /// <inheritdoc />
        public PairEnergySetGraph Duplicate()
        {
            var copy = new PairEnergySetGraph
            {
                Name = Name,
                centerPosition = centerPosition.Duplicate(),
                partnerPosition = partnerPosition.Duplicate(),
                distance = distance,
                startVector = startVector.Duplicate(),
                endVector = endVector.Duplicate(),
                pairInteractionIndex = pairInteractionIndex,
                pairEnergyEntries = pairEnergyEntries.Select(x => x.Duplicate()).ToList()
            };
            return copy;
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate()
        {
            return Duplicate();
        }
    }
}