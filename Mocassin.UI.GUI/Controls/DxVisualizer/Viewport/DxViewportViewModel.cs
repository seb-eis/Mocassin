using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using HelixToolkit.Wpf.SharpDX;
using Mocassin.Framework.Extensions;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Attributes;
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Base;
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Commands;
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Enums;
using Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Helper;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.Viewport
{
    /// <summary>
    ///     The <see cref="ViewModelBase" /> implementation that manages 3D scene data for the <see cref="DxViewportView" />
    /// </summary>
    public class DxViewportViewModel : ViewModelBase, IDxSceneHost
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
        private DxSceneMemoryCost sceneMemoryCostPreference = DxSceneMemoryCost.Low;
        private ICommand invalidateSceneCommand;

        /// <summary>
        ///     Get the <see cref="HelixToolkit.Wpf.SharpDX.EffectsManager" /> for the 3D system
        /// </summary>
        public EffectsManager EffectsManager { get; }

        /// <inheritdoc />
        public IDxSceneController SceneController { get; protected set; }

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

        /// <inheritdoc />
        public DxSceneMemoryCost SceneMemoryCostPreference
        {
            get => sceneMemoryCostPreference;
            set
            {
                if (sceneMemoryCostPreference == value) return;
                SetProperty(ref sceneMemoryCostPreference, value);
                InvalidateSceneCommand?.Execute(null);
            }
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
        public IEnumerable<SSAOQuality> SsaoQualities => EnumerateSsaoQualities();

        /// <summary>
        ///     Get the selectable <see cref="DxCameraType" /> options
        /// </summary>
        public IEnumerable<DxCameraType> CameraTypes => EnumerateCameraTypes();

        /// <summary>
        ///     Get the selectable <see cref="LightSetting" /> options
        /// </summary>
        public IEnumerable<DxSceneLightSetting> SceneLightSettings => EnumerateLightSettings();

        /// <summary>
        ///     Get the selectable <see cref="SceneMemoryCostPreference" />
        /// </summary>
        public IEnumerable<DxSceneMemoryCost> SceneMemoryPreferences => EnumerateMemoryPreferences();

        /// <summary>
        ///     Get the <see cref="ObservableElement3DCollection" /> that supplies the <see cref="Element3D" /> containing light
        ///     information
        /// </summary>
        public ObservableElement3DCollection SceneLights { get; }

        /// <summary>
        ///     Get the <see cref="ObservableElement3DCollection" /> that supplies the scene <see cref="Element3D" /> instances
        /// </summary>
        public ObservableElement3DCollection SceneElements { get; }

        /// <summary>
        ///     Get a <see cref="ParameterlessCommand" /> to reset the camera
        /// </summary>
        public ParameterlessCommand ResetCameraCommand { get; }

        /// <summary>
        ///     Get the <see cref="ExportDxViewportImageCommand" /> to export a viewport to an image
        /// </summary>
        public ExportDxViewportImageCommand ExportDxImageCommand { get; }

        /// <summary>
        ///     Get the <see cref="ICommand" /> to instruct the <see cref="IDxSceneController" /> to invalidate the scene
        /// </summary>
        public ICommand InvalidateSceneCommand
        {
            get => invalidateSceneCommand;
            set => SetProperty(ref invalidateSceneCommand, value);
        }

        /// <summary>
        ///     Creates a new <see cref="DxViewportViewModel" />
        /// </summary>
        public DxViewportViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            SceneElements = new ObservableElement3DCollection();
            SceneLights = new ObservableElement3DCollection();
            PropertyChanged += DX3DViewportViewModel_PropertyChanged;
            ResetCameraCommand = new RelayCommand(ResetCamera);
            ExportDxImageCommand = new ExportDxViewportImageCommand(() => (ImageExportWidth, ImageExportHeight));
            ResetScene(true);
        }

        /// <summary>
        ///     Cleans all <see cref="ObservableElement3DCollection" /> containers
        /// </summary>
        private void ClearSceneCollections()
        {
            SceneElements.Clear();
            SceneLights.Clear();
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
            SceneLights.Clear();
            switch (value)
            {
                case DxSceneLightSetting.Default:
                    SceneLights.Add(DxLightFactory.DefaultLightModel3D(LightColor));
                    break;
                case DxSceneLightSetting.OmniDirectional:
                    SceneLights.Add(DxLightFactory.DefaultOmniDirectionalLightModel3D(LightColor, .5));
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

        /// <inheritdoc />
        public void ResetScene(bool resetCamera)
        {
            void ResetInternal()
            {
                ClearSceneCollections();
                ResetLight();
                if (resetCamera) ResetCamera();
            }

            ExecuteOnAppThread(ResetInternal);
        }

        /// <inheritdoc />
        public void ClearScene()
        {
            ExecuteOnAppThread(ClearSceneCollections);
        }

        /// <inheritdoc />
        public void AddSceneItem(Element3D element)
        {
            ExecuteOnAppThread(() => SceneElements.Add(element));
        }

        /// <inheritdoc />
        public void AddSceneItems(IEnumerable<Element3D> elements)
        {
            ExecuteOnAppThread(() => SceneElements.AddRange(elements));
        }

        /// <inheritdoc />
        public bool RemoveSceneItem(Element3D element)
        {
            return ExecuteOnAppThread(() => SceneElements.Remove(element));
        }

        /// <inheritdoc />
        public void AttachController(IDxSceneController controller)
        {
            DetachController();
            SceneController = controller;
            InvalidateSceneCommand = controller?.InvalidateSceneCommand;
        }

        /// <inheritdoc />
        public void DetachController()
        {
            if (SceneController == null) return;
            SceneController = null;
            InvalidateSceneCommand = null;
        }

        /// <summary>
        ///     Resets the <see cref="Camera" /> to default settings
        /// </summary>
        public virtual void ResetCamera()
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
        protected virtual IEnumerable<SSAOQuality> EnumerateSsaoQualities()
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
        ///     Enumerates the selectable <see cref="DxSceneMemoryCost" /> preferences
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<DxSceneMemoryCost> EnumerateMemoryPreferences()
        {
            yield return DxSceneMemoryCost.Lowest;
            yield return DxSceneMemoryCost.Low;
            yield return DxSceneMemoryCost.Medium;
            yield return DxSceneMemoryCost.High;
            yield return DxSceneMemoryCost.Highest;
            yield return DxSceneMemoryCost.Unlimited;
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
            foreach (var property in properties.Where(property => property.Name != propertyName)) property.SetValue(this, false);
        }
    }
}