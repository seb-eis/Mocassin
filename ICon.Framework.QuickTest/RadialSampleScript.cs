using System.Linq;
using Microsoft.EntityFrameworkCore;
using Mocassin.Tools.Evaluation.Context;

namespace Mocassin.Framework.QuickTest
{
    public static class RadialSampleScript
    {
        public static void Run()
        {
            using var evalContext = MslEvaluationContext.Create(".\\MySimulations.msl");
            var jobModels = evalContext
                            .EvaluationJobSet()
                            .Include(x => x.SimulationLatticeModel);
            var evaluableSet = evalContext.MakeEvaluableSet(jobModels);
            var jobContext = evaluableSet.First();
        }
    }
}