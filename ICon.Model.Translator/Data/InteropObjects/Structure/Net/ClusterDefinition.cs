using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.Data
{
    public class ClusterDefinition : InteropObject<CClusterDefinition>
    {
        public ClusterDefinition()
        {
        }

        public ClusterDefinition(CClusterDefinition structure) : base(structure)
        {
        }
    }
}
