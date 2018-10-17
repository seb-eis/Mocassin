using System;
using System.Collections.Generic;
using Mocassin.Mathematics.Extensions;

namespace Mocassin.Symmetry.CrystalSystems
{
    /// <summary>
    ///     Crystal system context for handling of soft crystal system settings (Crystal hierarchy is not enforced)
    /// </summary>
    public class SoftCrystalSystemContext : CrystalSystemContext
    {
        /// <summary>
        ///     Static settings dictionary for all existing soft settings (12 in total)
        /// </summary>
        protected static Dictionary<(int SystemId, string SettingName), CrystalSystemSetting> StaticDictionary;

        /// <summary>
        ///     Default open angle (not fixed, from 0 to 2*PI)
        /// </summary>
        public static (bool IsFixed, double Min, double Max) OpenAngle = (false, 0.0, MocassinMath.Radian360);

        /// <summary>
        ///     Default closed angle that is dependent from another (fixed, from 0 to 2*PI)
        /// </summary>
        public static (bool IsFixed, double Min, double Max) DependentAngle = (true, 0.0, MocassinMath.Radian360);

        /// <summary>
        ///     Default fixed right or cubic angle (closed, fixed to 0.5*PI)
        /// </summary>
        public static (bool IsFixed, double Min, double Max) CubicAngle = (true, MocassinMath.Radian90, MocassinMath.Radian90);

        /// <summary>
        ///     Default fixed hexagonal angle (closed, fixed to 2/3*PI)
        /// </summary>
        public static (bool IsFixed, double Min, double Max) HexagonalAngle = (true, MocassinMath.Radian120, MocassinMath.Radian120);

        /// <summary>
        ///     The default parameter fixes (all open)
        /// </summary>
        public static (bool A, bool B, bool C) OpenParams = (false, false, false);

        /// <summary>
        ///     The cubic parameter fix (only a is open, rest dependent)
        /// </summary>
        public static (bool A, bool B, bool C) CubicParams = (false, true, true);

        /// <summary>
        ///     The hexagonal parameter fixes (a,c are open and b is dependent)
        /// </summary>
        public static (bool A, bool B, bool C) HexagonalParams = (false, true, false);

        /// <summary>
        ///     Property access for the static soft settings dictionary
        /// </summary>
        protected override Dictionary<(int SystemID, string VariationName), CrystalSystemSetting> SettingsDictionary
        {
            get => StaticDictionary;
            set => StaticDictionary = value;
        }

        /// <summary>
        ///     Internal constructor, creates the static soft settings dictionary
        /// </summary>
        internal SoftCrystalSystemContext()
        {
        }

        /// <inheritdoc />
        protected override void AddTriclinicSettings(Dictionary<(int SystemID, string VariationName), CrystalSystemSetting> dictionary)
        {
            Func<CrystalSystem> construct = Create<SoftTriclinicCrystalSystem>;

            dictionary[(0, "None")] = new CrystalSystemSetting
            {
                SystemName = "Triclinic",
                SystemID = CrystalSystemId.Triclinic,
                Variation = CrystalVariation.None,
                DefaultConstruct = construct,
                ParamFixed = OpenParams,
                Alpha = OpenAngle,
                Beta = OpenAngle,
                Gamma = OpenAngle
            };
        }

        /// <inheritdoc />
        protected override void AddMonoclinicSettings(Dictionary<(int SystemID, string VariationName), CrystalSystemSetting> dictionary)
        {
            Func<CrystalSystem> construct = Create<SoftMonoclinicCrystalSystem>;

            dictionary[(1, "Unique Axis A")] = new CrystalSystemSetting
            {
                SystemName = "Monoclinic",
                SystemID = CrystalSystemId.Monoclinic,
                Variation = CrystalVariation.UniqueAxisA,
                DefaultConstruct = construct,
                ParamFixed = OpenParams,
                Alpha = OpenAngle,
                Beta = CubicAngle,
                Gamma = CubicAngle
            };

            dictionary[(1, "Unique Axis B")] = new CrystalSystemSetting
            {
                SystemName = "Monoclinic",
                SystemID = CrystalSystemId.Monoclinic,
                Variation = CrystalVariation.UniqueAxisB,
                DefaultConstruct = construct,
                ParamFixed = OpenParams,
                Alpha = CubicAngle,
                Beta = OpenAngle,
                Gamma = CubicAngle
            };

            dictionary[(1, "Unique Axis C")] = new CrystalSystemSetting
            {
                SystemName = "Monoclinic",
                SystemID = CrystalSystemId.Monoclinic,
                Variation = CrystalVariation.UniqueAxisC,
                DefaultConstruct = construct,
                ParamFixed = OpenParams,
                Alpha = CubicAngle,
                Beta = CubicAngle,
                Gamma = OpenAngle
            };
        }

