using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ICon.Framework.Collections;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Basic;
using ICon.Model.Structures;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Represents an asymmetric pair interaction used to describe pair interactions within unstable position environments
    /// </summary>
    [DataContract]
    public class AsymmetricPairInteraction : PairInteraction, IAsymmetricPairInteraction
    {
        /// <summary>
        /// The polar pair energy dictionary that assigns each possible particle pair an energy value
        /// </summary>
        [DataMember]
        public Dictionary<AsymParticlePair, double> EnergyDictionary { get; set; }

        /// <summary>
        /// Read only interface access to the energy dictionary
        /// </summary>
        [IgnoreDataMember]
        IReadOnlyDictionary<AsymParticlePair, double> IAsymmetricPairInteraction.EnergyDictionary => EnergyDictionary;

        /// <summary>
        /// Default construction with null energy dictionary
        /// </summary>
        public AsymmetricPairInteraction()
        {
        }

        /// <summary>
        /// Creates new polar pair interaction from pair candidate and energy dictionary
        /// </summary>
        /// <param name="candidate"></param>
        /// <param name="energyDictionary"></param>
        public AsymmetricPairInteraction(in PairCandidate candidate, Dictionary<AsymParticlePair, double> energyDictionary) :  base(candidate)
        {
            EnergyDictionary = energyDictionary ?? throw new ArgumentNullException(nameof(energyDictionary));
        }

        /// <summary>
        /// Get the model object name
        /// </summary>
        /// <returns></returns>
        public override string GetModelObjectName()
        {
            return "'Unpolar Pair Interaction'";
        }

        /// <summary>
        /// Populates the object by a passed interface and retruns the object as a generic moel object (Returns null if failed)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateObject(IModelObject obj)
        {
            if (CastWithDepricatedCheck<IAsymmetricPairInteraction>(obj) is var interaction)
            {
                base.PopulateObject(obj);

                EnergyDictionary = new Dictionary<AsymParticlePair, double>(interaction.EnergyDictionary.Count);
                foreach (var item in interaction.EnergyDictionary)
                {
                    EnergyDictionary.Add(item.Key, item.Value);
                }
            }
            return null;
        }
    }
}
