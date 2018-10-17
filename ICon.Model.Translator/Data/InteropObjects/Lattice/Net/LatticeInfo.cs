using System;
using System.Collections.Generic;
using System.Text;

namespace Mocassin.Model.Translator
{
    public class LatticeInfo : InteropObject<CLatticeInfo>
    {
        public LatticeInfo()
        {
        }

        public LatticeInfo(CLatticeInfo structure) : base(structure)
        {
        }
    }
}
