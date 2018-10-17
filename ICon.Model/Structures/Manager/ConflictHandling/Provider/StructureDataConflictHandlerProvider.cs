using System;
using System.Collections.Generic;
using System.Text;

using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Structures.ConflictHandling
{
    /// <summary>
    /// Resolver provider for all structure conflict resolvers that handle internal data conflicts of the particle manager
    /// </summary>
    public class StructureDataConflictHandlerProvider : DataConflictHandlerProvider<StructureModelData>
    {
        /// <summary>
        /// Creates new structure data conflict resolver provider with access to the provided project services
        /// </summary>
        /// <param name="modelProject"></param>
        public StructureDataConflictHandlerProvider(IModelProject modelProject) : base(modelProject)
        {

        }

        /// <summary>
        /// Creation method for the structure parameter change conflict resolver
        /// </summary>
        /// <returns></returns>
        [HandlerFactoryMethod(DataOperationType.ParameterChange)]
        protected object CreateParameterResolver()
        {
            return new StructureParameterConflictHandler(ModelProject);
        }
    }
}
