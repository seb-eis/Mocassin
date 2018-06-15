using System;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Structures.ConflictHandling
{
    /// <summary>
    /// Resolver provider for all structure conflict resolvers that handle internal data conflicts of the particle manager
    /// </summary>
    public class StructureDataConflictHandlerProvider : DataConflictHandlerProvider<StructureModelData>
    {
        /// <summary>
        /// Creates new structure data conflict resolver provider with access to the provided project services
        /// </summary>
        /// <param name="projectServices"></param>
        public StructureDataConflictHandlerProvider(IProjectServices projectServices) : base(projectServices)
        {

        }

        /// <summary>
        /// Creation method for the structure parameter change conflict resolver
        /// </summary>
        /// <returns></returns>
        [HandlerFactoryMethod(DataOperationType.ParameterChange)]
        protected object CreateParameterResolver()
        {
            return new StructureParameterConflictHandler(ProjectServices);
        }
    }
}
