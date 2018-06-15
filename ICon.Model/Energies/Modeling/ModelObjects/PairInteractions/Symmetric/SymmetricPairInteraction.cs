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
        /// Read only interface access to the energy dictionary
        /// </summary>
        [IgnoreDataMember]
        IReadOnlyDictionary<SymParticlePair, double> ISymmetricPairInteraction.EnergyDictionary => EnergyDictionary;

        /// <summary>
        /// Default construction with null energy dictionary
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

                EnergyDictionary = new Dictionary<SymParticlePair, double>(interaction.EnergyDictionary.Count);
                foreach (var item in interaction.EnergyDictionary)
                {
                    EnergyDictionary.Add(item.Key, item.Value);
                }
            }
            return null;
        }
    }
}
