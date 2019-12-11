using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Media;
using HelixToolkit.Wpf.SharpDX;
using Mocassin.Framework.Random;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Attributes;
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Commands;
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Enums;
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Helper;
using SharpDX;
using Color = System.Windows.Media.Color;
using Matrix = SharpDX.Matrix;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.Viewport
{
    /// <summary>
    ///     The <see cref="ViewModelBase" /> implementation that manages 3D scene data for the <see cref="DxViewportView" />
    /// </summary>
    public class DxViewportViewModel : ViewModelBase, IDisposable
    {
        private static EffectsManager _effectsManager = new DefaultEffectsManager();

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
        private DxCameraType dxCameraType = DxCameraType.Perspective;
        private double cameraFarPlaneDistance = 10000;
        private double cameraNearPlaneDistance = 0.1;
        private double cameraFieldOfView = 45;
        private DxSceneLightSetting lightSetting = DxSceneLightSetting.None;
        private Color lightColor = Colors.White;
        private bool settingsOverlayActive;
        private int imageExportHeight;
        private int imageExportWidth;
        private bool itemsOverlayActive;
        private EffectsManager effectsManager;

        /// <summary>
        ///     Get the <see cref="HelixToolkit.Wpf.SharpDX.EffectsManager" /> for the 3D system
        /// </summary>
        public EffectsManager EffectsManager
        {
            get => effectsManager;
            private set => SetProperty(ref effectsManager, value);
        }

        /// <summary>
        ///     Get or set the <see cref="HelixToolkit.Wpf.SharpDX.Camera" />
        /// </summary>
        [RaiseInvalidateRender]
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
        ///     Gets or set the used <see cref="DxCameraType" />
        /// </summary>
        [RaiseInvalidateRender]
        public DxCameraType DxCameraType
        {
            get => dxCameraType;
            set
            {
                if (dxCameraType == value) return;
                SetProperty(ref dxCameraType, value);
                OnCameraTypeChanged(value);
            }
        }

        /// <summary>
        ///     Gets or sets the basic <see cref="DxSceneLightSetting" /> for the scene
        /// </summary>
        public DxSceneLightSetting LightSetting
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
        ///     Get or set a boolean flag if the settings overlay is active
        /// </summary>
        [TogglesOverlay]
        public bool SettingsOverlayActive
        {
            get => settingsOverlayActive;
            set => SetProperty(ref settingsOverlayActive, value);
        }

        /// <summary>
        ///     Get or set a boolean flag if the object overlay is active
        /// </summary>
        [TogglesOverlay]
        public bool ItemsOverlayActive
        {
            get => itemsOverlayActive;
            set => SetProperty(ref itemsOverlayActive, value);
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
        ///     Get the selectable <see cref="DxCameraType" /> options
        /// </summary>
        public IEnumerable<DxCameraType> CameraTypes => EnumerateCameraTypes();

        /// <summary>
        ///     Get the selectable <see cref="LightSetting" /> options
        /// </summary>
        public IEnumerable<DxSceneLightSetting> SceneLightSettings => EnumerateLightSettings();

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
        ///     Get the <see cref="ExportDxViewportImageCommand"/> to export a viewport to an image
        /// </summary>
        public ExportDxViewportImageCommand ExportDxImageCommand { get; }

        /// <summary>
        ///     Creates a new <see cref="DxViewportViewModel" />
        /// </summary>
        public DxViewportViewModel()
        {
            effectsManager = _effectsManager;
            HitTestVisibleSceneElements = new ObservableElement3DCollection();
            HitTestInvisibleSceneElements = new ObservableElement3DCollection();
            SceneLightCollection = new ObservableElement3DCollection();
            PropertyChanged += DX3DViewportViewModel_PropertyChanged;
            ResetCameraCommand = new RelayCommand(ResetCamera);
            ExportDxImageCommand = new ExportDxViewportImageCommand(() => (ImageExportWidth, ImageExportHeight));
            Reset();
        }

        /// <summary>
        ///     Adds an <see cref="Element3D"/> to the viewport
        /// </summary>
        /// <param name="element"></param>
        public void AddSceneElement(Element3D element)
        {
            if (!element.CheckAccess())
            {
                element.Dispatcher?.Invoke(() => AddSceneElement(element));
                return;
            }
            if (element.IsHitTestVisible)
            {
                HitTestVisibleSceneElements.Add(element);
                return;
            }
            HitTestInvisibleSceneElements.Add(element);
        }

        /// <summary>
        ///     Adds a sequence of <see cref="Element3D"/> objects to the viewport
        /// </summary>
        /// <param name="elements"></param>
        public void AddSceneElements(IEnumerable<Element3D> elements)
        {
            foreach (var element in elements) AddSceneElement(element);
        }

        /// <summary>
        ///     Resets the <see cref="DxViewportViewModel"/> to default settings and clears all scene data (Always executed on the main UI thread)
        /// </summary>
        public void Reset(bool resetCamera = true)
        {
            void ResetInternal()
            {
                ClearSceneCollections();
                ResetLight();
                if (resetCamera) ResetCamera();
            }

            ExecuteOnAppThread(ResetInternal);
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
            EffectsManager = null;
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
        ///     Action that handles the <see cref="DxCameraType" /> property change
        /// </summary>
        /// <param name="value"></param>
        protected virtual void OnCameraTypeChanged(DxCameraType value)
        {
            Camera = value switch
            {
                DxCameraType.Perspective => (Camera) new PerspectiveCamera
                {
                    CreateLeftHandSystem = Camera.CreateLeftHandSystem,
                    LookDirection = Camera.LookDirection,
                    Position = Camera.Position,
                    UpDirection = Camera.UpDirection,
                    FarPlaneDistance = CameraFarPlaneDistance,
                    NearPlaneDistance = CameraNearPlaneDistance,
                    FieldOfView = CameraFieldOfView
                },
                DxCameraType.Orthographic => new OrthographicCamera
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
        protected virtual void OnLightSettingChanged(DxSceneLightSetting value)
        {
            SceneLightCollection.Clear();
            switch (value)
            {
                case DxSceneLightSetting.Default:
                    SceneLightCollection.Add(DxLightFactory.DefaultLightModel3D(LightColor));
                    break;
                case DxSceneLightSetting.OmniDirectional:
                    SceneLightCollection.Add(DxLightFactory.DefaultOmniDirectionalLightModel3D(LightColor, .5));
                    break;
                case DxSceneLightSetting.None:
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
        ///     Resets the <see cref="Camera" /> to default settings
        /// </summary>
        protected virtual void ResetCamera()
        {
            CameraFieldOfView = 45;
            CameraFarPlaneDistance = 10000;
            CameraNearPlaneDistance = .1;
            DxCameraType = DxCameraType.Perspective;
            Camera = new PerspectiveCamera
            {
                FarPlaneDistance = CameraFarPlaneDistance,
                NearPlaneDistance = CameraNearPlaneDistance,
                FieldOfView = CameraFieldOfView
            };
        }

        /// <summary>
        ///     Resets the scene light to default settings
        /// </summary>
        protected virtual void ResetLight()
        {
            LightSetting = DxSceneLightSetting.None;
            LightColor = Colors.White;
            LightSetting = DxSceneLightSetting.Default;
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
        ///     Enumerates the selectable <see cref="DxCameraType" /> options
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<DxCameraType> EnumerateCameraTypes()
        {
            yield return DxCameraType.Perspective;
            yield return DxCameraType.Orthographic;
        }

        /// <summary>
        ///     Enumerates the selectable <see cref="DxSceneLightSetting" /> options
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<DxSceneLightSetting> EnumerateLightSettings()
        {
            yield return DxSceneLightSetting.Default;
            yield return DxSceneLightSetting.OmniDirectional;
        }

        /// <summary>
        ///     General reaction to a property change of the view model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DX3DViewportViewModel_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            CheckPropertyChangeInvalidateRender(args.PropertyName);
            CheckPropertyChangeTogglesOverlay(args.PropertyName);
        }

        /// <summary>
        ///     Checks if a property change invalidates the render and triggers affiliated actions if required
        /// </summary>
        /// <param name="propertyName"></param>
        private void CheckPropertyChangeInvalidateRender(string propertyName)
        {
            if (!(GetType().GetProperty(propertyName)?.GetCustomAttribute<RaiseInvalidateRenderAttribute>() is { } attribute)) return;
            Task.Delay(attribute.Delay).ContinueWith(task => ExecuteOnAppThread(EffectsManager.RaiseInvalidateRender));
        }

        /// <summary>
        ///     Checks if a property change toggles an overlay and triggers affiliated actions if required
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="bindingFlags"></param>
        private void CheckPropertyChangeTogglesOverlay(string propertyName, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            if (!(GetType().GetProperty(propertyName)?.GetCustomAttribute<TogglesOverlayAttribute>() is { } attribute)) return;
            if (!attribute.IsUniqueOverlay) return;
            var properties = GetType().GetProperties(bindingFlags).Where(x => x.GetCustomAttribute<TogglesOverlayAttribute>()?.IsUniqueOverlay ?? false).ToList();
            if (properties.Count(x => (bool) x.GetValue(this)) <= 1) return;
            foreach (var property in properties.Where(property => property.Name != propertyName))
            {
                property.SetValue(this, false);
            }
        }

        /// <summary>
        ///     Loads a test scene of spheres and returns the time it took to fully created the scene
        /// </summary>
        /// <param name="itemCount"></param>
        /// <param name="batchGeometry"></param>
        /// <returns></returns>
        public async Task<TimeSpan> LoadTestScene(int itemCount = 5000, bool batchGeometry = true)
        {
            Reset(false);
            var watch = Stopwatch.StartNew();
            var meshBuilder = new MeshBuilder();
            meshBuilder.AddSphere(new Vector3(0, 0, 0));
            var mesh = meshBuilder.ToMesh();

            var rng = new PcgRandom32();
            var material = PhongMaterials.Gold;
            material.Freeze();

            var sceneBuilder = new DxSceneBuilder();
            var transforms = new List<Matrix>(itemCount);
            for (var i = 0; i < itemCount; i++) transforms.Add(Matrix.Translation(rng.NextFloat(0, 200), rng.NextFloat(0, 200), rng.NextFloat(0, 200)));
            if (batchGeometry)
            {
                await sceneBuilder.BeginAddBatchedMeshTransforms(mesh, material, transforms);
            }
            else
            {
                await sceneBuilder.BeginAddMeshTransforms(mesh, material, transforms);
            }
            var geometryModel = await sceneBuilder.ToModelAsync();
            ExecuteOnAppThread(() => HitTestVisibleSceneElements.Add(geometryModel));
            watch.Stop();
            return watch.Elapsed;
        }
    }
}