using System;
using System.Collections.Generic;
using System.Text;

namespace Mocassin.Model.Translator
{
    public class JumpRule : InteropObject<CJumpRule>
    {
        public JumpRule()
        {
        }

        public JumpRule(CJumpRule structure) : base(structure)
        {
        }
    }
}
