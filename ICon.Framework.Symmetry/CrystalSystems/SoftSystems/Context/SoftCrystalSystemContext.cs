using System;
using System.Collections.Generic;
using Mocassin.Mathematics.Extensions;

namespace Mocassin.Symmetry.CrystalSystems
{
    /// <summary>
    ///     A <see cref="CrystalSystemContext"/> for handling of soft crystal system settings (Crystal hierarchy is not strictly enforced)
    /// </summary>
    public sealed class SoftCrystalSystemContext : CrystalSystemContext
    {
        /// <summary>
        ///     Static settings dictionary for all existing soft settings (12 in total)
        /// </summary>
        private static Dictionary<CrystalSystemIdentification, CrystalSystemDefinition> SoftSystemDictionary { get; set; }

        /// <summary>
        ///     Default open angle range (context mutable, from 0 to 2*PI)
        /// </summary>
        public static CrystalParameterRange OpenAngleRange = new CrystalParameterRange(0.0, MocassinMath.Radian360, false);

        /// <summary>
        ///     Default closed angle range that is dependent from another (context immutable, from 0 to 2*PI)
        /// </summary>
        public static CrystalParameterRange DependentAngleRange = new CrystalParameterRange(0.0, MocassinMath.Radian360, true);

        /// <summary>
        ///     Default fixed right or cubic angle range (fully immutable, always 0.5*PI)
        /// </summary>
        public static CrystalParameterRange CubicAngleRange = new CrystalParameterRange(MocassinMath.Radian90, MocassinMath.Radian90, true);

        /// <summary>
        ///     Default fixed hexagonal angle range (fully immutable, always 2/3*PI)
        /// </summary>
        public static CrystalParameterRange HexagonalAngleRange = new CrystalParameterRange(MocassinMath.Radian120, MocassinMath.Radian120, true);

        /// <summary>
        ///     Property access for the static soft settings dictionary
        /// </summary>
        protected override Dictionary<CrystalSystemIdentification, CrystalSystemDefinition> SettingsDictionary => SoftSystemDictionary ??= CreateSystemDictionary();

        /// <summary>
        ///     Sets the <see cref="CrystalParameterRange"/> data for A,B,C to fit all immutable triclinic systems on a <see cref="CrystalSystemDefinition"/>
        /// </summary>
        /// <param name="definition"></param>
        public void SetLengthRangesToTriclinic(CrystalSystemDefinition definition)
        {
            definition.ParamARange = new CrystalParameterRange(0, double.MaxValue, false);
            definition.ParamBRange = new CrystalParameterRange(0, double.MaxValue, false);
            definition.ParamCRange = new CrystalParameterRange(0, double.MaxValue, false);
        }

        /// <summary>
        ///     Sets the <see cref="CrystalParameterRange"/> data for A,B,C to fit partial immutable cubic systems on a <see cref="CrystalSystemDefinition"/>
        /// </summary>
        /// <param name="definition"></param>
        public void SetLengthRangesToCubic(CrystalSystemDefinition definition)
        {
            definition.ParamARange = new CrystalParameterRange(0, double.MaxValue, false);
            definition.ParamBRange = new CrystalParameterRange(0, double.MaxValue, true);
            definition.ParamCRange = new CrystalParameterRange(0, double.MaxValue, true);
        }

        /// <summary>
        ///     Sets the <see cref="CrystalParameterRange"/> data for A,B,C to fit partial immutable hexagonal systems on a <see cref="CrystalSystemDefinition"/>
        /// </summary>
        /// <param name="definition"></param>
        public void SetLengthRangesToHexagonal(CrystalSystemDefinition definition)
        {
            definition.ParamARange = new CrystalParameterRange(0, double.MaxValue, false);
            definition.ParamBRange = new CrystalParameterRange(0, double.MaxValue, true);
            definition.ParamCRange = new CrystalParameterRange(0, double.MaxValue, false);
        }

        /// <inheritdoc />
        protected override void AddTriclinicSettings(Dictionary<CrystalSystemIdentification, CrystalSystemDefinition> dictionary)
        {
            Func<CrystalSystem> construct = Create<SoftTriclinicCrystalSystem>;
            var definition = new CrystalSystemDefinition
            {
                SystemName = "Triclinic",
                CrystalType = CrystalSystemType.Triclinic,
                CrystalVariation = CrystalSystemVariation.NoneOrOriginChoice,
                Factory = construct,
                AlphaRange = OpenAngleRange,
                BetaRange = OpenAngleRange,
                GammaRange = OpenAngleRange
            };
            SetLengthRangesToTriclinic(definition);
            dictionary[definition.GetIdentification()] = definition;
        }

