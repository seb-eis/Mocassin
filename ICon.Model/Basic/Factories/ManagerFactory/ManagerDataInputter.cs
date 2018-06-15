using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;

using ICon.Framework.Operations;
using ICon.Model.ProjectServices;
using System.Collections;

namespace ICon.Model.Basic.Debug
{
    /// <summary>
    /// Represents an input operation described by an object and an operation type
    /// </summary>
    public class InputRequest
    {
        /// <summary>
        /// The input delegate function that takes the model object and manager interface to perform the input task
        /// </summary>
        [JsonIgnore]
        public Func<IModelManager, object, Task<IOperationReport>> InputDelegate { get; set; }

        /// <summary>
        /// The input object
        /// </summary>
        public object InputObject { get; set; }

        /// <summary>
        /// Start the input operation, await the results and return the report
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public IOperationReport CallAndAwait(IModelManager manager)
        {
            return InputDelegate(manager, InputObject).Result;
        }
    }

    /// <summary>
    /// Data class for generic input of model data into a manager system
    /// </summary>
    public class ManagerDataInputter : IEnumerable<InputRequest>
    {
        /// <summary>
        /// The list of input requests
        /// </summary>
        public List<InputRequest> InputRequests { get; }

        /// <summary>
        /// The input operation report
        /// </summary>
        [JsonIgnore]
        public List<IOperationReport> InputReports { get; set; }

        public ManagerDataInputter()
        {
            InputRequests = new List<InputRequest>();
        }

        /// <summary>
        /// Adds a new input request for the passed object
        /// </summary>
        /// <param name="inputObject"></param>
        public void Add(object inputObject)
        {
            InputRequests.Add(new InputRequest() { InputObject = inputObject });
        }

        /// <summary>
        /// Inpuits all data using the passed project service and returns all operation reports of the input
        /// </summary>
        /// <param name="projectServices"></param>
        public void AutoInputData(IProjectServices projectServices)
        {
            AutoAssignInputDelegates();
            var callList = MakeCallList(projectServices);

            InputReports = new List<IOperationReport>(callList.Count);
            var totalReport = new OperationReport("Invoke input list");
            for (int i = 0; i < InputRequests.Count; i++)
            {
                InputReports.Add(InputRequests[i].CallAndAwait(callList[i]));
                if (!InputReports[i].IsGood)
                {
                    totalReport.AddException(new InvalidOperationException($"Input failed for \n {InputReports[i].ToString()}"));
                    break;
                }
            }
            InputReports.Add(totalReport);
        }

        /// <summary>
        /// Generates the list which managers have to be called for the input
        /// </summary>
        /// <param name="projectServices"></param>
        /// <returns></returns>
        public List<IModelManager> MakeCallList(IProjectServices projectServices)
        {
            var callList = new List<IModelManager>(InputRequests.Count);
            var callLookup = GetCallDictionary(projectServices);

            foreach (var request in InputRequests)
            {
                var manager = callLookup.First(keyValue => keyValue.Value.Any(objType => objType.IsAssignableFrom(request.InputObject.GetType()))).Key;
                callList.Add(manager);
            }
            return callList;
        }

        /// <summary>
        /// Makes a call dictionary that assigns each managers its set of supported input objects
        /// </summary>
        /// <param name="projectServices"></param>
        /// <returns></returns>
        public Dictionary<IModelManager, IEnumerable<Type>> GetCallDictionary(IProjectServices projectServices)
        {
            var result = new Dictionary<IModelManager, IEnumerable<Type>>(10);
            foreach (var manager in projectServices.GetAllManagers())
            {
                result[manager] = manager.InputPort.GetSupportedModelTypes();
            }
            return result;
        }

        /// <summary>
        /// Assigns all known input requests their input delegate
        /// </summary>
        /// <param name="projectServices"></param>
        public void AutoAssignInputDelegates()
        {
            var objInput = GetObjectInputDelegate();
            var paramSet = GetParameterSetDelegate();

            foreach (var item in InputRequests)
            {
                if (item.InputObject is IModelObject)
                {
                    item.InputDelegate = objInput;
                    continue;
                }
                if (item.InputObject is IModelParameter)
                {
                    item.InputDelegate = paramSet;
                    continue;
                }
                throw new NotSupportedException("Object type is not supported");
            }
        }

        /// <summary>
        /// Get a delegate to input an object into a manager
        /// </summary>
        /// <returns></returns>
        protected Func<IModelManager, object, Task<IOperationReport>> GetObjectInputDelegate()
        {
            return (manager, obj) => manager.InputPort.InputModelObject((IModelObject)obj);
        }

        /// <summary>
        /// Get a delegate to set a parameter in a manager
        /// </summary>
        /// <returns></returns>
        protected Func<IModelManager, object, Task<IOperationReport>> GetParameterSetDelegate()
        {
            return (manager, obj) => manager.InputPort.SetModelParameter((IModelParameter)obj);
        }

        /// <summary>
        /// Get enumerator for the input requests
        /// </summary>
        /// <returns></returns>
        public IEnumerator<InputRequest> GetEnumerator()
        {
            return ((IEnumerable<InputRequest>)InputRequests).GetEnumerator();
        }

        /// <summary>
        /// Get enumerator for the input requests
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<InputRequest>)InputRequests).GetEnumerator();
        }

        /// <summary>
        /// Serialiaze to json with type handling and indentation
        /// </summary>
        /// <returns></returns>
        public string JsonSerialize()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings() { Formatting = Formatting.Indented, TypeNameHandling = TypeNameHandling.Auto });
        }

        /// <summary>
        /// Retruns a json formatted string of the current input report list
        /// </summary>
        /// <returns></returns>
        public string GetReportJson()
        {
            return JsonConvert.SerializeObject(InputReports, new JsonSerializerSettings() { Formatting = Formatting.Indented});
        }
    }
}
