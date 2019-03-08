using System.Collections.Generic;
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
        /// <summary>
        ///     Get or set the abstract transition key for the transition logic
        /// </summary>
        [XmlAttribute("Abstract")]
        public string AbstractTransitionKey { get; set; }

        /// <summary>
        ///     Get or set the geometry sequence of the kinetic transition
        /// </summary>
        [XmlArray("BaseGeometry")]
        [XmlArrayItem("Position")]
        public List<VectorGraph3D> PositionVectors { get; set; }

        /// <summary>
        ///     Creates new <see cref="KineticTransitionGraph" /> wit empty component lists
        /// </summary>
        public KineticTransitionGraph()
        {
            PositionVectors = new List<VectorGraph3D>();
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