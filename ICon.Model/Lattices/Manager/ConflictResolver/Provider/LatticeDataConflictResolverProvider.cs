using System;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Resolver provider for all lattice conflict resolvers
    /// </summary>
    public class LatticeDataConflictResolverProvider : DataConflictHandlerProvider<LatticeModelData>
    {
        /// <summary>
        /// Creates new Lattice data conflict resolver provider with access to the provided project services
        /// </summary>
        /// <param name="projectServices"></param>
        public LatticeDataConflictResolverProvider(IProjectServices projectServices) : base(projectServices)
        {

        }
    }
}
