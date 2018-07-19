using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Combines a unique 64 bit unsigned integer code and an energy value to encode a cluster permutation energy info
    /// </summary>
    public struct EnergyCode
    {
        /// <summary>
        /// The unsigned long code to identify the cluster based on particle indexing
        /// </summary>
        public ulong Code { get; }

        /// <summary>
        /// The energy that belongs to the cluster entry
        /// </summary>
        public double Energy { get; }

        /// <summary>
        /// Create new cluster entry from code and energy value
        /// </summary>
        /// <param name="code"></param>
        /// <param name="energy"></param>
        public EnergyCode(ulong code, double energy) : this()
        {
            Code = code;
            Energy = energy;
        }
    }
}
