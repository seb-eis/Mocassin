using ICon.Framework.Collections;
using ICon.Model.Particles;
using System;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Random;
using ICon.Mathematics.Comparers;
using System.Linq;

using ICon.Framework.Extensions;

namespace ICon.Model.Lattices
{
    public class DopingPool
    {
        public int OriginalSize { get; set; }
        public List<CellEntry> Entries { get; set; }
    }

    /// <summary>
    /// Executes dopings on the Worklattice
    /// </summary>
    public class DopingExecuter
    {
        DoubleRangeComparer Comparer { get; }
        double DopingTolerance { get; }

        public DopingExecuter(double doubleCompareTolerance, double dopingTolerance)
        {
            Comparer = new DoubleRangeComparer(doubleCompareTolerance);
            DopingTolerance = dopingTolerance;
        }



        /// <summary>
        /// Execute the doping process for multiple dopings to a worklattice
        /// </summary>
        /// <param name="lattice"></param>
        /// <param name="dopings"></param>
        public void ExecuteMultiple(CellEntry[,,][] lattice, IDictionary<IDoping, double> dopingsAndConcentration)
        {
            var dopingDict = new Dictionary<int, List<IDoping>>();
            foreach(var doping in dopingsAndConcentration.Keys)
            {
                if(dopingDict.ContainsKey(doping.DopingGroup) == false)
                {
                    dopingDict[doping.DopingGroup] = new List<IDoping>();
                }
                dopingDict[doping.DopingGroup].Add(doping);
            }

            foreach (var dopingID in (dopingDict.Keys.OrderBy(x => x)))
            {
                var dopingPool = GenerateDopingPool(lattice);

                foreach(var doping in dopingDict[dopingID])
                {
                    Execute(doping, dopingsAndConcentration[doping], dopingPool);
                }
            }
        }

        public Dictionary<CellEntry, DopingPool> GenerateDopingPool(CellEntry[,,][] lattice)
        {
            var dopingPoolDict = new Dictionary<CellEntry, DopingPool>();

            foreach (var cell in lattice)
            {
                foreach(var entry in cell)
                {
                    if (dopingPoolDict.ContainsKey(entry) == false)
                    {
                        dopingPoolDict[entry] = new DopingPool() { OriginalSize = 0, Entries = new List<CellEntry>() };
                    }

                    dopingPoolDict[entry].OriginalSize += 1;
                    dopingPoolDict[entry].Entries.Add(entry);
                }
            }

            return dopingPoolDict;
        }

        /// <summary>
        /// Update original occupation with current occupation for next grouped doping process
        /// </summary>
        /// <param name="lattice"></param>
        public void UpdateOriginalOccupations(WorkCell[,,] lattice)
        {
            foreach (var cell in lattice)
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
        public void Execute(IDoping doping, double concentration, Dictionary<CellEntry, DopingPool> dopingPool)
        {
            CellEntry dopedCellEntry = new CellEntry()
            {   Particle = doping.DopingInfo.DopedParticle,
                CellPosition = doping.DopingInfo.UnitCellPosition,
                Block = doping.DopingInfo.BuildingBlock,
            };

            CellEntry counterDopedCellEntry = new CellEntry()
            {
                Particle = doping.CounterDopingInfo.DopedParticle,
                CellPosition = doping.CounterDopingInfo.UnitCellPosition,
                Block = doping.CounterDopingInfo.BuildingBlock,
            };

            (int,int) dopingParticleCount = CalculateDopingCount(dopingPool[dopedCellEntry].OriginalSize, dopingPool[counterDopedCellEntry].OriginalSize, doping, concentration);

            ApplyDoping(dopingPool[dopedCellEntry].Entries, dopingParticleCount.Item1, doping.DopingInfo);

            ApplyDoping(dopingPool[counterDopedCellEntry].Entries, dopingParticleCount.Item2, doping.CounterDopingInfo);
        }

        /// <summary>
        /// Get all cellentries which can be doped and are not doped yet
        /// </summary>
        /// <param name="lattice"></param>
        /// <param name="doping"></param>
        /// <returns></returns>
        protected (int, List<CellEntry>) GetDopableCellEntries(WorkCell[,,] lattice, IDopingCombination doping)
        {
            int count = 0;
            List<CellEntry> cellEntries = new List<CellEntry>();

            foreach (var cell in lattice)
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
            var dopedEntries = dopableCellEntries.SelectRandom(dopingParticleCount, new PcgRandom32());

            foreach (var item in dopedEntries)
            {
                item.Particle = doping.Dopant;
                dopableCellEntries.Remove(item);
            }
        }

        /// <summary>
        /// Calculate the number of counter dopants
        /// </summary>
        /// <param name="dopableCellEntriesCount"></param>
        /// <param name="doping"></param>
        /// <returns></returns>
        public (int,int) CalculateDopingCount(int dopableCount, int counterDopableCount, IDoping doping, double concentration)
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

            int dopandCount = (int)(Math.Round(dopableCount * concentration));

            if (doping.UseCounterDoping == false)
            {
                return (dopandCount, 0);
            }

            double counterDopandCount = dopandCount * counterDopingMultiplier;

            if ( Comparer.Compare(counterDopandCount % 1, 0) == 0)
            {
                return (dopandCount, (int)counterDopandCount);
            }

            for (int i = -(int)(dopandCount*DopingTolerance); i <= (int)(dopandCount * DopingTolerance); i++)
            {
                counterDopandCount = (dopandCount+i) * counterDopingMultiplier;

                if (Comparer.Compare(counterDopandCount % 1, 0) == 0)
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

            if (Comparer.Compare((DeltaChargeDopant - DeltaChargeCounterDopant),0) == 0)
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
