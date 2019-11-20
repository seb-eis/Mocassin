using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf.SharpDX;
using Mocassin.Framework.Random;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Controls.VisualizerDX.Viewport.Helper;
using SharpDX;
using Camera = HelixToolkit.Wpf.SharpDX.Camera;
using PerspectiveCamera = HelixToolkit.Wpf.SharpDX.PerspectiveCamera;

namespace Mocassin.UI.GUI.Controls.VisualizerDX.Viewport
{
    /// <summary>
    ///     The <see cref="ViewModelBase"/> implementation that manages 3D object data for the <see cref="DX3DModelDataView"/>
    /// </summary>
    public class DX3DViewportViewModel : ViewModelBase, IDisposable
    {
        private static EffectsManager SharedEffectsManager { get; } = new DefaultEffectsManager();

        private Camera camera = new PerspectiveCamera();
        private CameraMode cameraMode = CameraMode.Inspect;
        private CameraRotationMode cameraRotationMode = CameraRotationMode.Turntable;
        private System.Windows.Media.Color backgroundColor = Colors.Transparent;
        private bool showViewCube;
        private bool showCoordinateSystem;
        private MSAALevel msaaLevel = MSAALevel.Four;
        private FXAALevel fxaaLevel = FXAALevel.None;
        private bool isSsaoEnabled;
        private SSAOQuality ssaoQuality = SSAOQuality.Low;
        private bool showFrameRate = true;
        private bool showTriangleCountInfo = true;
        private bool showCameraInfo = true;
        private bool showFrameDetails = true;
        private Light3DCollection sceneLight3D;

        /// <summary>
        ///     Get the <see cref="HelixToolkit.Wpf.SharpDX.EffectsManager"/> for the 3D system
        /// </summary>
        public EffectsManager EffectsManager => SharedEffectsManager;

        /// <summary>
        ///     Get or set the <see cref="HelixToolkit.Wpf.SharpDX.Camera"/>
        /// </summary>
        public Camera Camera
        {
            get => camera;
            set => SetProperty(ref camera, value);
        }

        /// <summary>
        ///     Get or set the <see cref="HelixToolkit.Wpf.SharpDX.CameraMode"/> 
        /// </summary>
        public CameraMode CameraMode
        {
            get => cameraMode;
            set => SetProperty(ref cameraMode, value);
        }

        /// <summary>
        ///     Get or set the <see cref="HelixToolkit.Wpf.SharpDX.CameraRotationMode"/>
        /// </summary>
        public CameraRotationMode CameraRotationMode
        {
            get => cameraRotationMode;
            set => SetProperty(ref cameraRotationMode, value);
        }

        /// <summary>
        ///     Get or set the background <see cref="System.Windows.Media.Color"/>
        /// </summary>
        public System.Windows.Media.Color BackgroundColor
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
        ///     Get or set a boolean flag if the framerate is shown
        /// </summary>
        public bool ShowFrameRate
        {
            get => showFrameRate;
            set => SetProperty(ref showFrameRate, value);
        }

        /// <summary>
        ///     Get or set a boolean flag if the triangle count is shown
        /// </summary>
        public bool ShowTriangleCountInfo
        {
            get => showTriangleCountInfo;
            set => SetProperty(ref showTriangleCountInfo, value);
        }

        /// <summary>
        ///     Get or set a boolean flag if the camera info is shown
        /// </summary>
        public bool ShowCameraInfo
        {
            get => showCameraInfo;
            set => SetProperty(ref showCameraInfo, value);
        }

        /// <summary>
        ///     Get or set a boolean flag if the frame details are shown
        /// </summary>
        public bool ShowFrameDetails
        {
            get => showFrameDetails;
            set => SetProperty(ref showFrameDetails, value);
        }

        /// <summary>
        ///     Get or set the <see cref="MSAALevel"/> for rendering
        /// </summary>
        public MSAALevel MsaaLevel
        {
            get => msaaLevel;
            set => SetProperty(ref msaaLevel, value);
        }

        /// <summary>
        ///     Get or set the <see cref="FXAALevel"/> for rendering (Has no effect if <see cref="MSAALevel.Disable"/> is not set)
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
        ///     Get or set the <see cref="SSAOQuality"/> for rendering
        /// </summary>
        public SSAOQuality SsaoQuality
        {
            get => ssaoQuality;
            set => SetProperty(ref ssaoQuality, value);
        }

        /// <summary>
        ///     Get the <see cref="Light3DCollection"/> that supplies the <see cref="Light3D"/> items for the scene
        /// </summary>
        public Light3DCollection SceneLight3D
        {
            get => sceneLight3D;
            set => SetProperty(ref sceneLight3D, value);
        }

        /// <summary>
        ///     Get the <see cref="ObservableElement3DCollection"/> that supplies the <see cref="Element3D"/> items to the
        /// </summary>
        public ObservableElement3DCollection SceneElements3D { get; }

        /// <summary>
        ///     Creates a new <see cref="DX3DViewportViewModel"/>
        /// </summary>
        public DX3DViewportViewModel()
        {
            SceneElements3D = new ObservableElement3DCollection();
            SceneLight3D = LightHelper.GetDefaultLight();
            LoadTestData();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var element in SceneElements3D) element.Dispose();
            SceneElements3D.Clear();
        }

        private void LoadTestData()
        {
            var meshBuilder = new MeshBuilder();
            meshBuilder.AddSphere(new Vector3(0,0,0), 1, 16, 16);
            var mesh = meshBuilder.ToMesh();

            var geometryModel = new GroupModel3D();
            var rng = new PcgRandom32();
            var material = DiffuseMaterials.Glass;
            material.Freeze();
            for (var i = 0; i < 500; i++)
            {
                var transform = new TranslateTransform3D(rng.NextDouble() * 100, rng.NextDouble()* 100, rng.NextDouble()* 100);
                transform.Freeze();
                var model = new MeshGeometryModel3D {Geometry = mesh, Material = material, Transform = transform};
                geometryModel.Children.Add(model);
            }

            geometryModel.IsHitTestVisible = false;
            SceneElements3D.Add(geometryModel);
        }
    }
}