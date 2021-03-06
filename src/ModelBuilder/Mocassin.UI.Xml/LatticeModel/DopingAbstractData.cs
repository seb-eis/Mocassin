﻿using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Lattices;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.UI.Data.Base;

namespace Mocassin.UI.Data.LatticeModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Lattices.IDopingCombination" /> model object creation
    /// </summary>
    [XmlRoot]
    public class DopingAbstractData : ModelDataObject
    {
        private ModelObjectReference<CellSite> cellReferencePosition = new ModelObjectReference<CellSite>();
        private ModelObjectReference<Particle> dopable = new ModelObjectReference<Particle>();
        private ModelObjectReference<Particle> dopant = new ModelObjectReference<Particle>();

        /// <summary>
        ///     Particle that is replaced by dopant
        /// </summary>
        [XmlElement]
        public ModelObjectReference<Particle> Dopable
        {
            get => dopable;
            set => SetProperty(ref dopable, value);
        }

        /// <summary>
        ///     Particle that is used as dopant
        /// </summary>
        [XmlElement]
        public ModelObjectReference<Particle> Dopant
        {
            get => dopant;
            set => SetProperty(ref dopant, value);
        }

        /// <summary>
        ///     Unit cell position on which the doping is applied
        /// </summary>
        [XmlElement]
        public ModelObjectReference<CellSite> CellReferencePosition
        {
            get => cellReferencePosition;
            set => SetProperty(ref cellReferencePosition, value);
        }

        /// <inheritdoc />
        protected override ModelObject GetModelObjectInternal()
        {
            var obj = new DopingCombination
            {
                Dopable = new Particle {Key = Dopable.Key},
                Dopant = new Particle {Key = Dopant.Key},
                CellSite = new CellSite {Key = CellReferencePosition.Key}
            };
            return obj;
        }
    }
}