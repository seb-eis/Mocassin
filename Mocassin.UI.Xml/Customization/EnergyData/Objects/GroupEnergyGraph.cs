using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Mocassin.Model.Energies;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Model;

namespace Mocassin.UI.Xml.Customization
{
    /// <summary>
    ///     Serializable helper object for serialization of <see cref="Mocassin.Model.Energies.GroupEnergyEntry" /> data
    ///     objects
    /// </summary>
    [XmlRoot("GroupEnergyEntry")]
    public class GroupEnergyGraph : ProjectObjectGraph, IComparable<GroupEnergyGraph>
    {
        /// <summary>
        ///     Get or set the center particle key
        /// </summary>
        [XmlAttribute("Center")]
        public string CenterParticleKey { get; set; }

        /// <summary>
        ///     Get or set the energy value in [eV]
        /// </summary>
        [XmlAttribute("Energy")]
        public double Energy { get; set; }

        /// <summary>
        ///     Get or set the occupation state of the surrounding positions
        /// </summary>
        [XmlElement("Surroundings")]
        public OccupationStateGraph OccupationState { get; set; }


        /// <summary>
        ///     Get the contained information as a <see cref="GroupEnergyEntry" /> entry that is valid in the context of the passed
        ///     <see cref="IModelProject" /> and describes an occupation permutation an <see cref="IGroupInteraction" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        public GroupEnergyEntry ToInternal(IModelProject modelProject)
        {
            var centerParticle = modelProject.DataTracker.FindObjectByKey<IParticle>(CenterParticleKey);
            return new GroupEnergyEntry(centerParticle, OccupationState.ToInternal(modelProject), Energy);
        }

        /// <summary>
        ///     Creates a new serializable <see cref="GroupEnergyGraph" /> by pulling the required data from the passed
        ///     <see cref="GroupEnergyEntry" /> context and <see cref="ProjectModelGraph"/> parent
        /// </summary>
        /// <param name="energyEntry"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static GroupEnergyGraph Create(in GroupEnergyEntry energyEntry, ProjectModelGraph parent)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            var obj = new GroupEnergyGraph
            {
                CenterParticleKey = energyEntry.CenterParticle.Key,
                Energy = energyEntry.Energy,
                OccupationState = OccupationStateGraph.Create(energyEntry.GroupOccupation, parent.ParticleModelGraph.Particles)
            };

            return obj;
        }

        /// <inheritdoc />
        public int CompareTo(GroupEnergyGraph other)
        {
            if (ReferenceEquals(this, other))
                return 0;

            if (other is null)
                return 1;

            var centerParticleKeyComparison = string.Compare(CenterParticleKey, other.CenterParticleKey, StringComparison.Ordinal);
            return centerParticleKeyComparison != 0
                ? centerParticleKeyComparison
                : Energy.CompareTo(other.Energy);
        }
    }
}