        /// <inheritdoc />
        protected override void AddOrthorhombicSettings(Dictionary<(int SystemID, string VariationName), CrystalSystemSetting> dictionary)
        {
            Func<CrystalSystem> construct = Create<SoftOrthorhombicCrystalSystem>;

            dictionary[(2, "None")] = new CrystalSystemSetting
            {
                SystemName = "Orthorhombic",
                SystemID = CrystalSystemId.Orthorhombic,
                Variation = CrystalVariation.None,
                DefaultConstruct = construct,
                ParamFixed = OpenParams,
                Alpha = CubicAngle,
                Beta = CubicAngle,
                Gamma = CubicAngle
            };
            dictionary[(2, "Origin Choice 1")] = dictionary[(2, "None")];
            dictionary[(2, "Origin Choice 2")] = dictionary[(2, "None")];
        }

        /// <inheritdoc />
        protected override void AddTetragonalSettings(Dictionary<(int SystemID, string VariationName), CrystalSystemSetting> dictionary)
        {
            Func<CrystalSystem> construct = Create<SoftTetragonalCrystalSystem>;

            dictionary[(3, "None")] = new CrystalSystemSetting
            {
                SystemName = "Tetragonal",
                SystemID = CrystalSystemId.Tetragonal,
                Variation = CrystalVariation.None,
                DefaultConstruct = construct,
                ParamFixed = HexagonalParams,
                Alpha = CubicAngle,
                Beta = CubicAngle,
                Gamma = CubicAngle
            };
            dictionary[(3, "Origin Choice 1")] = dictionary[(3, "None")];
            dictionary[(3, "Origin Choice 2")] = dictionary[(3, "None")];
        }

        /// <inheritdoc />
        protected override void AddTrigonalSettings(Dictionary<(int SystemID, string VariationName), CrystalSystemSetting> dictionary)
        {
            Func<CrystalSystem> construct = Create<SoftTrigonalCrystalSystem>;

            dictionary[(4, "Hexagonal Axes")] = new CrystalSystemSetting
            {
                SystemName = "Trigonal",
                SystemID = CrystalSystemId.Trigonal,
                Variation = CrystalVariation.HexagonalAxes,
                DefaultConstruct = construct,
                ParamFixed = HexagonalParams,
                Alpha = CubicAngle,
                Beta = CubicAngle,
                Gamma = HexagonalAngle
            };

            dictionary[(4, "Rhombohedral Axes")] = new CrystalSystemSetting
            {
                SystemName = "Trigonal",
                SystemID = CrystalSystemId.Trigonal,
                Variation = CrystalVariation.RhombohedralAxes,
                DefaultConstruct = construct,
                ParamFixed = CubicParams,
                Alpha = OpenAngle,
                Beta = DependentAngle,
                Gamma = DependentAngle
            };

            dictionary[(4, "None")] = dictionary[(4, "Hexagonal Axes")];
        }

        /// <inheritdoc />
        protected override void AddHexagonalSettings(Dictionary<(int SystemID, string VariationName), CrystalSystemSetting> dictionary)
        {
            Func<CrystalSystem> construct = Create<SoftHexagonalCrystalSystem>;

            dictionary[(5, "None")] = new CrystalSystemSetting
            {
                SystemName = "Hexagonal",
                SystemID = CrystalSystemId.Hexagonal,
                Variation = CrystalVariation.None,
                DefaultConstruct = construct,
                ParamFixed = HexagonalParams,
                Alpha = CubicAngle,
                Beta = CubicAngle,
                Gamma = HexagonalAngle
            };
        }

        /// <inheritdoc />
        protected override void AddCubicSettings(Dictionary<(int SystemID, string VariationName), CrystalSystemSetting> dictionary)
        {
            Func<CrystalSystem> construct = Create<SoftCubicCrystalSystem>;

            dictionary[(6, "None")] = new CrystalSystemSetting
            {
                SystemName = "Cubic",
                SystemID = CrystalSystemId.Cubic,
                Variation = CrystalVariation.None,
                DefaultConstruct = construct,
                ParamFixed = CubicParams,
                Alpha = CubicAngle,
                Beta = CubicAngle,
                Gamma = CubicAngle
            };
            dictionary[(6, "Origin Choice 1")] = dictionary[(6, "None")];
            dictionary[(6, "Origin Choice 2")] = dictionary[(6, "None")];
        }
    }
}