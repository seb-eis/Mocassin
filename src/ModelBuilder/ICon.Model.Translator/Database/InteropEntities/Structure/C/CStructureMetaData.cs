using System;
using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation structure meta data Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 608)]
    public struct CStructureMetaData
    {
        /// <summary>
        ///     The array for <see cref="Model.Particles.IParticle" /> charges for each index
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        private double[] particleCharges;

        /// <summary>
        ///     The normalized electric field vector
        /// </summary>
        public CVector3 NormalizedElectricFieldVector { get; set; }

        /// <summary>
        ///     The cell vector for direction A in units of [Ang]
        /// </summary>
        public CVector3 UnitCellVectorA { get; set; }

        /// <summary>
        ///     The cell vector for direction B in units of [Ang]
        /// </summary>
        public CVector3 UnitCellVectorB { get; set; }

        /// <summary>
        ///     The cell vector for direction C in units of [Ang]
        /// </summary>
        public CVector3 UnitCellVectorC { get; set; }

        /// <summary>
        ///     Get or set the cluster id encoding by <see cref="CPairInteraction" /> objects
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