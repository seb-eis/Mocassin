using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Structures
{
    /// <summary>
    /// Represents a query port of the structure manager that provides safe access to data and internal logic
    /// </summary>
    public interface IStructureQueryPort : IModelQueryPort<IStructureDataPort>, IModelQueryPort<IStructureCachePort>
    {
    }
}
