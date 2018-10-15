using System.Collections.Generic;
using System.Xml.Serialization;

namespace ICon.Framework.Xml
{
    /// <summary>
    ///     Class to store multiple event handlers for all types of Xml deserialization events
    /// </summary>
    public class XmlEventHandlers
    {
        /// <summary>
        ///     Creates new package with empty lists
        /// </summary>
        public XmlEventHandlers()
        {
            AttributeHandlers = new List<XmlAttributeEventHandler>();
            ElementHandlers = new List<XmlElementEventHandler>();
            NodeHandlers = new List<XmlNodeEventHandler>();
            ObjectHandlers = new List<UnreferencedObjectEventHandler>();
        }

        /// <summary>
        ///     All to be called event handlers for unknown attribute events
        /// </summary>
        public List<XmlAttributeEventHandler> AttributeHandlers { get; set; }

        /// <summary>
        ///     All to be called event handlers for unknown element events
        /// </summary>
        public List<XmlElementEventHandler> ElementHandlers { get; set; }

        /// <summary>
        ///     All to be called event handlers for unknown nodes events
        /// </summary>
        public List<XmlNodeEventHandler> NodeHandlers { get; set; }

        /// <summary>
        ///     All to be called event handlers for unreferenced object events
        /// </summary>
        public List<UnreferencedObjectEventHandler> ObjectHandlers { get; set; }
    }
}