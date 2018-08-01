using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator
{
    /// <summary>
    /// The cluster interaction struct that describes a cluster by two 64 bit position index code blocks and a cluster energy table index
    /// </summary>
    public readonly struct ClusterInteraction
    {
        /// <summary>
        /// The first position index code block. Contains the postitions 0-3 as signed short indexing
        /// </summary>
        public long CodeBlock0 { get; }

        /// <summary>
        /// The second position index code block. Contains the positions 4-7 as signed short indexing
        /// </summary>
        public long CodeBlock1 { get; }

        /// <summary>
        /// The cluster table index for energy lookup
        /// </summary>
        public int ClusterTableIndex { get; }

        /// <summary>
        /// Creates new cluster interaction from two code blocks and a cluster table index
        /// </summary>
        /// <param name="codeBlock0"></param>
        /// <param name="codeBlock1"></param>
        /// <param name="clusterTableIndex"></param>
        public ClusterInteraction(long codeBlock0, long codeBlock1, int clusterTableIndex) : this()
        {
            CodeBlock0 = codeBlock0;
            CodeBlock1 = codeBlock1;
            ClusterTableIndex = clusterTableIndex;
        }

        /// <summary>
        /// Create a new cluster interaction from integer position indexing array and a cluster table index
        /// </summary>
        /// <param name="indexing"></param>
        /// <param name="clusterTableIndex"></param>
        /// <returns></returns>
        public static ClusterInteraction FromArray(int[] indexing, int clusterTableIndex)
        {
            if (indexing.Length != 8)
            {
                throw new ArgumentException("Position indexing array has to be of length 8", nameof(indexing));
            }
            long block0 = 0, block1 = 0;
            for (int i = 0; i < 4; i++)
            {
                block0 += ((short)indexing[i]) << (i * 16);
                block1 += ((short)indexing[i + 4]) << (i * 16);
            }
            return new ClusterInteraction(block0, block1, clusterTableIndex);
        }
    }
}
