using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Mathematics.Extensions;
using ICon.Mathematics.ValueTypes;
using ICon.Mathematics.Comparers;
using ICon.Framework.Collections;

namespace ICon.Mathematics.Coordinates
{
    /// <summary>
    /// Vector encoder that handles the transformations between 3D (spherical, fractional, cartesian) and 4D encoded supercell coordinates
    /// </summary>
    public class UnitCellVectorEncoder
    {
        /// <summary>
        /// The sorted list (Binary searchable) of possible fractional coordinates any 3D vector could point to
        /// </summary>
        public SetList<Fractional3D> PositionList { get; protected set; }

        /// <summary>
        /// Instance of the vector transformer that handles 3D coordinate system conversions
        /// </summary>
        public VectorTransformer Transformer { get; protected set; }

        /// <summary>
        /// Get the number of fractional positions supported by the encoder
        /// </summary>
        public int PositionCount => PositionList.Count;

        /// <summary>
        /// Creates new vector encoder with specififed position list and vector transformer
        /// </summary>
        /// <param name="positionList"></param>
        /// <param name="vectorTransformer"></param>
        public UnitCellVectorEncoder(SetList<Fractional3D> positionList, VectorTransformer vectorTransformer)
        {
            PositionList = positionList ?? throw new ArgumentNullException(nameof(positionList));
            Transformer = vectorTransformer ?? throw new ArgumentNullException(nameof(vectorTransformer));
        }

        /// <summary>
        /// Encodes a generic fractional vector into the 4D crystal vector representation (Returns false if vector cannot be encoded )
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public bool TryEncodeFractional<T1>(T1 vector, out CrystalVector4D encoded) where T1 : IFractional3D
        {
            var index = PositionList.IndexOf(GetOriginCellTrimmedVector(vector, out var offset));
            if (index < 0)
            {
                encoded = default;
                return false;
            }
            encoded = new CrystalVector4D(offset.A, offset.B, offset.C, index);
            return true;
        }

        /// <summary>
        /// Tries to encode the fractional vector that starts a specific fractional position into an absolute 4D vector (Returns false if not possible)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="vector"></param>
        /// <param name="origin"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public bool TryEncodeFractional<T1, T2>(T1 origin, T2 vector, out CrystalVector4D encoded) where T1 : IFractional3D where T2 : IFractional3D
        {
            return TryEncodeFractional(new Fractional3D(vector.A + origin.A, vector.B + origin.B, vector.C + origin.C), out encoded);
        }

        /// <summary>
        /// Tries to encode a sequence of fractionl vectors into a list of encoded crystal 4D vectors (Retruns false if one of the encodings fails)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="decoded"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public bool TryEncodeFractional<T1>(IEnumerable<T1> decoded, out List<CrystalVector4D> encoded) where T1 : IFractional3D
        {
            encoded = new List<CrystalVector4D>(100);
            foreach (var item in decoded)
            {
                if (!TryEncodeFractional(item, out var singleEncoded))
                {
                    return false;
                }
                encoded.Add(singleEncoded);
            }
            return true;
        }

        /// <summary>
        /// Tries to encode a vector that points from a start to another position into a relative 4D vector (Returns false if not possible)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="vector"></param>
        /// <param name="origin"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public bool TryEncodeFractionalAsRelative<T1, T2>(T1 origin, T2 vector, out CrystalVector4D encoded) where T1 : IFractional3D where T2 : IFractional3D
        {
            var startSuccess = TryEncodeFractional(origin, out var encodedStart);
            var endSuccess = TryEncodeFractional(origin, vector, out var encodedEnd);
            if (startSuccess && endSuccess)
            {
                encoded = encodedEnd - encodedStart;
                return true;
            }
            encoded = default;
            return false;
        }

        /// <summary>
        /// tries to encode a cartesian vector that starts at unit cell origin point into a 4D vector (Returns false if not possible)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="vector"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public bool TryEncodeCartesian<T1>(T1 vector, out CrystalVector4D encoded) where T1 : ICartesian3D
        {
            return TryEncodeFractional(Transformer.FractionalFromCartesian(vector), out encoded);
        }

        /// <summary>
        /// Tries to encode a cartesian vector that starts at another cartesian position into an absolute 4D vector (Returns false if not possible)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="origin"></param>
        /// <param name="vector"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public bool TryEncodeCartesian<T1, T2>(T1 origin, T2 vector, out CrystalVector4D encoded) where T1 : ICartesian3D where T2 : ICartesian3D
        {
            return TryEncodeFractional(Transformer.FractionalFromCartesian(origin), Transformer.FractionalFromCartesian(vector), out encoded);
        }

        /// <summary>
        /// Tries to encode a cartesian vector that starts at another cartesian vector into a relative 4D vector (Returns false if not possible)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="origin"></param>
        /// <param name="vector"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public bool TryEncodeCartesianAsRelative<T1, T2>(T1 origin, T2 vector, out CrystalVector4D encoded) where T1 : ICartesian3D where T2 : ICartesian3D
        {
            return TryEncodeFractionalAsRelative(Transformer.FractionalFromCartesian(origin), Transformer.FractionalFromCartesian(vector), out encoded);
        }

