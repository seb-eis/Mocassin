﻿using System.Collections.Generic;
using ICon.Framework.Collections;
using ICon.Mathematics.ValueTypes;

namespace ICon.Mathematics.Coordinates
{
    /// <summary>
    /// Encoder system that handles the transformations between 3D (spherical, fractional, cartesian) and 4D encoded super-cell coordinates
    /// </summary>
    public interface IUnitCellVectorEncoder
    {
        /// <summary>
        /// The sorted list (Binary searchable) of possible fractional coordinates any 3D vector could point to
        /// </summary>
        SetList<Fractional3D> PositionList { get; }

        /// <summary>
        /// Instance of the vector transformer that handles 3D coordinate system conversions
        /// </summary>
        IVectorTransformer Transformer { get; }

        /// <summary>
        /// Get the number of fractional positions supported by the encoder
        /// </summary>
        int PositionCount { get; }

        /// <summary>
        /// Encodes a fractional vector into the 4D crystal vector representation (Returns false if vector cannot be encoded )
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        bool TryEncode(IFractional3D vector, out CrystalVector4D encoded);

        /// <summary>
        /// Tries to encode the fractional vector that starts a specific fractional position into an absolute 4D vector (Returns false if not possible)
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="origin"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        bool TryEncode(IFractional3D origin, IFractional3D vector, out CrystalVector4D encoded);


        /// <summary>
        /// Tries to encode a sequence of fractional vectors into a list of encoded crystal 4D vectors (Returns false if one of the encodings fails)
        /// </summary>
        /// <param name="decoded"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        bool TryEncode(IEnumerable<IFractional3D> decoded, out List<CrystalVector4D> encoded);

        /// <summary>
        /// Tries to encode a vector that points from a start to another position into a relative 4D vector (Returns false if not possible)
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="origin"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        bool TryEncodeAsRelative(IFractional3D origin, IFractional3D vector, out CrystalVector4D encoded);

        /// <summary>
        /// tries to encode a cartesian vector that starts at unit cell origin point into a 4D vector (Returns false if not possible)
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        bool TryEncode(ICartesian3D vector, out CrystalVector4D encoded);

        /// <summary>
        /// Tries to encode a cartesian vector that starts at another cartesian position into an absolute 4D vector (Returns false if not possible)
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="vector"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        bool TryEncode(ICartesian3D origin, ICartesian3D vector, out CrystalVector4D encoded);

        /// <summary>
        /// Tries to encode a cartesian vector that starts at another cartesian vector into a relative 4D vector (Returns false if not possible)
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="vector"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        bool TryEncodeAsRelative(ICartesian3D origin, ICartesian3D vector, out CrystalVector4D encoded);

        /// <summary>
        /// tries to encode a spherical vector that starts at unit cell origin point into a 4D vector (Returns false if not possible)
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        bool TryEncode(ISpherical3D vector, out CrystalVector4D encoded);

        /// <summary>
        /// Tries to encode a spherical vector that starts at another arbitrarily defined vector position (Defined from unit cell origin) into an absolute 4D vector (Returns false if not possible)
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="vector"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        bool TryEncode(IVector3D origin, ISpherical3D vector, out CrystalVector4D encoded);

        /// <summary>
        /// Tries to encode a spherical vector that starts at another arbitrarily defined vector start (From unit cell origin) into a relative 4D vector (Returns false if not possible)
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="vector"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        bool TryEncodeAsRelative(IVector3D origin, ICartesian3D vector, out CrystalVector4D encoded);

        /// <summary>
        /// Tries to encode any 3D vector that starts at unit cell origin into an absolute 4D vector (Returns false if not possible)
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        bool TryEncode(IVector3D vector, out CrystalVector4D encoded);

        /// <summary>
        /// Tries to encode any vector that starts at any other start vector into an absolute 4D vector (Returns false if not possible)
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="vector"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        bool TryEncode(IVector3D origin, IVector3D vector, out CrystalVector4D encoded);

