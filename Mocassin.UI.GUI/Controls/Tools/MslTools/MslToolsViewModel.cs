using System;
using System.Collections.ObjectModel;
using Mocassin.Framework.Extensions;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.Tools.MslTools.SubControls;

namespace Mocassin.UI.GUI.Controls.Tools.MslTools
{
    /// <summary>
    ///     The <see cref="ViewModelBase"/> to supply msl file tools through <see cref="MslToolsView"/>
    /// </summary>
    public class MslToolsViewModel : ViewModelBase, IDisposable
    {
        /// <summary>
        ///     Get the <see cref="CollectionControlViewModel{T}"/> for <see cref="ToolContainer"/> instances
        /// </summary>
        public CollectionControlViewModel<ToolContainer> ToolContainers { get; }

        /// <inheritdoc />
        public MslToolsViewModel()
        {
            ToolContainers = new CollectionControlViewModel<ToolContainer>();
            LoadToolContainers();
        }

        /// <summary>
        ///     Loads all available <see cref="ToolContainer"/> items
        /// </summary>
        protected void LoadToolContainers()
        {
            var list = new ObservableCollection<ToolContainer>
            {
                CreateLatticeRecycleTool(),
                CreateMmcfeImportTool()
            };
            ToolContainers.SetCollection(list);
        }

        /// <summary>
        ///     Creates a <see cref="ToolContainer"/> for the lattice recycling tool
        /// </summary>
        /// <returns></returns>
        protected ToolContainer CreateLatticeRecycleTool()
        {
            const string name = "Lattice Recycle Tool";
            const string description = "Exports result lattice data from a populated msl database and imports the states as initial lattice into a compatible target msl.";
            return new ToolContainer(() => new LatticeRecycleToolView(), () => new LatticeRecycleToolViewModel(), name, description);
        }

        /// <summary>
        ///     Creates a new <see cref="ToolContainer"/> for the mmcfe data import tool
        /// </summary>
        /// <returns></returns>
        protected ToolContainer CreateMmcfeImportTool()
        {
            const string name = "Mmcfe Import Tool";
            const string description = "Imports data from the MMCFE routine and creates the required raw and evaluation databases.";
            return new ToolContainer(() => new MmcfeImportToolView(), () => new MmcfeImportToolViewModel(), name, description);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            ToolContainers.Items.Action(x => x.Dispose()).Load();
        }
    }
}