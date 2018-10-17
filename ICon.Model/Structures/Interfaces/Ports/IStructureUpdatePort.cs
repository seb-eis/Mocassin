using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.Particles;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Structures
{
    /// <summary>
    /// Represents an update port for a structure manager that accepts other modules event ports for update subscriptions
    /// </summary>
    public interface IStructureUpdatePort : IModelUpdatePort
    {
    }
}
