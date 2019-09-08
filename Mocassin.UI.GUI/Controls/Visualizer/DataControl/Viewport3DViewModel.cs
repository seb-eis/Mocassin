using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using Mocassin.Framework.Extensions;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Controls.Visualizer.Objects;

namespace Mocassin.UI.GUI.Controls.Visualizer.DataControl
{
    /// <summary>
    ///     A <see cref="ViewModelBase" /> instance that manages a <see cref="ModelVisual3D" /> collection an affiliated data
    ///     for a
    ///     <see cref="HelixViewport3D" />
    /// </summary>
    public class Viewport3DViewModel : ViewModelBase, IDisposable
    {
        private LightSetup lightSetup;
        private ModelVisual3D selectedVisual;
        private bool isAutoUpdating;
        private bool isVisualCubeActive;
        private bool isCoordinateSystemActive;
        private bool isFrameRateCounterActive;
        private bool isCameraInfoActive;
        private bool isRenderInfoActive;

        /// <summary>
        ///     Get the default light setup for the visual collection
        /// </summary>
        public LightSetup DefaultLightSetup { get; }

        /// <summary>
        ///     Get the <see cref="ObservableCollection{T}" /> for the <see cref="ModelVisual3D" /> data
        /// </summary>
        public ObservableCollection<ModelVisual3D> Visuals { get; }

        /// <summary>
        ///     Get the set of <see cref="IVisualGroupViewModel" /> instances that define the content
        /// </summary>
        public ObservableCollection<IVisualGroupViewModel> VisualGroups { get; }

        /// <summary>
        ///     Get or set the <see cref="LightSetup" /> of the collection
        /// </summary>
        public LightSetup LightSetup
        {
            get => lightSetup;
            set
            {
                ReplaceUniqueVisual(lightSetup, value);
                SetProperty(ref lightSetup, value);
            }
        }

        /// <summary>
        ///     Get or set the currently selected <see cref="ModelVisual3D" />
        /// </summary>
        public ModelVisual3D SelectedVisual
        {
            get => selectedVisual;
            set => SetProperty(ref selectedVisual, value);
        }

        /// <summary>
        ///     Get or set a boolean flag if the system should auto update changes on the visual group
        /// </summary>
        public bool IsAutoUpdating
        {
            get => isAutoUpdating;
            set => SetProperty(ref isAutoUpdating, value);
        }

        /// <summary>
        ///     Get or set a boolean flag if the visual cube is active
        /// </summary>
        public bool IsVisualCubeActive
        {
            get => isVisualCubeActive;
            set => SetProperty(ref isVisualCubeActive, value);
        }

        /// <summary>
        ///     Get or set a boolean flag if the coordinate system visual is active
        /// </summary>
        public bool IsCoordinateSystemActive
        {
            get => isCoordinateSystemActive;
            set => SetProperty(ref isCoordinateSystemActive, value);
        }

        /// <summary>
        ///     Get or set a boolean flag if the frame-rate counter is active
        /// </summary>
        public bool IsFrameRateCounterActive
        {
            get => isFrameRateCounterActive;
            set => SetProperty(ref isFrameRateCounterActive, value);
        }

        /// <summary>
        ///     Get or set a boolean flag if the render info is active
        /// </summary>
        public bool IsRenderInfoActive
        {
            get => isRenderInfoActive;
            set => SetProperty(ref isRenderInfoActive, value);
        }

        /// <summary>
        ///     Get or set a boolean flag if the camera info is active
        /// </summary>
        public bool IsCameraInfoActive
        {
            get => isCameraInfoActive;
            set => SetProperty(ref isCameraInfoActive, value);
        }

        /// <summary>
        ///     Get a <see cref="ParameterlessCommand" /> to clear the visual data and reset to minimum contents
        /// </summary>
        public ParameterlessCommand ClearVisualCommand { get; }

        /// <summary>
        ///     Get a <see cref="ParameterlessCommand" /> to resynchronize the visual collection with the visual groups
        /// </summary>
        public ParameterlessCommand UpdateVisualCommand { get; }

        /// <summary>
        ///     Create sa new <see cref="Viewport3DViewModel" /> with default settings
        /// </summary>
        public Viewport3DViewModel()
        {
            isAutoUpdating = true;
            Visuals = new ObservableCollection<ModelVisual3D>();
            VisualGroups = new ObservableCollection<IVisualGroupViewModel>();
            ClearVisualCommand = new RelayCommand(ClearVisual);
            UpdateVisualCommand = new RelayCommand(UpdateVisual);
            DefaultLightSetup = new DefaultLights();
            ClearVisual();
        }


