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

namespace ICon.Framework.QuickTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var binArray0 = InteropBinaryArray<int>.FromArray(new int[1000, 100, 100]);
            var binArray1 = InteropBinaryArray<int>.FromArray(new int[1000, 100, 100]);
            var binArray2 = InteropBinaryArray<int>.FromArray(new int[1000, 100, 100]);
            var binArray3 = InteropBinaryArray<int>.FromArray(new int[1000, 100, 100]);

            var tasks = new Task[4];
            var clock = Stopwatch.StartNew();
            using (var provider = new MarshalProvider())
            {
                tasks[0] = Task.Run(() => binArray0.ChangeStateToBinary(provider));
                tasks[1] = Task.Run(() => binArray1.ChangeStateToBinary(provider));
                tasks[2] = Task.Run(() => binArray2.ChangeStateToBinary(provider));
                tasks[3] = Task.Run(() => binArray3.ChangeStateToBinary(provider));
            }
            Task.WaitAll(tasks);
            using (var provider = new MarshalProvider())
            {
                tasks[0] = Task.Run(() => binArray0.ChangeStateToObject(provider));
                tasks[1] = Task.Run(() => binArray1.ChangeStateToObject(provider));
                tasks[2] = Task.Run(() => binArray2.ChangeStateToObject(provider));
                tasks[3] = Task.Run(() => binArray3.ChangeStateToObject(provider));
            }
            Task.WaitAll(tasks);

            Console.WriteLine("Time is {0}", clock.ElapsedMilliseconds);

            //var context = new InteropDbContext("./interopTest.db", true);
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
