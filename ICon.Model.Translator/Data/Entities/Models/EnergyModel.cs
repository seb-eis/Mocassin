﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Mocassin.Model.Translator
{
    public class EnergyModel : InteropEntityBase
    {
        private static IList<InteropStateChangeAction> stateChangeDelegates;

        protected override IList<InteropStateChangeAction> StateChangeActions
        {
            get => stateChangeDelegates;
            set => stateChangeDelegates = value;
        }

        public SimulationPackage SimulationPackage { get; set; }

        [Column("PackageId")]
        [ForeignKey(nameof(SimulationPackage))]
        public int SimulationPackageId { get; set; }

        [Column("NumOfPairTables")]
        public int PairEnergyTableCount { get; set; }

        [Column("NumOfClusterTables")]
        public int ClusterEnergyTableCount { get; set; }

        public List<PairEnergyTableEntity> PairEnergyTables { get; set; }

        public List<ClusterEnergyTableEntity> ClusterEnergyTables { get; set; }
    }
}
