using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Symmetry.SpaceGroups
{
    /// <summary>
    ///     Serializable non entity version of the matrix based symmetry operation
    /// </summary>
    public class SymmetryOperation : SymmetryOperationBase
    {
        /// <inheritdoc />
        public override double TrimTolerance { get; set; } = 1.0e-10;

        /// <summary>
        ///     Creates a new <see cref="SymmetryOperation" /> by adding a shift to an existing <see cref="ISymmetryOperation" />
        /// </summary>
        /// <param name="source"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public static SymmetryOperation CreateShifted(ISymmetryOperation source, in Fractional3D shift)
        {
            var result = new SymmetryOperation();
            result.SetCore(source.Core.Offset(shift));
            return result;
        }
    }
}