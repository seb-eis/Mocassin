using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media.Media3D;

namespace Mocassin.UI.GUI.Controls.Visualizer.Objects
{
    /// <summary>
    ///     Represents a view model for groups of of <see cref="Visual3D" /> instances
    /// </summary>
    public interface IVisualGroupViewModel : INotifyPropertyChanged
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
        ///     Get the <see cref="IEnumerable{T}" /> of <see cref="Visual3D" /> items
        /// </summary>
        IEnumerable<Visual3D> Items { get; }
    }
}