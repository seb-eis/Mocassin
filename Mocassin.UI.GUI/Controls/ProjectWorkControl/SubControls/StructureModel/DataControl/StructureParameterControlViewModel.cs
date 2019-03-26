using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Mocassin.Symmetry.CrystalSystems;
using Mocassin.Symmetry.SpaceGroups;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.StructureModel.Adapter;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.StructureModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.StructureModel.DataControl
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for <see cref="StructureParameterControlView" /> that controls unique
    ///     structure model parameters
    /// </summary>
    public class StructureParameterControlViewModel : PrimaryControlViewModel, IContentSupplier<MocassinProjectGraph>
    {
        private static IList<ISpaceGroup> spaceGroups;
        private StructureModelGraph modelGraph;
        private ISpaceGroup selectedSpaceGroup;
        private CrystalParameterSetter parameterSetter;
        private StructureInfoGraph structureInfo;

        /// <summary>
        ///     Get or set the current <see cref="StructureModelGraph"/>
        /// </summary>
        public StructureModelGraph ModelGraph
        {
            get => modelGraph;
            protected set => SetProperty(ref modelGraph, value);
        }

        /// <summary>
        ///     Get or set the selected <see cref="ISpaceGroup"/>
        /// </summary>
        public ISpaceGroup SelectedSpaceGroup
        {
            get => selectedSpaceGroup;
            set
            {
                SetProperty(ref selectedSpaceGroup,value);
                ModelGraph?.SpaceGroupInfo?.PopulateFrom(value?.GetGroupEntry());
                ParameterSetter = CreateParameterSetter(value);
                SpaceGroupService.LoadGroup(value ?? SpaceGroups.First());
            }
        }

        /// <summary>
        ///     Get or set the <see cref="CrystalParameterSetter"/> to manipulate the crystal parameters
        /// </summary>
        public CrystalParameterSetter ParameterSetter
        {
            get => parameterSetter;
            protected set => SetProperty(ref parameterSetter, value);
        }

        /// <summary>
        ///     Get or set the <see cref="StructureInfoGraph"/> that controls misc information
        /// </summary>
        public StructureInfoGraph StructureInfo
        {
            get => structureInfo;
            protected set => SetProperty(ref structureInfo, value);
        }

        /// <summary>
        ///     Get a <see cref="IReadOnlyList{T}"/> of all available <see cref="ISpaceGroup"/> instances
        /// </summary>
        public IList<ISpaceGroup> SpaceGroups { get; }

        /// <summary>
        ///     Get the local <see cref="ISpaceGroupService"/> of the control
        /// </summary>
        public ISpaceGroupService SpaceGroupService { get; }

        /// <inheritdoc />
        public StructureParameterControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            spaceGroups = spaceGroups ?? projectControl?.ServiceModelProject.SpaceGroupService.GetFullGroupList();
            SpaceGroupService = new SpaceGroupService(ProjectControl.ServiceModelProject.GeometryNumeric.RangeComparer);
            SpaceGroups = spaceGroups;
        }

        /// <inheritdoc />
        public void ChangeContentSource(object contentSource)
        {
            if (ContentSource is MocassinProjectGraph projectGraph) ChangeContentSource(projectGraph);
        }

        /// <inheritdoc />
        public MocassinProjectGraph ContentSource { get; set; }

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            ModelGraph = ContentSource?.ProjectModelGraph?.StructureModelGraph;
            StructureInfo = ModelGraph?.StructureInfo;
            SelectedSpaceGroup = FindSpaceGroup(ModelGraph?.SpaceGroupInfo?.GetSpaceGroupEntry());
        }

        /// <summary>
        ///     Finds the <see cref="ISpaceGroup"/> that matches the passed <see cref="SpaceGroupEntry"/>
        /// </summary>
        /// <param name="spaceGroupEntry"></param>
        /// <returns></returns>
        private ISpaceGroup FindSpaceGroup(SpaceGroupEntry spaceGroupEntry)
        {
            if (spaceGroupEntry != null)
            {
                return SpaceGroups.SingleOrDefault(x => x.GetGroupEntry().Equals(spaceGroupEntry))
                       ?? SpaceGroups.First(x => x.Index == 1);
            }

            return SpaceGroups.First(x => x.Index == 1);
        }

        /// <summary>
        ///     Creates a <see cref="CrystalParameterSetter"/> for the current <see cref="MocassinProjectGraph"/> using the provided <see cref="ISpaceGroup"/>
        /// </summary>
        /// <param name="spaceGroup"></param>
        /// <returns></returns>
        public CrystalParameterSetter CreateParameterSetter(ISpaceGroup spaceGroup)
        {
            var crystalSystem = ProjectControl.ServiceModelProject.CrystalSystemService.GetSystem(spaceGroup);
            var parameterGraph = ModelGraph?.CellParameters;
            if (parameterGraph != null) return new CrystalParameterSetter(crystalSystem, parameterGraph);

            parameterGraph = new CellParametersGraph();
            parameterGraph.PopulateFrom(crystalSystem.GetDefaultParameterSet());
            return new CrystalParameterSetter(crystalSystem, parameterGraph);
        }
    }
}