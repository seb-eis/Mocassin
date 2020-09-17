using System;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Flag for properties of transition rules
    /// </summary>
    [Flags]
    public enum TransitionRuleFlags
    {
        /// <summary>
        ///     Flag for physically meaningless rules
        /// </summary>
        PhysicallyInvalid = 1
    }
}