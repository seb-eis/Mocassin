using System.Collections.Generic;
using System.Linq;
using Mocassin.Symmetry.SpaceGroups;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.StructureModel.Adapter;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.StructureModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.StructureModel.DataControl
{
    /// <summary>
    ///     The <see cref="ProjectGraphControlViewModel" /> for <see cref="StructureParameterControlView" /> that controls
    ///     unique
    ///     structure model parameters
    /// </summary>
    public class StructureParameterControlViewModel : ProjectGraphControlViewModel
    {
        private static IList<ISpaceGroup> spaceGroups;
        private StructureModelData structureModelData;
        private ISpaceGroup selectedSpaceGroup;
        private CrystalParameterSetter parameterSetter;
        private StructureInfoData structureInfo;

        /// <summary>
        ///     Get or set the current <see cref="StructureModelData" />
        /// </summary>
        public StructureModelData StructureModelData
        {
            get => structureModelData;
            protected set => SetProperty(ref structureModelData, value);
        }

        /// <summary>
        ///     Get or set the selected <see cref="ISpaceGroup" />
        /// </summary>
        public ISpaceGroup SelectedSpaceGroup
        {
            get => selectedSpaceGroup;
            set
            {
                SetProperty(ref selectedSpaceGroup, value);
                StructureModelData?.SpaceGroupInfo?.PopulateFrom(value?.GetGroupEntry());
                ParameterSetter = CreateParameterSetter(value);
                SpaceGroupService.LoadGroup(value ?? SpaceGroups.First());
                OnPropertyChanged(nameof(CurrentSymmetryOperations));
            }
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}"/> of the current <see cref="ISymmetryOperation"/> collection
        /// </summary>
        public IEnumerable<ISymmetryOperation> CurrentSymmetryOperations => SelectedSpaceGroup?.Operations;

        /// <summary>
        ///     Get or set the <see cref="CrystalParameterSetter" /> to manipulate the crystal parameters
        /// </summary>
        public CrystalParameterSetter ParameterSetter
        {
            get => parameterSetter;
            protected set => SetProperty(ref parameterSetter, value);
        }

        /// <summary>
        ///     Get or set the <see cref="StructureInfoData" /> that controls misc information
        /// </summary>
        public StructureInfoData StructureInfo
        {
            get => structureInfo;
            protected set => SetProperty(ref structureInfo, value);
        }

        /// <summary>
        ///     Get a <see cref="IReadOnlyList{T}" /> of all available <see cref="ISpaceGroup" /> instances
        /// </summary>
        public IList<ISpaceGroup> SpaceGroups { get; }

        /// <summary>
        ///     Get the local <see cref="ISpaceGroupService" /> of the control
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
        public override void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            StructureModelData = ContentSource?.ProjectModelData?.StructureModelData;
            StructureInfo = StructureModelData?.StructureInfo;
            SelectedSpaceGroup = FindSpaceGroup(StructureModelData?.SpaceGroupInfo?.GetSpaceGroupEntry());
        }

        /// <summary>
        ///     Finds the <see cref="ISpaceGroup" /> that matches the passed <see cref="SpaceGroupEntry" />
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
        ///     Creates a <see cref="CrystalParameterSetter" /> for the current <see cref="MocassinProject" /> using the
        ///     provided <see cref="ISpaceGroup" />
        /// </summary>
        /// <param name="spaceGroup"></param>
        /// <returns></returns>
        public CrystalParameterSetter CreateParameterSetter(ISpaceGroup spaceGroup)
        {
            var crystalSystem = ProjectControl.ServiceModelProject.CrystalSystemService.GetSystem(spaceGroup);
            var parameterGraph = StructureModelData?.CellParameters;
            if (parameterGraph != null) return new CrystalParameterSetter(crystalSystem, parameterGraph);

            parameterGraph = new CellParametersData();
            parameterGraph.PopulateFrom(crystalSystem.GetDefaultParameterSet());
            return new CrystalParameterSetter(crystalSystem, parameterGraph);
        }
    }
}