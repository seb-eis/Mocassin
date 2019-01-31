using System;
using System.Threading.Tasks;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;

namespace Mocassin.Model.ModelProject
{
    /// <summary>
    ///     Object input requests for automated input into the correct <see cref="IModelProject" /> input pipeline
    /// </summary>
    public class ProjectInputRequest
    {
        /// <summary>
        ///     Get or set the input delegate that takes the model object and manager interface to perform the input task
        /// </summary>
        public Func<IModelManager, object, Task<IOperationReport>> InputDelegate { get; set; }

        /// <summary>
        ///     Get or set the input object
        /// </summary>
        public object InputObject { get; set; }

        /// <summary>
        ///     Invokes the input operation on the passed <see cref="IModelManager" /> and returns the created
        ///     <see cref="IOperationReport" />
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public IOperationReport Invoke(IModelManager manager)
        {
            return InputDelegate(manager, InputObject).Result;
        }
    }
}