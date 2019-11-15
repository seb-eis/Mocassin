using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Framework.Extensions;
using Mocassin.Model.Lattices;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Translator.Jobs;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.Jobs
{
    /// <summary>
    ///     Describes a doped lattice configuration by size and doping information
    /// </summary>
    [XmlRoot("LatticeConfiguration")]
    public class LatticeConfigurationGraph : ProjectObjectGraph, IDuplicable<LatticeConfigurationGraph>
    {
        private int sizeA;
        private int sizeB;
        private int sizeC;
        private ObservableCollection<DopingValueGraph> dopingValues;

        /// <summary>
        ///     Get or set the number of unit cells in 'A' direction
        /// </summary>
        [XmlAttribute("SizeA")]
        public int SizeA
        {
            get => sizeA;
            set => SetProperty(ref sizeA, value);
        }

        /// <summary>
        ///     Get or set the number of unit cells in 'B' direction
        /// </summary>
        [XmlAttribute("SizeB")]
        public int SizeB
        {
            get => sizeB;
            set => SetProperty(ref sizeB, value);
        }

        /// <summary>
        ///     Get or set the number of unit cells in 'C' direction
        /// </summary>
        [XmlAttribute("SizeC")]
        public int SizeC
        {
            get => sizeC;
            set => SetProperty(ref sizeC, value);
        }

        /// <summary>
        ///     Get or set the list of <see cref="DopingValueGraph" /> information
        /// </summary>
        [XmlArray("DopingValues")]
        [XmlArrayItem("DopingValue")]
        public ObservableCollection<DopingValueGraph> DopingValues
        {
            get => dopingValues;
            set => SetProperty(ref dopingValues, value);
        }

        /// <summary>
        ///     Creates a new <see cref="LatticeConfigurationGraph" /> with default size
        /// </summary>
        public LatticeConfigurationGraph()
        {
            SizeA = SizeB = SizeC = 10;
            DopingValues = new ObservableCollection<DopingValueGraph>();
        }

        /// <summary>
        ///     Creates an internal <see cref="LatticeConfiguration" /> from the serializable data object
        /// </summary>
        /// <param name="modelProject"></param>
        /// <returns></returns>
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

        /// <inheritdoc />
        public LatticeConfigurationGraph Duplicate()
        {
            var result = new LatticeConfigurationGraph
            {
                SizeA = SizeA,
                SizeB = SizeB,
                SizeC = SizeC,
                DopingValues = DopingValues.Select(x => x.Duplicate()).ToObservableCollection()
            };
            return result;
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate()
        {
            return Duplicate();
        }
    }
}