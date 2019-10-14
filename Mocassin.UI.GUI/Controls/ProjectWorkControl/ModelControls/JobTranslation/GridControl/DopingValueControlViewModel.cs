using System;
using System.Linq;
using Mocassin.Model.Lattices;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Jobs;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.GridControl
{
    /// <summary>
    ///     Get the <see cref="CollectionControlViewModel{T}"/> for <see cref="DopingValueGraph"/> instances
    /// </summary>
    public class DopingValueControlViewModel : CollectionControlViewModel<DopingValueGraph>, IContentSupplier<MocassinProjectGraph>
    {
        /// <inheritdoc />
        public MocassinProjectGraph ContentSource { get; set; }

        /// <summary>
        ///     Get the <see cref="LatticeConfigurationGraph"/> of the model parent
        /// </summary>
        public LatticeConfigurationGraph ParentLatticeConfiguration { get; }

        public DopingValueControlViewModel(LatticeConfigurationGraph parentLatticeConfiguration)
        {
            ParentLatticeConfiguration = parentLatticeConfiguration ?? throw new ArgumentNullException(nameof(parentLatticeConfiguration));
        }

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            if (ContentSource != null)
            {
                if (ParentLatticeConfiguration.DopingValues.Count != ContentSource.ProjectModelGraph.LatticeModelGraph.Dopings.Count)
                {
                    ParentLatticeConfiguration.DopingValues = ContentSource.ProjectModelGraph.LatticeModelGraph.Dopings
                        .Select(x => new DopingValueGraph {Value = 0, Doping = new ModelObjectReferenceGraph<Doping>(x)})
                        .ToList();
                }
            }

            SetCollection(ParentLatticeConfiguration.DopingValues);
        }
    }
}