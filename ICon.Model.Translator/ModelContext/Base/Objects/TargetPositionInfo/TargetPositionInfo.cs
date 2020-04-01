using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class TargetPositionInfo : ITargetPositionInfo
    {
        /// <inheritdoc />
        public IPairInteractionModel PairInteractionModel { get; set; }

        /// <inheritdoc />
        public ICellSite CellSite { get; set; }

        /// <inheritdoc />
        public double Distance { get; set; }

        /// <inheritdoc />
        public Fractional3D AbsoluteFractional { get; set; }

        /// <inheritdoc />
        public Fractional3D RelativeFractional { get; set; }

        /// <inheritdoc />
        public Cartesian3D AbsoluteCartesian { get; set; }

        /// <inheritdoc />
        public CrystalVector4D RelativeCrystalVector { get; set; }
    }
}