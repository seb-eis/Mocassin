using System;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Energies;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Model;

namespace Mocassin.UI.Xml.Customization
{
    /// <summary>
    ///     Serializable helper object for serialization of <see cref="Mocassin.Model.Energies.PairEnergyEntry" /> data objects
    /// </summary>
    [XmlRoot]
    public class PairEnergyData : ProjectDataObject, IComparable<PairEnergyData>, IDuplicable<PairEnergyData>
    {
        private ModelObjectReference<Particle> centerParticle;
        private double energy;
        private ModelObjectReference<Particle> partnerParticle;

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReference{T}" /> that targets the center particle
        /// </summary>
        [XmlElement]
        public ModelObjectReference<Particle> CenterParticle
        {
            get => centerParticle;
            set => SetProperty(ref centerParticle, value);
        }

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReference{T}" /> that targets the center particle
        /// </summary>
        [XmlElement]
        public ModelObjectReference<Particle> PartnerParticle
        {
            get => partnerParticle;
            set => SetProperty(ref partnerParticle, value);
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

        /// <inheritdoc />
        public int CompareTo(PairEnergyData other)
        {
            if (ReferenceEquals(this, other))
                return 0;

            if (other is null)
                return 1;

            var centerParticleKeyComparison = string.Compare(CenterParticle.Key, other.CenterParticle.Key, StringComparison.Ordinal);
            if (centerParticleKeyComparison != 0)
                return centerParticleKeyComparison;

            var partnerParticleKeyComparison = string.Compare(PartnerParticle.Key, other.PartnerParticle.Key, StringComparison.Ordinal);
            return partnerParticleKeyComparison != 0
                ? partnerParticleKeyComparison
                : Energy.CompareTo(other.Energy);
        }

        /// <inheritdoc />
        public PairEnergyData Duplicate()
        {
            var copy = new PairEnergyData
            {
                Name = Name,
                energy = energy,
                centerParticle = centerParticle.Duplicate(),
                partnerParticle = partnerParticle.Duplicate()
            };
            return copy;
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate()
        {
            return Duplicate();
        }

        /// <summary>
        ///     Get the contained information as a <see cref="PairEnergyEntry" /> entry that is valid in the context of the passed
        ///     <see cref="IModelProject" /> and type of <see cref="IPairInteraction" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="pairInteraction"></param>
        /// <returns></returns>
        public PairEnergyEntry ToInternal(IModelProject modelProject, IPairInteraction pairInteraction)
        {
            return pairInteraction switch
            {
                IStablePairInteraction _ => ToSymmetricInternal(modelProject),
                IUnstablePairInteraction _ => ToAsymmetricInternal(modelProject),
                _ => throw new ArgumentException("Type of pair interaction is not supported", nameof(pairInteraction)),
            };
        }

        /// <summary>
        ///     Get the contained information as a <see cref="PairEnergyEntry" /> entry that is valid in the context of the passed
        ///     <see cref="IModelProject" /> and describes an interaction between a <see cref="SymmetricParticleInteractionPair" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        public PairEnergyEntry ToSymmetricInternal(IModelProject modelProject)
        {
            var particlePair = new SymmetricParticleInteractionPair
            {
                Particle0 = modelProject.DataTracker.FindObjectByKey<IParticle>(CenterParticle.Key),
                Particle1 = modelProject.DataTracker.FindObjectByKey<IParticle>(PartnerParticle.Key)
            };

            return new PairEnergyEntry(particlePair, Energy);
        }

        /// <summary>
        ///     Get the contained information as a <see cref="PairEnergyEntry" /> entry that is valid in the context of the passed
        ///     <see cref="IModelProject" /> and describes an interaction between a <see cref="AsymmetricParticleInteractionPair" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        public PairEnergyEntry ToAsymmetricInternal(IModelProject modelProject)
        {
            var particlePair = new AsymmetricParticleInteractionPair
            {
                Particle0 = modelProject.DataTracker.FindObjectByKey<IParticle>(CenterParticle.Key),
                Particle1 = modelProject.DataTracker.FindObjectByKey<IParticle>(PartnerParticle.Key)
            };

            return new PairEnergyEntry(particlePair, Energy);
        }

        /// <summary>
        ///     Creates a new serializable <see cref="PairEnergyData" /> by pulling the required data from the passed
        ///     <see cref="PairEnergyEntry" /> and <see cref="ProjectModelData" /> parent
        /// </summary>
        /// <param name="energyEntry"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static PairEnergyData Create(in PairEnergyEntry energyEntry, ProjectModelData parent)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            var (key0, key1) = (energyEntry.ParticleInteractionPair.Particle0.Key, energyEntry.ParticleInteractionPair.Particle1.Key);

            var centerParticle = parent.ParticleModelData.Particles.Single(x => x.Key == key0);
            var partnerParticle = parent.ParticleModelData.Particles.Single(x => x.Key == key1);

            var obj = new PairEnergyData
            {
                Name = $"[{energyEntry.ParticleInteractionPair.Particle0.GetIonString()}][{energyEntry.ParticleInteractionPair.Particle1.GetIonString()}]",
                CenterParticle = new ModelObjectReference<Particle>(centerParticle),
                PartnerParticle = new ModelObjectReference<Particle>(partnerParticle),
                Energy = energyEntry.Energy
            };

            return obj;
        }
    }
}