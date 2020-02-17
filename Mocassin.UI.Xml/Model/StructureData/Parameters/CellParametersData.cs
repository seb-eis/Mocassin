using System;
using System.Xml.Serialization;
using Mocassin.Mathematics.Extensions;
using Mocassin.Model.Basic;
using Mocassin.Model.Structures;
using Mocassin.Symmetry.CrystalSystems;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.StructureModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Structures.ICellParameters" /> model parameter creation
    /// </summary>
    [XmlRoot]
    public class CellParametersData : ModelParameterObject
    {
        private double alpha;
        private double beta;
        private double gamma;
        private bool isRadian;
        private double paramA;
        private double paramB;
        private double paramC;

        /// <summary>
        ///     Boolean flag to enable radian based angle definition
        /// </summary>
        [XmlAttribute]
        public bool IsRadian
        {
            get => isRadian;
            set => SetProperty(ref isRadian, value);
        }

        /// <summary>
        ///     The cell parameter A in [Ang]
        /// </summary>
        [XmlAttribute]
        public double ParamA
        {
            get => paramA;
            set => SetProperty(ref paramA, value);
        }

        /// <summary>
        ///     The cell parameter B in [Ang]
        /// </summary>
        [XmlAttribute]
        public double ParamB
        {
            get => paramB;
            set => SetProperty(ref paramB, value);
        }

        /// <summary>
        ///     The cell parameter C in [Ang]
        /// </summary>
        [XmlAttribute]
        public double ParamC
        {
            get => paramC;
            set => SetProperty(ref paramC, value);
        }

        /// <summary>
        ///     The unit cell angle alpha in radian or degree
        /// </summary>
        [XmlAttribute]
        public double Alpha
        {
            get => alpha;
            set => SetProperty(ref alpha, value);
        }

        /// <summary>
        ///     The unit cell angle beta in radian or degree
        /// </summary>
        [XmlAttribute]
        public double Beta
        {
            get => beta;
            set => SetProperty(ref beta, value);
        }

        /// <summary>
        ///     The unit cell angle gamma in radian or degree
        /// </summary>
        [XmlAttribute]
        public double Gamma
        {
            get => gamma;
            set => SetProperty(ref gamma, value);
        }

        /// <summary>
        ///     Creates a new <see cref="CellParametersData" /> with default triclinic settings
        /// </summary>
        public CellParametersData()
        {
            Alpha = Beta = Gamma = 90;
            ParamA = ParamB = ParamC = 1.0;
        }


        /// <inheritdoc />
        protected override ModelParameter GetModelObjectInternal()
        {
            var obj = new CellParameters
            {
                ParameterSet = GetParameterSet()
            };
            return obj;
        }

        /// <summary>
        ///     Translates the set properties into a crystal parameter set
        /// </summary>
        /// <returns></returns>
        public CrystalParameterSet GetParameterSet()
        {
            var result = new CrystalParameterSet
            {
                ParamA = Math.Abs(ParamA),
                ParamB = Math.Abs(ParamB),
                ParamC = Math.Abs(ParamC),
                Alpha = Alpha,
                Beta = Beta,
                Gamma = Gamma
            };
            if (IsRadian)
                return result;

            result.Alpha = MocassinMath.DegreeToRadian(result.Alpha);
            result.Beta = MocassinMath.DegreeToRadian(result.Beta);
            result.Gamma = MocassinMath.DegreeToRadian(result.Gamma);
            return result;
        }

        /// <summary>
        ///     Populates from the passed <see cref="CrystalParameterSet" />
        /// </summary>
        /// <param name="parameterSet"></param>
        public void PopulateFrom(CrystalParameterSet parameterSet)
        {
            parameterSet ??= CrystalParameterSet.CreateDefault();
            ParamA = Math.Abs(parameterSet.ParamA);
            ParamB = Math.Abs(parameterSet.ParamB);
            ParamC = Math.Abs(parameterSet.ParamC);
            Alpha = parameterSet.Alpha;
            Beta = parameterSet.Beta;
            Gamma = parameterSet.Gamma;

            if (IsRadian) return;
            Alpha = MocassinMath.RadianToDegree(Alpha);
            Beta = MocassinMath.RadianToDegree(Beta);
            Gamma = MocassinMath.RadianToDegree(Gamma);
        }
    }
}