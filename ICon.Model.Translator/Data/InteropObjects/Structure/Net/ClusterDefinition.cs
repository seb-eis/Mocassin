using System;
using System.Collections.Generic;
using System.Text;

namespace Mocassin.Model.Translator.Data
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
