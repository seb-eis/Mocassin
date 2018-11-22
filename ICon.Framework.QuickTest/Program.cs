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
using Mocassin.Model.Jobs;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Framework.QuickTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var package = new JobPackage
            {
                SimulationId = "OxygenMigration",
                GlobalJobProperties = new HashSet<JobProperty>
                {
                    new JobProperty("Temperature", "500"), new JobProperty("Mcsp", "100")
                },
                JobSpecifications = new List<JobSpecification>
                {
                    new JobSpecification
                    { 
                        JobProperties = new HashSet<JobProperty>()
                        {
                            new JobProperty("Temperature", "200"),
                            new JobProperty("Mcsp", "50")
                        }
                    },
                    new JobSpecification
                    {
                        JobProperties = new HashSet<JobProperty>()
                        {
                            new JobProperty("Temperature", "200"),
                            new JobProperty("Mcsp", "50"),
                            new JobProperty("JobCount","25")
                        }
                    }
                }
            };
            var service = XmlStreamService.CreateFor(package);
            service.TrySerialize(Console.OpenStandardOutput(), package);
            var fs = File.OpenRead("C:\\Users\\hims-user\\Documents\\Gitlab\\TestFiles\\job.xml");
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
