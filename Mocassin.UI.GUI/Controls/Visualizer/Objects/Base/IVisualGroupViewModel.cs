using System;
using System.ComponentModel;
using System.Windows.Media.Media3D;

namespace Mocassin.UI.GUI.Controls.Visualizer.Objects
{
    /// <summary>
    ///     Represents a view model for a <see cref="ModelVisual3D" />
    /// </summary>
    public interface IVisualGroupViewModel : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        ///     Get or set a boolean flag if the group is visible
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        ///     Get or set a name for the group
        /// </summary>
        string Name { get; set; }

        /// <summary>
        ///     Get the number of items in the group
        /// </summary>
        int ItemCount { get; }

        /// <summary>
        ///     Get the <see cref="ModelVisual3D" /> of the group
        /// </summary>
        ModelVisual3D ModelVisual { get; }
    }
}