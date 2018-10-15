using System;
using System.Collections.Generic;

using ICon.Mathematics.Extensions;

namespace ICon.Symmetry.CrystalSystems
{
    /// <summary>
    /// Crystal system context for handling of soft crystal system settings (Crystal hierachy is not enforced)
    /// </summary>
    public class SoftCrystalSystemContext :  CrystalSystemContext
    {
        /// <summary>
        /// Static settings dictionray for all existing soft settings (12 in total)
        /// </summary>
        protected static Dictionary<(Int32 SystemID, String SettingName), CrystalSystemSetting> StaticDictionary = null;

        /// <summary>
        /// Default open angle (not fixed, from 0 to 2*PI)
        /// </summary>
        public static (Boolean Fixed, Double Min, Double Max) OpenAngle = (false, 0.0, MocassinMath.Radian360);

        /// <summary>
        /// Default closed angle that is dependent from another (fixed, from 0 to 2*PI)
        /// </summary>
        public static (Boolean Fixed, Double Min, Double Max) DependentAngle = (true, 0.0, MocassinMath.Radian360);

        /// <summary>
        /// Default fixed right or cubic angle (closed, fixed to 0.5*PI)
        /// </summary>
        public static (Boolean Fixed, Double Min, Double Max) CubicAngle = (true, MocassinMath.Radian90, MocassinMath.Radian90);

        /// <summary>
        /// Default fixed hexagonal angle (closed, fixed to 2/3*PI)
        /// </summary>
        public static (Boolean Fixed, Double Min, Double Max) HexagonalAngle = (true, MocassinMath.Radian120, MocassinMath.Radian120);

        /// <summary>
        /// The default parameter fixes (all open)
        /// </summary>
        public static (Boolean A, Boolean B, Boolean C) OpenParams = (false, false, false);

        /// <summary>
        /// The cubic parameter fix (only a is open, rest dependent)
        /// </summary>
        public static (Boolean A, Boolean B, Boolean C) CubicParams = (false, true, true);

        /// <summary>
        /// The hexagonal parameter fixes (a,c are open and b is dependent)
        /// </summary>
        public static (Boolean A, Boolean B, Boolean C) HexagonalParams = (false, true, false);

        /// <summary>
        /// Property access for the static soft settings dictionary
        /// </summary>
        protected override Dictionary<(Int32 SystemID, String VariationName), CrystalSystemSetting> SettingsDictionary
        {
            get { return StaticDictionary; }

            set { StaticDictionary = value; }
        }

        /// <summary>
        /// Internal constructor, creates the static soft settings dictionary
        /// </summary>
        internal SoftCrystalSystemContext() : base()
        {
        }

        /// <summary>
        /// Adds all soft triclinic settings to a settings dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        protected override void AddTriclinicSettings(Dictionary<(Int32 SystemID, String VariationName), CrystalSystemSetting> dictionary)
        {
            Func<CrystalSystem> construct = Create<SoftTriclinicSystem>;

            dictionary[(0, "None")] = new CrystalSystemSetting()
            {
                SystemName = "Triclinic",
                SystemID = CrystalSystemID.Triclinic,
                Variation = CrystalVariation.None,
                DefaultConstruct = construct,
                ParamFixed = OpenParams,
                Alpha = OpenAngle,
                Beta = OpenAngle,
                Gamma = OpenAngle
            };
        }

        /// <summary>
        /// Adds all monoclinic settings to the dictionary (Default value is set to unrestricted gamma angle)
        /// </summary>
        /// <param name="dictionary"></param>
        protected override void AddMonoclinicSettings(Dictionary<(Int32 SystemID, String VariationName), CrystalSystemSetting> dictionary)
        {
            Func<CrystalSystem> construct = Create<SoftMonoclinicSystem>;

            dictionary[(1, "Unique Axis A")] = new CrystalSystemSetting()
            {
                SystemName = "Monoclinic",
                SystemID = CrystalSystemID.Monoclinic,
                Variation = CrystalVariation.UniqueAxisA,
                DefaultConstruct = construct,
                ParamFixed = OpenParams,
                Alpha = OpenAngle,
                Beta = CubicAngle,
                Gamma = CubicAngle
            };

            dictionary[(1, "Unique Axis B")] = new CrystalSystemSetting()
            {
                SystemName = "Monoclinic",
                SystemID = CrystalSystemID.Monoclinic,
                Variation = CrystalVariation.UniqueAxisB,
                DefaultConstruct = construct,
                ParamFixed = OpenParams,
                Alpha = CubicAngle,
                Beta = OpenAngle,
                Gamma = CubicAngle };

            dictionary[(1, "Unique Axis C")] = new CrystalSystemSetting()
            {
                SystemName = "Monoclinic",
                SystemID = CrystalSystemID.Monoclinic,
                Variation = CrystalVariation.UniqueAxisC,
                DefaultConstruct = construct,
                ParamFixed = OpenParams,
                Alpha = CubicAngle,
                Beta = CubicAngle,
                Gamma = OpenAngle };
        }

