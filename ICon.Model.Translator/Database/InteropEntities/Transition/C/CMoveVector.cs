using System.Runtime.InteropServices;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation Vector 3 object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 32)]
    public struct CMoveVector
    {
        public CVector3 Vector { get; set; }

        [MarshalAs(UnmanagedType.R8)]
        private readonly double paddingDouble;

        public CMoveVector(IVector3D vector)
            : this()
        {
            Vector = new CVector3(vector);
        }
    }
}