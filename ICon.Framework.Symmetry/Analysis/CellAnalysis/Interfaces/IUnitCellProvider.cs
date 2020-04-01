using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.ValueTypes;
using Moccasin.Mathematics.ValueTypes;

namespace Mocassin.Symmetry.Analysis
{
    /// <summary>
    ///     Represents a provider of unit cells that supplies a unit cell for each (a,b,c) cell offset and carries the
    ///     geometric transformer for the basic cell
    /// </summary>
    public interface IUnitCellProvider<T1>
    {
        /// <summary>
        ///     The size information (a,b,c,p) of the supercell by reference
        /// </summary>
        ref Coordinates4I CellSize { get; }

        /// <summary>
        ///     Access the geometric vector encoder that supplies the basic cell position info and conversions between the
        ///     coordinate systems
        /// </summary>
        IUnitCellVectorEncoder VectorEncoder { get; }

        /// <summary>
        ///     Get the cell at arbitrary (a,b,c) position
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        IUnitCell<T1> GetUnitCell(int a, int b, int c);

        /// <summary>
        ///     Get the cell at arbitrary (a,b,c) position
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        IUnitCell<T1> GetUnitCell(in VectorI3 offset);

        /// <summary>
        ///     Get the cell entry at the specified (a,b,c,p) coordinates
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        LatticePoint<T1> GetCellEntry(int a, int b, int c, int p);

        /// <summary>
        ///     Get the cell entry at position 'p' with the specified offset coordinates
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        LatticePoint<T1> GetCellEntry(in VectorI3 offset, int p);

        /// <summary>
        ///     Get the cell entry at the specified 4D crystal vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        LatticePoint<T1> GetCellEntry(in CrystalVector4D vector);

        /// <summary>
        ///     Get the cell entry value at the specified absolute 3d fractional vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        T1 GetEntryValueAt(in Fractional3D vector);
    }
}