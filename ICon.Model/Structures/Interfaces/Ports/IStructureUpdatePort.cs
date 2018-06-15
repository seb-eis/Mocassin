using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;
using ICon.Model.Particles;

namespace ICon.Model.Structures
{
    /// <summary>
    /// Represents an update port for a structure manager that accepts other modules event ports for update subscriptions
    /// </summary>
    public interface IStructureUpdatePort : IModelUpdatePort
    {
    }
}
