using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.Data
{
    public class ClusterDefinition : InteropObject<C_ClusterDefinition>
    {
        public ClusterDefinition()
        {
        }

        public ClusterDefinition(C_ClusterDefinition structure) : base(structure)
        {
        }
    }
}
