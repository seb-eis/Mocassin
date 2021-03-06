﻿using System;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Particles;

namespace Mocassin.Tools.Evaluation.Queries.Data
{
    /// <summary>
    ///     Struct that contains the tracking data of a single mobile <see cref="IParticle" />
    /// </summary>
    public readonly struct MobileTrackerResult
    {
        /// <summary>
        ///     Get the index of the current position id
        /// </summary>
        public int PositionId { get; }

        /// <summary>
        ///     Get the <see cref="IParticle" /> that the movement belongs to
        /// </summary>
        public IParticle Particle { get; }

        /// <summary>
        ///     Get the <see cref="Cartesian3D" /> that describes the displacement in [m] in x,y,z directions
        /// </summary>
        public Cartesian3D Displacement { get; }

        /// <inheritdoc />
        public MobileTrackerResult(IParticle particle, int positionId, in Cartesian3D displacement)
            : this()
        {
            Particle = particle ?? throw new ArgumentNullException(nameof(particle));
            PositionId = positionId;
            Displacement = displacement;
        }
    }
}