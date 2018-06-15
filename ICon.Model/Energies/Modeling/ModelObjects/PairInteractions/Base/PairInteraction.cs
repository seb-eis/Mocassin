using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ICon.Model.Basic;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Structures;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Abstract base class for pair interaction model object implementations that describe a reference interaction that involves two positions
    /// </summary>
    [DataContract]
    public abstract class PairInteraction : ModelObject, IPairInteraction
    {
        /// <summary>
        /// The first unit cell position (The position vector is correct)
        /// </summary>
        [DataMember]
        public IUnitCellPosition Position0 { get; set; }

        /// <summary>
        /// The second unit cell position (The position vector is not correct)
        /// </summary>
        [DataMember]
        public IUnitCellPosition Position1 { get; set; }

        /// <summary>
        /// The actual position vector for the second unit cell position
        /// </summary>
        [DataMember]
        public DataVector3D SecondPositionVector { get; set; }

        /// <summary>
        /// The distance between the interacting positions in internal units
        /// </summary>
        [DataMember]
        public double Distance { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        protected PairInteraction()
        {
        }

        /// <summary>
        /// Construct new pair interaction from pair candidate
        /// </summary>
        protected PairInteraction(in PairCandidate candidate)
        {
            Index = candidate.Index;
            Position0 = candidate.Position0;
            Position1 = candidate.Position1;
            SecondPositionVector = new DataVector3D(candidate.PositionVector);
            Distance = candidate.Distance;
        }

        /// <summary>
        /// Get the actual vector describing where the second unit cell position is located
        /// </summary>
        /// <returns></returns>
        public Fractional3D GetSecondPositionVector()
        {
            return SecondPositionVector.AsFractional();
        }

        /// <summary>
        /// Populates the base class properties froma model object interafce and retruns this object (Returns null if population failed)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateObject(IModelObject obj)
        {
            if (CastWithDepricatedCheck<IPairInteraction>(obj) is var interaction)
            {
                Position0 = interaction.Position0;
                Position1 = interaction.Position1;
                SecondPositionVector = new DataVector3D(interaction.GetSecondPositionVector());
                Distance = interaction.Distance;
                return this;
            }
            return null;
        }
    }
}
