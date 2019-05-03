﻿using Mocassin.Model.Particles;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using Mocassin.Framework.Collections;

using Mocassin.Framework.Random;
using Mocassin.Mathematics.Comparers;
using System.Linq;

using Mocassin.Framework.Extensions;

namespace Mocassin.Model.Lattices
{
	/// <summary>
	/// Executes dopings on the Worklattice
	/// </summary>
	/// <remarks>
	/// The doping executer applies provided dopings with concentrations to a provided lattice. The lattice is a multidimensional
	/// jagged array of LatticeEntries and has the structure of unitcells ordered in a supercell. The user may define simutaneous and
	/// subsequental doping, thus a doping order is calculated and applied successively. For the doping process itself position pools are
	/// first generated by iterating through the lattice and ordered in a dictionary. Then the particle count of dopant 
	/// and counter dopant are calculated.Finally random entries are selected from the  corresponding position pools and their occupation 
	/// altered. By this the entries in the provided lattice are also altered. The altered entries are subjequently removed from the position pool.
	/// </remarks>
	public class DopingExecuter
	{
		/// <summary>
		/// Contains information about positions which can be doped and the original number of these
		/// </summary>
		protected class PositionPool
		{
			public int OriginalSize { get; set; }
			public List<LatticeEntry> Entries { get; set; }
		}

		/// <summary>
		/// Double comparer
		/// </summary>
		NumericRangeComparer Comparer { get; }

		/// <summary>
		/// Doping tolerance value
		/// </summary>
		double DopingTolerance { get; }

