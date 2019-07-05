using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Queries;

namespace Mocassin.Framework.QuickTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var mslFilename = @"C:\Users\hims-user\Documents\Gitlab\MocassinTestFiles\GuiTesting\GdCeO.Filled.msl";
            var evalContext = MslEvaluationContext.Create(mslFilename);
            var data = evalContext.EvaluationJobSet();
            var evaluable = evalContext.MakeEvaluable(data);

            var watch = Stopwatch.StartNew();
            
            var evaluation = new ParticleCountEvaluation(evaluable);
            var result = evaluation.Results;

            var table = new DataTable();
            var column = new DataColumn("Name");
            table.Columns.Add(column);
            for (var i = 0; i < result.First().Count; i++)
            {
                table.Columns.Add(new DataColumn($"Particle_{i}", i.GetType()));
            }

            foreach (var item in evaluable)
            {
                var row = table.NewRow();
                row["Name"] = item.FullConfigName;
                var index = 0;
                foreach (var i in result[item.DataId])
                {
                    row[$"Particle_{index++}"] = i;
                }

                table.Rows.Add(row);
            }
            

            DisplayWatch(watch);
            ExitOnKeyPress("Finished successfully...");
        }


        private static void DisplayWatch(Stopwatch watch)
        {
            watch.Stop();
            Console.WriteLine("Watch Dump: {0}", watch.Elapsed.ToString());
            watch.Reset();
            watch.Start();
        }

        private static void ExitOnKeyPress(string message)
        {
            Console.WriteLine(message + "\nPress button to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}