        /// <summary>
        /// Adds the default soft orthorhobic settings to a dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        protected override void AddOrthorhombicSettings(Dictionary<(Int32 SystemID, String VariationName), CrystalSystemSetting> dictionary)
        {
            Func<CrystalSystem> construct = Create<SoftOrthorhombicSystem>;

            dictionary[(2, "None")] = new CrystalSystemSetting()
            {
                SystemName = "Orthorhombic",
                SystemID = CrystalSystemID.Orthorhombic,
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

        /// <summary>
        /// Adds the default soft tetragonal setting to a dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        protected override void AddTetragonalSettings(Dictionary<(Int32 SystemID, String VariationName), CrystalSystemSetting> dictionary)
        {
            Func<CrystalSystem> construct = Create<SoftTetragonalSystem>;

            dictionary[(3, "None")] = new CrystalSystemSetting()
            {
                SystemName = "Tetragonal",
                SystemID = CrystalSystemID.Tetragonal,
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

        /// <summary>
        /// Adds hexagonal and rhombohedral system options of the trigonal soft system (Hexagonal as default) to a dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        protected override void AddTrigonalSettings(Dictionary<(Int32 SystemID, String VariationName), CrystalSystemSetting> dictionary)
        {
            Func<CrystalSystem> construct = Create<SoftTrigonalSystem>;

            dictionary[(4, "Hexagonal Axes")] = new CrystalSystemSetting()
            {
                SystemName = "Trigonal",
                SystemID = CrystalSystemID.Trigonal,
                Variation = CrystalVariation.HexagonalAxes,
                DefaultConstruct = construct,
                ParamFixed = HexagonalParams,
                Alpha = CubicAngle,
                Beta = CubicAngle,
                Gamma = HexagonalAngle
            };

            dictionary[(4, "Rhombohedral Axes")] = new CrystalSystemSetting()
            {
                SystemName = "Trigonal",
                SystemID = CrystalSystemID.Trigonal,
                Variation = CrystalVariation.RhombohedralAxes,
                DefaultConstruct = construct,
                ParamFixed = CubicParams,
                Alpha = OpenAngle,
                Beta = DependentAngle,
                Gamma = DependentAngle
            };

            dictionary[(4, "None")] = dictionary[(4, "Hexagonal Axes")];
        }

        /// <summary>
        /// Adds the default soft hexagonal setting to a dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        protected override void AddHexagonalSettings(Dictionary<(Int32 SystemID, String VariationName), CrystalSystemSetting> dictionary)
        {
            Func<CrystalSystem> construct = Create<SoftHexagonalSystem>;

            dictionary[(5, "None")] = new CrystalSystemSetting()
            {
                SystemName = "Hexagonal",
                SystemID = CrystalSystemID.Hexagonal,
                Variation = CrystalVariation.None,
                DefaultConstruct = construct,
                ParamFixed = HexagonalParams,
                Alpha = CubicAngle,
                Beta = CubicAngle,
                Gamma = HexagonalAngle
            };
        }

        /// <summary>
        /// Adds the default soft cubic setting to a dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        protected override void AddCubicSettings(Dictionary<(Int32 SystemID, String VariationName), CrystalSystemSetting> dictionary)
        {
            Func<CrystalSystem> construct = Create<SoftCubicSystem>;

            dictionary[(6, "None")] = new CrystalSystemSetting()
            {
                SystemName = "Cubic",
                SystemID = CrystalSystemID.Cubic,
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
