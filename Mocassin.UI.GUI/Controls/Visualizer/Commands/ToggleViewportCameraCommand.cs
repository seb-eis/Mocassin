using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using Mocassin.UI.Base.Commands;

namespace Mocassin.UI.GUI.Controls.Visualizer.Commands
{
    /// <summary>
    ///     The <see cref="Command{T}" /> implementation to toggle the camera mode of a <see cref="HelixViewport3D" /> between
    ///     orthographic and perspective
    /// </summary>
    public class ToggleViewportCameraCommand : Command<HelixViewport3D>
    {
        /// <inheritdoc />
        public override bool CanExecute(HelixViewport3D parameter)
        {
            return parameter != null && base.CanExecute(parameter);
        }

        /// <inheritdoc />
        public override void Execute(HelixViewport3D parameter)
        {
            var oldCamera = parameter.Camera;
            var newCamera = parameter.Camera.GetType().IsAssignableFrom(typeof(PerspectiveCamera))
                ? (ProjectionCamera) new OrthographicCamera()
                : new PerspectiveCamera();
            CopyCameraData(oldCamera, newCamera);
            parameter.Camera = newCamera;
        }

        /// <summary>
        ///     Copies the position and look direction information from one <see cref="ProjectionCamera" /> to another
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void CopyCameraData(ProjectionCamera source, ProjectionCamera target)
        {
            target.FarPlaneDistance = source.FarPlaneDistance;
            target.LookDirection = source.LookDirection;
            target.NearPlaneDistance = source.NearPlaneDistance;
            target.Position = source.Position;
            target.UpDirection = source.UpDirection;
        }
    }
}