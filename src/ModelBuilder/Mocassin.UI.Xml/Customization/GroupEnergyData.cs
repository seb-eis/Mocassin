using System;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Mocassin.Framework.Extensions;
using Mocassin.Model.Energies;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;
using Mocassin.UI.Data.Base;

namespace Mocassin.UI.Data.Customization
{
    /// <summary>
    ///     Serializable helper object for serialization of <see cref="Mocassin.Model.Energies.GroupEnergyEntry" /> data
    ///     objects
    /// </summary>
    [XmlRoot]
    public class GroupEnergyData : ProjectDataObject, IComparable<GroupEnergyData>, IDuplicable<GroupEnergyData>
    {
        private ModelObjectReference<Particle> centerParticle;
        private double energy;
        private OccupationStateData occupationState;

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReference{T}" /> of the <see cref="Particle" /> that describes the center
        ///     occupation
        /// </summary>
        [XmlElement]
        public ModelObjectReference<Particle> CenterParticle
        {
            get => centerParticle;
            set => SetProperty(ref centerParticle, value);
        }

        /// <summary>
        ///     Get or set the energy value in [eV]
        /// </summary>
        [XmlAttribute]
        public double Energy
        {
            get => energy;
            set => SetProperty(ref energy, value);
        }

        /// <summary>
        ///     Get or set the occupation state of the surrounding positions
        /// </summary>
        [XmlElement]
        public OccupationStateData OccupationState
        {
            get => occupationState;
            set => SetProperty(ref occupationState, value);
        }

        /// <inheritdoc />
        public int CompareTo(GroupEnergyData other)
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

        /// <inheritdoc />
        public GroupEnergyData Duplicate()
        {
            var copy = new GroupEnergyData
            {
                Name = Name,
                energy = energy,
                centerParticle = centerParticle.Duplicate(),
                occupationState = occupationState.Duplicate()
            };
            return copy;
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate() => Duplicate();

        /// <summary>
        ///     Get the contained information as a <see cref="GroupEnergyEntry" /> entry that is valid in the context of the passed
        ///     <see cref="IModelProject" /> and describes an occupation permutation an <see cref="IGroupInteraction" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        public GroupEnergyEntry ToInternal(IModelProject modelProject)
        {
            var center = modelProject.DataTracker.FindObject<IParticle>(CenterParticle.Key);
            return new GroupEnergyEntry(center, OccupationState.ToInternal(modelProject), Energy);
        }

        /// <summary>
        ///     Creates a new serializable <see cref="GroupEnergyData" /> by pulling the required data from the passed
        ///     <see cref="GroupEnergyEntry" /> context and <see cref="ProjectModelData" /> parent
        /// </summary>
        /// <param name="energyEntry"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static GroupEnergyData Create(in GroupEnergyEntry energyEntry, ProjectModelData parent)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            var centerKey = energyEntry.CenterParticle.Key;
            var centerParticle = parent.ParticleModelData.Particles.Single(x => x.Key == centerKey);

            var obj = new GroupEnergyData
            {
                Name = GetOccupationIonString(energyEntry.CenterParticle, energyEntry.GroupOccupation),
                CenterParticle = new ModelObjectReference<Particle>(centerParticle),
                Energy = energyEntry.Energy,
                OccupationState = OccupationStateData.Create(energyEntry.GroupOccupation, parent.ParticleModelData.Particles)
            };

            return obj;
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
    }
}