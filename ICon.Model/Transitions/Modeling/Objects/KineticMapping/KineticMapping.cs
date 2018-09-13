﻿using System;
using System.Collections.Generic;
using System.Text;

using ICon.Mathematics.ValueTypes;
using ICon.Model.Structures;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Mapping for a kinetic transition that describes a 4D encoded transition path belonging to a specific kinetic reference transition
    /// </summary>
    public readonly struct KineticMapping
    {
        /// <summary>
        /// The unit cell position that describes the start point
        /// </summary>
        public IUnitCellPosition StartUnitCellPosition { get; }

        /// <summary>
        /// The unit cell position that describes the end point
        /// </summary>
        public IUnitCellPosition EndUnitCellPosition { get; }

        /// <summary>
        /// The interface of the transition the mapping is valid for
        /// </summary>
        public IKineticTransition Transition { get; }

        /// <summary>
        /// The transition path encoded as a set of 4D vectors (Cannot be null)
        /// </summary>
        public CrystalVector4D[] EncodedPath { get; }

        /// <summary>
        /// The transition path in fractional coordinates (Can be null)
        /// </summary>
        public Fractional3D[] FractionalPath { get; }

        /// <summary>
        /// The path length of the mapping
        /// </summary>
        public int PathLength => Transition.GeometryStepCount;

        /// <summary>
        /// The 4D encoded start position vector of the mapping
        /// </summary>
        public CrystalVector4D StartVector4D => EncodedPath[0];

        /// <summary>
        /// The 4D encoded end position vector of the mapping
        /// </summary>
        public CrystalVector4D EndVector4D => EncodedPath[PathLength - 1];

        /// <summary>
        /// The 3D fractional start position vector of the mapping
        /// </summary>
        public Fractional3D StartVector3D => FractionalPath[0];

        /// <summary>
        /// The 3D fractional end position vector of the mapping
        /// </summary>
        public Fractional3D EndVector3D => FractionalPath[PathLength - 1];

        /// <summary>
        /// Create a new kinetic mapping from transition interface, cell positions, 4D encoded transition path and fractional path
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="startUnitCellPosition"></param>
        /// <param name="endUnitCellPosition"></param>
        /// <param name="encodedPath"></param>
        /// <param name="fractionalPath"></param>
        public KineticMapping(IKineticTransition transition, IUnitCellPosition startUnitCellPosition, IUnitCellPosition endUnitCellPosition, CrystalVector4D[] encodedPath, Fractional3D[] fractionalPath) : this()
        {
            Transition = transition;
            StartUnitCellPosition = startUnitCellPosition;
            EndUnitCellPosition = endUnitCellPosition;
            EncodedPath = encodedPath ?? throw new ArgumentNullException(nameof(encodedPath));
            FractionalPath = fractionalPath;
        }

        /// <summary>
        /// Create a new kinetic mapping from transition, 4D encoded transition path (The fractional path is set to null)
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="encodedPath"></param>
        public KineticMapping(IKineticTransition transition, CrystalVector4D[] encodedPath) : this(transition, null,null, encodedPath, null)
        {

        }
    }
}
