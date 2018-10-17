using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Text;

using Mocassin.Framework.Events;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    /// Basic lattice notification manager that handles distribution of push based update notifications about changes in the lattice manager base data
    /// </summary>
    internal class LatticeEventManager : ModelEventManager, ILatticeEventPort
    {

    }
}
