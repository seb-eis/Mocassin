using ICon.Framework.Collections;
using ICon.Model.Particles;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Executes dopings on the Worklattice
    /// </summary>
    public class DopingExecuter
    {
        /// <summary>
        /// Execute the doping process for multiple dopings to a worklattice
        /// </summary>
        /// <param name="lattice"></param>
        /// <param name="dopings"></param>
        public void ExecuteMultible(WorkLattice lattice, ReadOnlyList<IDoping> dopings)
        {
            foreach (var doping in dopings)
            {
                Execute(lattice, doping);
            }
        }

        /// <summary>
        /// Execute the doping process for a single doping to a worklattice
        /// </summary>
        /// <param name="lattice"></param>
        /// <param name="doping"></param>
        public void Execute(WorkLattice lattice, IDoping doping)
        {
            List<CellEntry> dopableCellEntries = GetDopableCellEntries(lattice, doping.DopingInfo);

            int dopingParticleCount = CalculateDopantCount(dopableCellEntries.Count, doping);

            ApplyDoping(dopableCellEntries, dopingParticleCount, doping.DopingInfo);



            List<CellEntry> dopableCounterCellEntries = GetDopableCellEntries(lattice, doping.CounterDopingInfo);

            int counterDopingCount = CalculateCounterDopantCount(dopableCellEntries.Count, doping);

            ApplyDoping(dopableCounterCellEntries, counterDopingCount, doping.CounterDopingInfo);
        }

        /// <summary>
        /// Get All cellentries that can be doped
        /// </summary>
        /// <param name="lattice"></param>
        /// <param name="doping"></param>
        /// <returns></returns>
        protected List<CellEntry> GetDopableCellEntries(WorkLattice lattice, IDopingCombination doping)
        {
            List<CellEntry> cellEntries = new List<CellEntry>();

            foreach (var cell in lattice.WorkCells)
            {
                if (cell.BuildingBlockID != doping.BuildingBlock.Index)
                {
                    continue;
                }

                foreach (var cellEntry in cell.CellEntries)
                {
                    if (cellEntry.Particle == doping.DopedParticle && cellEntry.CellPosition.Index == doping.UnitCellPosition.Index)
                    {
                        cellEntries.Add(cellEntry);
                    }
                }
            }

            return cellEntries;
        }

        /// <summary>
        /// Apply Doping to the worklattice
        /// </summary>
        /// <param name="dopableCellEntries"></param>
        /// <param name="dopingParticleCount"></param>
        /// <param name="doping"></param>
        protected void ApplyDoping(List<CellEntry> dopableCellEntries, int dopingParticleCount, IDopingCombination doping)
        {
            Action<CellEntry> applyDoping = (entry) => { entry.Particle = doping.Dopant; };

            (new UniquePoolSampler<CellEntry>()).ApplyToSamples(dopableCellEntries, Convert.ToUInt32(dopingParticleCount), applyDoping);
        }

        /// <summary>
        /// Calculate the number of dopants
        /// </summary>
        /// <param name="dopableCellEntriesCount"></param>
        /// <param name="doping"></param>
        /// <returns></returns>
        protected int CalculateDopantCount(int dopableCellEntriesCount, IDoping doping)
        {
            return Convert.ToInt32(Math.Round(dopableCellEntriesCount * doping.Concentration));
        }

        /// <summary>
        /// Calculate the number of counter dopants
        /// </summary>
        /// <param name="dopableCellEntriesCount"></param>
        /// <param name="doping"></param>
        /// <returns></returns>
        protected int CalculateCounterDopantCount(int dopableCellEntriesCount, IDoping doping)
        {
            if (doping.CounterDopingInfo.Dopant.Equals(doping.CounterDopingInfo.DopedParticle))
            {
                return 0;
            }

            double chargeMultiplier;

            double DeltaChargeDopant = doping.DopingInfo.Dopant.Charge - doping.DopingInfo.DopedParticle.Charge;
            double DeltaChargeCounterDopant = doping.CounterDopingInfo.Dopant.Charge - doping.CounterDopingInfo.DopedParticle.Charge;

            if (DeltaChargeDopant == DeltaChargeCounterDopant)
            {
                chargeMultiplier = -1;
            }
            else
            {
                if (DeltaChargeDopant == 0 || DeltaChargeCounterDopant == 0 )
                {
                    throw new ArgumentException("DopingExecuter", "Inconsistent doping! Only one doping is charge neutral!");
                }

                chargeMultiplier = DeltaChargeDopant / DeltaChargeCounterDopant;
            }

            if (chargeMultiplier > 0)
            {
                throw new ArgumentException("DopingExecuter", "Inconsistent doping! Counter doping cannot compensate doping!");
            }

            return Convert.ToInt32(Math.Round(Math.Abs(dopableCellEntriesCount * doping.Concentration * chargeMultiplier)));
        }
    }


}
