using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mocassin.Framework.Extensions
{
    /// <summary>
    ///     Contains ICon extension methods for threading and tasking
    /// </summary>
    public static class MocassinTaskingExtensions
    {
        /// <summary>
        ///     Starts many tasks on the task pool and returns an array of the task objects
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="runActions"></param>
        /// <returns></returns>
        public static Task<T1>[] RunMany<T1>(IEnumerable<Func<T1>> runActions)
        {
            return runActions.Select(Task.Run).ToArray();
        }

        /// <summary>
        ///     Starts multiple tasks on the task pool, await all results and returns the results in order
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="runActions"></param>
        /// <returns></returns>
        public static IEnumerable<T1> RunAndGetResults<T1>(IEnumerable<Func<T1>> runActions)
        {
            return RunMany(runActions).Select(value => value.Result);
        }
    }
}