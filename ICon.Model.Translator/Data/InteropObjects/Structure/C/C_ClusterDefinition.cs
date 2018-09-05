using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Simulation cluster definition object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 9 * sizeof(int))]
    public struct C_ClusterDefinition
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8 * sizeof(int))]
        private int[] relativePositionIds;

        [MarshalAs(UnmanagedType.I4)]
        private int tableId;

        public int[] RelativePositionIds
        {
            get => relativePositionIds;
            set
            {
                if ((value?.Length ?? throw new ArgumentNullException(nameof(value))) != 8)
                {
                    throw new ArgumentException("Array has to be of size 8", nameof(value));
                }
                relativePositionIds = value;
            }
        }

        public int TableId { get => tableId; set => tableId = value; }
    }
}
