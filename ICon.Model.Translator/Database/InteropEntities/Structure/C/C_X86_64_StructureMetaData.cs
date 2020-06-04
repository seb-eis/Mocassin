using System;
using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation structure meta data Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 608)]
    public struct C_X86_64_StructureMetaData
    {
        /// <summary>
        ///     The array for <see cref="Model.Particles.IParticle" /> charges for each index
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        private double[] particleCharges;

        /// <summary>
        ///     The normalized electric field vector
        /// </summary>
        public C_X86_64_Vector3D NormalizedElectricFieldVector { get; set; }

        /// <summary>
        ///     The cell vector for direction A in units of [Ang]
        /// </summary>
        public C_X86_64_Vector3D UnitCellVectorA { get; set; }

        /// <summary>
        ///     The cell vector for direction B in units of [Ang]
        /// </summary>
        public C_X86_64_Vector3D UnitCellVectorB { get; set; }

        /// <summary>
        ///     The cell vector for direction C in units of [Ang]
        /// </summary>
        public C_X86_64_Vector3D UnitCellVectorC { get; set; }

        /// <summary>
        ///     Get or set the cluster id encoding by <see cref="C_X86_64_PairInteraction" /> objects
        /// </summary>
        public double[] ParticleCharges
        {
            get => particleCharges;
            set
            {
                if ((value?.Length ?? throw new ArgumentNullException(nameof(value))) != 64)
                    throw new ArgumentException("Array has to be of size 64", nameof(value));
                particleCharges = value;
            }
        }
    }
}