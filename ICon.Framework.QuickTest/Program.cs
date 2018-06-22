using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Linq;
using ICon.Mathematics;
using ICon.Mathematics.Coordinates;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using ICon.Mathematics.Permutation;
using ICon.Mathematics.Extensions;
using ICon.Mathematics.Comparers;
using ICon.Mathematics.Solver;
using ICon.Symmetry.SpaceGroups;
using ICon.Framework.Extensions;
using ICon.Mathematics.ValueTypes;
using ICon.Mathematics.Bitmasks;
using ICon.Framework.Messaging;
using ICon.Framework.Xml;
using ICon.Framework.Collections;
using ICon.Symmetry.CrystalSystems;
using ICon.Model.ProjectServices;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.Particles;
using ICon.Model.Structures;
using ICon.Model.Energies;
using ICon.Framework.Processing;
using ICon.Symmetry.Analysis;
using ICon.Mathematics.Constraints;
using ICon.Framework.Reflection;
using ICon.Model.Transitions;
using ICon.Framework.Random;
using System.Linq;
using ICon.Model.DataManagement;

namespace ICon.Framework.QuickTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var package = ManagerFactory.DebugFactory.CreateFullManagementSystem();
            var inputter = ManagerFactory.DebugFactory.MakeCeriaDataInputter();
            inputter.AutoInputData(package.ProjectServices);
            var report = inputter.GetReportJson();

            var center = package.StructureManager.QueryPort.Query(port => port.GetUnitCellPosition(0));
            var geometry = new List<Fractional3D> { new Fractional3D(-.25, -.25, -.25), new Fractional3D(.25, .25, .25) };

            var operations = package.ProjectServices.SpaceGroupService.GetPointOperationGroup(center.Vector, geometry);

            var permProvider = new SlotMachinePermuter<int>(new int[] { 1, 2, 3 }, new int[] { 1, 2, 3 });

            var unSeq = operations.GetUniqueSequenceOperations().Select(op => op.ApplyUntrimmed(geometry).ToList()).ToList();
            var unPoint = operations.GetSelfProjectionOperations().Select(op => op.ApplyUntrimmed(geometry).ToList()).ToList();
            var symIndexing = operations.GetProjectionIndexing().ToList();

            Console.ReadLine();
        }

        static void DisplayWatch(Stopwatch watch)
        {
            watch.Stop();
            Console.WriteLine("Watch Dump: {0}", watch.Elapsed.ToString());
            watch.Reset();
            watch.Start();
        }
    }
}
