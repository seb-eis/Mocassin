using System;
using Mocassin.Mathematics.Comparer;
using Mocassin.Mathematics.Constraints;

namespace Mocassin.Symmetry.CrystalSystems
{
    /// <summary>
    ///     Defines a crystal system through information about parameter constraints, default construction, name and variation
    /// </summary>
    public class CrystalSystemDefinition
    {
        /// <summary>
        ///     Stores default <see cref="Func{TResult}" /> factory delegate
        /// </summary>
        public Func<CrystalSystem> Factory { get; set; }

        /// <summary>
        ///     The specific <see cref="CrystalSystemVariation" />
        /// </summary>
        public CrystalSystemVariation CrystalVariation { get; set; }

        /// <summary>
        ///     The specific <see cref="CrystalSystemType" />
        /// </summary>
        public CrystalSystemType CrystalType { get; set; }

        /// <summary>
        ///     The crystal system literal name
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        ///     Get or set the <see cref="CrystalParameterRange" /> for the parameter A
        /// </summary>
        public CrystalParameterRange ParamARange { get; set; }

        /// <summary>
        ///     Get or set the <see cref="CrystalParameterRange" /> for the parameter B
        /// </summary>
        public CrystalParameterRange ParamBRange { get; set; }

        /// <summary>
        ///     Get or set the <see cref="CrystalParameterRange" /> for the parameter C
        /// </summary>
        public CrystalParameterRange ParamCRange { get; set; }

        /// <summary>
        ///     Get or set the <see cref="CrystalParameterRange" /> for the alpha angle (in radian)
        /// </summary>
        public CrystalParameterRange AlphaRange { get; set; }

        /// <summary>
        ///     Get or set the <see cref="CrystalParameterRange" /> for the beta angle (in radian)
        /// </summary>
        public CrystalParameterRange BetaRange { get; set; }

        /// <summary>
        ///     Get or set the <see cref="CrystalParameterRange" /> for the gamma angle (in radian)
        /// </summary>
        public CrystalParameterRange GammaRange { get; set; }

        /// <summary>
        ///     Internal crystal system settings constructor
        /// </summary>
        internal CrystalSystemDefinition()
        {
        }

        /// <summary>
        ///     Get a <see cref="CrystalSystemIdentification" /> for the system definition
        /// </summary>
        /// <returns></returns>
        public CrystalSystemIdentification GetIdentification()
        {
            return new CrystalSystemIdentification(CrystalType, CrystalVariation);
        }

        /// <summary>
        ///     Applies the settings to a crystal system withe specified tolerance range and parameter max value
        /// </summary>
        /// <param name="system"></param>
        /// <param name="toleranceRange"></param>
        /// <param name="parameterMaxValue"></param>
        public void ApplySettings(CrystalSystem system, double toleranceRange, double parameterMaxValue)
        {
            if (system == null) throw new ArgumentNullException(nameof(system));
            SetParameterAndAngles(system);
            SetConstraints(system, parameterMaxValue, toleranceRange);
        }

        /// <summary>
        ///     Sets the parameters and angles to their default (min values) values and set the fixed flags + system ID
        /// </summary>
        /// <param name="system"></param>
        protected void SetParameterAndAngles(CrystalSystem system)
        {
            system.SystemName = SystemName;
            system.SystemType = CrystalType;
            system.SystemVariation = CrystalVariation;
            system.Alpha = AlphaRange.GetMinimalParameter();
            system.Beta = BetaRange.GetMinimalParameter();
            system.Gamma = GammaRange.GetMinimalParameter();
            system.ParamA = ParamARange.GetMinimalParameter();
            system.ParamB = ParamBRange.GetMinimalParameter();
            system.ParamC = ParamCRange.GetMinimalParameter();
        }

        /// <summary>
        ///     Creates and sets the resulting <see cref="NumericConstraint" /> instances on a <see cref="CrystalSystem" /> with a
        ///     limited max unit cell length
        /// </summary>
        /// <param name="system"></param>
        /// <param name="parameterMaxValue"></param>
        /// <param name="toleranceValue"></param>
        protected void SetConstraints(CrystalSystem system, double parameterMaxValue, double toleranceValue)
        {
            var comparer = NumericComparer.CreateRanged(toleranceValue);
            system.AlphaConstraint = AlphaRange.ToNumericConstraint(comparer);
            system.BetaConstraint = BetaRange.ToNumericConstraint(comparer);
            system.GammaConstraint = GammaRange.ToNumericConstraint(comparer);
            system.BasicConstraint = new NumericConstraint(true, 0.0, double.MaxValue, false, comparer);
            system.ParameterConstraint = new NumericConstraint(false, 0.0, parameterMaxValue, true, comparer);
        }
    }
}