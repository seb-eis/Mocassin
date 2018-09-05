using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator
{
    public class LatticeInfo : InteropObjectBase<C_LatticeInfo>
    {
        public LatticeInfo()
        {
        }

        public LatticeInfo(C_LatticeInfo structure) : base(structure)
        {
        }
    }
}
