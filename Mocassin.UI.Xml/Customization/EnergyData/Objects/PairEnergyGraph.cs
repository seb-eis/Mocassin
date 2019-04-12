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
    [XmlRoot("PairEnergyEntry")]
    public class PairEnergyGraph : ProjectObjectGraph, IComparable<PairEnergyGraph>
    {
        /// <summary>
        ///     Get or set the <see cref="ModelObjectReferenceGraph{T}" /> that targets the center particle
        /// </summary>
        [XmlAttribute("CenterParticle")]
        public ModelObjectReferenceGraph<Particle> CenterParticle { get; set; }

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReferenceGraph{T}" /> that targets the center particle
        /// </summary>
        [XmlAttribute("PartnerParticle")]
        public ModelObjectReferenceGraph<Particle> PartnerParticle { get; set; }

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
                Particle0 = modelProject.DataTracker.FindObjectByKey<IParticle>(CenterParticle.Key),
                Particle1 = modelProject.DataTracker.FindObjectByKey<IParticle>(PartnerParticle.Key)
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
                Particle0 = modelProject.DataTracker.FindObjectByKey<IParticle>(CenterParticle.Key),
                Particle1 = modelProject.DataTracker.FindObjectByKey<IParticle>(PartnerParticle.Key)
            };

            return new PairEnergyEntry(particlePair, Energy);
        }

        /// <summary>
        ///     Creates a new serializable <see cref="PairEnergyGraph" /> by pulling the required data from the passed
        ///     <see cref="PairEnergyEntry" /> and <see cref="ProjectModelGraph" /> parent
        /// </summary>
        /// <param name="energyEntry"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static PairEnergyGraph Create(in PairEnergyEntry energyEntry, ProjectModelGraph parent)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            var (key0, key1) = (energyEntry.ParticlePair.Particle0.Key, energyEntry.ParticlePair.Particle1.Key);

            var centerParticle = parent.ParticleModelGraph.Particles.SingleOrDefault(x => x.Key == key0)
                                 ?? throw new InvalidOperationException("Parent model does not contain the center particle");

            var partnerParticle = parent.ParticleModelGraph.Particles.SingleOrDefault(x => x.Key == key1)
                                  ?? throw new InvalidOperationException("Parent model does not contain the center particle");;

            var obj = new PairEnergyGraph
            {
                Name = $"Pair.[{centerParticle.Name}-{partnerParticle.Name}]",
                CenterParticle = new ModelObjectReferenceGraph<Particle>(centerParticle),
                PartnerParticle = new ModelObjectReferenceGraph<Particle>(partnerParticle),
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

            var centerParticleKeyComparison = string.Compare(CenterParticle.Key, other.CenterParticle.Key, StringComparison.Ordinal);
            if (centerParticleKeyComparison != 0)
                return centerParticleKeyComparison;

            var partnerParticleKeyComparison = string.Compare(PartnerParticle.Key, other.PartnerParticle.Key, StringComparison.Ordinal);
            return partnerParticleKeyComparison != 0
                ? partnerParticleKeyComparison
                : Energy.CompareTo(other.Energy);
        }
    }
}