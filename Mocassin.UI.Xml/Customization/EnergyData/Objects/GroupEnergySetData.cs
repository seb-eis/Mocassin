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
    ///     Serializable base data object for <see cref="Mocassin.Model.Energies.IGroupInteraction" /> model object parameter
    ///     customization
    /// </summary>
    [XmlRoot]
    public class GroupEnergySetData : ExtensibleProjectDataObject, IComparable<GroupEnergySetData>, IDuplicable<GroupEnergySetData>
    {
        private ObservableCollection<VectorData3D> baseGeometry;
        private ModelObjectReference<CellReferencePosition> centerPosition;
        private ObservableCollection<GroupEnergyData> energyEntries;
        private ModelObjectReference<GroupInteraction> groupInteraction;
        private int modelIndex;

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReference{T}" /> of the group interaction that the graph is based upon
        /// </summary>
        [XmlElement]
        public ModelObjectReference<GroupInteraction> GroupInteraction
        {
            get => groupInteraction;
            set => SetProperty(ref groupInteraction, value);
        }

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReference{T}" /> of the center position that the graph is based upon
        /// </summary>
        [XmlElement]
        public ModelObjectReference<CellReferencePosition> CenterPosition
        {
            get => centerPosition;
            set => SetProperty(ref centerPosition, value);
        }

        /// <summary>
        ///     Get or set the base geometry list of the surroundings
        /// </summary>
        [XmlArray]
        public ObservableCollection<VectorData3D> BaseGeometry
        {
            get => baseGeometry;
            set => SetProperty(ref baseGeometry, value);
        }

        /// <summary>
        ///     Get or set the list of affiliated energy entries
        /// </summary>
        [XmlArray]
        public ObservableCollection<GroupEnergyData> EnergyEntries
        {
            get => energyEntries;
            set => SetProperty(ref energyEntries, value);
        }

        /// <summary>
        ///     Get or set the internal index of the interaction
        /// </summary>
        [XmlAttribute]
        public int ModelIndex
        {
            get => modelIndex;
            set => SetProperty(ref modelIndex, value);
        }

        /// <inheritdoc />
        public int CompareTo(GroupEnergySetData other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;

            var groupInteractionKeyComparison = string.Compare(GroupInteraction?.Key, other.GroupInteraction?.Key, StringComparison.Ordinal);
            return groupInteractionKeyComparison != 0
                ? groupInteractionKeyComparison
                : string.Compare(CenterPosition?.Key, other.CenterPosition?.Key, StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public GroupEnergySetData Duplicate()
        {
            var copy = new GroupEnergySetData
            {
                Name = Name,
                groupInteraction = groupInteraction.Duplicate(),
                centerPosition = centerPosition.Duplicate(),
                modelIndex = modelIndex,
                baseGeometry = baseGeometry.Select(x => x.Duplicate()).ToObservableCollection(),
                energyEntries = energyEntries.Select(x => x.Duplicate()).ToObservableCollection()
            };

            return copy;
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate()
        {
            return Duplicate();
        }

        /// <summary>
        ///     Set all data on the passed <see cref="IGroupEnergySetter" /> and push the values to the affiliated
        ///     <see cref="Mocassin.Model.ModelProject.IModelProject" />
        /// </summary>
        /// <param name="energySetter"></param>
        /// <param name="modelProject"></param>
        public void PushToModel(IModelProject modelProject, IGroupEnergySetter energySetter)
        {
            energySetter.SetEnergyEntries(EnergyEntries.Select(x => x.ToInternal(modelProject)));
            energySetter.PushData();
        }

        /// <summary>
        ///     Creates a new serializable <see cref="GroupEnergySetData" /> by pulling all data defined in the passed
        ///     <see cref="IGroupEnergySetter" /> context and <see cref="ProjectModelData" /> parent
        /// </summary>
        /// <param name="energySetter"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static GroupEnergySetData Create(IGroupEnergySetter energySetter, ProjectModelData parent)
        {
            if (energySetter == null) throw new ArgumentNullException(nameof(energySetter));
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            var groupInteraction =
                parent.EnergyModelData.GroupInteractions.Single(x => x.Key == energySetter.GroupInteraction.Key);

            var centerPosition =
                parent.StructureModelData.CellReferencePositions.Single(x => x.Key == energySetter.GroupInteraction.CenterCellReferencePosition.Key);

            var obj = new GroupEnergySetData
            {
                Name = $"Group.Energy.Set.{energySetter.GroupInteraction.Index}",
                ModelIndex = energySetter.GroupInteraction.Index,
                BaseGeometry = energySetter.GroupInteraction.GetSurroundingGeometry().Select(x => VectorData3D.Create(x)).ToObservableCollection(),
                GroupInteraction = new ModelObjectReference<GroupInteraction>(groupInteraction),
                CenterPosition = new ModelObjectReference<CellReferencePosition>(centerPosition),
                EnergyEntries = energySetter.EnergyEntries.Select(x => GroupEnergyData.Create(x, parent)).ToObservableCollection()
            };
            return obj;
        }

        /// <summary>
        ///     Gets the surrounding geometry as a <see cref="IEnumerable{T}" /> of <see cref="Fractional3D" /> (Center position
        ///     not included on default)
        /// </summary>
        /// <param name="includeCenter"></param>
        /// <returns></returns>
        public IEnumerable<Fractional3D> AsVectorPath(bool includeCenter = false)
        {
            var baseEnum = BaseGeometry.Select(x => new Fractional3D(x.A, x.B, x.C));
            return includeCenter ? ((ICellReferencePosition) CenterPosition.Target.GetInputObject()).Vector.AsSingleton().Concat(baseEnum) : baseEnum;
        }
    }
}