﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.Serialization;

using ICon.Model.Basic;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Transitions;
using ICon.Model.Particles;
using ICon.Model.Structures;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Group information that describes a 4D encoded grouped interaction with the affiliated set of energies. Valid for all stable environment descriptions
    /// </summary>
    [DataContract(Name = "GroupInteraction")]
    public class GroupInteraction : ModelObject, IGroupInteraction
    {
        /// <summary>
        /// Get the group size (Is number of geometry vectors + 1)
        /// </summary>
        [IgnoreDataMember]
        public int GroupSize => GeometryVectors.Count + 1;

        /// <summary>
        /// The unit cell position that describes the start of the grouping
        /// </summary>
        [DataMember]
        [IndexResolvable]
        public IUnitCellPosition UnitCellPosition { get; set; }

        /// <summary>
        /// The list of 3D fractional vectors that describe the group geometry
        /// </summary>
        [DataMember]
        public List<DataVector3D> GeometryVectors { get; set; }

        /// <summary>
        /// The enrgy dictionaries for each center particle and surrounding occupation state (Auto managed by model)
        /// </summary>
        [DataMember]
        public Dictionary<IParticle, Dictionary<OccupationState, double>> EnergyDictionarySet { get; set; }

        /// <summary>
        /// Get the base geometry sequence excluding the start position
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Fractional3D> GetBaseGeometry()
        {
            return (GeometryVectors ?? new List<DataVector3D>()).Select(a => a.AsFractional());
        }

        /// <summary>
        /// Get a read only interface for the dictionary set
        /// </summary>
        /// <returns></returns>
        public IReadOnlyDictionary<IParticle, IReadOnlyDictionary<OccupationState, double>> GetEnergyDictionarySet()
        {
            var result = new Dictionary<IParticle, IReadOnlyDictionary<OccupationState, double>>();
            if (EnergyDictionarySet == null)
            {
                return result;
            }
            foreach (var item in EnergyDictionarySet)
            {
                result.Add(item.Key, item.Value);
            }
            return result;
        }

        /// <summary>
        /// Get the model object name
        /// </summary>
        /// <returns></returns>
        public override string GetModelObjectName()
        {
            return "'Group Interaction'";
        }

        /// <summary>
        /// Popultaes the model object from a model object interafec and retruns it (Retruns null if population failed)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateObject(IModelObject obj)
        {
            if (CastWithDepricatedCheck<IGroupInteraction>(obj) is var interaction)
            {
                UnitCellPosition = interaction.UnitCellPosition;
                GeometryVectors = interaction.GetBaseGeometry().Select(vector => new DataVector3D(vector)).ToList();
                EnergyDictionarySet = new Dictionary<IParticle, Dictionary<OccupationState, double>>();
                foreach (var item in interaction.GetEnergyDictionarySet())
                {
                    EnergyDictionarySet.Add(item.Key, new Dictionary<OccupationState, double>(item.Value.Count));
                    foreach (var inner in item.Value)
                    {
                        EnergyDictionarySet[item.Key].Add(inner.Key, inner.Value);
                    }
                }
                return this;
            }
            return null;
        }
    }
}
