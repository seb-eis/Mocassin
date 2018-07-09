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
        public Dictionary<AsymmetricParticlePair, double> EnergyDictionary { get; set; }

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
        public AsymmetricPairInteraction(in PairCandidate candidate, Dictionary<AsymmetricParticlePair, double> energyDictionary) :  base(candidate)
        {
            EnergyDictionary = energyDictionary ?? throw new ArgumentNullException(nameof(energyDictionary));
        }

        /// <summary>
        /// Get a read only access to the enrgy dictionary
        /// </summary>
        /// <returns></returns>
        public IReadOnlyDictionary<AsymmetricParticlePair, double> GetEnergyDictionary()
        {
            return EnergyDictionary ?? new Dictionary<AsymmetricParticlePair, double>();
        }


        /// <summary>
        /// Get the model object name
        /// </summary>
        /// <returns></returns>
        public override string GetModelObjectName()
        {
            return "'Asymmetric Pair Interaction'";
        }

        /// <summary>
        /// Populates the object by a passed interface and retruns the object as a generic moel object (Returns null if failed)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (CastWithDepricatedCheck<IAsymmetricPairInteraction>(obj) is var interaction)
            {
                base.PopulateFrom(obj);

                EnergyDictionary = new Dictionary<AsymmetricParticlePair, double>(interaction.GetEnergyDictionary().Count);
                foreach (var item in interaction.GetEnergyDictionary())
                {
                    EnergyDictionary.Add(item.Key, item.Value);
                }
            }
            return null;
        }

        /// <summary>
        /// Tries to set the passed energy entry in the enrgy dictionary. Returns false if the value cannot be set
        /// </summary>
        /// <param name="energyEntry"></param>
        /// <returns></returns>
        public override bool TrySetEnergyEntry(in PairEnergyEntry energyEntry)
        {
            if (energyEntry.ParticlePair is AsymmetricParticlePair particlePair)
            {
                if (EnergyDictionary.ContainsKey(particlePair))
                {
                    EnergyDictionary[particlePair] = energyEntry.Energy;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get an enumerable sequence that contains all energy entries of the pair interaction
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<PairEnergyEntry> GetEnergyEntries()
        {
            foreach (var item in EnergyDictionary)
            {
                yield return new PairEnergyEntry(item.Key, item.Value);
            }
        }
    }
}
