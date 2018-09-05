using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator
{
    public class MmcJobHeader : InteropObjectBase<C_MmcJobHeader>
    {
        public MmcJobHeader()
        {
        }

        public MmcJobHeader(C_MmcJobHeader structure) : base(structure)
        {
        }
    }
}
