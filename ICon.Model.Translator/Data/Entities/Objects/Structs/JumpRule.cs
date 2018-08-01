using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Jump rule struct that describes the behavior and physical properties of a specific jump starting configuration in an integer encoded format
    /// </summary>
    public readonly struct JumpRule
    {
        /// <summary>
        /// The jump path occupation code for the initial state
        /// </summary>
        public ulong StateCode0 { get; }

        /// <summary>
        /// The jump path occupation code for the transition state
        /// </summary>
        public ulong StateCode1 { get; }

        /// <summary>
        /// The jump path occupation code for the final state
        /// </summary>
        public ulong StateCode2 { get; }

        /// <summary>
        /// The jump attempt rate for jump success normalization
        /// </summary>
        public double AttemptRate { get; }

        /// <summary>
        /// The elctric field factor in range [-1;1]. Value is based upon magnitude and prefix of effective charge transport
        /// </summary>
        public double FieldFactor { get; }

        /// <summary>
        /// Tracking reorder code. Describes the new index order of the involved dynamic trackers as 8 x byte indices
        /// </summary>
        public ulong ReorderCode { get; }
    }
}
