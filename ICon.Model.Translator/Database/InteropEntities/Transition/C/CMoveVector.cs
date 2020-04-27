using System.Runtime.InteropServices;
using Mocassin.Mathematics.ValueTypes;
#pragma warning disable 1591

namespace Mocassin.Model.Translator
{
    /// <summary>
    ///     Simulation Vector 3 object. Layout marshals to its binary unmanaged 'C' representation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 24)]
    public struct CMoveVector
    {
        public CVector3 Vector { get; set; }

        public CMoveVector(IVector3D vector)
            : this()
        {
            Vector = new CVector3(vector);
        }
    }
}