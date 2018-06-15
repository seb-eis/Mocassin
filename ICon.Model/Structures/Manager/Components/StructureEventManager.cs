using System;

using ICon.Framework.Events;
using ICon.Model.Basic;

namespace ICon.Model.Structures
{
    /// <summary>
    /// Basic structure notification manager that handles distribution of push based update notifications about changes in the structure manager base data
    /// </summary>
    internal class StructureEventManager : ModelEventManager, IStructureEventPort
    {

    }
}
