using System;
using System.Xml.Serialization;
using Mocassin.Model.Energies;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.EnergyModel
{
    /// <summary>
    ///     Serializable data object for storing defect energies
    /// </summary>
    [XmlRoot]
    public class DefectEnergyGraph : ProjectObjectGraph, IComparable<DefectEnergyGraph>
    {
        private ModelObjectReferenceGraph<Particle> particle;
        private ModelObjectReferenceGraph<UnitCellPosition> unitCellPosition;
        private double energy;

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReferenceGraph{T}" /> for the defect <see cref="Particle" />
        /// </summary>
        [XmlElement("Particle")]
        public ModelObjectReferenceGraph<Particle> Particle
        {
            get => particle;
            set => SetProperty(ref particle, value);
        }

        /// <summary>
        ///     Get or set the <see cref="ModelObjectReferenceGraph{T}" /> for the defect <see cref="UnitCellPosition" />
        /// </summary>
        [XmlElement("Position")]
        public ModelObjectReferenceGraph<UnitCellPosition> UnitCellPosition
        {
            get => unitCellPosition;
            set => SetProperty(ref unitCellPosition, value);
        }

        /// <summary>
        ///     Get or set the energy value in [eV]
        /// </summary>
        [XmlAttribute("Energy")]
        public double Energy
        {
            get => energy;
            set => SetProperty(ref energy, value);
        }

        /// <summary>
        ///     Get an <see cref="DefectEnergy" /> object for the model input pipeline
        /// </summary>
        /// <returns></returns>
        public DefectEnergy GetInputObject()
        {
            return new DefectEnergy
            {
                Energy = Energy,
                Particle = (IParticle) Particle.GetInputObject(),
                UnitCellPosition = (IUnitCellPosition) UnitCellPosition.GetInputObject()
            };
        }

        /// <inheritdoc />
        public int CompareTo(DefectEnergyGraph other)
        {
            if (other == null) return -1;
            if (ReferenceEquals(this, other)) return 0;
            var positionComp = string.CompareOrdinal(UnitCellPosition?.Key, other.UnitCellPosition?.Key);
            return positionComp == 0 ? string.CompareOrdinal(Particle?.Key, other.Particle?.Key) : positionComp;
        }
    }
}