		/// <summary>
		/// Random generator
		/// </summary>
		public Random RandomGenerator { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="doubleCompareTolerance"></param>
		/// <param name="dopingTolerance"></param>
		/// <param name="randomGenerator"></param>
		public DopingExecuter(double doubleCompareTolerance, double dopingTolerance, Random randomGenerator)
		{
			Comparer = new NumericRangeComparer(doubleCompareTolerance);
			DopingTolerance = dopingTolerance;
			RandomGenerator = randomGenerator;
		}

		/// <summary>
		/// Execute the doping process for multiple dopings to a worklattice
		/// </summary>
		/// <param name="lattice"></param>
		/// <param name="dopings"></param>
		public void DopeLattice(LatticeEntry[,,][] lattice, ReadOnlyListAdapter<IDoping> dopings,
			IDictionary<IDoping, double> dopingConcentrations)
		{
			var orderedDopings = GenerateDopingOrder(dopings);

			foreach (var dopingID in (orderedDopings.Keys.OrderBy(x => x)))
			{
				var dopingPool = GeneratePositionPool(lattice);

				foreach (var doping in orderedDopings[dopingID])
				{
					Execute(doping, dopingConcentrations[doping], dopingPool);
				}
			}

		}

		/// <summary>
		/// Generates the order of doping for simultaneous and consecutive dopings
		/// </summary>
		/// <param name="dopings"></param>
		/// <returns></returns>
		protected Dictionary<int, List<IDoping>> GenerateDopingOrder(ICollection<IDoping> dopings)
		{
			var orderedDopings = new Dictionary<int, List<IDoping>>();
			foreach (var doping in dopings)
			{
				if (orderedDopings.ContainsKey(doping.Priority) == false)
				{
					orderedDopings[doping.Priority] = new List<IDoping>();
				}

				orderedDopings[doping.Priority].Add(doping);
			}

			return orderedDopings;
		}

		/// <summary>
		/// Generate a position pool for every occuring LatticeEntry
		/// </summary>
		/// <param name="lattice"></param>
		/// <returns></returns>
		protected Dictionary<LatticeEntry, PositionPool> GeneratePositionPool(LatticeEntry[,,][] lattice)
		{
			var dopingPoolDict = new Dictionary<LatticeEntry, PositionPool>();

			foreach (var cell in lattice)
			{
				foreach (var entry in cell)
				{
					if (dopingPoolDict.ContainsKey(entry) == false)
					{
						dopingPoolDict[entry.ShallowCopy()] = new PositionPool() {OriginalSize = 0, Entries = new List<LatticeEntry>()};
					}

					dopingPoolDict[entry].OriginalSize += 1;
					dopingPoolDict[entry].Entries.Add(entry);
				}
			}

			return dopingPoolDict;
		}

		/// <summary>
		/// Execute the doping process
		/// </summary>
		/// <param name="lattice"></param>
		/// <param name="doping"></param>
		protected void Execute(IDoping doping, double concentration, Dictionary<LatticeEntry, PositionPool> dopingPool)
		{
			LatticeEntry dopedCellEntry = new LatticeEntry()
			{
				Particle = doping.PrimaryDoping.Dopable,
				CellPosition = doping.PrimaryDoping.UnitCellPosition,
				Block = doping.BuildingBlock
			};

			LatticeEntry counterDopedCellEntry = new LatticeEntry()
			{
				Particle = doping.CounterDoping.Dopable,
				CellPosition = doping.CounterDoping.UnitCellPosition,
				Block = doping.BuildingBlock
			};

			if (doping.UseCounterDoping == false)
			{
				int dopingParticleCount = (int) Math.Round(dopingPool[dopedCellEntry].OriginalSize * concentration);

				ApplyDoping(dopingPool[dopedCellEntry].Entries,
					dopingParticleCount,
					doping.PrimaryDoping);
			}
			else
            {
				(int, int) dopingParticleCount = CalculateDopingCount(
													dopingPool[dopedCellEntry].OriginalSize,
													doping,
													concentration);

				ApplyDoping(dopingPool[dopedCellEntry].Entries,
					dopingParticleCount.Item1,
					doping.PrimaryDoping);

				ApplyDoping(dopingPool[counterDopedCellEntry].Entries,
					dopingParticleCount.Item2,
					doping.CounterDoping);
			}
		}

		/// <summary>
		/// Apply Doping to the PositionPool (and therefore to the WorkLattice)
		/// </summary>
		/// <param name="dopableCellEntries"></param>
		/// <param name="dopingParticleCount"></param>
		/// <param name="doping"></param>
		protected void ApplyDoping(List<LatticeEntry> dopableCellEntries, int dopingParticleCount, IDopingCombination doping)
		{
			List<int> dopedEntriesIndex = dopableCellEntries.SelectRandomIndex(dopingParticleCount, RandomGenerator).ToList();

			for (int index = dopedEntriesIndex.Count - 1; index >= 0; index--)
			{
				dopableCellEntries[dopedEntriesIndex[index]].Particle = doping.Dopant;
				dopableCellEntries.RemoveAt(dopedEntriesIndex[index]);
			}
		}

		/// <summary>
		/// Calculate the number of counter dopants
		/// </summary>
		/// <param name="dopableCellEntriesCount"></param>
		/// <param name="doping"></param>
		/// <returns></returns>
		protected (int, int) CalculateDopingCount(int dopableEntries, IDoping doping, double concentration)
		{
			// calculate the difference between the atom and the dopand for the doping and counter doping
			double deltaCharge = Math.Abs(doping.PrimaryDoping.Dopant.Charge - doping.PrimaryDoping.Dopable.Charge);
			double deltaCounterCharge = Math.Abs(doping.CounterDoping.Dopant.Charge - doping.CounterDoping.Dopable.Charge);

			// now calculate the number of atoms which have to be replaced. Tries to avoid charging the lattice in the process,
			// but also to not deviate from the specified concentration. For the former condition the dopand number has
			// to be dividable by the counter doping charge
			int dopingNumber = (int) Math.Round(dopableEntries * concentration);
			int correctedDopingNumber = (int) Math.Round(CalculateClosestDividable(dopingNumber, deltaCounterCharge));
			if ((dopingNumber - correctedDopingNumber) / (double) dopingNumber > DopingTolerance)
			{
				correctedDopingNumber = dopingNumber;
			}

			int counterDopingNumber = correctedDopingNumber * (int) Math.Round(deltaCharge / deltaCounterCharge);
			return (correctedDopingNumber, counterDopingNumber);
		}

		protected double CalculateClosestDividable(int number, double denominator)
		{
			return number + (denominator - (number % denominator));
		}
	}
}
