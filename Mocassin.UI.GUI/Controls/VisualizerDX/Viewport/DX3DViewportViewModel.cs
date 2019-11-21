using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using Mocassin.Framework.Random;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Controls.VisualizerDX.Viewport.Helper;
using SharpDX;
using Color = System.Windows.Media.Color;
using Matrix = SharpDX.Matrix;

namespace Mocassin.UI.GUI.Controls.VisualizerDX.Viewport
{
    /// <summary>
    ///     The <see cref="ViewModelBase" /> implementation that manages 3D object data for the
    ///     <see cref="DX3DModelDataView" />
    /// </summary>
    public class DX3DViewportViewModel : ViewModelBase, IDisposable
    {
        public static EffectsManager SharedEffectsManager { get; } = new DefaultEffectsManager();

        private Camera camera = new PerspectiveCamera();
        private CameraMode cameraMode = CameraMode.Inspect;
        private CameraRotationMode cameraRotationMode = CameraRotationMode.Turntable;
        private Color backgroundColor = Colors.Transparent;
        private bool showViewCube;
        private bool showCoordinateSystem;
        private bool showRenderInformation = true;
        private MSAALevel msaaLevel = MSAALevel.Four;
        private FXAALevel fxaaLevel = FXAALevel.None;
        private bool isSsaoEnabled;
        private SSAOQuality ssaoQuality = SSAOQuality.Low;
        private bool useMeshBatching;

        /// <summary>
        ///     Get the <see cref="HelixToolkit.Wpf.SharpDX.EffectsManager" /> for the 3D system
        /// </summary>
        public EffectsManager EffectsManager { get; private set; }

        /// <summary>
        ///     Get or set the <see cref="HelixToolkit.Wpf.SharpDX.Camera" />
        /// </summary>
        public Camera Camera
        {
            get => camera;
            set => SetProperty(ref camera, value);
        }

        /// <summary>
        ///     Get or set the <see cref="HelixToolkit.Wpf.SharpDX.CameraMode" />
        /// </summary>
        public CameraMode CameraMode
        {
            get => cameraMode;
            set => SetProperty(ref cameraMode, value);
        }

        /// <summary>
        ///     Get or set the <see cref="HelixToolkit.Wpf.SharpDX.CameraRotationMode" />
        /// </summary>
        public CameraRotationMode CameraRotationMode
        {
            get => cameraRotationMode;
            set => SetProperty(ref cameraRotationMode, value);
        }

        /// <summary>
        ///     Get or set the background <see cref="System.Windows.Media.Color" />
        /// </summary>
        public Color BackgroundColor
        {
            get => backgroundColor;
            set => SetProperty(ref backgroundColor, value);
        }

        /// <summary>
        ///     Get or set a boolean value if the view cube is shown
        /// </summary>
        public bool ShowViewCube
        {
            get => showViewCube;
            set => SetProperty(ref showViewCube, value);
        }

        /// <summary>
        ///     Get or set a boolean value if the coordinate system is shown
        /// </summary>
        public bool ShowCoordinateSystem
        {
            get => showCoordinateSystem;
            set => SetProperty(ref showCoordinateSystem, value);
        }

        /// <summary>
        ///     Get or set a boolean flag if the render information (Frame info, triangle count, camera info, etc. are shown)
        /// </summary>
        public bool ShowRenderInformation
        {
            get => showRenderInformation;
            set => SetProperty(ref showRenderInformation, value);
        }

        /// <summary>
        ///     Get or set the <see cref="MSAALevel" /> for rendering
        /// </summary>
        public MSAALevel MsaaLevel
        {
            get => msaaLevel;
            set => SetProperty(ref msaaLevel, value);
        }

        /// <summary>
        ///     Get or set the <see cref="FXAALevel" /> for rendering (Has no effect if <see cref="MSAALevel.Disable" /> is not
        ///     set)
        /// </summary>
        public FXAALevel FxaaLevel
        {
            get => fxaaLevel;
            set => SetProperty(ref fxaaLevel, value);
        }

        /// <summary>
        ///     Get or set a boolean value that indicates if SSAO is enabled during rendering
        /// </summary>
        public bool IsSsaoEnabled
        {
            get => isSsaoEnabled;
            set => SetProperty(ref isSsaoEnabled, value);
        }

        /// <summary>
        ///     Get or set the <see cref="SSAOQuality" /> for rendering
        /// </summary>
        public SSAOQuality SsaoQuality
        {
            get => ssaoQuality;
            set => SetProperty(ref ssaoQuality, value);
        }

        /// <summary>
        ///     Get or set a boolean flag if the system should batch meshes
        /// </summary>
        public bool UseMeshBatching
        {
            get => useMeshBatching;
            set => SetProperty(ref useMeshBatching, value);
        }

        /// <summary>
        ///     Get the <see cref="ObservableElement3DCollection" /> that supplies the <see cref="Light3D" /> items for the scene
        /// </summary>
        public ObservableElement3DCollection SceneLight3D { get; }

        /// <summary>
        ///     Get the <see cref="ObservableElement3DCollection" /> that supplies the <see cref="Element3D" /> items to the
        /// </summary>
        public ObservableElement3DCollection SceneElements3D { get; }

        /// <summary>
        ///     Creates a new <see cref="DX3DViewportViewModel" />
        /// </summary>
        public DX3DViewportViewModel()
        {
            EffectsManager = SharedEffectsManager;
            SceneElements3D = new ObservableElement3DCollection();
            SceneLight3D = new ObservableElement3DCollection {Light3DHelper.CreateDefaultLightModel()};
            LoadTestDataNodes();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            CleanElementCollections();
            EffectsManager = null;
            OnPropertyChanged(nameof(EffectsManager));
        }

        private async void LoadTestDataNodes()
        {
            var meshBuilder = new MeshBuilder();
            meshBuilder.AddSphere(new Vector3(0, 0, 0));
            var mesh = meshBuilder.ToMesh();

            var rng = new PcgRandom32();
            var material = PhongMaterials.Red;
            material.Freeze();

            var sceneBuilder = new SceneBuilder();
            await sceneBuilder.AddMeshTransformsAsync(mesh, material, new List<Matrix>() {Matrix.Identity});
            var geometryModel = sceneBuilder.ToModel();
            geometryModel.IsHitTestVisible = false;
            ExecuteOnAppThread(() => SceneElements3D.Add(geometryModel));
        }

        /// <summary>
        ///     Cleans the <see cref="Element3D"/> containers
        /// </summary>
        private void CleanElementCollections()
        {
            foreach (var item in SceneElements3D) item.Dispose();
            foreach (var item in SceneLight3D) item.Dispose();
            SceneElements3D.Clear();
            SceneLight3D.Clear();
        }
    }
}