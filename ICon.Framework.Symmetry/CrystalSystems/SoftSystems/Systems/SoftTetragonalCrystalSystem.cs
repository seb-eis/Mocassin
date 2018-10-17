using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Extensions;

namespace Mocassin.Symmetry.CrystalSystems
{
    /// <summary>
    ///     Class for a soft tetragonal crystal system that supports the crystal system hierarchy (i.e. the hard tetragonal
    ///     condition is not enforced)
    /// </summary>
    public class SoftTetragonalCrystalSystem : CrystalSystem
    {
        /// <inheritdoc />
        public override void ApplyParameterDependencies(CrystalParameterSet paramSet)
        {
            paramSet.Alpha = MocassinMath.Radian90;
            paramSet.Beta = MocassinMath.Radian90;
            paramSet.Gamma = MocassinMath.Radian90;
            paramSet.ParamB = paramSet.ParamB;
        }

        /// <inheritdoc />
        public override bool ValidateSoftAngleCondition(double alpha, double beta, double gamma)
        {
            return MocassinMath.Radian90.CountMatches(BasicConstraint.Comparer, alpha, beta, gamma) == 3;
        }

        /// <inheritdoc />
        public override bool ValidateSoftParameterCondition(double paramA, double paramB, double paramC)
        {
            return BasicConstraint.Comparer.Equals(paramA, paramB);
        }

        /// <inheritdoc />
        public override CrystalParameterSet GetDefaultParameterSet()
        {
            return new CrystalParameterSet(1.0, 1.0, 1.0, MocassinMath.Radian90, MocassinMath.Radian90, MocassinMath.Radian90);
        }
    }
}