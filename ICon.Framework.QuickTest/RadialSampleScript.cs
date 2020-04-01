using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Mocassin.Tools.Evaluation.Context;
using Mocassin.Tools.Evaluation.Extensions;
using Mocassin.Tools.Evaluation.Queries;

namespace Mocassin.Framework.QuickTest
{
    public static class RadialSampleScript
    {
        public static void Run()
        {
            var mslPath = @"C:\Users\Sebastian\Documents\Promotion\MocassinTests\ceria.filled.msl";
            var dumpPathFormat = @"C:\Users\Sebastian\Documents\Promotion\MocassinTests\dumps\{0}.dat";
            using var evalContext = MslEvaluationContext.Create(mslPath);
            var jobSet = evalContext.EvaluationJobSet().Include(x => x.SimulationLatticeModel);
            var evalSet = evalContext.MakeEvaluableSet(jobSet);
            var watch = Stopwatch.StartNew();
            var evaluation = new CellSiteCoordinationEvaluation(evalSet, 30);
            var result = evaluation.Result.First().First();
            System.Globalization.CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            foreach (var pair in result.Value)
            {
                var siteName = result.Key.Name;
                var occName = pair.Key.Name;
                foreach (var distribution in pair.Value)
                {
                    var partnerName = distribution.Particle.Name;
                    var datPath = string.Format(dumpPathFormat, siteName + "_" + occName + "_" + partnerName);
                    distribution.WriteDatToFile(datPath);
                }
            }

            Console.WriteLine($"Elapsed: {watch.Elapsed}; Per Item: {watch.Elapsed / evalSet.Count}");
        }
    }
}