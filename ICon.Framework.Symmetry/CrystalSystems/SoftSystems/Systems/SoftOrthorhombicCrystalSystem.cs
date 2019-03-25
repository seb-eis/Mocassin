using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Extensions;

namespace Mocassin.Symmetry.CrystalSystems
{
    /// <summary>
    ///     Class for a soft orthorhombic crystal system that supports the crystal system hierarchy (i.e. the hard orthorhombic
    ///     condition is not enforced)
    /// </summary>
    public class SoftOrthorhombicCrystalSystem : CrystalSystem
    {
        /// <inheritdoc />
        public override void ApplyParameterDependencies(CrystalParameterSet paramSet)
        {
            paramSet.Alpha = Alpha.Value;
            paramSet.Beta = Beta.Value;
            paramSet.Gamma = Gamma.Value;
        }

        /// <inheritdoc />
        public override bool ValidateSoftAngleCondition(double alpha, double beta, double gamma)
        {
            return MocassinMath.Radian90.CountMatches(BasicConstraint.Comparer, alpha, beta, gamma) == 3;
        }

        /// <inheritdoc />
        public override bool ValidateSoftParameterCondition(double paramA, double paramB, double paramC)
        {
            return true;
        }

        /// <inheritdoc />
        public override CrystalParameterSet GetDefaultParameterSet()
        {
            return new CrystalParameterSet(1.0, 1.0, 1.0, MocassinMath.Radian90, MocassinMath.Radian90, MocassinMath.Radian90);
        }
    }
}