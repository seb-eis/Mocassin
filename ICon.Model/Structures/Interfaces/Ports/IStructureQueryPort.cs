using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;

namespace ICon.Model.Structures
{
    /// <summary>
    /// Represents a query port of the structure manager that provides safe access to data and internal logic
    /// </summary>
    public interface IStructureQueryPort : IModelQueryPort<IStructureDataPort>, IModelQueryPort<IStructureCachePort>
    {
    }
}
