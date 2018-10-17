using System;

using Mocassin.Framework.Events;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    /// Basic transitions notification manager that handles distribution of push based update notifications about changes in the transition manager base data
    /// </summary>
    internal class TransitionEventManager : ModelEventManager, ITransitionEventPort
    {

    }
}
