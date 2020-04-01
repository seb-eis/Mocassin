using System.Collections.Generic;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Energies
{
    /// <inheritdoc cref="IPairInteraction" />
    public abstract class PairInteraction : ModelObject, IPairInteraction
    {
        /// <inheritdoc />
        [UseTrackedData]
        public ICellSite Position0 { get; set; }

        /// <inheritdoc />
        [UseTrackedData]
        public ICellSite Position1 { get; set; }

        /// <summary>
        ///     The actual position vector for the second unit cell position
        /// </summary>
        public Fractional3D SecondPositionVector { get; set; }

        /// <inheritdoc />
        public double Distance { get; set; }

        /// <inheritdoc />
        public bool IsChiral => ChiralPartner != null;

        /// <inheritdoc />
        public virtual bool IsSymmetric => Position0 == Position1;

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
        protected PairInteraction(PairCandidate candidate)
        {
            Index = candidate.Index;
            Position0 = candidate.Position0;
            Position1 = candidate.Position1;
            SecondPositionVector = candidate.PositionVector;
            Distance = candidate.Distance;
        }

        /// <inheritdoc />
        public abstract IEnumerable<PairEnergyEntry> GetEnergyEntries();

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IPairInteraction>(obj) is { } interaction))
                return null;

            Position0 = interaction.Position0;
            Position1 = interaction.Position1;
            SecondPositionVector = interaction.SecondPositionVector;
            Distance = interaction.Distance;
            return this;
        }

        /// <summary>
        ///     Tries to set the passed <see cref="PairEnergyEntry"/> in the pair interaction energy dictionary. Returns false if value cannot be
        ///     set
        /// </summary>
        /// <param name="energyEntry"></param>
        /// <returns></returns>
        public abstract bool TrySetEnergyEntry(PairEnergyEntry energyEntry);
    }
}