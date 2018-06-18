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

            var particles = package.ParticleManager.QueryPort.Query(port => port.GetParticles());
            var transitions = package.TransitionManager.QueryPort.Query(port => port.GetAbstractTransitions());

            var ruleGenerator = new QuickRuleGenerator<KineticRule>(particles);
            var rules = ruleGenerator.MakeUniqueRules(transitions).Select(a => a.ToList()).ToList();

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
