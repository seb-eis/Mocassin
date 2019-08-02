using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using Mocassin.Framework.Collections;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.ModelProject;
using Mocassin.Symmetry.SpaceGroups;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.Collections;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.Visualizer.DataControl;
using Mocassin.UI.GUI.Controls.Visualizer.Objects;
using Mocassin.UI.GUI.Extensions;
using Mocassin.UI.GUI.Properties;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ProjectLibrary;
using Mocassin.UI.Xml.StructureModel;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.GUI.Controls.Visualizer
{
    /// <summary>
    ///     A <see cref="ProjectGraphControlViewModel" /> to manages the provision of model data as visual 3D objects
    /// </summary>
    public class ModelViewport3DViewModel : ProjectGraphControlViewModel
    {
        /// <summary>
        ///     Get or set a boolean flag if the viewport is synchronized with the model data
        /// </summary>
        private bool IsSynchronizedWithModel { get; set; }

        /// <summary>
        ///     Get the <see cref="IModelProject" /> the visualizer uses for utility functions
        /// </summary>
        private IModelProject UtilityProject { get; }

        /// <summary>
        ///     Get the active <see cref="IVectorTransformer" /> matching the model content source
        /// </summary>
        private IVectorTransformer VectorTransformer => UtilityProject.CrystalSystemService.VectorTransformer;

        /// <summary>
        ///     Get the active <see cref="ISpaceGroupService" /> matching the model content source
        /// </summary>
        private ISpaceGroupService SpaceGroupService => UtilityProject.SpaceGroupService;

        /// <summary>
        ///     Get the <see cref="Viewport3DViewModel" /> that manages the visual objects
        /// </summary>
        public Viewport3DViewModel VisualViewModel { get; }

        /// <summary>
        ///     Provides an <see cref="ObservableCollectionViewModel{T}" /> for <see cref="ModelObject3DViewModel" />
        /// </summary>
        public ObservableCollectionViewModel<ModelObject3DViewModel> ModelObjectViewModels { get; }

        /// <summary>
        ///     Get a <see cref="ParameterlessCommand" /> to update the model object render data list
        /// </summary>
        public ParameterlessCommand UpdateObjectViewModelsCommand { get; }

        /// <summary>
        ///     Get a <see cref="ParameterlessCommand" /> to refresh the visual object layer contents
        /// </summary>
        public ParameterlessCommand RefreshVisualGroupsCommand { get; }

        /// <inheritdoc />
        public ModelViewport3DViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            UtilityProject = projectControl.CreateModelProject();
            VisualViewModel = new Viewport3DViewModel();
            ModelObjectViewModels = new ObservableCollectionViewModel<ModelObject3DViewModel>();
            UpdateObjectViewModelsCommand = new RelayCommand(SynchronizeWithModel);
            RefreshVisualGroupsCommand = new RelayCommand(RefreshVisualGroups);
        }

        /// <inheritdoc />
        public override async void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            if (ContentSource == null) return;
            await ExecuteIfConstantContentSource(RefreshVisualContent, TimeSpan.FromMilliseconds(250), true);
        }

        /// <summary>
        ///     Executes a full visual content update of the view model
        /// </summary>
        private void RefreshVisualContent()
        {
            IsSynchronizedWithModel = false;
            VisualViewModel.ClearVisual();
            SynchronizeWithModel();
            RefreshVisualGroups();
            VisualViewModel.UpdateVisual();
        }

        /// <summary>
        ///     Prepares the utility project for usage with the passed <see cref="MocassinProjectGraph" />
        /// </summary>
        /// <param name="projectGraph"></param>
        private void PrepareUtilityProject(MocassinProjectGraph projectGraph)
        {
            UtilityProject.ResetProject();
            if (projectGraph == null) return;

            var spaceGroupData = projectGraph.ProjectModelGraph.StructureModelGraph.SpaceGroupInfo.GetInputObject();
            var cellData = projectGraph.ProjectModelGraph.StructureModelGraph.CellParameters.GetInputObject();
            UtilityProject.InputPipeline.PushToProject(spaceGroupData);
            UtilityProject.InputPipeline.PushToProject(cellData);
        }

        /// <summary>
        ///     Generates the <see cref="ModelObject3DViewModel" /> instances for all displayable <see cref="ModelObjectGraph" />
        ///     instances in the content source
        /// </summary>
        public void UpdateObjectViewModels()
        {
            if (ContentSource == null)
            {
                ModelObjectViewModels.ClearCollection();
                return;
            }

            var results = new List<ModelObject3DViewModel>(ModelObjectViewModels.ObservableItems.Count);

            foreach (var position in ContentSource.ProjectModelGraph.StructureModelGraph.UnitCellPositions)
                results.Add(GetModelObjectViewModel(position));

            foreach (var transition in ContentSource.ProjectModelGraph.TransitionModelGraph.KineticTransitions)
                results.Add(GetModelObjectViewModel(transition));

            ModelObjectViewModels.ClearCollection();
            ModelObjectViewModels.AddCollectionItems(results);
        }

        /// <summary>
        ///     Get the <see cref="ModelObject3DViewModel" /> for the passed <see cref="ModelObjectGraph" /> or creates a new one
        ///     if none exists
        /// </summary>
        /// <param name="objectGraph"></param>
        /// <returns></returns>
        private ModelObject3DViewModel GetModelObjectViewModel(ModelObjectGraph objectGraph)
        {
            return ModelObjectViewModels.ObservableItems.FirstOrDefault(x => x.ObjectGraph == objectGraph)
                   ?? new ModelObject3DViewModel(objectGraph);
        }

        /// <summary>
        ///     Refreshes the set of <see cref="IVisualGroupViewModel" /> for each displayable model object
        /// </summary>
        public void RefreshVisualGroups()
        {
            if (ContentSource == null) return;

            try
            {
                VisualViewModel.ClearVisualGroups();
                SynchronizeWithModel();

                foreach (var item in ContentSource.ProjectModelGraph.StructureModelGraph.UnitCellPositions)
                    VisualViewModel.AddVisualGroup(EnumeratePositionVisuals(item), item.Name);
                foreach (var item in ContentSource.ProjectModelGraph.TransitionModelGraph.KineticTransitions)
                    VisualViewModel.AddVisualGroup(EnumerateTransitionVisuals(item), item.Name);

                if (VisualViewModel.IsAutoUpdating) VisualViewModel.UpdateVisual();
            }
            catch (Exception e)
            {
                OnRenderError(e);
            }
        }

        /// <summary>
        ///     Action to call to inform about an exception in the render process
        /// </summary>
        /// <param name="e"></param>
        /// <param name="callMemberName"></param>
        private void OnRenderError(Exception e, [CallerMemberName] string callMemberName = null)
        {
            SendCallErrorMessage(e, callMemberName);
            MessageBox.Show(Resources.Viewer3D_Error_Visual_Generation, 
                Resources.Viewer3D_Error_Box_Caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        ///     Enumerates the <see cref="SphereVisual3D" /> objects for a <see cref="UnitCellPositionGraph" />
        /// </summary>
        /// <param name="positionGraph"></param>
        /// <returns></returns>
        private IEnumerable<SphereVisual3D> EnumeratePositionVisuals(UnitCellPositionGraph positionGraph)
        {
            var graphViewModel = GetModelObjectViewModel(positionGraph);
            var vector = new Fractional3D(positionGraph.A, positionGraph.B, positionGraph.C);
            var cellPositions = SpaceGroupService.GetPositionsInCuboid(vector, new Fractional3D(), new Fractional3D(1, 1, 1));

            var brush = new SolidColorBrush(graphViewModel.ObjectColor);
            var generator = VisualViewModel.CreateSphereGenerator(graphViewModel.ObjectScaling, brush);
            foreach (var center in cellPositions.Select(x => VectorTransformer.ToCartesian(x).AsPoint3D()))
            {
                var visual = VisualViewModel.CreateVisual(center, generator);
                yield return visual;
            }
        }

        /// <summary>
        ///     Enumerates the <see cref="ArrowVisual3D" /> objects for a <see cref="KineticTransitionGraph" />
        /// </summary>
        /// <param name="transitionGraph"></param>
        /// <returns></returns>
        private IEnumerable<ArrowVisual3D> EnumerateTransitionVisuals(KineticTransitionGraph transitionGraph)
        {
            var graphViewModel = GetModelObjectViewModel(transitionGraph);
            var vectorSequence = transitionGraph.PositionVectors.Select(x => new Fractional3D(x.A, x.B, x.C));

            var sequences = SpaceGroupService.GetAllWyckoffOriginSequences(vectorSequence);
            RemoveInverseSequences(sequences, SpaceGroupService.Comparer);

            var brush = new SolidColorBrush(graphViewModel.ObjectColor);
            var generator = VisualViewModel.CreateDirectionArrowGenerator(graphViewModel.ObjectScaling, brush);
            foreach (var sequence in sequences)
            {
                for (var i = 1; i < sequence.Length; i++)
                {
                    var point1 = VectorTransformer.ToCartesian(sequence[i - 1]).AsPoint3D();
                    var point2 = VectorTransformer.ToCartesian(sequence[i]).AsPoint3D();
                    var direction = CreateDirectionWithPositionCorrection(point1, point2);
                    var visual = VisualViewModel.CreateVisual((point1, direction), generator);
                    yield return visual;
                }
            }
        }

        /// <summary>
        ///     Calculates the corrected direction <see cref="Vector3D" /> under consideration of existing positions spheres
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private Vector3D CreateDirectionWithPositionCorrection(in Point3D start, in Point3D end)
        {
            var direction = end - start;
            var length = direction.Length;
            direction.Normalize();

            var positionVisuals = VisualViewModel.VisualGroups.SelectMany(x => x.Items).Where(x => x is SphereVisual3D);
            var endVector = new Fractional3D(end.X, end.Y, end.Z);

            foreach (var visual in positionVisuals.Cast<SphereVisual3D>())
            {
                var centerVector = new Fractional3D(visual.Center.X, visual.Center.Y, visual.Center.Z);
                if (SpaceGroupService.Comparer.Compare(endVector, centerVector) == 0) return (length - visual.Radius) * direction;
            }

            return length * direction;
        }

        /// <summary>
        ///     Removes the inverse duplicate entries form a list of <see cref="Fractional3D" /> sequences
        /// </summary>
        /// <param name="sequences"></param>
        /// <param name="comparer"></param>
        private void RemoveInverseSequences(IList<Fractional3D[]> sequences, IComparer<Fractional3D> comparer)
        {
            bool IsEqual(Fractional3D[] lhs, Fractional3D[] rhs)
            {
                for (var (i, j) = (0, rhs.Length - 1); i < rhs.Length; (i, j) = (i + 1, j - 1))
                {
                    if (comparer.Compare(lhs[i], rhs[j]) != 0)
                        return false;
                }

                return true;
            }

            sequences.RemoveDuplicates(new EqualityCompareAdapter<Fractional3D[]>(IsEqual, x => x.Length));
        }

        /// <summary>
        ///     Synchronizes the viewport system with the model
        /// </summary>
        private void SynchronizeWithModel()
        {
            if (ContentSource != null && !IsSynchronizedWithModel)
            {
                PrepareUtilityProject(ContentSource);
                UpdateObjectViewModels();
            }

            IsSynchronizedWithModel = true;
        }

        /// <inheritdoc />
        protected override void OnProjectLibraryChangedInternal(IMocassinProjectLibrary newProjectLibrary)
        {
            ChangeContentSource(null);
        }

        /// <inheritdoc />
        protected override void OnProjectContentChangedInternal()
        {
            base.OnProjectContentChangedInternal();
            IsSynchronizedWithModel = false;
        }
    }
}