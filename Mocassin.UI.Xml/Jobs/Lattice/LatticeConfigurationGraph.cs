using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Lattices;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Translator.Jobs;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.Jobs
{
    [XmlRoot("LatticeConfiguration")]
    public class LatticeConfigurationGraph : ProjectObjectGraph
    {
        [XmlAttribute("SizeA")]
        public int SizeA { get; set; }

        [XmlAttribute("SizeB")]
        public int SizeB { get; set; }

        [XmlAttribute("SizeC")]
        public int SizeC { get; set; }

        [XmlArray("DopingValues")]
        [XmlArrayItem("DopingValue")]
        public List<DopingValueGraph> DopingValues { get; set; }

        public LatticeConfigurationGraph()
        {
            SizeA = SizeB = SizeC = 1;
            DopingValues = new List<DopingValueGraph>();
        }

        public LatticeConfiguration ToInternal(IModelProject modelProject)
        {
            if (modelProject == null) throw new ArgumentNullException(nameof(modelProject));

            var result = new LatticeConfiguration
            {
                SizeA = SizeA,
                SizeB = SizeB,
                SizeC = SizeC,
                DopingConcentrations = DopingValues
                    .ToDictionary(x => modelProject.DataTracker.FindObjectByKey<IDoping>(x.Doping.Key), y => y.Value)
            };

            return result;
        }
    }
}