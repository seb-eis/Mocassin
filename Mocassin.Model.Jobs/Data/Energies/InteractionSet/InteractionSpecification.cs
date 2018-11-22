using System.Collections.Generic;
using System.Xml.Serialization;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Model.Jobs
{
    [XmlRoot]
    public class InteractionSpecification
    {
        [XmlAttribute]
        public bool IsPair => VectorSet?.Count == 2;

        [XmlAttribute]
        public int ContextId { get; set; }

        public List<DataVector3D> VectorSet { get; set; }

        public List<InteractionSetting> InteractionSettings { get; set; }
    }
}