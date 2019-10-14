using System;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Mocassin.Framework.Extensions;
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
    public class GroupEnergyGraph : ProjectObjectGraph, IComparable<GroupEnergyGraph>, IDuplicable<GroupEnergyGraph>
    {
        private ModelObjectReferenceGraph<Particle> centerParticle;
        private double energy;
        private OccupationStateGraph occupationState;

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReferenceGraph{T}" /> of the <see cref="Particle" /> that describes the center
        ///     occupation
        /// </summary>
        [XmlElement("CenterParticle")]
        public ModelObjectReferenceGraph<Particle> CenterParticle
        {
            get => centerParticle;
            set => SetProperty(ref centerParticle, value);
        }

        /// <summary>
        ///     Get or set the energy value in [eV]
        /// </summary>
        [XmlAttribute("Energy")]
        public double Energy
        {
            get => energy;
            set => SetProperty(ref energy, value);
        }

        /// <summary>
        ///     Get or set the occupation state of the surrounding positions
        /// </summary>
        [XmlElement("Surroundings")]
        public OccupationStateGraph OccupationState
        {
            get => occupationState;
            set => SetProperty(ref occupationState, value);
        }

        /// <summary>
        ///     Get the contained information as a <see cref="GroupEnergyEntry" /> entry that is valid in the context of the passed
        ///     <see cref="IModelProject" /> and describes an occupation permutation an <see cref="IGroupInteraction" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        public GroupEnergyEntry ToInternal(IModelProject modelProject)
        {
            var center = modelProject.DataTracker.FindObjectByKey<IParticle>(CenterParticle.Key);
            return new GroupEnergyEntry(center, OccupationState.ToInternal(modelProject), Energy);
        }

        /// <summary>
        ///     Creates a new serializable <see cref="GroupEnergyGraph" /> by pulling the required data from the passed
        ///     <see cref="GroupEnergyEntry" /> context and <see cref="ProjectModelGraph" /> parent
        /// </summary>
        /// <param name="energyEntry"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static GroupEnergyGraph Create(in GroupEnergyEntry energyEntry, ProjectModelGraph parent)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            var centerKey = energyEntry.CenterParticle.Key;
            var centerParticle = parent.ParticleModelGraph.Particles.Single(x => x.Key == centerKey);

            var obj = new GroupEnergyGraph
            {
                Name = GetOccupationIonString(energyEntry.CenterParticle, energyEntry.GroupOccupation),
                CenterParticle = new ModelObjectReferenceGraph<Particle>(centerParticle),
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

            var centerParticleKeyComparison = string.Compare(CenterParticle?.Key, other.CenterParticle?.Key, StringComparison.Ordinal);
            return centerParticleKeyComparison != 0
                ? centerParticleKeyComparison
                : Energy.CompareTo(other.Energy);
        }

        /// <summary>
        ///     Creates a short <see cref="string" /> description for a <see cref="IOccupationState" /> interface and center
        ///     <see cref="IParticle" />
        /// </summary>
        /// <param name="center"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static string GetOccupationIonString(IParticle center, IOccupationState state)
        {
            var builder = new StringBuilder(100);
            foreach (var particle in center.AsSingleton().Concat(state)) builder.Append($"[{particle.GetIonString()}]");

            return builder.ToString();
        }

        /// <inheritdoc />
        public GroupEnergyGraph Duplicate()
        {
            var copy = new GroupEnergyGraph
            {
                Name = Name,
                energy = energy,
                centerParticle = centerParticle.Duplicate(),
                occupationState = occupationState.Duplicate()
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