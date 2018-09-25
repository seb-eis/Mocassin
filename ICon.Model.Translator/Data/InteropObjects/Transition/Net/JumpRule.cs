using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator
{
    public class JumpRule : InteropObject<C_JumpRule>
    {
        public JumpRule()
        {
        }

        public JumpRule(C_JumpRule structure) : base(structure)
        {
        }
    }
}
