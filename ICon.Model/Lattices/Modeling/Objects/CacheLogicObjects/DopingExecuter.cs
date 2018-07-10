using ICon.Framework.Collections;
using ICon.Model.Particles;
using System;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Random;
using ICon.Mathematics.Comparers;
using System.Linq;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Executes dopings on the Worklattice
    /// </summary>
    public class DopingExecuter
    {
        DoubleRangeComparer comparer;

        public DopingExecuter(double doubleCompareTolerance)
        {
            comparer = new DoubleRangeComparer(doubleCompareTolerance);
        }

        /// <summary>
        /// Execute the doping process for multiple dopings to a worklattice
        /// </summary>
        /// <param name="lattice"></param>
        /// <param name="dopings"></param>
        public void ExecuteMultible(WorkLattice lattice, ReadOnlyList<IDoping> dopings, double dopingTolerance)
        {
            var dopingDict = new Dictionary<int, List<IDoping>>();
            foreach(var doping in dopings)
            {
                if(dopingDict.ContainsKey(doping.DopingGroup) == false)
                {
                    dopingDict[doping.DopingGroup] = new List<IDoping>();
                }
                dopingDict[doping.DopingGroup].Add(doping);
            }

            foreach (var dopingID in (dopingDict.Keys.OrderBy(x => x)))
            {
                foreach(var doping in dopingDict[dopingID])
                {
                    Execute(lattice, doping, dopingTolerance);
                }
                UpdateOriginalOccupations(lattice);
            }
        }

        /// <summary>
        /// Update original occupation with current occupation for next grouped doping process
        /// </summary>
        /// <param name="lattice"></param>
        public void UpdateOriginalOccupations(WorkLattice lattice)
        {
            foreach (var cell in lattice.WorkCells)
            {
                foreach (var entry in cell.CellEntries)
                {
                    entry.OriginalOccupation = entry.Particle;
                }
            }
        }

        /// <summary>
        /// Execute the doping process for a single doping to a worklattice
        /// </summary>
        /// <param name="lattice"></param>
        /// <param name="doping"></param>
        public void Execute(WorkLattice lattice, IDoping doping, double dopingTolerance)
        {
            (int, List<CellEntry>) dopableCellEntries = GetDopableCellEntries(lattice, doping.DopingInfo);

            (int, List<CellEntry>) dopableCounterCellEntries = GetDopableCellEntries(lattice, doping.CounterDopingInfo);

            (int,int) dopingParticleCount = CalculateDopingCount(dopableCellEntries.Item1, dopableCounterCellEntries.Item1, doping, dopingTolerance);

            ApplyDoping(dopableCellEntries.Item2, dopingParticleCount.Item1, doping.DopingInfo);

            ApplyDoping(dopableCounterCellEntries.Item2, dopingParticleCount.Item2, doping.CounterDopingInfo);
        }

        /// <summary>
        /// Get the number of positions which can be doped
        /// </summary>
        /// <param name="lattice"></param>
        /// <param name="doping"></param>
        /// <returns></returns>
        public int GetDopableCellEntryCount(WorkLattice lattice, IDopingCombination doping)
        {
            int count = 0;

            foreach (var cell in lattice.WorkCells)
            {
                if (cell.BuildingBlockID != doping.BuildingBlock.Index)
                {
                    continue;
                }

                foreach (var cellEntry in cell.CellEntries)
                {
                    if (cellEntry.OriginalOccupation == doping.DopedParticle && cellEntry.CellPosition.Index == doping.UnitCellPosition.Index)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Get all cellentries which can be doped and are not doped yet
        /// </summary>
        /// <param name="lattice"></param>
        /// <param name="doping"></param>
        /// <returns></returns>
        protected (int, List<CellEntry>) GetDopableCellEntries(WorkLattice lattice, IDopingCombination doping)
        {
            int count = 0;
            List<CellEntry> cellEntries = new List<CellEntry>();

            foreach (var cell in lattice.WorkCells)
            {
                if (cell.BuildingBlockID != doping.BuildingBlock.Index)
                {
                    continue;
                }

                foreach (var cellEntry in cell.CellEntries)
                {

                    if (cellEntry.OriginalOccupation == doping.DopedParticle && 
                        cellEntry.CellPosition.Index == doping.UnitCellPosition.Index)
                    {
                        count++;
                    }

                    if (cellEntry.Particle == doping.DopedParticle && 
                        cellEntry.CellPosition.Index == doping.UnitCellPosition.Index &&
                        cellEntry.CellPosition == cellEntry.OriginalOccupation)
                    {
                        cellEntries.Add(cellEntry);
                    }
                }
            }

            return (count, cellEntries);
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
            return (int)(Math.Round(dopableCellEntriesCount * doping.Concentration));
        }

        /// <summary>
        /// Calculate the number of counter dopants
        /// </summary>
        /// <param name="dopableCellEntriesCount"></param>
        /// <param name="doping"></param>
        /// <returns></returns>
        public (int,int) CalculateDopingCount(int dopableCount, int counterDopableCount, IDoping doping, double dopingTolerance)
        {
            double counterDopingMultiplier;
            if (doping.UseCustomMultiplier == false)
            {
                counterDopingMultiplier = CalculateChargeBasedMultiplier(doping);
            }
            else
            {
                counterDopingMultiplier = doping.CounterDopingMultiplier;
            }

            int dopandCount = (int)(Math.Round(dopableCount * doping.Concentration));

            if (doping.UseCounterDoping == false)
            {
                return (dopandCount, 0);
            }

            double counterDopandCount = dopandCount * counterDopingMultiplier;

            if ( comparer.Compare(counterDopandCount % 1, 0) == 0)
            {
                return (dopandCount, (int)counterDopandCount);
            }

            for (int i = -(int)(dopandCount*dopingTolerance); i <= (int)(dopandCount * dopingTolerance); i++)
            {
                counterDopandCount = (dopandCount+i) * counterDopingMultiplier;

                if (comparer.Compare(counterDopandCount % 1, 0) == 0)
                {
                    return (dopandCount+i, (int)counterDopandCount);
                }
            }

            counterDopandCount = (Math.Round(dopandCount * counterDopingMultiplier));
            return (dopandCount, (int)counterDopandCount);


        }

        public double CalculateChargeBasedMultiplier(IDoping doping)
        {
            double DeltaChargeDopant = doping.DopingInfo.Dopant.Charge - doping.DopingInfo.DopedParticle.Charge;
            double DeltaChargeCounterDopant = doping.CounterDopingInfo.Dopant.Charge - doping.CounterDopingInfo.DopedParticle.Charge;

            if (comparer.Compare((DeltaChargeDopant - DeltaChargeCounterDopant),0) == 0)
            {
                return 1.0;
            }
            if (DeltaChargeDopant == 0 || DeltaChargeCounterDopant == 0)
            {
                throw new ArgumentException("DopingExecuter", "Inconsistent doping! Only one doping is charge neutral!");
            }

            return DeltaChargeDopant / DeltaChargeCounterDopant;
        }
    }


}
