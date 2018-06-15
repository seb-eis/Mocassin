using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Text;

using ICon.Framework.Events;
using ICon.Model.Basic;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Basic transitions notification manager that handles distribution of push based update notifications about changes in the transition manager base data
    /// </summary>
    internal class EnergyEventManager : ModelEventManager, IEnergyEventPort
    {

    }
}
