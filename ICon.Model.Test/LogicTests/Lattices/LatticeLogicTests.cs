using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;


using ICon.Model.Basic;
using ICon.Model.Lattices;
using System.Collections.Generic;
using ICon.Model.Particles;
using ICon.Framework.Collections;
using System.Collections.Immutable;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Structures;
using ICon.Symmetry.Analysis;
using ICon.Mathematics.Coordinates;
using ICon.Mathematics.Comparers;
using System.Linq;
using ICon.Framework.Random;
using ICon.Model.Simulations;

namespace ICon.Model.Test
{
    /// <summary>
    /// Lattice logic test class
    /// </summary>
    [TestClass]
    public class LatticeLogicTests
    {

        /// <summary>
        /// Generate lattice test
        /// </summary>
        [TestMethod]
        public void TestLatticeCreation()
        {
            var package = ManagerFactory.DebugFactory.CreateFullManagementSystem();
            var inputter = ManagerFactory.DebugFactory.MakeCeriaDataInputter();
            inputter.AutoInputData(package.ProjectServices);
            var report = inputter.GetReportJson();

            var lattice = package.LatticeManager.QueryPort.Query(port => port.CreateLattice());

            Dictionary<string, int> elementCount = new Dictionary<string, int>();
            for (int x = 0; x < lattice.CellSizeInfo.A; x++)
            {
                for (int y = 0; y < lattice.CellSizeInfo.B; y++)
                {
                    for (int z = 0; z < lattice.CellSizeInfo.C; z++)
                    {
                        for (int p = 0; p < lattice.CellSizeInfo.D; p++)
                        {
                            var elem = lattice.GetCellEntry(x, y, z, p).Entry.Name;
                            if (elementCount.ContainsKey(elem))
                            {
                                elementCount[elem]++;
                            }
                            else
                            {
                                elementCount[elem] = 1;
                            }
                        }
                    }
                }
            }

            Console.ReadLine();

        }

        [TestMethod]
        public void TestDopingProcess()
        {
            var package = ManagerFactory.DebugFactory.CreateFullManagementSystem();
            var inputter = ManagerFactory.DebugFactory.MakeCeriaDataInputter();
            inputter.AutoInputData(package.ProjectServices);
            var report = inputter.GetReportJson();
            var project = inputter.JsonSerialize();

            var dopings = package.LatticeManager.QueryPort.Query(port => port.GetDopings());

            var latticeBlueprint = new LatticeBlueprint()
            {
                CustomRng = new PcgRandom32(),
                SizeVector = new CartesianInt3D(10, 10, 10),
                DopingConcentrations = new Dictionary<IDoping, double>
                {
                    { dopings[0], 0.1 }
                }
            };

            var latticeCreationProvider = new LatticeCreationProvider();

            var lattice = latticeCreationProvider.ConstructCustomLattice(package, latticeBlueprint);

            //var lattice = package.LatticeManager.QueryPort.Query(port => port.CreateLattice());

            Dictionary<string, int> elementCount = new Dictionary<string, int>();
            for (int x = 0; x < lattice.CellSizeInfo.A; x++)
            {
                for (int y = 0; y < lattice.CellSizeInfo.B; y++)
                {
                    for (int z = 0; z < lattice.CellSizeInfo.C; z++)
                    {
                        for (int p = 0; p < lattice.CellSizeInfo.D; p++)
                        {
                            var elem = lattice.GetCellEntry(x, y, z, p).Entry.Name;
                            if (elementCount.ContainsKey(elem))
                            {
                                elementCount[elem]++;
                            }
                            else
                            {
                                elementCount[elem] = 1;
                            }
                        }
                    }
                }
            }

            Console.ReadLine();
        }

        [TestMethod]
        public void CompareToTest()
        {
            int i = 5;
            Console.WriteLine(i.CompareTo(64));
        }

        [TestMethod]
        public void HashTest()
        {
            var package = ManagerFactory.DebugFactory.CreateFullManagementSystem();
            var inputter = ManagerFactory.DebugFactory.MakeCeriaDataInputter();
            inputter.AutoInputData(package.ProjectServices);
            var report = inputter.GetReportJson();


            var particles = package.ParticleManager.QueryPort.Query(port => port.GetParticles());
            var unitCellEntries = package.StructureManager.QueryPort.Query(port => port.GetExtendedIndexToPositionDictionary());
            var buildingBlocks = package.LatticeManager.QueryPort.Query(port => port.GetBuildingBlocks());

            var cellEntry1 = new CellEntry()
            {
                Particle = particles[1],
                CellPosition = unitCellEntries[1],
                Block = buildingBlocks[0],
                IsDoped = false
            };

            var cellEntry2 = new CellEntry()
            {
                Particle = particles[1],
                CellPosition = unitCellEntries[1],
                Block = buildingBlocks[0],
                IsDoped = false
            };

            Console.WriteLine(cellEntry1.Equals(cellEntry2));
            Console.WriteLine(cellEntry1.GetHashCode().ToString());
            Console.WriteLine(cellEntry2.GetHashCode().ToString());

            var dict = new Dictionary<CellEntry, int>();
            dict[cellEntry1] = 0;
            dict[cellEntry2]++;


        }

    }
}
