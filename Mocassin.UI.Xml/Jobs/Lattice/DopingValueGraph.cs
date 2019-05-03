using System.Xml.Serialization;
using Mocassin.Model.Lattices;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.Jobs
{
    [XmlRoot("DopingValue")]
    public class DopingValueGraph : ProjectObjectGraph
    {
        [XmlElement("Doping")]
        public ModelObjectReferenceGraph<Doping> Doping { get; set; }

        [XmlAttribute("Value")]
        public double Value { get; set; }
    }
}