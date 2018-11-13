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
using Mocassin.Mathematics;
using Mocassin.Mathematics.Coordinates;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Mocassin.Framework.Constraints;
using Mocassin.Mathematics.Permutation;
using Mocassin.Mathematics.Extensions;
using Mocassin.Mathematics.Comparers;
using Mocassin.Mathematics.Solver;
using Mocassin.Symmetry.SpaceGroups;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Mathematics.Bitmasks;
using Mocassin.Framework.Messaging;
using Mocassin.Framework.Xml;
using Mocassin.Framework.Provider;
using Mocassin.Framework.Collections;
using Mocassin.Symmetry.CrystalSystems;
using Mocassin.Model.ModelProject;
using Mocassin.Framework.Operations;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.Model.Energies;
using Mocassin.Framework.Processing;
using Mocassin.Symmetry.Analysis;
using Mocassin.Mathematics.Constraints;
using Mocassin.Framework.Reflection;
using Mocassin.Model.Transitions;
using Mocassin.Model.Lattices;
using Mocassin.Framework.Random;
using System.Linq;
using Mocassin.Model.DataManagement;
using Mocassin.Model.Simulations;
using Mocassin.Model.Translator;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using Mocassin.Model.Basic;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Framework.QuickTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var defaultPath = "C:/Users/hims-user/source/repos/ICon.Program/ICon.Framework.QuickTest";
            //var context = new CInteropDbContext("./mcsop.db", true);
            var packages = ManagerFactory.DebugFactory.CreateManageSystemForCeria();
            var mainContextBuilder = new ProjectModelContextBuilder(packages.ModelProject);
            var watch = Stopwatch.StartNew();
            var result = mainContextBuilder.BuildNewContext().Result;
            var count = (from IKineticMappingModel mappingModel in result.SimulationModelContext.KineticSimulationModels
                    .First().MappingAssignMatrix
                where mappingModel != null
                select mappingModel).Count();
            DisplayWatch(watch);
            Console.ReadLine();
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
