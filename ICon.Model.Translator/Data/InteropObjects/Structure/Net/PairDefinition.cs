using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Interop object wrapper for the pair definition type used in the unmanaged simulation
    /// </summary>
    public class PairDefinition : InteropObject<C_PairDefinition>
    {
        public PairDefinition()
        {
        }

        public PairDefinition(C_PairDefinition structure) : base(structure)
        {
        }
    }
}
