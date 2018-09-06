using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator
{
    public class KmcJobHeader : InteropObject<C_KmcJobHeader>
    {
        public KmcJobHeader()
        {
        }

        public KmcJobHeader(C_KmcJobHeader structure) : base(structure)
        {
        }
    }
}
