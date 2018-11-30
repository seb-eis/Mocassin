using System;
using System.Xml.Serialization;
using Mocassin.Model.Energies;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     A pair interaction setting that assigns a single energy value to a particle pair and carries a
    ///     state description for identification by the user
    /// </summary>
    [XmlRoot]
    public class MmlPairEnergy : MmlEnergy
    {
        /// <summary>
        ///     Get or set the center particle index of the interaction
        /// </summary>
        [XmlAttribute("CenterId")]
        public int CenterParticleId { get; set; }

        /// <summary>
        ///     Get or set the partner particle index of the interaction
        /// </summary>
        [XmlAttribute("PartnerId")]
        public int PartnerParticleId { get; set; }

        /// <summary>
        ///     Get or set the boolean flag that indicates if the interaction is symmetric
        /// </summary>
        [XmlAttribute("IsSymmetric")]
        public bool IsSymmetric { get; set; }

        /// <summary>
        ///     Creates a new Mml pair energy object from a energy context pair energy entry struct
        /// </summary>
        /// <param name="pairEnergyEntry"></param>
        /// <returns></returns>
        public static MmlPairEnergy FromContextObject(in PairEnergyEntry pairEnergyEntry)
        {
            throw new NotImplementedException();
        }
    }
}