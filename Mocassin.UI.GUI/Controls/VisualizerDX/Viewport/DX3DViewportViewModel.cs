using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Media;
using HelixToolkit.Wpf.SharpDX;
using Mocassin.Framework.Random;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Controls.VisualizerDX.Viewport.Attributes;
using Mocassin.UI.GUI.Controls.VisualizerDX.Viewport.Commands;
using Mocassin.UI.GUI.Controls.VisualizerDX.Viewport.Enums;
using Mocassin.UI.GUI.Controls.VisualizerDX.Viewport.Helper;
using SharpDX;
using SharpDX.Direct3D;
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
        /// <summary>
        ///     Get or set the maximal supported image height of the image exporter. Default is 2160 pixels
        /// </summary>
        public static int MaxImageHeight { get; set; } = 2160;

        /// <summary>
        ///     Get or set the maximal supported image width of the image exporter. Default is 3840 pixels
        /// </summary>
        public static int MaxImageWidth { get; set; } = 3840;

        private Camera camera = new PerspectiveCamera();
        private CameraMode cameraMode = CameraMode.Inspect;
        private CameraRotationMode cameraRotationMode = CameraRotationMode.Turntable;
        private bool showViewCube = true;
        private bool showCoordinateSystem = true;
        private bool showRenderInformation;
        private MSAALevel msaaLevel = MSAALevel.Four;
        private FXAALevel fxaaLevel = FXAALevel.None;
        private bool isSsaoEnabled;
        private SSAOQuality ssaoQuality = SSAOQuality.Low;
        private bool useMeshBatching = true;
        private bool disableMsaaOnInteraction = true;
        private bool isInteracting;
        private Brush infoBackgroundBrush = Brushes.Transparent;
        private Brush infoForegroundBrush = Brushes.Black;
        private Color backgroundColor = Colors.Transparent;
        private bool enableInfiniteSpin;
        private CameraType cameraType = CameraType.Perspective;
        private double cameraFarPlaneDistance = 10000;
        private double cameraNearPlaneDistance = 0.1;
        private double cameraFieldOfView = 45;
        private SceneLightSetting lightSetting = SceneLightSetting.Default;
        private Color lightColor = Colors.White;
        private bool isSettingsActive;
        private int imageExportHeight;
        private int imageExportWidth;

        /// <summary>
        ///     Get the <see cref="HelixToolkit.Wpf.SharpDX.EffectsManager" /> for the 3D system
        /// </summary>
        public EffectsManager EffectsManager { get; }

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
        ///     Get or set the base <see cref="System.Windows.Media.Color" /> of the scene light
        /// </summary>
        [RaiseInvalidateRender]
        public Color LightColor
        {
            get => lightColor;
            set
            {
                if (lightColor == value) return;
                SetProperty(ref lightColor, value);
                OnLightColorChanged(value);
            }
        }

        /// <summary>
        ///     Get or set the background <see cref="System.Windows.Media.Brush" /> for the info area
        /// </summary>
        [RaiseInvalidateRender]
        public Brush InfoBackgroundBrush
        {
            get => infoBackgroundBrush;
            set => SetProperty(ref infoBackgroundBrush, value);
        }

        /// <summary>
        ///     Get or set the foreground <see cref="System.Windows.Media.Brush" /> for the info area
        /// </summary>
        [RaiseInvalidateRender]
        public Brush InfoForegroundBrush
        {
            get => infoForegroundBrush;
            set => SetProperty(ref infoForegroundBrush, value);
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
        ///     Get or set a boolean flag to enable infinite spin behavior
        /// </summary>
        public bool EnableInfiniteSpin
        {
            get => enableInfiniteSpin;
            set => SetProperty(ref enableInfiniteSpin, value);
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
        ///     Get or set a boolean flag if MSAA should be disabled on user interaction
        /// </summary>
        public bool DisableMsaaOnInteraction
        {
            get => disableMsaaOnInteraction;
            set => SetProperty(ref disableMsaaOnInteraction, value);
        }

        /// <summary>
        ///     Get or set a boolean flag if user interaction is in progress
        /// </summary>
        public bool IsInteracting
        {
            get => isInteracting;
            set
            {
                if (isInteracting == value) return;
                SetProperty(ref isInteracting, value);
                OnIsInteractingChanged(value);
            }
        }

        /// <summary>
        ///     Gets or set the used <see cref="CameraType" />
        /// </summary>
        [RaiseInvalidateRender]
        public CameraType CameraType
        {
            get => cameraType;
            set
            {
                if (cameraType == value) return;
                SetProperty(ref cameraType, value);
                OnCameraTypeChanged(value);
            }
        }

        /// <summary>
        ///     Gets or sets the basic <see cref="SceneLightSetting" /> for the scene
        /// </summary>
        public SceneLightSetting LightSetting
        {
            get => lightSetting;
            set
            {
                if (lightSetting == value) return;
                SetProperty(ref lightSetting, value);
                OnLightSettingChanged(value);
            }
        }

        /// <summary>
        ///     Get or set the far plane distance value of the <see cref="Camera" />
        /// </summary>
        [RaiseInvalidateRender]
        public double CameraFarPlaneDistance
        {
            get => cameraFarPlaneDistance;
            set
            {
                SetProperty(ref cameraFarPlaneDistance, value);
                OnCameraFarPlaneDistanceChanged(value);
            }
        }

        /// <summary>
        ///     Get or set  the far plane distance value of the <see cref="Camera" />
        /// </summary>
        [RaiseInvalidateRender]
        public double CameraNearPlaneDistance
        {
            get => cameraNearPlaneDistance;
            set
            {
                SetProperty(ref cameraNearPlaneDistance, value);
                OnCameraNearPlaneDistanceChanged(value);
            }
        }

        /// <summary>
        ///     Get or set the field of view of the <see cref="Camera" />
        /// </summary>
        [RaiseInvalidateRender]
        public double CameraFieldOfView
        {
            get => cameraFieldOfView;
            set
            {
                SetProperty(ref cameraFieldOfView, value);
                OnFieldOfViewChanged(value);
            }
        }

        /// <summary>
        ///     Get or set the image export height in pixels
        /// </summary>
        public int ImageExportHeight
        {
            get => imageExportHeight;
            set => SetProperty(ref imageExportHeight, Math.Min(Math.Abs(value), MaxImageHeight));
        }

        /// <summary>
        ///     Get or set the image export width in pixels
        /// </summary>
        public int ImageExportWidth
        {
            get => imageExportWidth;
            set => SetProperty(ref imageExportWidth, Math.Min(Math.Abs(value), MaxImageWidth));
        }

        /// <summary>
        ///     Get or set a boolean flag if the settings are active
        /// </summary>
        public bool IsSettingsActive
        {
            get => isSettingsActive;
            set => SetProperty(ref isSettingsActive, value);
        }

        /// <summary>
        ///     Get the selectable <see cref="MSAALevel" /> options
        /// </summary>
        public IEnumerable<MSAALevel> MsaaLevels => EnumerateMsaaLevels();

        /// <summary>
        ///     Get the selectable <see cref="FXAALevel" /> options
        /// </summary>
        public IEnumerable<FXAALevel> FxaaLevels => EnumerateFxaaLevels();

        /// <summary>
        ///     Get the selectable <see cref="CameraMode" /> options
        /// </summary>
        public IEnumerable<CameraMode> CameraModes => EnumerateCameraModes();

        /// <summary>
        ///     Get the selectable <see cref="CameraRotationMode" /> options
        /// </summary>
        public IEnumerable<CameraRotationMode> CameraRotationModes => EnumerateCameraRotationModes();

        /// <summary>
        ///     Get the selectable <see cref="SsaoQuality" /> options
        /// </summary>
        public IEnumerable<SSAOQuality> SsaoQualities => EnumeratSsaoQualities();

        /// <summary>
        ///     Get the selectable <see cref="CameraType" /> options
        /// </summary>
        public IEnumerable<CameraType> CameraTypes => EnumerateCameraTypes();

        /// <summary>
        ///     Get the selectable <see cref="LightSetting" /> options
        /// </summary>
        public IEnumerable<SceneLightSetting> SceneLightSettings => EnumerateLightSettings();

        /// <summary>
        ///     Get the <see cref="ObservableElement3DCollection" /> that supplies the <see cref="Light3D" /> items for the scene
        /// </summary>
        public ObservableElement3DCollection SceneLightCollection { get; }

        /// <summary>
        ///     Get the <see cref="ObservableElement3DCollection" /> that supplies the hit test visible <see cref="Element3D" />
        ///     items
        /// </summary>
        public ObservableElement3DCollection HitTestVisibleSceneElements { get; }

        /// <summary>
        ///     Get the <see cref="ObservableElement3DCollection" /> that supplies the hit test invisible <see cref="Element3D" />
        ///     items
        /// </summary>
        public ObservableElement3DCollection HitTestInvisibleSceneElements { get; }

        /// <summary>
        ///     Get a <see cref="ParameterlessCommand" /> to reset the camera
        /// </summary>
        public ParameterlessCommand ResetCameraCommand { get; }

        /// <summary>
        ///     Get the <see cref="ExportViewportImageCommand"/> to export a viewport to an image
        /// </summary>
        public ExportViewportImageCommand ExportImageCommand { get; }

        /// <summary>
        ///     Creates a new <see cref="DX3DViewportViewModel" />
        /// </summary>
        public DX3DViewportViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            HitTestVisibleSceneElements = new ObservableElement3DCollection();
            HitTestInvisibleSceneElements = new ObservableElement3DCollection();
            SceneLightCollection = new ObservableElement3DCollection {LightFactory.DefaultLightModel3D(LightColor)};
            PropertyChanged += DX3DViewportViewModel_PropertyChanged;
            ResetCameraCommand = new RelayCommand(ResetCamera);
            ExportImageCommand = new ExportViewportImageCommand(() => (ImageExportWidth, ImageExportHeight));
            LoadTestDataNodes();
        }

        private async void LoadTestDataNodes()
        {
            var meshBuilder = new MeshBuilder();
            meshBuilder.AddSphere(new Vector3(0, 0, 0));
            var mesh = meshBuilder.ToMesh();

            var rng = new PcgRandom32();
            var material = PhongMaterials.PolishedSilver;
            material.Freeze();

            var sceneBuilder = new SceneBuilder();
            var transforms = new List<Matrix>(5000);
            for (var i = 0; i < 5000; i++) transforms.Add(Matrix.Translation(rng.NextFloat(0, 100), rng.NextFloat(0, 100), rng.NextFloat(0, 100)));
            await sceneBuilder.AddBatchedMeshTransformsAsync(mesh, material, transforms);
            var geometryModel = sceneBuilder.ToModel();
            geometryModel.IsHitTestVisible = false;
            ExecuteOnAppThread(() => HitTestVisibleSceneElements.Add(geometryModel));
        }

        /// <summary>
        ///     Cleans all <see cref="ObservableElement3DCollection" /> containers
        /// </summary>
        private void ClearSceneCollections()
        {
            HitTestVisibleSceneElements.Clear();
            HitTestInvisibleSceneElements.Clear();
            SceneLightCollection.Clear();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            ClearSceneCollections();
            EffectsManager.Dispose();
        }

        /// <summary>
        ///     Action that handles the <see cref="IsInteracting" /> property change
        /// </summary>
        /// <param name="value"></param>
        protected virtual void OnIsInteractingChanged(bool value)
        {
            if (!DisableMsaaOnInteraction) return;
            if (value)
            {
                SetPropertyBackup(MsaaLevel, nameof(MsaaLevel));
                MsaaLevel = MSAALevel.Disable;
                return;
            }

            MsaaLevel = GetPropertyBackup<MSAALevel>(nameof(MsaaLevel));
        }

        /// <summary>
        ///     Action that handles the <see cref="CameraType" /> property change
        /// </summary>
        /// <param name="value"></param>
        protected virtual void OnCameraTypeChanged(CameraType value)
        {
            Camera = value switch
            {
                CameraType.Perspective => (Camera) new PerspectiveCamera
                {
                    CreateLeftHandSystem = Camera.CreateLeftHandSystem,
                    LookDirection = Camera.LookDirection,
                    Position = Camera.Position,
                    UpDirection = Camera.UpDirection,
                    FarPlaneDistance = CameraFarPlaneDistance,
                    NearPlaneDistance = CameraNearPlaneDistance,
                    FieldOfView = CameraFieldOfView
                },
                CameraType.Orthographic => new OrthographicCamera
                {
                    CreateLeftHandSystem = Camera.CreateLeftHandSystem,
                    LookDirection = Camera.LookDirection,
                    Position = Camera.Position,
                    UpDirection = Camera.UpDirection,
                    FarPlaneDistance = CameraFarPlaneDistance,
                    NearPlaneDistance = CameraNearPlaneDistance
                },
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }

        /// <summary>
        ///     Action that is called if the <see cref="CameraNearPlaneDistance" /> changes
        /// </summary>
        /// <param name="value"></param>
        protected virtual void OnCameraNearPlaneDistanceChanged(double value)
        {
            if (!(Camera is ProjectionCamera projectionCamera)) return;
            projectionCamera.NearPlaneDistance = value;
        }

        /// <summary>
        ///     Action that is called if the <see cref="CameraFarPlaneDistance" /> changes
        /// </summary>
        /// <param name="value"></param>
        protected virtual void OnCameraFarPlaneDistanceChanged(double value)
        {
            if (!(Camera is ProjectionCamera projectionCamera)) return;
            projectionCamera.FarPlaneDistance = value;
        }

        /// <summary>
        ///     Action that is called if the <see cref="CameraFieldOfView" /> changes
        /// </summary>
        /// <param name="value"></param>
        protected virtual void OnFieldOfViewChanged(double value)
        {
            if (!(Camera is PerspectiveCamera perspectiveCamera)) return;
            perspectiveCamera.FieldOfView = value;
        }

        /// <summary>
        ///     Action that is called if the <see cref="LightSetting" /> changes
        /// </summary>
        /// <param name="value"></param>
        protected virtual void OnLightSettingChanged(SceneLightSetting value)
        {
            SceneLightCollection.Clear();
            switch (value)
            {
                case SceneLightSetting.Default:
                    SceneLightCollection.Add(LightFactory.DefaultLightModel3D(LightColor));
                    break;
                case SceneLightSetting.OmniDirectional:
                    SceneLightCollection.Add(LightFactory.DefaultOmniDirectionalLightModel3D(LightColor, .5));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }

        /// <summary>
        ///     Action that is called if the <see cref="LightSetting" /> changes
        /// </summary>
        /// <param name="value"></param>
        protected virtual void OnLightColorChanged(in Color value)
        {
            OnLightSettingChanged(LightSetting);
        }

        /// <summary>
        ///     Resets the <see cref="Camera" /> settings
        /// </summary>
        protected virtual void ResetCamera()
        {
            CameraFieldOfView = 45;
            CameraFarPlaneDistance = 10000;
            CameraNearPlaneDistance = .1;
            CameraType = CameraType.Perspective;
            Camera = new PerspectiveCamera
            {
                FarPlaneDistance = CameraFarPlaneDistance,
                NearPlaneDistance = CameraNearPlaneDistance,
                FieldOfView = CameraFieldOfView
            };
        }

        /// <summary>
        ///     Enumerates the selectable <see cref="CameraMode" /> options
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<CameraMode> EnumerateCameraModes()
        {
            yield return CameraMode.FixedPosition;
            yield return CameraMode.Inspect;
            yield return CameraMode.WalkAround;
        }

        /// <summary>
        ///     Enumerates the selectable <see cref="CameraRotationMode" /> options
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<CameraRotationMode> EnumerateCameraRotationModes()
        {
            yield return CameraRotationMode.Turntable;
            yield return CameraRotationMode.Trackball;
            yield return CameraRotationMode.Turnball;
        }

        /// <summary>
        ///     Enumerates the selectable <see cref="MSAALevel" /> options
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<MSAALevel> EnumerateMsaaLevels()
        {
            yield return MSAALevel.Disable;
            yield return MSAALevel.Two;
            yield return MSAALevel.Four;
            yield return MSAALevel.Eight;
        }

        /// <summary>
        ///     Enumerates the selectable <see cref="FXAALevel" /> options
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<FXAALevel> EnumerateFxaaLevels()
        {
            yield return FXAALevel.None;
            yield return FXAALevel.Low;
            yield return FXAALevel.Medium;
            yield return FXAALevel.High;
            yield return FXAALevel.Ultra;
        }

        /// <summary>
        ///     Enumerates the selectable <see cref="SSAOQuality" /> options
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<SSAOQuality> EnumeratSsaoQualities()
        {
            yield return SSAOQuality.Low;
            yield return SSAOQuality.High;
        }

        /// <summary>
        ///     Enumerates the selectable <see cref="CameraType" /> options
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<CameraType> EnumerateCameraTypes()
        {
            yield return CameraType.Perspective;
            yield return CameraType.Orthographic;
        }

        /// <summary>
        ///     Enumerates the selectable <see cref="SceneLightSetting" /> options
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<SceneLightSetting> EnumerateLightSettings()
        {
            yield return SceneLightSetting.Default;
            yield return SceneLightSetting.OmniDirectional;
        }

        /// <summary>
        ///     General reaction to a property change of the view model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DX3DViewportViewModel_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (GetType().GetProperty(args.PropertyName)?.GetCustomAttribute<RaiseInvalidateRenderAttribute>() is { } attribute)
                Task.Delay(attribute.Delay).ContinueWith(task => ExecuteOnAppThread(EffectsManager.RaiseInvalidateRender));
        }
    }
}