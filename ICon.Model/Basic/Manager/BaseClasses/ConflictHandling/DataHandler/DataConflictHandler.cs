using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ICon.Framework.Operations;
using ICon.Framework.Processing;
using ICon.Framework.Reflection;
using ICon.Model.ProjectServices;

namespace ICon.Model.Basic
{
    /// <summary>
    ///     Abstract generic base class for data conflict resolver implementations that support automated pipeline generation
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public abstract class DataConflictHandler<T1, T2> : IDataConflictHandler<T1, T2>
        where T1 : ModelData
    {
        /// <summary>
        ///     Access to the current project service instance
        /// </summary>
        protected IProjectServices ProjectServices { get; set; }

        /// <summary>
        ///     The conflict resolver break pipeline that processes the resolve requests
        /// </summary>
        protected BreakPipeline<IConflictReport> ResolverPipeline { get; set; }

        /// <summary>
        ///     Creates new data conflict resolver that uses the provided project services
        /// </summary>
        /// <param name="projectServices"></param>
        protected DataConflictHandler(IProjectServices projectServices)
        {
            ProjectServices = projectServices ?? throw new ArgumentNullException(nameof(projectServices));
            ResolverPipeline = new BreakPipeline<IConflictReport>(CreateOnCannotProcessProcessor(), CreateResolverProcessors().ToList());
        }

        /// <inheritdoc />
        public IConflictReport ResolveConflicts(T2 source, IDataAccessor<T1> dataAccess)
        {
            return ResolverPipeline.Process(source, dataAccess);
        }

        /// <summary>
        ///     Searches the data conflict resolver for all marked methods and creates the resolver processors for the pipeline
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<IObjectProcessor<IConflictReport>> CreateResolverProcessors()
        {
            bool DelegateSearchMethod(MethodInfo methodInfo)
            {
                return methodInfo.GetCustomAttribute(typeof(ConflictHandlingMethodAttribute)) != null;
            }

            return new ObjectProcessorCreator().CreateProcessors<IConflictReport>(this, DelegateSearchMethod,
                BindingFlags.Instance | BindingFlags.NonPublic);
        }

        /// <summary>
        ///     Creates the processor that is called if the end of the pipeline is reached (Default reaction is to return empty OK
        ///     resolver report)
        /// </summary>
        /// <returns></returns>
        protected virtual IObjectProcessor<IConflictReport> CreateOnCannotProcessProcessor()
        {
            return new ObjectProcessor<object, IConflictReport>(obj => new ConflictReport());
        }
    }
}