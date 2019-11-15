using System.Collections.Generic;
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
    [XmlRoot("KineticTransition")]
    public class KineticTransitionGraph : ModelObjectGraph
    {
        private string abstractTransitionKey;
        private ObservableCollection<VectorGraph3D> positionVectors;

        /// <summary>
        ///     Get or set the abstract transition key for the transition logic
        /// </summary>
        [XmlAttribute("Abstract")]
        public string AbstractTransitionKey
        {
            get => abstractTransitionKey;
            set => SetProperty(ref abstractTransitionKey, value);
        }

        /// <summary>
        ///     Get or set the geometry sequence of the kinetic transition
        /// </summary>
        [XmlArray("BaseGeometry")]
        [XmlArrayItem("Position")]
        public ObservableCollection<VectorGraph3D> PositionVectors
        {
            get => positionVectors;
            set => SetProperty(ref positionVectors, value);
        }

        /// <summary>
        ///     Creates new <see cref="KineticTransitionGraph" /> wit empty component lists
        /// </summary>
        public KineticTransitionGraph()
        {
            PositionVectors = new ObservableCollection<VectorGraph3D>();
        }

        /// <inheritdoc />
        protected override ModelObject GetModelObjectInternal()
        {
            var obj = new KineticTransition
            {
                AbstractTransition = new AbstractTransition {Key = AbstractTransitionKey},
                PathGeometry = PositionVectors.Select(x => x.AsDataVector3D()).ToList()
            };
            return obj;
        }
    }
}