        /// <summary>
        ///     Resets the visual collection to contain only the light setup
        /// </summary>
        public void ClearVisual()
        {
            ExecuteOnDispatcher(() =>
            {
                Visuals.Clear();
                Visuals.Add(LightSetup ?? DefaultLightSetup);
            });
        }

        /// <summary>
        ///     Updates the visual collection to the current state of the visual groups
        /// </summary>
        public void UpdateVisual()
        {
            ClearVisual();
            ExecuteOnDispatcher(() =>
            {
                foreach (var visualGroup in VisualGroups.Where(x => x.IsVisible && x.ModelVisual != null))
                    Visuals.Add(visualGroup.ModelVisual);
            });
        }

        /// <summary>
        ///     Clears the visual groups
        /// </summary>
        public void ClearVisualGroups()
        {
            foreach (var visualGroup in VisualGroups.Where(x => x != null))
            {
                visualGroup.Dispose();
                visualGroup.PropertyChanged -= AutoUpdateVisualInternal;
            }

            ExecuteOnDispatcher(() => VisualGroups.Clear());
            ClearVisual();
        }

        /// <summary>
        ///     Adds a new <see cref="IVisualGroupViewModel" /> to the view port
        /// </summary>
        /// <param name="visualGroup"></param>
        public void AddVisualGroup(IVisualGroupViewModel visualGroup)
        {
            if (visualGroup == null) throw new ArgumentNullException(nameof(visualGroup));
            if (VisualGroups.Contains(visualGroup)) return;

            visualGroup.PropertyChanged += AutoUpdateVisualInternal;
            ExecuteOnDispatcher(() => VisualGroups.Add(visualGroup));
        }

        /// <summary>
        ///     Adds a sequence of <see cref="Visual3D" /> objects as a new named <see cref="IVisualGroupViewModel" />
        /// </summary>
        /// <param name="visuals"></param>
        /// <param name="name"></param>
        /// <param name="isVisible"></param>
        public void AddVisualGroup(IEnumerable<Visual3D> visuals, string name, bool isVisible = true)
        {
            var modelVisual = new ModelVisual3D();
            modelVisual.Children.AddMany(visuals);

            var visualGroup = new VisualGroupViewModel
            {
                IsVisible = isVisible, Name = name ?? "New group", ModelVisual = modelVisual
            };
            AddVisualGroup(visualGroup);
        }

        /// <summary>
        ///     Replaces a unique visual component of the collection
        /// </summary>
        /// <param name="oldVisual"></param>
        /// <param name="newVisual"></param>
        private void ReplaceUniqueVisual(ModelVisual3D oldVisual, ModelVisual3D newVisual)
        {
            Visuals.Remove(oldVisual);
            Visuals.Add(newVisual);
        }

        /// <summary>
        ///     Creates a <see cref="Visual3D" /> from a data object, and ensures that the API call is made from the dispatcher
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TVisual"></typeparam>
        /// <param name="data"></param>
        /// <param name="generator"></param>
        /// <returns></returns>
        public TVisual CreateVisual<TData, TVisual>(TData data, Func<TData, TVisual> generator)
        {
            return ExecuteOnDispatcher(() => generator(data));
        }

        /// <summary>
        ///     Creates a read only collection of <see cref="Visual3D" /> from a data object sequence and a generator function, and
        ///     ensures that the API call is made from the dispatcher
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TVisual"></typeparam>
        /// <param name="data"></param>
        /// <param name="generator"></param>
        /// <returns></returns>
        public ReadOnlyCollection<TVisual> CreateVisual<TData, TVisual>(ICollection<TData> data, Func<TData, TVisual> generator)
        {
            ReadOnlyCollection<TVisual> CreateInternal()
            {
                var result = new List<TVisual>(data.Count);
                result.AddRange(data.Select(generator));

                return result.AsReadOnly();
            }

            return ExecuteOnDispatcher(CreateInternal);
        }

        /// <summary>
        ///     Creates a factory <see cref="Func{T,TResult}" /> to produce <see cref="MeshGeometryVisual3D" /> sharing a common
        ///     <see cref="MeshGeometry3D" /> and a custom <see cref="Transform3D" />
        /// </summary>
        /// <param name="meshGeometry"></param>
        /// <returns></returns>
        public Func<Transform3D, MeshGeometryVisual3D> BuildMeshVisualFactory(MeshGeometry3D meshGeometry)
        {
            MeshGeometryVisual3D CreateInternal(Transform3D transform)
            {
                return new MeshGeometryVisual3D
                {
                    MeshGeometry = meshGeometry, Transform = transform
                };
            }

            return CreateInternal;
        }

