using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;

namespace ICon.Model.Structures
{
    /// <summary>
    /// Represents a structure manager that handles input, output and distribution of structural data and positions inside a simulation project
    /// </summary>
    public interface IStructureManager : IModelManager<IStructureInputPort, IStructureEventPort, IStructureQueryPort>
    {

    }
}
