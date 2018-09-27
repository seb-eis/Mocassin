using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator
{
    public class KmcJobHeader : InteropObject<CKmcJobHeader>
    {
        public KmcJobHeader()
        {
        }

        public KmcJobHeader(CKmcJobHeader structure) : base(structure)
        {
        }
    }
}
