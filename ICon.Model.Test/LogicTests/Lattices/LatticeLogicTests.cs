﻿using System;
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
using ICon.Framework.Extensions;

namespace ICon.Model.Test
{
    /// <summary>
    /// Lattice logic test class
    /// </summary>
    [TestClass]
    public class LatticeLogicTests
    {
        [TestMethod]
        public void TestDopingProcess()
        {
            var package = ManagerFactory.DebugFactory.CreateFullManagementSystem();
            var inputter = ManagerFactory.DebugFactory.MakeCeriaDataInputter();
            inputter.AutoInputData(package.ProjectServices);
            var report = inputter.GetReportJson();
            var project = inputter.JsonSerialize();

            var dopings = package.LatticeManager.QueryPort.Query(port => port.GetDopings());

            //var simulationSeries = new LatticeBlueprint()
            //{
            //    CustomRng = new PcgRandom32(),
            //    SizeVector = new VectorInt3D(16, 16, 16),
            //    DopingConcentrations = new Dictionary<IDoping, double>
            //    {
            //        { dopings[0], 0.2 },
            //        { dopings[1], 0.2 },
            //        { dopings[2], 0.2 },
            //        { dopings[3], 0.0 }
            //    }
            //};

            //var latticeCreationProvider = package.LatticeManager.QueryPort.Query(port => port.GetLatticeCreationProvider());

            //var lattice = latticeCreationProvider.ConstructLattice(latticeBlueprint);

            ////var lattice = package.LatticeManager.QueryPort.Query(port => port.CreateLattice());

            //Dictionary<string, int> elementCount = new Dictionary<string, int>();
            //for (int x = 0; x < lattice.CellSizeInfo.A; x++)
            //{
            //    for (int y = 0; y < lattice.CellSizeInfo.B; y++)
            //    {
            //        for (int z = 0; z < lattice.CellSizeInfo.C; z++)
            //        {
            //            for (int p = 0; p < lattice.CellSizeInfo.D; p++)
            //            {
            //                var elem = lattice.GetCellEntry(x, y, z, p).Entry.Name;
            //                if (elementCount.ContainsKey(elem))
            //                {
            //                    elementCount[elem]++;
            //                }
            //                else
            //                {
            //                    elementCount[elem] = 1;
            //                }
            //            }
            //        }
            //    }
            //}

            //Console.WriteLine();
        }
    }
}
