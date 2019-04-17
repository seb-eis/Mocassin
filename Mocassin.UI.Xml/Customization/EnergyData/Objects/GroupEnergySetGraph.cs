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
    ///     Serializable base data object for <see cref="Mocassin.Model.Energies.IGroupInteraction" /> model object parameter
    ///     customization
    /// </summary>
    [XmlRoot]
    public class GroupEnergySetGraph : ProjectObjectGraph, IComparable<GroupEnergySetGraph>
    {
        /// <summary>
        ///     Get or set the <see cref="ModelObjectReferenceGraph{T}"/> of the group interaction that the graph is based upon
        /// </summary>
        [XmlElement("GroupInteraction")]
        public ModelObjectReferenceGraph<GroupInteraction> GroupInteraction { get; set; }

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReferenceGraph{T}"/> of the center position that the graph is based upon
        /// </summary>
        [XmlElement("CenterPosition")]
        public ModelObjectReferenceGraph<UnitCellPosition> CenterPosition { get; set; }

        /// <summary>
        ///     Get or set the base geometry list of the surroundings
        /// </summary>
        [XmlArray("BaseGeometry")]
        [XmlArrayItem("Position")]
        public List<VectorGraph3D> BaseGeometry { get; set; }

        /// <summary>
        ///     Get or set the list of affiliated energy entries
        /// </summary>
        [XmlArray("Permutations")]
        [XmlArrayItem("Permutation")]
        public List<GroupEnergyGraph> EnergyEntries { get; set; }

        /// <summary>
        /// Get or set the internal index of the interaction
        /// </summary>
        [XmlAttribute("AutoId")]
        public int GroupInteractionIndex { get; set; }

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
    }
}