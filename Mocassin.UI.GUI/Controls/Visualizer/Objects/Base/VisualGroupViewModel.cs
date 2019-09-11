using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using Mocassin.UI.GUI.Base.ViewModels;

namespace Mocassin.UI.GUI.Controls.Visualizer.Objects
{
    /// <summary>
    ///     A Generic <see cref="ViewModelBase" /> implementation for a <see cref="ModelVisual3D" /> that represent a visual
    ///     group with common properties
    /// </summary>
    public class VisualGroupViewModel : ViewModelBase, IVisualGroupViewModel
    {
        private string name;
        private ModelVisual3D modelVisual;
        private bool isVisible;

        /// <inheritdoc />
        public int ItemCount => modelVisual?.Children.Count ?? 0;

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
        public ModelVisual3D ModelVisual
        {
            get => modelVisual;
            set
            {
                SetProperty(ref modelVisual, value);
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
            ExecuteOnDispatcher(() => ModelVisual?.Children?.Clear());
            ModelVisual = null;
        }
    }
}