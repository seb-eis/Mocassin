using System;

using ICon.Mathematics.Constraints;
using ICon.Mathematics.Comparers;

namespace ICon.Symmetry.CrystalSystems
{
    /// <summary>
    /// Defines a crystal system setting containing information which parameters can be set as well as the range constraints
    /// </summary>
    public class CrystalSystemSetting
    {
        /// <summary>
        /// Internal crystal system settings constructor
        /// </summary>
        internal CrystalSystemSetting()
        {
        }

        /// <summary>
        /// Stores default constructor delegate for this type
        /// </summary>
        public Func<CrystalSystem> DefaultConstruct { get; set; }

        /// <summary>
        /// The specific crystal system variation
        /// </summary>
        public CrystalVariation Variation { get; set; }

        /// <summary>
        /// The basic crystal sytem ID (0 to 6)
        /// </summary>
        public CrystalSystemID SystemID { get; set; }

        /// <summary>
        /// The cyrstal system literal name
        /// </summary>
        public String SystemName { get; set; }

        /// <summary>
        /// The parameter fixed flags
        /// </summary>
        public (Boolean A, Boolean B, Boolean C) ParamFixed { get; set; }

        /// <summary>
        /// Min and max value for the alpha angle (in radian) and the fixed flag
        /// </summary>
        public (Boolean Fixed, Double Min, Double Max) Alpha { get; set; }

        /// <summary>
        /// Min and max value for the beta angle (in radian) and the fixed flag
        /// </summary>
        public (Boolean Fixed, Double Min, Double Max) Beta { get; set; }

        /// <summary>
        /// Min and max value for the gamma angle (in radian) and the fixed flag
        /// </summary>
        public (Boolean Fixed, Double Min, Double Max) Gamma { get; set; }

        /// <summary>
        /// Applies the settings to a crystal system withe specified tolerance range, systemID and parameter max value
        /// </summary>
        /// <param name="system"></param>
        public void ApplySettings(CrystalSystem system, Double toleranceRange, Double parameterMaxValue)
        {
            if (system == null)
            {
                throw new ArgumentNullException(nameof(system));
            }
            SetParameterAndAngles(system);
            SetConstraints(system, parameterMaxValue, toleranceRange);
        }

        /// <summary>
        /// Sets the parameters and angles to their default (min values) values and set the fixed flags + system ID
        /// </summary>
        /// <param name="system"></param>
        /// <param name="subStrings"></param>
        protected void SetParameterAndAngles(CrystalSystem system)
        {
            system.SystemName = SystemName;
            system.SystemID = SystemID;
            system.Variation = Variation;
            system.Alpha = (Alpha.Min, Alpha.Fixed);
            system.Beta = (Beta.Min, Beta.Fixed);
            system.Gamma = (Gamma.Min, Gamma.Fixed);
            system.ParamA = (1.0, ParamFixed.A);
            system.ParamB = (1.0, ParamFixed.B);
            system.ParamC = (1.0, ParamFixed.C);
        }

        /// <summary>
        /// Creates and sets all the system constraints
        /// </summary>
        /// <param name="system"></param>
        /// <param name="toleranceValue"></param>
        /// <param name="subStrings"></param>
        protected void SetConstraints(CrystalSystem system, Double parameterMaxValue, Double toleranceValue)
        {
            var comparer = DoubleComparer.CreateRanged(toleranceValue);
            system.AlphaConstraint = new DoubleConstraint(true, Alpha.Min, Alpha.Max, true, comparer);
            system.BetaConstraint = new DoubleConstraint(true, Beta.Min, Beta.Max, true, comparer);
            system.GammaConstraint = new DoubleConstraint(true, Gamma.Min, Gamma.Max, true, comparer);
            system.BasicConstraint = new DoubleConstraint(true, 0.0, Double.MaxValue, false, comparer);
            system.ParameterConstraint = new DoubleConstraint(false, 0.0, parameterMaxValue, true, comparer);
        }
    }
}
