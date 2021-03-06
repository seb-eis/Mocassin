﻿using System.Collections.Generic;
using System.Linq;
using Mocassin.Symmetry.SpaceGroups;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.StructureModel.Adapter;
using Mocassin.UI.Data.Main;
using Mocassin.UI.Data.StructureModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.StructureModel.DataControl
{
    /// <summary>
    ///     The <see cref="ProjectGraphControlViewModel" /> for <see cref="StructureParameterControlView" /> that controls
    ///     unique
    ///     structure model parameters
    /// </summary>
    public class StructureParameterControlViewModel : ProjectGraphControlViewModel
    {
        private CrystalParameterSetter parameterSetter;
        private ISpaceGroup selectedSpaceGroup;
        private StructureInfoData structureInfo;
        private StructureModelData structureModelData;
        private static IList<ISpaceGroup> CachedGroups { get; set; }

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
        ///     Get the <see cref="IEnumerable{T}" /> of the current <see cref="ISymmetryOperation" /> collection
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
        public StructureParameterControlViewModel(IProjectAppControl projectControl)
            : base(projectControl)
        {
            CachedGroups ??= projectControl?.ServiceModelProject.SpaceGroupService.GetFullGroupList();
            SpaceGroupService = new SpaceGroupService(ProjectControl.ServiceModelProject.GeometryNumeric.RangeComparer);
            SpaceGroups = CachedGroups;
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
                       ?? SpaceGroups.First(x => x.InternationalIndex == 1);
            }

            return SpaceGroups.First(x => x.InternationalIndex == 1);
        }

        /// <summary>
        ///     Creates a <see cref="CrystalParameterSetter" /> for the current <see cref="MocassinProject" /> using the provided
        ///     <see cref="ISpaceGroup" />
        /// </summary>
        /// <param name="spaceGroup"></param>
        /// <returns></returns>
        public CrystalParameterSetter CreateParameterSetter(ISpaceGroup spaceGroup)
        {
            var crystalSystem = ProjectControl.ServiceModelProject.CrystalSystemService.GetSystem(spaceGroup);
            var cellParametersData = StructureModelData?.CellParameters;
            if (cellParametersData != null) return new CrystalParameterSetter(crystalSystem, cellParametersData);

            cellParametersData = new CellParametersData();
            cellParametersData.PopulateFrom(crystalSystem.GetDefaultParameterSet());
            return new CrystalParameterSetter(crystalSystem, cellParametersData);
        }
    }
}