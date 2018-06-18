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
    /// Represents a symmetric pair interaction used to describe pair interactions within stable position environments
    /// </summary>
    [DataContract]
    public class SymmetricPairInteraction : PairInteraction, ISymmetricPairInteraction
    {
        /// <summary>
        /// The unpolar pair energy dictionary that assigns each possible particle pair an energy value
        /// </summary>
        [DataMember]
        public Dictionary<SymParticlePair, double> EnergyDictionary { get; set; }

        /// <summary>
        /// Default construction
        /// </summary>
        public SymmetricPairInteraction()
        {
        }

        /// <summary>
        /// Creates new unpolar pair interaction from pair candidate and unpolar energy dictionary
        /// </summary>
        /// <param name="candidate"></param>
        /// <param name="energyDictionary"></param>
        public SymmetricPairInteraction(in PairCandidate candidate, Dictionary<SymParticlePair, double> energyDictionary) : base(candidate)
        {
            EnergyDictionary = energyDictionary ?? throw new ArgumentNullException(nameof(energyDictionary));
        }

        /// <summary>
        /// Get a read only access t the energy dictionary
        /// </summary>
        /// <returns></returns>
        public IReadOnlyDictionary<SymParticlePair, double> GetEnergyDictionary()
        {
            return EnergyDictionary ?? new Dictionary<SymParticlePair, double>();
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
            if (CastWithDepricatedCheck<ISymmetricPairInteraction>(obj) is var interaction)
            {
                base.PopulateObject(obj);

                EnergyDictionary = new Dictionary<SymParticlePair, double>(interaction.GetEnergyDictionary().Count);
                foreach (var item in interaction.GetEnergyDictionary())
                {
                    EnergyDictionary.Add(item.Key, item.Value);
                }
            }
            return null;
        }
    }
}
