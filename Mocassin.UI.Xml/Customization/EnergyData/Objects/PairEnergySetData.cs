using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.ValueTypes;
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
    public class PairEnergySetData : ExtensibleProjectDataObject, IComparable<PairEnergySetData>, IDuplicable<PairEnergySetData>
    {
        private ModelObjectReference<CellReferencePosition> centerPosition;
        private int chiralPartnerModelIndex = -1;
        private double distance;
        private VectorData3D endVector;
        private int modelIndex;
        private ObservableCollection<PairEnergyData> pairEnergyEntries;
        private ModelObjectReference<CellReferencePosition> partnerPosition;
        private VectorData3D startVector;

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReference{T}" /> that targets the center wyckoff position
        /// </summary>
        [XmlElement]
        public ModelObjectReference<CellReferencePosition> CenterPosition
        {
            get => centerPosition;
            set => SetProperty(ref centerPosition, value);
        }

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReference{T}" /> that targets the partner wyckoff position
        /// </summary>
        [XmlElement]
        public ModelObjectReference<CellReferencePosition> PartnerPosition
        {
            get => partnerPosition;
            set => SetProperty(ref partnerPosition, value);
        }

        /// <summary>
        ///     Get or set the distance in [Ang]
        /// </summary>
        [XmlAttribute]
        public double Distance
        {
            get => distance;
            set => SetProperty(ref distance, value);
        }

        /// <summary>
        ///     Get or set the 3D vector of the center
        /// </summary>
        [XmlElement]
        public VectorData3D StartVector
        {
            get => startVector;
            set => SetProperty(ref startVector, value);
        }

        /// <summary>
        ///     Get or set the 3D vector of the partner
        /// </summary>
        [XmlElement]
        public VectorData3D EndVector
        {
            get => endVector;
            set => SetProperty(ref endVector, value);
        }

        /// <summary>
        ///     Get or set the index of the pair interaction the data is valid for
        /// </summary>
        [XmlAttribute]
        public int ModelIndex
        {
            get => modelIndex;
            set => SetProperty(ref modelIndex, value);
        }

        /// <summary>
        ///     Get or set the pair energy entry permutations for this pair interaction
        /// </summary>
        [XmlArray]
        public ObservableCollection<PairEnergyData> PairEnergyEntries
        {
            get => pairEnergyEntries;
            set => SetProperty(ref pairEnergyEntries, value);
        }

        /// <summary>
        ///     Get or set the interaction index of a chiral partner. Negative values indicate that none exists
        /// </summary>
        [XmlAttribute]
        public int ChiralPartnerModelIndex
        {
            get => chiralPartnerModelIndex;
            set => SetProperty(ref chiralPartnerModelIndex, value);
        }

        /// <inheritdoc />
        public int CompareTo(PairEnergySetData other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;

            var distanceComparison = Distance.CompareTo(other.Distance);
            if (distanceComparison != 0) return distanceComparison;

            var centerPositionKeyComparison =
                string.Compare(CenterPosition.Key, other.CenterPosition.Key, StringComparison.Ordinal);

            if (centerPositionKeyComparison != 0) return centerPositionKeyComparison;

            var partnerPositionKeyComparison =
                string.Compare(PartnerPosition.Key, other.PartnerPosition.Key, StringComparison.Ordinal);

            return partnerPositionKeyComparison != 0
                ? distanceComparison
                : ModelIndex.CompareTo(other.ModelIndex);
        }

        /// <inheritdoc />
        public PairEnergySetData Duplicate()
        {
            var copy = new PairEnergySetData
            {
                Name = Name,
                centerPosition = centerPosition.Duplicate(),
                partnerPosition = partnerPosition.Duplicate(),
                distance = distance,
                startVector = startVector.Duplicate(),
                endVector = endVector.Duplicate(),
                modelIndex = modelIndex,
                chiralPartnerModelIndex = chiralPartnerModelIndex,
                pairEnergyEntries = pairEnergyEntries.Select(x => x.Duplicate()).ToObservableCollection()
            };
            return copy;
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate()
        {
            return Duplicate();
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
        ///     Creates a new serializable <see cref="PairEnergySetData" /> by pulling all data defined in the passed
        ///     <see cref="IPairEnergySetter" /> context and <see cref="ProjectModelData" /> parent
        /// </summary>
        /// <param name="energySetter"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static PairEnergySetData Create(IPairEnergySetter energySetter, ProjectModelData parent)
        {
            if (energySetter == null) throw new ArgumentNullException(nameof(energySetter));
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            var centerPosition =
                parent.StructureModelData.CellReferencePositions.Single(x => x.Key == energySetter.PairInteraction.Position0.Key);

            var partnerPosition =
                parent.StructureModelData.CellReferencePositions.Single(x => x.Key == energySetter.PairInteraction.Position1.Key);

            var obj = new PairEnergySetData
            {
                Name = $"Pair.Energy.Set.{energySetter.PairInteraction.Index}",
                ModelIndex = energySetter.PairInteraction.Index,
                ChiralPartnerModelIndex = energySetter.PairInteraction.ChiralPartner?.Index ?? -1,
                CenterPosition = new ModelObjectReference<CellReferencePosition>(centerPosition),
                PartnerPosition = new ModelObjectReference<CellReferencePosition>(partnerPosition),
                Distance = energySetter.PairInteraction.Distance,
                StartVector = VectorData3D.Create(energySetter.PairInteraction.Position0.Vector),
                EndVector = VectorData3D.Create(energySetter.PairInteraction.SecondPositionVector),
                PairEnergyEntries = energySetter.EnergyEntries.Select(x => PairEnergyData.Create(x, parent)).ToObservableCollection()
            };

            return obj;
        }

        /// <summary>
        ///     Gets the interaction geometry as a <see cref="Fractional3D" /> path (Yields always two positions)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Fractional3D> AsVectorPath()
        {
            yield return new Fractional3D(StartVector.A, StartVector.B, StartVector.C);
            yield return new Fractional3D(EndVector.A, EndVector.B, EndVector.C);
        }
    }
}