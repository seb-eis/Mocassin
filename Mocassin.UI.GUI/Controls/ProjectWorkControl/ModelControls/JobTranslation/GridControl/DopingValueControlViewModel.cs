using System;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.Model.Lattices;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Jobs;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.GridControl
{
    /// <summary>
    ///     Get the <see cref="CollectionControlViewModel{T}" /> for <see cref="DopingValueData" /> instances
    /// </summary>
    public class DopingValueControlViewModel : CollectionControlViewModel<DopingValueData>, IContentSupplier<MocassinProject>
    {
        /// <inheritdoc />
        public MocassinProject ContentSource { get; set; }

        /// <summary>
        ///     Get the <see cref="LatticeConfigData" /> of the model parent
        /// </summary>
        public LatticeConfigData ParentLatticeConfiguration { get; }

        /// <inheritdoc />
        public DopingValueControlViewModel(LatticeConfigData parentLatticeConfiguration)
        {
            ParentLatticeConfiguration = parentLatticeConfiguration ?? throw new ArgumentNullException(nameof(parentLatticeConfiguration));
        }

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            if (ContentSource != null)
            {
                if (ParentLatticeConfiguration.DopingValues.Count != ContentSource.ProjectModelData.LatticeModelData.Dopings.Count)
                {
                    ParentLatticeConfiguration.DopingValues = ContentSource.ProjectModelData.LatticeModelData.Dopings
                                                                           .Select(x => new DopingValueData
                                                                               {Value = 0, Doping = new ModelObjectReference<Doping>(x)})
                                                                           .ToObservableCollection();
                }
            }

            SetCollection(ParentLatticeConfiguration.DopingValues);
        }
    }
}