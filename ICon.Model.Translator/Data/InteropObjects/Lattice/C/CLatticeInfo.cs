using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Mocassin.Model.Translator
{
    /// <summary>
    /// Simulation lattice info object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 72)]
    public struct CLatticeInfo
    {
        private CVector4 sizeVector;

        [MarshalAs(UnmanagedType.I4)]
        private int numberOfMobiles;

        [MarshalAs(UnmanagedType.I4)]
        private int numberOfSelectables;

        private readonly CMdaAccess latticeAccess;

        private readonly CMdaAccess backgroundAccess;

        [MarshalAs(UnmanagedType.I8)]
        private readonly long paddingLong;

        public CVector4 SizeVector { get => sizeVector; set => sizeVector = value; }

        public int NumberOfMobiles { get => numberOfMobiles; set => numberOfMobiles = value; }

        public int NumberOfSelectables { get => numberOfSelectables; set => numberOfSelectables = value; }
    }
}
