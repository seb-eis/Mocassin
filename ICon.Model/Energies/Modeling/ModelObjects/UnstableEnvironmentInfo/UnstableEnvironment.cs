using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using ICon.Model.Basic;
using ICon.Model.Structures;

namespace ICon.Model.Energies
{
    /// <inheritdoc cref="ICon.Model.Energies.IUnstableEnvironment"/>
    [DataContract(Name = "UnstableEnvironmentInfo")]
    public class UnstableEnvironment : ModelObject, IUnstableEnvironment
    {
        /// <inheritdoc />
        [DataMember]
        public double MaxInteractionRange { get; set; }

        /// <inheritdoc />
        [DataMember]
        [IndexResolved]
        public IUnitCellPosition UnitCellPosition { get; set; }

        /// <summary>
        /// The set of unique ignored unit cell positions during environment sampling (Can be null)
        /// </summary>
        [DataMember]
        [IndexResolved]
        public List<IUnitCellPosition> IgnoredPositions { get; set; }

        /// <summary>
        /// The list of generated pair interactions (Can be null, automatically managed and linked property, not part of object population)
        /// </summary>
        [DataMember]
        [IndexResolved]
        public List<IAsymmetricPairInteraction> PairInteractions { get; set; }

        /// <summary>
        /// The list of generated group interactions (Automatically linked by the model)
        /// </summary>
        [DataMember]
        [IndexResolved]
        public List<IGroupInteraction> GroupInteractions { get; set; }

        /// <summary>
        /// Create new unstable environment and sets all lists to empty
        /// </summary>
        public UnstableEnvironment()
        {
            IgnoredPositions = new List<IUnitCellPosition>();
            PairInteractions = new List<IAsymmetricPairInteraction>();
            GroupInteractions = new List<IGroupInteraction>();
        }

        /// <inheritdoc />
        public IEnumerable<IUnitCellPosition> GetIgnoredPositions()
        {
            return (IgnoredPositions ?? new List<IUnitCellPosition>()).AsEnumerable();
        }

        /// <inheritdoc />
        public IEnumerable<IAsymmetricPairInteraction> GetPairInteractions()
        {
            return (PairInteractions ?? new List<IAsymmetricPairInteraction>()).AsEnumerable();
        }

        /// <inheritdoc />
        public IEnumerable<IGroupInteraction> GetGroupInteractions()
        {
            return (GroupInteractions ?? new List<IGroupInteraction>()).AsEnumerable();
        }

        /// <inheritdoc />
        public override string GetModelObjectName()
        {
            return "'Unstable Environment Info'";
        }

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastWithDepricatedCheck<IUnstableEnvironment>(obj) is IUnstableEnvironment info))
                return null;

            UnitCellPosition = info.UnitCellPosition;
            MaxInteractionRange = info.MaxInteractionRange;
            IgnoredPositions = info.GetIgnoredPositions().ToList();
            GroupInteractions = info.GetGroupInteractions().ToList();
            return this;
        }
    }
}