        /// <inheritdoc />
        protected override void AddMonoclinicSettings(Dictionary<CrystalSystemIdentification, CrystalSystemDefinition> dictionary)
        {
            Func<CrystalSystem> construct = Create<SoftMonoclinicCrystalSystem>;
            var definitionAxisA = new CrystalSystemDefinition
            {
                SystemName = "Monoclinic",
                CrystalType = CrystalSystemType.Monoclinic,
                CrystalVariation = CrystalSystemVariation.UniqueAxisA,
                Factory = construct,
                AlphaRange = OpenAngleRange,
                BetaRange = CubicAngleRange,
                GammaRange = CubicAngleRange
            };
            SetLengthRangesToTriclinic(definitionAxisA);
            dictionary[definitionAxisA.GetIdentification()] = definitionAxisA;

            var definitionAxisB = new CrystalSystemDefinition
            {
                SystemName = "Monoclinic",
                CrystalType = CrystalSystemType.Monoclinic,
                CrystalVariation = CrystalSystemVariation.UniqueAxisB,
                Factory = construct,
                AlphaRange = CubicAngleRange,
                BetaRange = OpenAngleRange,
                GammaRange = CubicAngleRange
            };
            SetLengthRangesToTriclinic(definitionAxisB);
            dictionary[definitionAxisB.GetIdentification()] = definitionAxisB;

            var definitionAxisC = new CrystalSystemDefinition
            {
                SystemName = "Monoclinic",
                CrystalType = CrystalSystemType.Monoclinic,
                CrystalVariation = CrystalSystemVariation.UniqueAxisC,
                Factory = construct,
                AlphaRange = CubicAngleRange,
                BetaRange = CubicAngleRange,
                GammaRange = OpenAngleRange
            };
            SetLengthRangesToTriclinic(definitionAxisC);
            dictionary[definitionAxisC.GetIdentification()] = definitionAxisC;
        }

        /// <inheritdoc />
        protected override void AddOrthorhombicSettings(Dictionary<CrystalSystemIdentification, CrystalSystemDefinition> dictionary)
        {
            Func<CrystalSystem> construct = Create<SoftOrthorhombicCrystalSystem>;
            var definition = new CrystalSystemDefinition
            {
                SystemName = "Orthorhombic",
                CrystalType = CrystalSystemType.Orthorhombic,
                CrystalVariation = CrystalSystemVariation.NoneOrOriginChoice,
                Factory = construct,
                AlphaRange = CubicAngleRange,
                BetaRange = CubicAngleRange,
                GammaRange = CubicAngleRange
            };
            SetLengthRangesToTriclinic(definition);
            dictionary[definition.GetIdentification()] = definition;
        }

        /// <inheritdoc />
        protected override void AddTetragonalSettings(Dictionary<CrystalSystemIdentification, CrystalSystemDefinition> dictionary)
        {
            Func<CrystalSystem> construct = Create<SoftTetragonalCrystalSystem>;
            var definition = new CrystalSystemDefinition
            {
                SystemName = "Tetragonal",
                CrystalType = CrystalSystemType.Tetragonal,
                CrystalVariation = CrystalSystemVariation.NoneOrOriginChoice,
                Factory = construct,
                AlphaRange = CubicAngleRange,
                BetaRange = CubicAngleRange,
                GammaRange = CubicAngleRange
            };
            SetLengthRangesToHexagonal(definition);
            dictionary[definition.GetIdentification()] = definition;
        }

        /// <inheritdoc />
        protected override void AddTrigonalSettings(Dictionary<CrystalSystemIdentification, CrystalSystemDefinition> dictionary)
        {
            Func<CrystalSystem> construct = Create<SoftTrigonalCrystalSystem>;
            var hexagonalDefinition = new CrystalSystemDefinition
            {
                SystemName = "Trigonal",
                CrystalType = CrystalSystemType.Trigonal,
                CrystalVariation = CrystalSystemVariation.HexagonalAxes,
                Factory = construct,
                AlphaRange = CubicAngleRange,
                BetaRange = CubicAngleRange,
                GammaRange = HexagonalAngleRange
            };
            SetLengthRangesToHexagonal(hexagonalDefinition);
            dictionary[hexagonalDefinition.GetIdentification()] = hexagonalDefinition;
            dictionary[new CrystalSystemIdentification(CrystalSystemType.Trigonal, CrystalSystemVariation.NoneOrOriginChoice)] = hexagonalDefinition;

            var rhombohedralDefinition =  new CrystalSystemDefinition
            {
                SystemName = "Trigonal",
                CrystalType = CrystalSystemType.Trigonal,
                CrystalVariation = CrystalSystemVariation.RhombohedralAxes,
                Factory = construct,
                AlphaRange = OpenAngleRange,
                BetaRange = DependentAngleRange,
                GammaRange = DependentAngleRange
            };
            SetLengthRangesToCubic(rhombohedralDefinition);
            dictionary[rhombohedralDefinition.GetIdentification()] = rhombohedralDefinition;
        }

        /// <inheritdoc />
        protected override void AddHexagonalSettings(Dictionary<CrystalSystemIdentification, CrystalSystemDefinition> dictionary)
        {
            Func<CrystalSystem> construct = Create<SoftHexagonalCrystalSystem>;
            var definition = new CrystalSystemDefinition
            {
                SystemName = "Hexagonal",
                CrystalType = CrystalSystemType.Hexagonal,
                CrystalVariation = CrystalSystemVariation.NoneOrOriginChoice,
                Factory = construct,
                AlphaRange = CubicAngleRange,
                BetaRange = CubicAngleRange,
                GammaRange = HexagonalAngleRange
            };
            SetLengthRangesToHexagonal(definition);
            dictionary[definition.GetIdentification()] = definition;
        }

        /// <inheritdoc />
        protected override void AddCubicSettings(Dictionary<CrystalSystemIdentification, CrystalSystemDefinition> dictionary)
        {
            Func<CrystalSystem> construct = Create<SoftCubicCrystalSystem>;
            var definition = new CrystalSystemDefinition
            {
                SystemName = "Cubic",
                CrystalType = CrystalSystemType.Cubic,
                CrystalVariation = CrystalSystemVariation.NoneOrOriginChoice,
                Factory = construct,
                AlphaRange = CubicAngleRange,
                BetaRange = CubicAngleRange,
                GammaRange = CubicAngleRange
            };
            SetLengthRangesToCubic(definition);
            dictionary[definition.GetIdentification()] = definition;
        }
    }
}