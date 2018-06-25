using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ICon.Model.Basic;

namespace ICon.Model.Particles
{
    /// <summary>
    /// Represents a single unique model particle
    /// </summary>
    [DataContract]
    public class Particle : ModelObject, IParticle
    {
        /// <summary>
        /// The charge value in electron volts
        /// </summary>
        [DataMember]
        public double Charge { get; set; }

        /// <summary>
        /// The long name of th particle
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// The short symbol definition of the particle
        /// </summary>
        [DataMember]
        public string Symbol { get; set; }

        /// <summary>
        /// Flag that marks the particle as capable of acting as a vacancy
        /// </summary>
        [DataMember]
        public bool IsVacancy { get; set; }

        /// <summary>
        /// Flag that marks the particle as a 'Null-Particle' that is not deprecated but does not exists in the current context
        /// </summary>
        [DataMember]
        public bool IsEmpty { get; set; }

        /// <summary>
        /// Compares particle by name, symbol and then charge (Charge is not compared with tolerance)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Int32 CompareTo(IParticle other)
        {
            Int32 nameCompare = Name.CompareTo(other.Name);
            if (nameCompare == 0)
            {
                Int32 symbolCompare = Symbol.CompareTo(other.Symbol);
                if (symbolCompare == 0)
                {
                    return Charge.CompareTo(other.Charge);
                }
                return symbolCompare;
            }
            return nameCompare;
        }

        /// <summary>
        /// Compares particles for equality with a specific comparer for the charge value
        /// </summary>
        /// <param name="other"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public Boolean EqualsInModelProperties(IParticle other, IComparer<Double> comparer)
        {
            return Name == other.Name && Symbol == other.Symbol && comparer.Compare(Charge, other.Charge) == 0;
        }

        /// <summary>
        /// Creates a void particle, this particle represents an active by context unavailable particle and should always have the index 0 in a particle manager
        /// </summary>
        /// <returns></returns>
        public static Particle CreateEmpty()
        {
            return new Particle() { Name = "Void", Symbol = "Void", Charge = 0.0, Index = 0, IsEmpty = true };
        }

        /// <summary>
        /// Get a string that represents the name of the object type Particle
        /// </summary>
        /// <returns></returns>
        public override String GetModelObjectName()
        {
            return "'Particle'";
        }


        /// <summary>
        /// Tries to create new particle object from the model object interface (Returns null for type mismatch or deprecated model object)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ModelObject PopulateObject(IModelObject obj)
        {
            if (CastWithDepricatedCheck<IParticle>(obj) is var particle)
            {
                if (particle.IsEmpty)
                {
                    throw new ArgumentException("Empty particle object interface reached consume function");
                }
                Name = particle.Name;
                Symbol = particle.Symbol;
                Charge = particle.Charge;
                IsVacancy = particle.IsVacancy;
                return this;
            }
            return null;
        }

        /// <summary>
        /// Compares to other particle based upon the particle index
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IParticle other)
        {
            return Index == other.Index;
        }

        /// <summary>
        /// Get the hash code of the particle based upon the particle index
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return 1 << Index;
        }
    }
}
