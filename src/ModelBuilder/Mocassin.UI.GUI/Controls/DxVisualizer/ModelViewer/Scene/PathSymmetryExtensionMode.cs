namespace Mocassin.UI.GUI.Controls.DxVisualizer.ModelViewer.Scene
{
    /// <summary>
    ///     Enumerates the possible extension modes for rendering of paths affected by symmetry operations and translation
    ///     invariance
    /// </summary>
    public enum PathSymmetryExtensionMode
    {
        /// <summary>
        ///     No extension, only the original object is rendered
        /// </summary>
        None,

        /// <summary>
        ///     Local extension around the original source position
        /// </summary>
        Local,

        /// <summary>
        ///     Full extension to the entire super-cell
        /// </summary>
        Full,
        
        /// <summary>
        ///     Full extension mode with boundary checks
        /// </summary>
        FullWithClip
    }
}