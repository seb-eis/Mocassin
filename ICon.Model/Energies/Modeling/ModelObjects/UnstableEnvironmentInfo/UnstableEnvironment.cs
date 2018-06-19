using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using ICon.Model.Basic;
using ICon.Model.Structures;

namespace ICon.Model.Energies
{
    /// <summary>
    /// An unstable environment info that describes the interaction parameters for a specific unstable unit cell position
    /// </summary>
    [DataContract(Name = "UnstableEnvironmentInfo")]
    public class UnstableEnvironment : ModelObject, IUnstableEnvironment
    {
        /// <summary>
        /// The interaction range of the environment
        /// </summary>
        [DataMember]
        public double MaxInteractionRange { get; set; }

        /// <summary>
        /// The unit cell position the environment info belongs to (Can be null)
        /// </summary>
        [DataMember]
        [IndexResolvable]
        public IUnitCellPosition UnitCellPosition { get; set; }

        /// <summary>
        /// The set of unique ignored unit cell positions during environment sampling (Can be null)
        /// </summary>
        [DataMember]
        [IndexResolvable]
        public List<IUnitCellPosition> IgnoredPositions { get; set; }

        /// <summary>
        /// The list of generated pair interactions (Can be null, automatically managed and linked property, not part of object population)
        /// </summary>
        [DataMember]
        [IndexResolvable]
        public List<IAsymmetricPairInteraction> PairInteractions { get; set; }

        /// <summary>
        /// The list of generated group interactions (Automatically linked by the model)
        /// </summary>
        [DataMember]
        [IndexResolvable]
        public List<IGroupInteraction> GroupInteractions { get; set; }

        /// <summary>
        /// Create new unstable environemnt and sets all lists to empty
        /// </summary>
        public UnstableEnvironment()
        {
            IgnoredPositions = new List<IUnitCellPosition>();
            PairInteractions = new List<IAsymmetricPairInteraction>();
            GroupInteractions = new List<IGroupInteraction>();
        }

        /// <summary>
        /// Get all unit cell positions that are ignored during the environment search (Never null)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IUnitCellPosition> GetIgnoredPositions()
        {
            return (IgnoredPositions ?? new List<IUnitCellPosition>()).AsEnumerable();
        }

        /// <summary>
        /// Get all pair interactions affiliated with this environment (Never null)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IAsymmetricPairInteraction> GetPairInteractions()
        {
            return (PairInteractions ?? new List<IAsymmetricPairInteraction>()).AsEnumerable();
        }

        /// <summary>
        /// Get all group interactions affiliated wit this environment (Never null)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IGroupInteraction> GetGroupInteractions()
        {
            return (GroupInteractions ?? new List<IGroupInteraction>()).AsEnumerable();
        }

        /// <summary>
        /// Get a string literal name for the model object
        /// </summary>
        /// <returns></returns>
        public override string GetModelObjectName()
        {
            return "'Unstable Environment Info'";
        }

        /// <summary>
        /// Copies the values of a model object interface into this one and returns this object (Returns null if interfacc if of wrong type)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateObject(IModelObject obj)
        {
            if (CastWithDepricatedCheck<IUnstableEnvironment>(obj) is var info)
            {
                UnitCellPosition = info.UnitCellPosition;
                MaxInteractionRange = info.MaxInteractionRange;
                IgnoredPositions = info.GetIgnoredPositions().ToList();
                GroupInteractions = info.GetGroupInteractions().ToList();
                return this;
            }
            return null;
        }
    }
}
