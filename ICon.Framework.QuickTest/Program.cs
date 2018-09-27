﻿using System;
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
using ICon.Framework.Constraints;
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
using ICon.Framework.Provider;
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
using ICon.Model.Lattices;
using ICon.Framework.Random;
using System.Linq;
using ICon.Model.DataManagement;
using ICon.Model.Simulations;
using ICon.Model.Translator;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using ICon.Model.Translator.ModelContext;

namespace ICon.Framework.QuickTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var connections = new List<ConnectorType>
            {
                ConnectorType.Dynamic,
                ConnectorType.Dynamic,
                ConnectorType.Dynamic,
                ConnectorType.Dynamic
            };
            var ucps = new List<IUnitCellPosition>
            {
                new UnitCellPosition {Status = PositionStatus.Stable},
                new UnitCellPosition {Status = PositionStatus.Unstable},
                new UnitCellPosition {Status = PositionStatus.Stable},
                new UnitCellPosition {Status = PositionStatus.Unstable},
                new UnitCellPosition {Status = PositionStatus.Stable},
                //new UnitCellPosition {Status = PositionStatus.Stable}
            };

            var result = ConnectorsToAbstractMovement(connections, ucps);

            //var defaultPath = "C:/Users/hims-user/source/repos/ICon.Program/ICon.Framework.QuickTest";
            //var watch = Stopwatch.StartNew();
            ////var context = new CInteropDbContext("./mcsop.db", true);
            //var packages = ManagerFactory.DebugFactory.CreateManageSystemForCeria();
            //var mainContextBuilder = new ProjectModelContextBuilder(packages.ProjectServices);
            //var transitionContextBuilder = new TransitionModelContextBuilder(mainContextBuilder);
            //var result = await transitionContextBuilder.CreateNewContext<TransitionModelContext>();
            //DisplayWatch(watch);
            Console.ReadLine();
        }

        static IList<int> ConnectorsToAbstractMovement(IList<ConnectorType> connectorTypes, IList<IUnitCellPosition> positions)
        {
            var result = new List<int>(connectorTypes.Count);

            for (var i = 0; i < connectorTypes.Count;)
            {
                var currentSum = 0;
                while (i++ < connectorTypes.Count && connectorTypes[i-1] == ConnectorType.Dynamic)
                {
                    currentSum++;
                    result.Add(1);
                }

                result.Add(-currentSum);
            }

            for (var i = 1; i < result.Count; i++)
            {
                if (positions[i].Status != PositionStatus.Unstable || connectorTypes[i - 1] != ConnectorType.Dynamic)
                    continue;

                result[i - 1]++;
                result[i] = 0;
            }

            return result;
        }


        private static void DisplayWatch(Stopwatch watch)
        {
            watch.Stop();
            Console.WriteLine("Watch Dump: {0}", watch.Elapsed.ToString());
            watch.Reset();
            watch.Start();
        }
    }
}
