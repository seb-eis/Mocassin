using ICon.Mathematics.ValueTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ICon.Mathematics
{
    public class LinearizedVectorSeeker<T1>
    {
        public T1 Seek(DataIntVector3D position, DataIntVector3D size, IEnumerable<T1> enumerable)
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

        public T1 Seek(DataIntVector3D position, DataIntVector3D size, IList<T1> list)
        {
            int index = position.A + position.B * size.A + position.C * size.B * size.A;
            return list[index];
        }
    }
}
