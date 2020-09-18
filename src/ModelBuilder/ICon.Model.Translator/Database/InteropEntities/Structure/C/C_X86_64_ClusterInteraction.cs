using System;
using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation cluster definition object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 36)]
    public struct C_X86_64_ClusterInteraction
    {
        /// <summary>
        ///     Backing field for cluster id encoding by <see cref="C_X86_64_PairInteraction" /> objects
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        private int[] pairInteractionIds;

        /// <summary>
        ///     The affiliated <see cref="ClusterEnergyTableEntity" /> object id
        /// </summary>
        [field: MarshalAs(UnmanagedType.I4)]
        public int ClusterTableId { get; set; }

        /// <summary>
        ///     Get or set the cluster id encoding by <see cref="C_X86_64_PairInteraction" /> objects
        /// </summary>
        public int[] RelativePositionIds
        {
            get => pairInteractionIds;
            set
            {
                if ((value?.Length ?? throw new ArgumentNullException(nameof(value))) != 8)
                    throw new ArgumentException("Array has to be of size 8", nameof(value));
                pairInteractionIds = value;
            }
        }
    }
}