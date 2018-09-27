using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Interop object wrapper for the pair definition type used in the unmanaged simulation
    /// </summary>
    public class PairDefinition : InteropObject<CPairDefinition>
    {
        public PairDefinition()
        {
        }

        public PairDefinition(CPairDefinition structure) : base(structure)
        {
        }
    }
}
