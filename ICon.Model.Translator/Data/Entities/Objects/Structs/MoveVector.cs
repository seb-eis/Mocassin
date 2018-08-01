using System;
using System.Collections.Generic;
using System.Text;
using ICon.Mathematics.ValueTypes;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Move vector struct that contains an (a,b,c) fractional movement information and and index to mark an assignment
    /// </summary>
    public readonly struct MoveVector
    {
        /// <summary>
        /// The movement vector as a fractional 3D vector
        /// </summary>
        public Fractional3D Vector { get; }

        /// <summary>
        /// The tracking index of the movement vector
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Creates a new move vector from fractional vector and tracking id
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="id"></param>
        public MoveVector(in Fractional3D vector, int id) : this()
        {
            Vector = vector;
            Id = id;
        }

        /// <summary>
        /// Creates a new move vector from fractional vector. The tracking id is set to -1
        /// </summary>
        /// <param name="vector"></param>
        public MoveVector(in Fractional3D vector) : this(vector, -1)
        {

        }
    }
}
