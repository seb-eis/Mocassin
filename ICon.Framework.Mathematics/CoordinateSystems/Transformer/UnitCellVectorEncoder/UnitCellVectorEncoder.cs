using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Mathematics.Comparers;
using Mocassin.Framework.Collections;
using Mocassin.Mathematics.Extensions;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Mathematics.Coordinates
{
    /// <inheritdoc />
    public class UnitCellVectorEncoder : IUnitCellVectorEncoder
    {
        /// <inheritdoc />
        public SetList<Fractional3D> PositionList { get; protected set; }

        /// <inheritdoc />
        public IVectorTransformer Transformer { get; protected set; }

        /// <inheritdoc />
        public int PositionCount => PositionList.Count;

        /// <summary>
        /// Creates new vector encoder with specified position list and vector transformer
        /// </summary>
        /// <param name="positionList"></param>
        /// <param name="vectorTransformer"></param>
        public UnitCellVectorEncoder(SetList<Fractional3D> positionList, IVectorTransformer vectorTransformer)
        {
            PositionList = positionList ?? throw new ArgumentNullException(nameof(positionList));
            Transformer = vectorTransformer ?? throw new ArgumentNullException(nameof(vectorTransformer));
        }

        /// <inheritdoc />
        public bool TryEncode(IFractional3D vector, out CrystalVector4D encoded)
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

        /// <inheritdoc />
        public bool TryEncode(IFractional3D origin, IFractional3D vector, out CrystalVector4D encoded)
        {
            return TryEncode(new Fractional3D(vector.A + origin.A, vector.B + origin.B, vector.C + origin.C), out encoded);
        }

        /// <inheritdoc />
        public bool TryEncode(IEnumerable<IFractional3D> decoded, out List<CrystalVector4D> encoded)
        {
            encoded = new List<CrystalVector4D>(100);
            foreach (var item in decoded)
            {
                if (!TryEncode(item, out var singleEncoded))
                {
                    return false;
                }
                encoded.Add(singleEncoded);
            }
            return true;
        }

        /// <inheritdoc />
        public bool TryEncodeAsRelative(IFractional3D origin, IFractional3D vector, out CrystalVector4D encoded)
        {
            var startSuccess = TryEncode(origin, out var encodedStart);
            var endSuccess = TryEncode(origin, vector, out var encodedEnd);
            if (startSuccess && endSuccess)
            {
                encoded = encodedEnd - encodedStart;
                return true;
            }
            encoded = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryEncode(ICartesian3D vector, out CrystalVector4D encoded)
        {
            return TryEncode(Transformer.ToFractional(vector), out encoded);
        }

        /// <inheritdoc />
        public bool TryEncode(ICartesian3D origin, ICartesian3D vector, out CrystalVector4D encoded)
        {
            return TryEncode(Transformer.ToFractional(origin), Transformer.ToFractional(vector), out encoded);
        }

        /// <inheritdoc />
        public bool TryEncodeAsRelative(ICartesian3D origin, ICartesian3D vector, out CrystalVector4D encoded)
        {
            return TryEncodeAsRelative(Transformer.ToFractional(origin), Transformer.ToFractional(vector), out encoded);
        }

        /// <inheritdoc />
        public bool TryEncode(ISpherical3D vector, out CrystalVector4D encoded)
        {
            return TryEncode(Transformer.ToFractional(vector), out encoded);
        }

        /// <inheritdoc />
        public bool TryEncode(IVector3D origin, ISpherical3D vector, out CrystalVector4D encoded)
        {
            return TryEncode(Transformer.ToFractional(origin), Transformer.ToFractional(vector), out encoded);
        }

        /// <inheritdoc />
        public bool TryEncodeAsRelative(IVector3D origin, ICartesian3D vector, out CrystalVector4D encoded)
        {
            return TryEncodeAsRelative(Transformer.ToFractional(origin), Transformer.ToFractional(vector), out encoded);
        }

        /// <inheritdoc />
        public bool TryEncode(IVector3D vector, out CrystalVector4D encoded)
        {
            return TryEncode(Transformer.ToFractional(vector), out encoded);
        }

        /// <inheritdoc />
        public bool TryEncode(IVector3D origin, IVector3D vector, out CrystalVector4D encoded)
        {
            return TryEncode(Transformer.ToFractional(origin), Transformer.ToFractional(vector), out encoded);
        }

        /// <inheritdoc />
        public bool TryEncodeAsRelative(IVector3D origin, IVector3D vector, out CrystalVector4D encoded)
        {
            return TryEncodeAsRelative(Transformer.ToFractional(origin), Transformer.ToFractional(vector), out encoded);
        }

        /// <inheritdoc />
        public bool TryDecode(ICrystalVector4D encoded, out Fractional3D decoded)
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

        /// <inheritdoc />
        public bool TryDecode(IEnumerable<ICrystalVector4D> encoded, out List<Fractional3D> decoded)
        {
            var result = new List<Fractional3D>();
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

        /// <inheritdoc />
        public bool TryDecode(ICrystalVector4D encoded, out Cartesian3D decoded)
        {
            if(!TryDecode(encoded, out Fractional3D fractional))
            {
                decoded = default;
                return false;
            }
            decoded = Transformer.ToCartesian(fractional);
            return true;
        }

        /// <inheritdoc />
        public bool TryDecode(ICrystalVector4D encoded, out Spherical3D decoded)
        {
            if (!TryDecode(encoded, out Fractional3D fractional))
            {
                decoded = default;
                return false;
            }
            decoded = Transformer.ToSpherical(fractional);
            return true;
        }

        /// <inheritdoc />
        public bool TryDecodeToRelative(ICrystalVector4D origin, ICrystalVector4D encoded, out Fractional3D decoded)
        {
            var total = new CrystalVector4D(origin.A + encoded.A, origin.B + encoded.B, origin.C + encoded.C, origin.P + encoded.P);
            var totalSuccess = TryDecode(total, out Fractional3D totalDecoded);
            var originSuccess = TryDecode(origin, out Fractional3D originDecoded);
            if (totalSuccess && originSuccess)
            {
                decoded = totalDecoded - originDecoded;
                return true;
            }
            decoded = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryDecodeToRelative(ICrystalVector4D origin, ICrystalVector4D encoded, out Cartesian3D decoded)
        {
            var total = new CrystalVector4D(origin.A + encoded.A, origin.B + encoded.B, origin.C + encoded.C, origin.P + encoded.P);
            var totalSuccess = TryDecode(total, out Cartesian3D totalDecoded);
            var originSuccess = TryDecode(origin, out Cartesian3D originDecoded);
            if (totalSuccess && originSuccess)
            {
                decoded = totalDecoded - originDecoded;
                return true;
            }
            decoded = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryDecodeToRelative(ICrystalVector4D origin, ICrystalVector4D encoded, out Spherical3D decoded)
        {
            if (!TryDecodeToRelative(origin, encoded, out Cartesian3D decodedCartesian))
            {
                decoded = default;
                return false;
            }
            decoded = Transformer.ToSpherical(decodedCartesian);
            return true;
        }

        /// <inheritdoc />
        public Coordinates<int, int, int> GetTargetCellOffset(IFractional3D vector)
        {
            var a = MocassinMath.FloorToInt(vector.A, Transformer.FractionalSystem.Comparer);
            var b = MocassinMath.FloorToInt(vector.B, Transformer.FractionalSystem.Comparer);
            var c = MocassinMath.FloorToInt(vector.C, Transformer.FractionalSystem.Comparer);
            return new Coordinates<int, int, int>(a, b, c);
        }

        /// <inheritdoc />
        public Fractional3D GetOriginCellTrimmedVector(IFractional3D vector, out Coordinates<int, int, int> offset)
        {
            offset = GetTargetCellOffset(vector);
            return new Fractional3D(vector.A - offset.A, vector.B - offset.B, vector.C - offset.C);
        }

        /// <inheritdoc />
        public (Cartesian3D A, Cartesian3D B, Cartesian3D C) GetBaseVectors()
        {
            return Transformer.FractionalSystem.GetBaseVectors();
        }

        /// <inheritdoc />
        public double GetCellVolume()
        {
            var (a, b, c) = GetBaseVectors();
            return Math.Abs(a.GetSpatProduct(b, c));
        }

        /// <inheritdoc />
        public Fractional3D GetPosition(int index)
        {
            return PositionList[index];
        }

        /// <inheritdoc />
        public Cartesian3D GetCartesianPosition(int index)
        {
            return Transformer.ToCartesian(PositionList[index]);
        }
    }
}
