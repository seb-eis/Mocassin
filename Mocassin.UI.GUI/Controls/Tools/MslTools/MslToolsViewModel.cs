using System;
using System.Collections.ObjectModel;
using Mocassin.Framework.Extensions;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Base.ViewModels.Collections;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.Tools.MslTools.SubControls;

namespace Mocassin.UI.GUI.Controls.Tools.MslTools
{
    public class MslToolsViewModel : ViewModelBase, IDisposable
    {
        public CollectionControlViewModel<ToolContainer> ToolContainers { get; }

        public MslToolsViewModel()
        {
            ToolContainers = new CollectionControlViewModel<ToolContainer>();
            LoadToolContainers();
        }

        protected void LoadToolContainers()
        {
            var list = new ObservableCollection<ToolContainer> {CreateRecycleToolContainer()};
            ToolContainers.SetCollection(list);
        }

        protected ToolContainer CreateRecycleToolContainer()
        {
            var name = "Lattice Recycle Tool";
            var description =
                "Exports result lattice data from a populated msl database and imports the states as initial lattice into a compatible target msl.";
            return new ToolContainer(() => new LatticeRecycleToolView(), () => new LatticeRecycleToolViewModel(), name, description);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            ToolContainers.Items.Action(x => x.Dispose()).Load();
        }
    }
}