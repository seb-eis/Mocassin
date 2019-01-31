using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Newtonsoft.Json;

namespace Mocassin.Model.ModelProject
{
    /// <summary>
    ///     Input system for the <see cref="IModelProject" /> that automatically targets the correct
    ///     <see cref="Mocassin.Model.Basic.IModelInputPort" />
    /// </summary>
    public class ModelProjectInputSystem : IEnumerable<ProjectInputRequest>
    {
        /// <summary>
        ///     The list of input requests
        /// </summary>
        public List<ProjectInputRequest> InputRequests { get; }

        /// <summary>
        ///     Get the input report list created during the input operations
        /// </summary>
        public List<IOperationReport> InputReports { get; protected set; }

        public ModelProjectInputSystem()
        {
            InputRequests = new List<ProjectInputRequest>();
        }

        /// <summary>
        ///     Adds a new input request for the passed object
        /// </summary>
        /// <param name="inputObject"></param>
        public void Add(object inputObject)
        {
            InputRequests.Add(new ProjectInputRequest {InputObject = inputObject});
        }

        /// <summary>
        ///     Adds many objects to the input request list
        /// </summary>
        /// <param name="objects"></param>
        public void AddMany(IEnumerable<object> objects)
        {
            foreach (var item in objects) Add(item);
        }

        /// <summary>
        ///     Inputs all data using the passed project service and returns all operation reports of the input
        /// </summary>
        /// <param name="modelProject"></param>
        public void PushData(IModelProject modelProject)
        {
            AutoAssignInputDelegates();
            var callList = MakeCallList(modelProject);

            InputReports = new List<IOperationReport>(callList.Count);
            var totalReport = new OperationReport("Invoke input list");
            for (var i = 0; i < InputRequests.Count; i++)
            {
                InputReports.Add(InputRequests[i].Invoke(callList[i]));
                if (InputReports[i].IsGood)
                    continue;

                totalReport.AddException(new InvalidOperationException($"Stopped due to invalid input\n {InputReports[i]}"));
                break;
            }

            InputReports.Add(totalReport);
        }

        /// <summary>
        ///     Generates the list which managers have to be called for the input
        /// </summary>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        public List<IModelManager> MakeCallList(IModelProject modelProject)
        {
            var callList = new List<IModelManager>(InputRequests.Count);
            var callLookup = GetCallDictionary(modelProject);

            foreach (var request in InputRequests)
            {
                var manager = callLookup
                    .First(keyValue => keyValue.Value
                        .Any(objType => objType.IsInstanceOfType(request.InputObject))).Key;
                callList.Add(manager);
            }

            return callList;
        }

        /// <summary>
        ///     Makes a call dictionary that assigns each managers its set of supported input objects
        /// </summary>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        public Dictionary<IModelManager, IEnumerable<Type>> GetCallDictionary(IModelProject modelProject)
        {
            var result = new Dictionary<IModelManager, IEnumerable<Type>>(10);
            foreach (var manager in modelProject.GetAllManagers())
                result[manager] = manager.InputPort.GetSupportedModelTypes();

            return result;
        }

        /// <summary>
        ///     Assigns all known input requests their input delegate
        /// </summary>
        public void AutoAssignInputDelegates()
        {
            var objInput = GetObjectInputDelegate();
            var paramSet = GetParameterSetDelegate();

            foreach (var item in InputRequests)
            {
                switch (item.InputObject)
                {
                    case IModelObject _:
                        item.InputDelegate = objInput;
                        continue;

                    case IModelParameter _:
                        item.InputDelegate = paramSet;
                        continue;

                    default:
                        throw new NotSupportedException("Object type is not supported");
                }
            }
        }

        /// <summary>
        ///     Get a delegate to input an object into a manager
        /// </summary>
        /// <returns></returns>
        protected Func<IModelManager, object, Task<IOperationReport>> GetObjectInputDelegate()
        {
            return (manager, obj) => manager.InputPort.InputModelObject((IModelObject) obj);
        }

        /// <summary>
        ///     Get a delegate to set a parameter in a manager
        /// </summary>
        /// <returns></returns>
        protected Func<IModelManager, object, Task<IOperationReport>> GetParameterSetDelegate()
        {
            return (manager, obj) => manager.InputPort.SetModelParameter((IModelParameter) obj);
        }

        /// <inheritdoc />
        public IEnumerator<ProjectInputRequest> GetEnumerator()
        {
            return ((IEnumerable<ProjectInputRequest>) InputRequests).GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ProjectInputRequest>) InputRequests).GetEnumerator();
        }

        /// <summary>
        ///     Returns a json formatted string of the current input report list
        /// </summary>
        /// <returns></returns>
        public string GetReportJson()
        {
            return JsonConvert.SerializeObject(InputReports, new JsonSerializerSettings {Formatting = Formatting.Indented});
        }
    }
}