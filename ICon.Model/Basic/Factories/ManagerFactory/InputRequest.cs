using System;
using System.Threading.Tasks;
using Mocassin.Framework.Operations;
using Newtonsoft.Json;

namespace Mocassin.Model.Basic.Debug
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

}