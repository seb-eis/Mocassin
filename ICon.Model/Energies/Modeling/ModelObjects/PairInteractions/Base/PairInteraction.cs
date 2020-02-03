using System.Collections.Generic;
using System.Runtime.Serialization;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Energies
{
    /// <inheritdoc cref="IPairInteraction" />
    [DataContract]
    public abstract class PairInteraction : ModelObject, IPairInteraction
    {
        /// <inheritdoc />
        [DataMember]
        public ICellReferencePosition Position0 { get; set; }

        /// <inheritdoc />
        [DataMember]
        public ICellReferencePosition Position1 { get; set; }

        /// <summary>
        ///     The actual position vector for the second unit cell position
        /// </summary>
        [DataMember]
        public DataVector3D SecondPositionVector { get; set; }

        /// <inheritdoc />
        [DataMember]
        public double Distance { get; set; }

        /// <inheritdoc />
        public bool IsChiral => ChiralPartner != null;

        /// <inheritdoc />
        public abstract IPairInteraction ChiralPartner { get; }

        /// <summary>
        ///     Default constructor
        /// </summary>
        protected PairInteraction()
        {
        }

        /// <summary>
        ///     Construct new pair interaction from pair candidate
        /// </summary>
        protected PairInteraction(in PairCandidate candidate)
        {
            Index = candidate.Index;
            Position0 = candidate.Position0;
            Position1 = candidate.Position1;
            SecondPositionVector = new DataVector3D(candidate.PositionVector);
            Distance = candidate.Distance;
        }

        /// <inheritdoc />
        public Fractional3D GetSecondPositionVector()
        {
            return SecondPositionVector.AsFractional();
        }

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IPairInteraction>(obj) is IPairInteraction interaction))
                return null;

            Position0 = interaction.Position0;
            Position1 = interaction.Position1;
            SecondPositionVector = new DataVector3D(interaction.GetSecondPositionVector());
            Distance = interaction.Distance;
            return this;
        }

        /// <summary>
        ///     Tries to set the passed energy entry in the pair interaction energy dictionary. Returns false if value cannot be
        ///     set
        /// </summary>
        /// <param name="energyEntry"></param>
        /// <returns></returns>
        public abstract bool TrySetEnergyEntry(in PairEnergyEntry energyEntry);

        /// <inheritdoc />
        public abstract IEnumerable<PairEnergyEntry> GetEnergyEntries();
    }
}