using System;
using System.Xml.Serialization;
using Mocassin.Model.Energies;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;

namespace Mocassin.UI.Xml.Customization
{
    /// <summary>
    ///     Serializable helper object for serialization of <see cref="Mocassin.Model.Energies.GroupEnergyEntry" /> data
    ///     objects
    /// </summary>
    [XmlRoot("GroupEnergyEntry")]
    public class XmlGroupEnergyEntry : IComparable<XmlGroupEnergyEntry>
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
        public XmlOccupationState OccupationState { get; set; }

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
        ///     Creates a new serializable <see cref="XmlGroupEnergyEntry" /> by pulling the required data from the passed
        ///     <see cref="GroupEnergyEntry" /> context
        /// </summary>
        /// <param name="energyEntry"></param>
        /// <returns></returns>
        public static XmlGroupEnergyEntry Create(in GroupEnergyEntry energyEntry)
        {
            var obj = new XmlGroupEnergyEntry
            {
                CenterParticleKey = energyEntry.CenterParticle.Key,
                Energy = energyEntry.Energy,
                OccupationState = XmlOccupationState.Create(energyEntry.GroupOccupation)
            };

            return obj;
        }

        /// <inheritdoc />
        public int CompareTo(XmlGroupEnergyEntry other)
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