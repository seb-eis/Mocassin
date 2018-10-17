using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Mocassin.Mathematics.Comparers;
using Mocassin.Mathematics.Extensions;
using Mocassin.Model.Basic;
using Mocassin.Symmetry.CrystalSystems;

namespace Mocassin.Model.Structures
{
    /// <summary>
    /// Cell parameters class (Simple wrapper for the crystal parameter set class to qualify as both model parameter and crystal parameter set)
    /// </summary>
    [DataContract(Name ="CellParameters")]
    public class CellParameters : ModelParameter, ICellParameters
    {
        /// <summary>
        /// The crystal parameter set
        /// </summary>
        [DataMember]
        public CrystalParameterSet ParameterSet { get; set; }

        /// <summary>
        /// The parameter length in A direction
        /// </summary>
        [IgnoreDataMember]
        public double ParamA => ParameterSet.ParamA;

        /// <summary>
        /// The parameter length in B direction
        /// </summary>
        [IgnoreDataMember]
        public double ParamB => ParameterSet.ParamB;

        /// <summary>
        /// The parameter length in C direction
        /// </summary>
        [IgnoreDataMember]
        public double ParamC => ParameterSet.ParamC;

        /// <summary>
        /// The angle alpha in radian
        /// </summary>
        [IgnoreDataMember]
        public double Alpha => ParameterSet.Alpha;

        /// <summary>
        /// The angle beta in radian
        /// </summary>
        [IgnoreDataMember]
        public double Beta => ParameterSet.Beta;

        /// <summary>
        /// The angle gamma in radian
        /// </summary>
        [IgnoreDataMember]
        public double Gamma => ParameterSet.Gamma;

        /// <summary>
        /// Angle alpha in degree
        /// </summary>
        [IgnoreDataMember]
        public double AlphaDegree => MocassinMath.RadianToDegree(Alpha);

        /// <summary>
        /// Angle beta in degree
        /// </summary>
        [IgnoreDataMember]
        public double BetaDegree => MocassinMath.RadianToDegree(Beta);

        /// <summary>
        /// Angle gamma in degree
        /// </summary>
        [IgnoreDataMember]
        public double GammaDegree => MocassinMath.RadianToDegree(Gamma);

        /// <summary>
        /// Implicit cast of crystal parameter set to the wrapper object
        /// </summary>
        /// <param name="paramSet"></param>
        public static implicit operator CellParameters(CrystalParameterSet paramSet)
        {
            return new CellParameters() { ParameterSet = paramSet };
        }

        /// <summary>
        /// Get the type name string
        /// </summary>
        /// <returns></returns>
        public override string GetParameterName()
        {
            return "'Cell Parameters'";
        }

        /// <summary>
        /// Copies the information from the provided parameter interface and returns the object (Retruns null if type mismatch)
        /// </summary>
        /// <param name="modelParameter"></param>
        /// <returns></returns>
        public override ModelParameter PopulateObject(IModelParameter modelParameter)
        {
            if (modelParameter is ICellParameters casted)
            {
                ParameterSet = new CrystalParameterSet(casted.ParamA, casted.ParamB, casted.ParamC, casted.Alpha, casted.Beta, casted.Gamma);
                return this;
            }
            return null;
        }

        /// <summary>
        /// Compares if the model parameter interface contains the same parameter info (Returns false if type cannot be cast)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(IModelParameter other)
        {
            var comparer = NumericComparer.CreateRanged(1.0e-10);
            if (other is ICellParameters castOther)
            {
                return comparer.Equals(ParamA, castOther.ParamA)
                    && comparer.Equals(ParamB, castOther.ParamB)
                    && comparer.Equals(ParamC, castOther.ParamC)
                    && comparer.Equals(Alpha, castOther.Alpha)
                    && comparer.Equals(Beta, castOther.Beta)
                    && comparer.Equals(Gamma, castOther.Gamma);
            }
            return false;
        }

        /// <summary>
        /// Creates a copy of the internal parameter set
        /// </summary>
        /// <returns></returns>
        public CrystalParameterSet AsParameterSet()
        {
            return new CrystalParameterSet(ParamA, ParamB, ParamC, Alpha, Beta, Gamma);
        }
    }
}
