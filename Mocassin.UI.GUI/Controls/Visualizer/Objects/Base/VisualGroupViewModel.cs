using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using Mocassin.UI.GUI.Base.ViewModels;

namespace Mocassin.UI.GUI.Controls.Visualizer.Objects
{
    /// <summary>
    ///     A Generic <see cref="ViewModelBase" /> implementation for sets of <see cref="Visual3D" /> that represent a visual
    ///     group with common properties
    /// </summary>
    public class VisualGroupViewModel<T> : ViewModelBase, IVisualGroupViewModel where T : Visual3D
    {
        private string name;
        private IList<Visual3D> visuals;
        private bool isVisible;

        /// <inheritdoc />
        public int ItemCount => visuals?.Count ?? 0;

        /// <inheritdoc />
        public IEnumerable<Visual3D> Items => Visuals;

        /// <summary>
        ///     Get or set boolean value if the layer should be visible
        /// </summary>
        public bool IsVisible
        {
            get => isVisible;
            set => SetProperty(ref isVisible, value);
        }

        /// <summary>
        ///     Get or set the <see cref="ICollection{T}" /> of visual elements
        /// </summary>
        public IEnumerable<Visual3D> Visuals
        {
            get => visuals;
            set
            {
                SetProperty(ref visuals, value?.ToList());
                OnPropertyChanged(nameof(Items));
                OnPropertyChanged(nameof(ItemCount));
            }
        }

        /// <summary>
        ///     Get or set a name for the group
        /// </summary>
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            visuals?.Clear();
            Visuals = null;
        }
    }
}