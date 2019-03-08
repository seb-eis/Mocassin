using System;
using System.Xml.Serialization;
using Mocassin.Model.Energies;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.Customization
{
    /// <summary>
    ///     Serializable helper object for serialization of <see cref="Mocassin.Model.Energies.PairEnergyEntry" /> data objects
    /// </summary>
    [XmlRoot("PairEnergyEntry")]
    public class PairEnergyGraph : ProjectObjectGraph, IComparable<PairEnergyGraph>
    {
        /// <summary>
        ///     Get or set the center particle key
        /// </summary>
        [XmlAttribute("From")]
        public string CenterParticleKey { get; set; }

        /// <summary>
        ///     Get or set the partner particle key
        /// </summary>
        [XmlAttribute("To")]
        public string PartnerParticleKey { get; set; }

        /// <summary>
        ///     Get or set the energy value in [eV]
        /// </summary>
        [XmlAttribute("Energy")]
        public double Energy { get; set; }

        /// <summary>
        ///     Get the contained information as a <see cref="PairEnergyEntry" /> entry that is valid in the context of the passed
        ///     <see cref="IModelProject" /> and type of <see cref="IPairInteraction" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="pairInteraction"></param>
        /// <returns></returns>
        public PairEnergyEntry ToInternal(IModelProject modelProject, IPairInteraction pairInteraction)
        {
            switch (pairInteraction)
            {
                case ISymmetricPairInteraction _:
                    return ToSymmetricInternal(modelProject);

                case IAsymmetricPairInteraction _:
                    return ToAsymmetricInternal(modelProject);

                default:
                    throw new ArgumentException("Type of pair interaction is not supported", nameof(pairInteraction));
            }
        }

        /// <summary>
        ///     Get the contained information as a <see cref="PairEnergyEntry" /> entry that is valid in the context of the passed
        ///     <see cref="IModelProject" /> and describes an interaction between a <see cref="SymmetricParticlePair" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        public PairEnergyEntry ToSymmetricInternal(IModelProject modelProject)
        {
            var particlePair = new SymmetricParticlePair
            {
                Particle0 = modelProject.DataTracker.FindObjectByKey<IParticle>(CenterParticleKey),
                Particle1 = modelProject.DataTracker.FindObjectByKey<IParticle>(PartnerParticleKey)
            };

            return new PairEnergyEntry(particlePair, Energy);
        }

        /// <summary>
        ///     Get the contained information as a <see cref="PairEnergyEntry" /> entry that is valid in the context of the passed
        ///     <see cref="IModelProject" /> and describes an interaction between a <see cref="AsymmetricParticlePair" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        public PairEnergyEntry ToAsymmetricInternal(IModelProject modelProject)
        {
            var particlePair = new AsymmetricParticlePair
            {
                Particle0 = modelProject.DataTracker.FindObjectByKey<IParticle>(CenterParticleKey),
                Particle1 = modelProject.DataTracker.FindObjectByKey<IParticle>(PartnerParticleKey)
            };

            return new PairEnergyEntry(particlePair, Energy);
        }

        /// <summary>
        ///     Creates a new serializable <see cref="PairEnergyGraph" /> by pulling the required data from the passed
        ///     <see cref="PairEnergyEntry" />
        /// </summary>
        /// <param name="energyEntry"></param>
        /// <returns></returns>
        public static PairEnergyGraph Create(in PairEnergyEntry energyEntry)
        {
            var obj = new PairEnergyGraph
            {
                CenterParticleKey = energyEntry.ParticlePair.Particle0.Key,
                PartnerParticleKey = energyEntry.ParticlePair.Particle1.Key,
                Energy = energyEntry.Energy
            };

            return obj;
        }

        /// <inheritdoc />
        public int CompareTo(PairEnergyGraph other)
        {
            if (ReferenceEquals(this, other))
                return 0;

            if (other is null)
                return 1;

            var centerParticleKeyComparison = string.Compare(CenterParticleKey, other.CenterParticleKey, StringComparison.Ordinal);
            if (centerParticleKeyComparison != 0)
                return centerParticleKeyComparison;

            var partnerParticleKeyComparison = string.Compare(PartnerParticleKey, other.PartnerParticleKey, StringComparison.Ordinal);
            return partnerParticleKeyComparison != 0
                ? partnerParticleKeyComparison
                : Energy.CompareTo(other.Energy);
        }
    }
}