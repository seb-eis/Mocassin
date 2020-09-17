using System.Collections.Generic;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;

namespace Mocassin.Model.ModelProject
{
    /// <summary>
    ///     Project input pipeline that handles <see cref="Mocassin.Model.Basic.IModelObject" /> and
    ///     <see cref="Mocassin.Model.Basic.IModelParameter" /> redirection to the affiliated
    ///     <see cref="Mocassin.Model.Basic.IModelInputPort" />
    /// </summary>
    public interface IProjectInputPipeline
    {
        /// <summary>
        ///     Get or set the <see cref="IModelProject" /> that the pipeline is targeting
        /// </summary>
        IModelProject ModelProject { get; }

        /// <summary>
        ///     Pushes the passed sequence of objects into the managed ´<see cref="IModelProject" />
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        IList<IOperationReport> PushToProject(IEnumerable<object> objects);

        /// <summary>
        ///     Pushes the passed <see cref="IModelObject" /> to the <see cref="IModelProject" /> and returns an
        ///     <see cref="IOperationReport" />
        /// </summary>
        /// <param name="modelObject"></param>
        /// <returns></returns>
        IOperationReport PushToProject(IModelObject modelObject);

        /// <summary>
        ///     Pushes the passed <see cref="IModelParameter" /> to the <see cref="IModelProject" /> and returns an
        ///     <see cref="IOperationReport" />
        /// </summary>
        /// <param name="modelParameter"></param>
        /// <returns></returns>
        IOperationReport PushToProject(IModelParameter modelParameter);
    }
}