        /// <summary>
        /// Tries to encode any vector that is relative to any start vector into a 4D relative vector (Returns false if possible)
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="vector"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        bool TryEncodeAsRelative(IVector3D origin, IVector3D vector, out CrystalVector4D encoded);

        /// <summary>
        /// Tries to decode the provided 4D encoded vector to an absolute fractional 3D vector (Returns false if not possible, happens if 4D vector has negative P entry)
        /// </summary>
        /// <param name="encoded"></param>
        /// <param name="decoded"></param>
        /// <returns></returns>
        bool TryDecode(ICrystalVector4D encoded, out Fractional3D decoded);

        /// <summary>
        /// Tries to decode multiple 4D vectors into a list of fractional vectors (Returns false if not possible)
        /// </summary>
        /// <param name="encoded"></param>
        /// <param name="decoded"></param>
        /// <returns></returns>
        bool TryDecode(IEnumerable<ICrystalVector4D> encoded, out List<Fractional3D> decoded);

        /// <summary>
        /// Tries to decode 4D encoded vector into an absolute cartesian vector starting at unit cell origin (Returns false if not possible)
        /// </summary>
        /// <param name="encoded"></param>
        /// <param name="decoded"></param>
        /// <returns></returns>
        bool TryDecode(ICrystalVector4D encoded, out Cartesian3D decoded);

        /// <summary>
        /// Tries to decoded 4D encoded vector into a spherical vector from unit cell origin (Returns false if not possible)
        /// </summary>
        /// <param name="encoded"></param>
        /// <param name="decoded"></param>
        /// <returns></returns>
        bool TryDecode(ICrystalVector4D encoded, out Spherical3D decoded);

        /// <summary>
        /// Tries to decode a relative 4D vector that starts at a 4D origin point into a relative 3D fractional vector (Returns false if not possible)
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="encoded"></param>
        /// <param name="decoded"></param>
        /// <returns></returns>
        bool TryDecodeToRelative(ICrystalVector4D origin, ICrystalVector4D encoded, out Fractional3D decoded);

        /// <summary>
        /// Tries to decode a relative 4D vector that starts at a 4D origin point into a relative 3D cartesian vector (Returns false if not possible)
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="encoded"></param>
        /// <param name="decoded"></param>
        /// <returns></returns>
        bool TryDecodeToRelative(ICrystalVector4D origin, ICrystalVector4D encoded, out Cartesian3D decoded);

        /// <summary>
        /// Tries to decode a relative 4D vector that starts at a 4D origin point into a relative 3D spherical vector (Returns false if not possible)
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="encoded"></param>
        /// <param name="decoded"></param>
        /// <returns></returns>
        bool TryDecodeToRelative(ICrystalVector4D origin, ICrystalVector4D encoded, out Spherical3D decoded);

        /// <summary>
        /// Calculates the cell offset counts from origin in a,b,c direction of a generic fractional vector (Uses tolerance of the fractional system)
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        Coordinates<int, int, int> GetTargetCellOffset(IFractional3D vector);

        /// <summary>
        /// Trims the coordinates of any fractional vector into the origin cell (Optional out parameter contains the calculated unit cell offset)
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Fractional3D GetOriginCellTrimmedVector(IFractional3D vector, out Coordinates<int, int, int> offset);

        /// <summary>
        /// Get the base vectors of the fractional system
        /// </summary>
        /// <returns></returns>
        (Cartesian3D A, Cartesian3D B, Cartesian3D C) GetBaseVectors();

        /// <summary>
        /// Calculates the volume of the unit cell as the spat product of the base vectors
        /// </summary>
        /// <returns></returns>
        double GetCellVolume();

        /// <summary>
        /// Get the fractional position at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Fractional3D GetPosition(int index);

        /// <summary>
        /// Get the position at the specified index as a cartesian vector
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Cartesian3D GetCartesianPosition(int index);
    }
}