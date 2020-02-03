using Mocassin.Mathematics.Comparers;
using Mocassin.Mathematics.Extensions;
using Mocassin.Model.Basic;
using Mocassin.Symmetry.CrystalSystems;

namespace Mocassin.Model.Structures
{
    /// <inheritdoc cref="Mocassin.Model.Structures.ICellParameters" />
    public class CellParameters : ModelParameter, ICellParameters
    {
        /// <summary>
        ///     The crystal parameter set
        /// </summary>
        public CrystalParameterSet ParameterSet { get; set; }

        /// <inheritdoc />
        public double ParamA => ParameterSet.ParamA;

        /// <inheritdoc />
        public double ParamB => ParameterSet.ParamB;

        /// <inheritdoc />
        public double ParamC => ParameterSet.ParamC;

        /// <inheritdoc />
        public double Alpha => ParameterSet.Alpha;

        /// <inheritdoc />
        public double Beta => ParameterSet.Beta;

        /// <inheritdoc />
        public double Gamma => ParameterSet.Gamma;

        /// <inheritdoc />
        public double AlphaDegree => MocassinMath.RadianToDegree(Alpha);

        /// <inheritdoc />
        public double BetaDegree => MocassinMath.RadianToDegree(Beta);

        /// <inheritdoc />
        public double GammaDegree => MocassinMath.RadianToDegree(Gamma);

        /// <summary>
        ///     Implicit cast of crystal parameter set to the wrapper object
        /// </summary>
        /// <param name="paramSet"></param>
        public static implicit operator CellParameters(CrystalParameterSet paramSet)
        {
            return new CellParameters {ParameterSet = paramSet};
        }

        /// <inheritdoc />
        public override string GetParameterName()
        {
            return "Cell Parameters";
        }

        /// <inheritdoc />
        public override ModelParameter PopulateObject(IModelParameter modelParameter)
        {
            if (!(modelParameter is ICellParameters cellParameters))
                return null;

            ParameterSet =
                new CrystalParameterSet(cellParameters.ParamA, cellParameters.ParamB, cellParameters.ParamC, cellParameters.Alpha,
                    cellParameters.Beta, cellParameters.Gamma);
            return this;
        }

        /// <inheritdoc />
        public override bool Equals(IModelParameter other)
        {
            var comparer = NumericComparer.CreateRanged(1.0e-10);
            if (other is ICellParameters otherParams)
            {
                return comparer.Equals(ParamA, otherParams.ParamA)
                       && comparer.Equals(ParamB, otherParams.ParamB)
                       && comparer.Equals(ParamC, otherParams.ParamC)
                       && comparer.Equals(Alpha, otherParams.Alpha)
                       && comparer.Equals(Beta, otherParams.Beta)
                       && comparer.Equals(Gamma, otherParams.Gamma);
            }

            return false;
        }

        /// <inheritdoc />
        public CrystalParameterSet AsParameterSet()
        {
            return new CrystalParameterSet(ParamA, ParamB, ParamC, Alpha, Beta, Gamma);
        }
    }
}