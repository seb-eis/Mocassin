﻿using System;
using System.Xml.Serialization;
using Mocassin.Mathematics.Extensions;
using Mocassin.Model.Basic;
using Mocassin.Model.Structures;
using Mocassin.Symmetry.CrystalSystems;
using Mocassin.UI.Xml.BaseData;

namespace Mocassin.UI.Xml.StructureData
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Structures.ICellParameters" /> model parameter creation
    /// </summary>
    [XmlRoot("CellParameters")]
    public class XmlCellParameters : XmlModelParameter
    {
        /// <summary>
        ///     Boolean flag to enable radian based angle definition
        /// </summary>
        [XmlAttribute("IsRadian")]
        public bool IsRadian { get; set; }

        /// <summary>
        ///     The cell parameter A in [Ang]
        /// </summary>
        [XmlAttribute("A")]
        public double ParamA { get; set; }

        /// <summary>
        ///     The cell parameter B in [Ang]
        /// </summary>
        [XmlAttribute("B")]
        public double ParamB { get; set; }

        /// <summary>
        ///     The cell parameter C in [Ang]
        /// </summary>
        [XmlAttribute("C")]
        public double ParamC { get; set; }

        /// <summary>
        ///     The unit cell angle alpha in radian or degree
        /// </summary>
        [XmlAttribute("Alpha")]
        public double Alpha { get; set; }

        /// <summary>
        ///     The unit cell angle beta in radian or degree
        /// </summary>
        [XmlAttribute("Beta")]
        public double Beta { get; set; }

        /// <summary>
        ///     The unit cell angle gamma in radian or degree
        /// </summary>
        [XmlAttribute("Gamma")]
        public double Gamma { get; set; }


        /// <inheritdoc />
        protected override ModelParameter GetPreparedModelObject()
        {
            var obj = new CellParameters
            {
                ParameterSet = GetParameterSet()
            };
            return obj;
        }

        /// <summary>
        ///     Translates the set properties into a cystal parameter set
        /// </summary>
        /// <returns></returns>
        private CrystalParameterSet GetParameterSet()
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
    }
}