using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;

namespace Mocassin.Model.ModelProject
{
    /// <inheritdoc />
    public class ProjectInputPipeline : IProjectInputPipeline
    {
        /// <summary>
        ///     The input port delegate dictionary that assigns <see cref="IModelObject" /> the correct input delegate
        /// </summary>
        private readonly Dictionary<Type, Func<IModelObject, Task<IOperationReport>>> objectInputDictionary;

        /// <summary>
        ///     The input port delegate dictionary that assigns <see cref="IModelParameter" /> the correct input delegate
        /// </summary>
        private readonly Dictionary<Type, Func<IModelParameter, Task<IOperationReport>>> parameterInputDictionary;

        /// <inheritdoc />
        public IModelProject ModelProject { get; }

        /// <summary>
        ///     Creates new <see cref="ProjectInputPipeline" /> for the passed <see cref="IModelProject" />
        /// </summary>
        /// <param name="modelProject"></param>
        public ProjectInputPipeline(IModelProject modelProject)
        {
            ModelProject = modelProject ?? throw new ArgumentNullException(nameof(modelProject));
            objectInputDictionary = new Dictionary<Type, Func<IModelObject, Task<IOperationReport>>>();
            parameterInputDictionary = new Dictionary<Type, Func<IModelParameter, Task<IOperationReport>>>();
        }


        /// <inheritdoc />
        public IList<IOperationReport> PushToProject(IEnumerable<object> objects)
        {
            var reports = new List<IOperationReport>();
            foreach (var obj in objects)
            {
                switch (obj)
                {
                    case IModelParameter modelParameter:
                        reports.Add(PushToProject(modelParameter));
                        break;

                    case IModelObject modelObject:
                        reports.Add(PushToProject(modelObject));
                        break;

                    default:
                        reports.Add(GetNotSupportedOperationReport(obj));
                        return reports;
                }

                if (!reports[reports.Count - 1].IsGood) break;
            }

            return reports;
        }

        /// <inheritdoc />
        public IOperationReport PushToProject(IModelObject modelObject)
        {
            var func = GetInputDelegate(modelObject);
            if (func == null) return GetNotSupportedOperationReport(modelObject);
            var task = func(modelObject);
            return task.Result;
        }

        /// <inheritdoc />
        public IOperationReport PushToProject(IModelParameter modelParameter)
        {
            var func = GetInputDelegate(modelParameter);
            if (func == null) return GetNotSupportedOperationReport(modelParameter);
            var task = func(modelParameter);
            return task.Result;
        }

        /// <summary>
        ///     Get an operation report that informs that the passed object could not be processed
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IOperationReport GetNotSupportedOperationReport(object obj)
        {
            var report = new OperationReport("Input Failure");
            report.AddException(new InvalidOperationException($"No manager for object [{obj.GetType()}] was found"));
            return report;
        }

        /// <summary>
        ///     Get the input delegate for the passed <see cref="IModelParameter" /> or null if the parameter is not supported
        /// </summary>
        /// <param name="modelParameter"></param>
        /// <returns></returns>
        public Func<IModelParameter, Task<IOperationReport>> GetInputDelegate(IModelParameter modelParameter)
        {
            var func = parameterInputDictionary.FirstOrDefault(x => x.Key.IsInstanceOfType(modelParameter)).Value;
            if (func != null)
                return func;

            foreach (var modelManager in ModelProject.GetAllManagers())
            {
                var type = modelManager.InputPort.GetSupportedModelTypes().FirstOrDefault(x => x.IsInstanceOfType(modelParameter));
                if (type == null)
                    continue;

                func = MakeParameterInputDelegate(modelManager.InputPort);
                parameterInputDictionary.Add(type, func);
                return func;
            }

            return null;
        }

        /// <summary>
        ///     Get the input delegate for the passed <see cref="IModelObject" /> or null if the parameter is not supported
        /// </summary>
        /// <param name="modelObject"></param>
        /// <returns></returns>
        public Func<IModelObject, Task<IOperationReport>> GetInputDelegate(IModelObject modelObject)
        {
            var func = objectInputDictionary.FirstOrDefault(x => x.Key.IsInstanceOfType(modelObject)).Value;
            if (func != null)
                return func;

            foreach (var modelManager in ModelProject.GetAllManagers())
            {
                var type = modelManager.InputPort.GetSupportedModelTypes().FirstOrDefault(x => x.IsInstanceOfType(modelObject));
                if (type == null)
                    continue;

                func = MakeObjectInputDelegate(modelManager.InputPort);
                objectInputDictionary.Add(type, func);
                return func;
            }

            return null;
        }

        /// <summary>
        ///     Creates an <see cref="IModelParameter" /> input delegate to the passed <see cref="IModelInputPort" />
        /// </summary>
        /// <param name="inputPort"></param>
        /// <returns></returns>
        private static Func<IModelParameter, Task<IOperationReport>> MakeParameterInputDelegate(IModelInputPort inputPort)
        {
            Task<IOperationReport> Operation(IModelParameter modelParameter)
            {
                return inputPort.SetModelParameter(modelParameter);
            }

            return Operation;
        }

        /// <summary>
        ///     Creates an <see cref="IModelObject" /> input delegate to the passed <see cref="IModelInputPort" />
        /// </summary>
        /// <param name="inputPort"></param>
        /// <returns></returns>
        private static Func<IModelObject, Task<IOperationReport>> MakeObjectInputDelegate(IModelInputPort inputPort)
        {
            Task<IOperationReport> Operation(IModelObject modelObject)
            {
                return inputPort.InputModelObject(modelObject);
            }

            return Operation;
        }
    }
}