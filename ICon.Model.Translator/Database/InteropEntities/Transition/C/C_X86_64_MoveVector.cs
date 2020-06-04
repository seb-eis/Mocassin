using System.Runtime.InteropServices;
using Mocassin.Mathematics.ValueTypes;
#pragma warning disable 1591

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation move vector object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 24)]
    public struct C_X86_64_MoveVector
    {
        public C_X86_64_Vector3D Vector { get; set; }

        public C_X86_64_MoveVector(IVector3D vector)
            : this()
        {
            Vector = new C_X86_64_Vector3D(vector);
        }
    }
}