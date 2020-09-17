using Mocassin.Mathematics.Extensions;

namespace Mocassin.Symmetry.CrystalSystems
{
    /// <summary>
    ///     Class for a soft triclinic crystal system that supports the crystal system hierarchy (i.e. the hard triclinic
    ///     condition is not enforced)
    /// </summary>
    public class SoftTriclinicCrystalSystem : CrystalSystem
    {
        /// <inheritdoc />
        public override void ApplyParameterDependencies(CrystalParameterSet paramSet)
        {
        }

        /// <inheritdoc />
        public override bool ValidateSoftAngleCondition(double alpha, double beta, double gamma) => true;

        /// <inheritdoc />
        public override bool ValidateSoftParameterCondition(double paramA, double paramB, double paramC) => true;

        /// <inheritdoc />
        public override CrystalParameterSet GetDefaultParameterSet() =>
            new CrystalParameterSet(1.0, 1.0, 1.0, MocassinMath.Radian90, MocassinMath.Radian90, MocassinMath.Radian90);
    }
}