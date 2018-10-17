using Mocassin.Mathematics.Coordinates;
using Mocassin.Symmetry.SpaceGroups;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Translator.ModelContext
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