        /// <summary>
        /// tries to encode a spherical vector that starts at unit cell origin point into a 4D vector (Returns false if not possible)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="vector"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public bool TryEncodeSpherical<T1>(T1 vector, out CrystalVector4D encoded) where T1 : ISpherical3D
        {
            return TryEncodeFractional(Transformer.FractionalFromSpherical(vector), out encoded);
        }

        /// <summary>
        /// Tries to encode a spherical vector that starts at another arbitrarily defined vector position (Defined from unit cell origin) into an absolute 4D vector (Returns false if not possible)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="origin"></param>
        /// <param name="vector"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public bool TryEncodeSpherical<T1, T2>(T1 origin, T2 vector, out CrystalVector4D encoded) where T1 : IVector3D where T2 : ISpherical3D
        {
            return TryEncodeFractional(Transformer.ToFractional(origin), Transformer.FractionalFromSpherical(vector), out encoded);
        }

        /// <summary>
        /// Tries to encode a spherical vector that starts at another abitrarily defined vector start (From unit cell origin) into a relative 4D vector (Returns false if not possible)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="origin"></param>
        /// <param name="vector"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public bool TryEncodeSphericalAsRelative<T1, T2>(T1 origin, T2 vector, out CrystalVector4D encoded) where T1 : IVector3D where T2 : ICartesian3D
        {
            return TryEncodeFractionalAsRelative(Transformer.ToFractional(origin), Transformer.FractionalFromCartesian(vector), out encoded);
        }

        /// <summary>
        /// Tries to encode any 3D vector that starts at unit cell origin into an absolute 4D vector (Returns false if not possible)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="vector"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public bool TryEncodeAny<T1>(T1 vector, out CrystalVector4D encoded) where T1 : IVector3D
        {
            return TryEncodeFractional(Transformer.ToFractional(vector), out encoded);
        }

        /// <summary>
        /// Tries to encode any vector that starts at any other start vector into an absolute 4D vector (Returns false if not possible)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="origin"></param>
        /// <param name="vector"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public bool TryEncodeMixed<T1, T2>(T1 origin, T2 vector, out CrystalVector4D encoded) where T1 : IVector3D where T2 : IVector3D
        {
            return TryEncodeFractional(Transformer.ToFractional(origin), Transformer.ToFractional(vector), out encoded);
        }

        /// <summary>
        /// Tries to encode any vector that is relative to any start vector into a 4D relative vector (Returns false if possible)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="origin"></param>
        /// <param name="vector"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public bool TryEncodeMixedAsRelative<T1, T2>(T1 origin, T2 vector, out CrystalVector4D encoded) where T1 : IVector3D where T2 : IVector3D
        {
            return TryEncodeFractionalAsRelative(Transformer.ToFractional(origin), Transformer.ToFractional(vector), out encoded);
        }

        /// <summary>
        /// Tries to decode the provided 4D encoded vector to an absolute fractional 3D vector (Returns false if not possible, happens if 4D vector has negative P entry)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="encoded"></param>
        /// <param name="decoded"></param>
        /// <returns></returns>
        public bool TryDecode<T1>(T1 encoded, out Fractional3D decoded) where T1 : ICrystalVector4D
        {
            if (encoded.P < 0 || encoded.P >= PositionList.Count)
            {
                decoded = default;
                return false;
            }
            var offset = PositionList[encoded.P];
            decoded = new Fractional3D(encoded.A + offset.A, encoded.B + offset.B, encoded.C + offset.C);
            return true;
        }

        /// <summary>
        /// Tries to decode multiple 4D vectors into a list of fractional vectors (Returns false if not possible)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="encoded"></param>
        /// <param name="decoded"></param>
        /// <returns></returns>
        public bool TryDecode<T1>(IEnumerable<T1> encoded, out List<Fractional3D> decoded) where T1 : ICrystalVector4D
        {
            var result = new List<Fractional3D>(encoded.Count());
            foreach (var item in encoded)
            {
                if (!TryDecode(item, out Fractional3D itemDecoded))
                {
                    decoded = default;
                    return false;
                }
                result.Add(itemDecoded);
            }
            decoded = result;
            return true;
        }

        /// <summary>
        /// Tries to decode 4D encoded vector into an absolute cartesian vector starting at unit cell origin (Returns false if not possible)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="encoded"></param>
        /// <param name="decoded"></param>
        /// <returns></returns>
        public bool TryDecode<T1>(T1 encoded, out Cartesian3D decoded) where T1 : ICrystalVector4D
        {
            if(!TryDecode(encoded, out Fractional3D fractional))
            {
                decoded = default;
                return false;
            }
            decoded = Transformer.CartesianFromFractional(fractional);
            return true;
        }

        /// <summary>
        /// Tries to decoded 4D encoded vector into a spherical vector from unit cell origin (Returns false if not possible)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="encoded"></param>
        /// <param name="decoded"></param>
        /// <returns></returns>
        public bool TryDecode<T1>(T1 encoded, out Spherical3D decoded) where T1 : ICrystalVector4D
        {
            if (!TryDecode(encoded, out Fractional3D fractional))
            {
                decoded = default;
                return false;
            }
            decoded = Transformer.SphericalFromFractional(fractional);
            return true;
        }

