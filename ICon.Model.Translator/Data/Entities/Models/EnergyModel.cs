using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ICon.Model.Translator
{
    public class EnergyModel : InteropEntityBase
    {
        [ForeignKey(nameof(SimulationPackage))]
        public int SimulationPackageId { get; set; }

        public SimulationPackage SimulationPackage { get; set; }

        public List<PairEnergyTableEntity> PairEnergyTables { get; set; }

        public List<ClusterEnergyTableEntity> ClusterEnergyTables { get; set; }

        public int PairEnergyTableCount { get; protected set; }

        public int ClusterEnergyTableCount { get; protected set; }
    }
}
