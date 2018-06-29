using ICon.Mathematics.ValueTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ICon.Mathematics
{
    /// <summary>
    /// Logic class to find the entry of a linearised multidimensional enumerable by vector coordinates
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public class LinearizedVectorSeeker<T1>
    {
        /// <summary>
        /// Find the entry of a linearised multidimensional enumerable by vector coordinates
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public T1 Seek(CartesianInt3D position, CartesianInt3D size, IEnumerable<T1> enumerable)
        {
            var enumerator = enumerable.GetEnumerator();

            for (int i = 0; i < position.A; i++)
            {
                enumerator.MoveNext();
            }

            for (int i = 0; i < position.B * size.A; i++)
            {
                enumerator.MoveNext();
            }

            for (int i = 0; i < position.C * size.A * size.B; i++)
            {
                enumerator.MoveNext();
            }

            return enumerator.Current;
        }

        /// <summary>
        /// find the entry of a linearised multidimensional list by vector coordinates
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public T1 Seek(CartesianInt3D position, CartesianInt3D size, IList<T1> list)
        {
            int index = position.A + position.B * size.A + position.C * size.B * size.A;
            return list[index];
        }
    }
}
