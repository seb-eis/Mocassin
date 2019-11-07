using System;
using System.Collections.Generic;
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
    public class GroupEnergySetGraph : ExtensibleProjectObjectGraph, IComparable<GroupEnergySetGraph>, IDuplicable<GroupEnergySetGraph>
    {
        private ModelObjectReferenceGraph<GroupInteraction> groupInteraction;
        private ModelObjectReferenceGraph<UnitCellPosition> centerPosition;
        private List<VectorGraph3D> baseGeometry;
        private List<GroupEnergyGraph> energyEntries;
        private int groupInteractionIndex;

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReferenceGraph{T}"/> of the group interaction that the graph is based upon
        /// </summary>
        [XmlElement("GroupInteraction")]
        public ModelObjectReferenceGraph<GroupInteraction> GroupInteraction
        {
            get => groupInteraction;
            set => SetProperty(ref groupInteraction, value);
        }

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReferenceGraph{T}"/> of the center position that the graph is based upon
        /// </summary>
        [XmlElement("CenterPosition")]
        public ModelObjectReferenceGraph<UnitCellPosition> CenterPosition
        {
            get => centerPosition;
            set => SetProperty(ref centerPosition, value);
        }

        /// <summary>
        ///     Get or set the base geometry list of the surroundings
        /// </summary>
        [XmlArray("BaseGeometry")]
        [XmlArrayItem("Position")]
        public List<VectorGraph3D> BaseGeometry
        {
            get => baseGeometry;
            set => SetProperty(ref baseGeometry, value);
        }

        /// <summary>
        ///     Get or set the list of affiliated energy entries
        /// </summary>
        [XmlArray("Permutations")]
        [XmlArrayItem("Permutation")]
        public List<GroupEnergyGraph> EnergyEntries
        {
            get => energyEntries;
            set => SetProperty(ref energyEntries, value);
        }

        /// <summary>
        /// Get or set the internal index of the interaction
        /// </summary>
        [XmlAttribute("AutoId")]
        public int GroupInteractionIndex
        {
            get => groupInteractionIndex;
            set => SetProperty(ref groupInteractionIndex, value);
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
        ///     Creates a new serializable <see cref="GroupEnergySetGraph" /> by pulling all data defined in the passed
        ///     <see cref="IGroupEnergySetter" /> context and <see cref="ProjectModelGraph"/> parent
        /// </summary>
        /// <param name="energySetter"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static GroupEnergySetGraph Create(IGroupEnergySetter energySetter, ProjectModelGraph parent)
        {
            if (energySetter == null) throw new ArgumentNullException(nameof(energySetter));
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            var groupInteraction = 
                parent.EnergyModelGraph.GroupInteractions.Single(x => x.Key == energySetter.GroupInteraction.Key);

            var centerPosition =
                parent.StructureModelGraph.UnitCellPositions.Single(x => x.Key == energySetter.GroupInteraction.CenterUnitCellPosition.Key);

            var obj = new GroupEnergySetGraph
            {
                Name = $"Group.Energy.Set.{energySetter.GroupInteraction.Index}",
                GroupInteractionIndex = energySetter.GroupInteraction.Index,
                BaseGeometry = energySetter.GroupInteraction.GetBaseGeometry().Select(x => VectorGraph3D.Create(x)).ToList(),
                GroupInteraction = new ModelObjectReferenceGraph<GroupInteraction>(groupInteraction),
                CenterPosition = new ModelObjectReferenceGraph<UnitCellPosition>(centerPosition),
                EnergyEntries = energySetter.EnergyEntries.Select(x => GroupEnergyGraph.Create(x, parent)).ToList()
            };

            obj.EnergyEntries.Sort();
            return obj;
        }

        /// <inheritdoc />
        public int CompareTo(GroupEnergySetGraph other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;

            var groupInteractionKeyComparison = string.Compare(GroupInteraction?.Key, other.GroupInteraction?.Key, StringComparison.Ordinal);
            return groupInteractionKeyComparison != 0
                ? groupInteractionKeyComparison
                : string.Compare(CenterPosition?.Key, other.CenterPosition?.Key, StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public GroupEnergySetGraph Duplicate()
        {
            var copy = new GroupEnergySetGraph
            {
                Name = Name,
                groupInteraction = groupInteraction.Duplicate(),
                centerPosition = centerPosition.Duplicate(),
                groupInteractionIndex = groupInteractionIndex,
                baseGeometry = baseGeometry.Select(x => x.Duplicate()).ToList(),
                energyEntries = energyEntries.Select(x => x.Duplicate()).ToList()
            };

            return copy;
        }

        /// <summary>
        ///     Gets the surrounding geometry as a <see cref="IEnumerable{T}"/> of <see cref="Fractional3D"/> (Center position not included on default)
        /// </summary>
        /// <param name="includeCenter"></param>
        /// <returns></returns>
        public IEnumerable<Fractional3D> AsVectorPath(bool includeCenter = false)
        {
            var baseEnum = BaseGeometry.Select(x => new Fractional3D(x.A, x.B, x.C));
            return includeCenter ? ((IUnitCellPosition) CenterPosition.TargetGraph.GetInputObject()).Vector.AsSingleton().Concat(baseEnum) : baseEnum;
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate()
        {
            return Duplicate();
        }
    }
}