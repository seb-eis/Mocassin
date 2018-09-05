using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Interop object wrapper for the vector 4 type used in the unmanaged simulation
    /// </summary>
    public class Vector4 : InteropObjectBase<C_Vector4>
    {
        public Vector4(C_Vector4 structure) : base(structure)
        {

        }

        protected Vector4()
        {
        }
    }
}
