using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mocassin.Tools.Evaluation.Context;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Generic base class for implementations of data queries against <see cref="JobContext" /> instances
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class JobEvaluation<T> : IJobEvaluation<T>
    {
        private readonly object lockObject = new object();
        private Task<IReadOnlyList<T>> ResultTask { get; set; }

        /// <inheritdoc />
        public IEvaluableJobSet JobSet { get; }

        /// <inheritdoc />
        public IReadOnlyList<T> Result => ResultTask?.Result ?? InvokeAsync().Result;

        /// <inheritdoc />
        public int Count => Result.Count;

        /// <inheritdoc />
        public T this[int index] => Result[index];

        /// <summary>
        ///     Get or set a boolean flag if the system should created results in parallel
        /// </summary>
        public bool ExecuteParallel { get; set; }

        /// <summary>
        ///     Creates a new <see cref="JobEvaluation{T}" /> for the passed <see cref="IEvaluableJobSet" /> data source
        /// </summary>
        /// <param name="jobSet"></param>
        protected JobEvaluation(IEvaluableJobSet jobSet)
        {
            JobSet = jobSet ?? throw new ArgumentNullException(nameof(jobSet));
        }

        /// <summary>
        ///     Executes the query for a single <see cref="JobContext" /> without checking if the passed context is part of the
        ///     data source
        /// </summary>
        /// <param name="jobContext"></param>
        /// <returns></returns>
        protected abstract T GetValue(JobContext jobContext);

        /// <summary>
        ///     Get the query result task or generates and invokes the task if required
        /// </summary>
        /// <returns></returns>
        private Task<IReadOnlyList<T>> InvokeAsync()
        {
            lock (lockObject)
            {
                if (ResultTask != null) return ResultTask;
                return ResultTask = ExecuteParallel ? ExecuteParallelAsync() : ExecuteSequentialAsync();
            }
        }

        /// <summary>
        ///     Executes the query async and sequential
        /// </summary>
        /// <returns></returns>
        private Task<IReadOnlyList<T>> ExecuteSequentialAsync()
        {
            IReadOnlyList<T> ExecuteLocal()
            {
                PrepareForExecution();
                var resultList = new List<T>(JobSet.Count);
                resultList.AddRange(JobSet.Select(GetValue));
                return resultList.AsReadOnly();
            }
            return Task.Run(ExecuteLocal);
        }

        /// <summary>
        ///     Executes the query async and in parallel
        /// </summary>
        /// <returns></returns>
        private Task<IReadOnlyList<T>> ExecuteParallelAsync()
        {
            IReadOnlyList<T> ExecuteLocal()
            {
                PrepareForExecution();
                var resultList = new List<T>(JobSet.Count);
                var taskList = new List<Task<T>>(JobSet.Count);
                taskList.AddRange(JobSet.Select(x => Task.Run(() => GetValue(x))));
                Task.WhenAll(taskList).Wait();
                resultList.AddRange(taskList.Select(x => x.Result));
                return resultList.AsReadOnly();
            }
            return Task.Run(ExecuteLocal);
        }

        /// <summary>
        ///     Performs required pre execution actions e.g. getting unspecified dependent data sources
        /// </summary>
        protected virtual void PrepareForExecution()
        {
        }

        /// <summary>
        ///     Provide the <see cref="JobEvaluation{T}" /> as a delegate
        /// </summary>
        /// <returns></returns>
        public Func<JobContext, T> AsSelector()
        {
            return GetValue;
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return Result.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}