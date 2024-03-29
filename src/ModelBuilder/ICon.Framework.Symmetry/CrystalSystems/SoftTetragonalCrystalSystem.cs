﻿using Mocassin.Framework.Extensions;
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
            paramSet.Alpha = Alpha.Value;
            paramSet.Beta = Beta.Value;
            paramSet.Gamma = Gamma.Value;
            paramSet.ParamB = paramSet.ParamA;
        }

        /// <inheritdoc />
        public override bool ValidateSoftAngleCondition(double alpha, double beta, double gamma) =>
            MocassinMath.Radian90.CountMatchesWithParameters(BasicConstraint.Comparer, alpha, beta, gamma) == 3;

        /// <inheritdoc />
        public override bool ValidateSoftParameterCondition(double paramA, double paramB, double paramC) => BasicConstraint.Comparer.Equals(paramA, paramB);

        /// <inheritdoc />
        public override CrystalParameterSet GetDefaultParameterSet() =>
            new CrystalParameterSet(1.0, 1.0, 1.0, MocassinMath.Radian90, MocassinMath.Radian90, MocassinMath.Radian90);
    }
}