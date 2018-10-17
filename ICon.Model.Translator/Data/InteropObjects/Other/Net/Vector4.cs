using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Mocassin.Model.Translator
{
    /// <summary>
    /// Interop object wrapper for the vector 4 type used in the unmanaged simulation
    /// </summary>
    public class Vector4 : InteropObject<CVector4>
    {
        public Vector4(CVector4 structure) : base(structure)
        {

        }

        protected Vector4()
        {
        }
    }
}
