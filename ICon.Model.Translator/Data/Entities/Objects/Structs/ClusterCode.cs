using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Wrapped 64 bit unsigned integer multiple particle code that describes an occupation of 1 to 8 positions
    /// </summary>
    public readonly struct ClusterCode
    {
        /// <summary>
        /// The unsigned long code to identify the cluster based on particle indexing
        /// </summary>
        public ulong Code { get; }

        /// <summary>
        /// Create new cluster code from unigned integer
        /// </summary>
        /// <param name="code"></param>
        /// <param name="energy"></param>
        public ClusterCode(ulong code) : this()
        {
            Code = code;
        }

        /// <summary>
        /// Creates new cluster code from the first 8 entries of a byte array
        /// </summary>
        /// <param name="bytes"></param>
        public ClusterCode(byte[] bytes)
        {
            Code = BitConverter.ToUInt64(bytes, 0);
        }
    }
}
