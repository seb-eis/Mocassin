using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Transitions;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.TransitionModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Transitions.IKineticTransition" /> model object creation
    /// </summary>
    [XmlRoot]
    public class KineticTransitionData : ModelDataObject
    {
        private ModelObjectReference<AbstractTransition> abstractTransition;
        private ObservableCollection<VectorData3D> pathVectors;

        /// <summary>
        ///     Get or set the abstract transition key for the transition logic
        /// </summary>
        [XmlElement]
        public ModelObjectReference<AbstractTransition> AbstractTransition
        {
            get => abstractTransition;
            set => SetProperty(ref abstractTransition, value);
        }

        /// <summary>
        ///     Get or set the geometry sequence of the kinetic transition
        /// </summary>
        [XmlArray]
        public ObservableCollection<VectorData3D> PathVectors
        {
            get => pathVectors;
            set => SetProperty(ref pathVectors, value);
        }

        /// <summary>
        ///     Creates new <see cref="KineticTransitionData" /> wit empty component lists
        /// </summary>
        public KineticTransitionData()
        {
            PathVectors = new ObservableCollection<VectorData3D>();
        }

        /// <inheritdoc />
        protected override ModelObject GetModelObjectInternal()
        {
            var obj = new KineticTransition
            {
                AbstractTransition = new AbstractTransition {Key = AbstractTransition.Key},
                PathGeometry = PathVectors.Select(x => x.AsFractional3D()).ToList()
            };
            return obj;
        }
    }
}