﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mocassin.Framework.Operations;
using Mocassin.Framework.Processing;
using Mocassin.Framework.Reflection;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Basic
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
        protected IModelProject ModelProject { get; set; }

        /// <summary>
        ///     The conflict resolver break pipeline that processes the resolve requests
        /// </summary>
        protected BreakPipeline<IConflictReport> ResolverPipeline { get; set; }

        /// <summary>
        ///     Creates new data conflict resolver that uses the provided project services
        /// </summary>
        /// <param name="modelProject"></param>
        protected DataConflictHandler(IModelProject modelProject)
        {
            ModelProject = modelProject ?? throw new ArgumentNullException(nameof(modelProject));
            ResolverPipeline = new BreakPipeline<IConflictReport>(CreateOnCannotProcessProcessor(), CreateResolverProcessors().ToList());
        }

        /// <inheritdoc />
        public IConflictReport ResolveConflicts(T2 source, IDataAccessor<T1> dataAccess) => ResolverPipeline.Process(source, dataAccess);

        /// <summary>
        ///     Searches the data conflict resolver for all marked methods and creates the resolver processors for the pipeline
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<IObjectProcessor<IConflictReport>> CreateResolverProcessors()
        {
            bool DelegateSearchMethod(MethodInfo methodInfo) => methodInfo.GetCustomAttribute(typeof(ConflictHandlingMethodAttribute)) != null;

            return new ObjectProcessorBuilder().CreateProcessors<IConflictReport>(this, DelegateSearchMethod,
                BindingFlags.Instance | BindingFlags.NonPublic);
        }

        /// <summary>
        ///     Creates the processor that is called if the end of the pipeline is reached (Default reaction is to return empty OK
        ///     resolver report)
        /// </summary>
        /// <returns></returns>
        protected IObjectProcessor<IConflictReport> CreateOnCannotProcessProcessor()
        {
            return new ObjectProcessor<object, IConflictReport>(obj => new ConflictReport());
        }
    }
}