using ICon.Mathematics.Coordinates;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Structures;
using ICon.Symmetry.SpaceGroups;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class TargetPositionInfo : ITargetPositionInfo
    {
        /// <inheritdoc />
        public IUnitCellPosition UnitCellPosition { get; set; }

        /// <inheritdoc />
        public double Distance { get; set; }

        /// <inheritdoc />
        public Fractional3D AbsoluteFractional3D { get; set; }

        /// <inheritdoc />
        public Fractional3D RelativeFractional3D { get; set; }

        /// <inheritdoc />
        public Cartesian3D AbsoluteCartesian3D { get; set; }

        /// <inheritdoc />
        public CrystalVector4D RelativeVector4D { get; set; }
    }
}