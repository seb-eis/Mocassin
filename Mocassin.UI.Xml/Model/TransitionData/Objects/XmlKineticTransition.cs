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
    public class XmlKineticTransition : XmlModelObject
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
        public List<XmlVector3D> PositionVectors { get; set; }

        /// <inheritdoc />
        protected override ModelObject GetPreparedModelObject()
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