        /// <summary>
        /// Tries to decode a relative 4D vector that starts at a 4D origin point into a relative 3D fractional vector (Returns false if not possible)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="origin"></param>
        /// <param name="encoded"></param>
        /// <param name="decoded"></param>
        /// <returns></returns>
        public bool TryDecodeToRelative<T1, T2>(T1 origin, T2 encoded, out Fractional3D decoded) where T1 : ICrystalVector4D where T2 : ICrystalVector4D
        {
            var total = new CrystalVector4D(origin.A + encoded.A, origin.B + encoded.B, origin.C + encoded.C, origin.P + encoded.P);
            bool totalSuccess = TryDecode(total, out Fractional3D totalDecoded);
            bool originSuccess = TryDecode(origin, out Fractional3D originDecoded);
            if (totalSuccess && originSuccess)
            {
                decoded = totalDecoded - originDecoded;
                return true;
            }
            decoded = default;
            return false;
        }

        /// <summary>
        /// Tries to decode a relative 4D vector that starts at a 4D origin point into a relative 3D cartesian vector (Returns false if not possible)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="origin"></param>
        /// <param name="encoded"></param>
        /// <param name="decoded"></param>
        /// <returns></returns>
        public bool TryDecodeToRelative<T1, T2>(T1 origin, T2 encoded, out Cartesian3D decoded) where T1 : ICrystalVector4D where T2 : ICrystalVector4D
        {
            var total = new CrystalVector4D(origin.A + encoded.A, origin.B + encoded.B, origin.C + encoded.C, origin.P + encoded.P);
            bool totalSuccess = TryDecode(total, out Cartesian3D totalDecoded);
            bool originSuccess = TryDecode(origin, out Cartesian3D originDecoded);
            if (totalSuccess && originSuccess)
            {
                decoded = totalDecoded - originDecoded;
                return true;
            }
            decoded = default;
            return false;
        }

        /// <summary>
        /// Tries to decode a relative 4D vector that starts at a 4D origin point into a relative 3D spherical vector (Returns false if not possible)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="origin"></param>
        /// <param name="encoded"></param>
        /// <param name="decoded"></param>
        /// <returns></returns>
        public bool TryDecodeToRelative<T1, T2>(T1 origin, T2 encoded, out Spherical3D decoded) where T1 : ICrystalVector4D where T2 : ICrystalVector4D
        {
            if (!TryDecodeToRelative(origin, encoded, out Cartesian3D decodedCartesian))
            {
                decoded = default;
                return false;
            }
            decoded = Transformer.SphericalFromCartesian(decodedCartesian);
            return true;
        }

        /// <summary>
        /// Calculates the cell offset counts from origin in a,b,c direction of a generic fractional vector (Uses tolerance of the fractional system)
        /// </summary>
        /// <typeparam name="TVector"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Coordinates<int, int, int> GetTargetCellOffset<TVector>(TVector vector) where TVector : IFractional3D
        {
            int a = ExtMath.FloorToInt(vector.A, Transformer.FractionalSystem.Comparer);
            int b = ExtMath.FloorToInt(vector.B, Transformer.FractionalSystem.Comparer);
            int c = ExtMath.FloorToInt(vector.C, Transformer.FractionalSystem.Comparer);
            return new Coordinates<int, int, int>(a, b, c);
        }

        /// <summary>
        /// Trims the coordinates of any fractional vector into the origin cell (Optional out parameter contains the calculated unit cell offset)
        /// </summary>
        /// <typeparam name="TVector"></typeparam>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Fractional3D GetOriginCellTrimmedVector<TVector>(TVector vector, out Coordinates<int, int, int> offset) where TVector : IFractional3D
        {
            offset = GetTargetCellOffset(vector);
            return new Fractional3D(vector.A - offset.A, vector.B - offset.B, vector.C - offset.C);
        }

        /// <summary>
        /// Get the base vectors of the fractional system (Repaly to the function in the fractional coordinate system)
        /// </summary>
        /// <returns></returns>
        public (Cartesian3D A, Cartesian3D B, Cartesian3D C) GetBaseVectors()
        {
            return Transformer.FractionalSystem.GetBaseVectors();
        }

        /// <summary>
        /// Calculates the volume of the unit cell as the spat product of the base vectors
        /// </summary>
        /// <returns></returns>
        public double GetCellVolume()
        {
            var (A, B, C) = GetBaseVectors();
            return Math.Abs(A.GetSpatProduct(B, C));
        }

        /// <summary>
        /// Get the fractional position at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Fractional3D GetPosition(int index)
        {
            return PositionList[index];
        }

        /// <summary>
        /// Get the position at the specififed index as a cartesian vector
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Cartesian3D GetCartesianPosition(int index)
        {
            return Transformer.CartesianFromFractional(PositionList[index]);
        }
    }
}
