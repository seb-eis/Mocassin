namespace Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Enums
{
    /// <summary>
    ///     Draw mode for DX 3D scenes with multiple instances of the same model
    /// </summary>
    public enum DxInstanceRenderMode
    {
        /// <summary>
        ///     Render multiple instances using instanced rendering
        /// </summary>
        Instanced,
        
        /// <summary>
        ///     Render each instance of the model individually
        /// </summary>
        Individual,
        
        /// <summary>
        ///     Batch instances for rendering
        /// </summary>
        Batched
    }
}