        /// <summary>
        ///     Get a generator <see cref="Func{T,TResult}" /> to produce multiple <see cref="MeshGeometryVisual3D" /> objects in a
        ///     sphere shape sharing a common mesh for multiple <see cref="Transform3D" /> operations
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="thetaDiv"></param>
        /// <param name="phiDiv"></param>
        /// <param name="freezeMesh"></param>
        /// <returns></returns>
        public Func<Transform3D, MeshGeometryVisual3D> BuildSphereVisualFactory(double radius, int thetaDiv, int phiDiv,
            bool freezeMesh = true)
        {
            var meshBuilder = new MeshBuilder();
            meshBuilder.AddSphere(new Point3D(0, 0, 0), radius, thetaDiv, phiDiv);
            var meshGeometry = meshBuilder.ToMesh(freezeMesh);

            return BuildMeshVisualFactory(meshGeometry);
        }

        /// <summary>
        ///     Get a generator <see cref="Func{T,TResult}" /> to produce multiple <see cref="MeshGeometryVisual3D" /> objects in a
        ///     cube shape sharing a common mesh for multiple <see cref="Transform3D" /> operations
        /// </summary>
        /// <param name="sideLength"></param>
        /// <param name="freezeMesh"></param>
        /// <returns></returns>
        public Func<Transform3D, MeshGeometryVisual3D> BuildCubeVisualFactory(double sideLength, bool freezeMesh = true)
        {
            var meshBuilder = new MeshBuilder(false);
            meshBuilder.AddBox(new Point3D(0, 0, 0), sideLength, sideLength, sideLength, BoxFaces.All);
            var meshGeometry = meshBuilder.ToMesh(freezeMesh);

            return BuildMeshVisualFactory(meshGeometry);
        }

        /// <summary>
        ///     Get a generator <see cref="Func{T,TResult}" /> to produce multiple <see cref="MeshGeometryVisual3D" /> objects in a
        ///     arrow shape sharing a common mesh for multiple <see cref="Transform3D" /> operations
        /// </summary>
        /// <param name="diameter"></param>
        /// <param name="headLength"></param>
        /// <param name="thetaDiv"></param>
        /// <param name="freezeMesh"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public Func<Transform3D, MeshGeometryVisual3D> BuildArrowVisualFactory(double diameter, in Point3D start, in Point3D end,
            double headLength, int thetaDiv, bool freezeMesh = true)
        {
            var meshBuilder = new MeshBuilder();
            meshBuilder.AddArrow(start, end, diameter, headLength, thetaDiv);
            var meshGeometry = meshBuilder.ToMesh(freezeMesh);

            return BuildMeshVisualFactory(meshGeometry);
        }


        /// <summary>
        ///     Get a <see cref="TranslateTransform3D" /> to the passed target <see cref="Point3D" /> from the origin (0,0,0)
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Transform3D GetOriginOffsetTransform3D(in Point3D target)
        {
            return new TranslateTransform3D(target.X, target.Y, target.Z);
        }

        /// <summary>
        ///     Colors a set of <see cref="MeshGeometryVisual3D" /> with the provided brush and optionally freezes the brush
        /// </summary>
        /// <param name="visuals"></param>
        /// <param name="brush"></param>
        /// <param name="freezeBrush"></param>
        public void SetMeshGeometryVisualBrush(IEnumerable<MeshGeometryVisual3D> visuals, Brush brush, bool freezeBrush = true)
        {
            if (visuals == null) throw new ArgumentNullException(nameof(visuals));
            if (brush == null) throw new ArgumentNullException(nameof(brush));

            if (freezeBrush && brush.CanFreeze) brush.Freeze();
            foreach (var visual in visuals) visual.Fill = brush;
        }

        /// <summary>
        ///     Update visual as an event delegate that triggers only if the system is set to auto updates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AutoUpdateVisualInternal(object sender, PropertyChangedEventArgs args)
        {
            if (IsAutoUpdating) UpdateVisual();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var visualGroup in VisualGroups) visualGroup?.Dispose();
            VisualGroups.Clear();
            Visuals.Clear();
        }
    }
}