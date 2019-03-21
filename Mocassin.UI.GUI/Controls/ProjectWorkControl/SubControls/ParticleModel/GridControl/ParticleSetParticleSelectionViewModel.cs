﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Mocassin.Model.Particles;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.Base.GridControl;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ParticleModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.ParticleModel.GridControl
{
    /// <summary>
    ///     Data control <see cref="HostGraphModelObjectSelectionViewModel{TModelObject,TObjectGraph}" /> for
    ///     <see cref="ParticleSetGraph" /> hosting <see cref="Particle" /> references
    /// </summary>
    public sealed class ParticleSetParticleSelectionViewModel 
        : HostGraphModelObjectSelectionViewModel<Particle, ParticleSetGraph>
    {
        /// <inheritdoc />
        public ParticleSetParticleSelectionViewModel(ParticleSetGraph hostObject)
            : base(hostObject,true)
        {
            DataCollection = GetTargetCollection(hostObject);
            HandleDropAddCommand = GetDropAddObjectCommand<ParticleGraph>();
        }

        /// <inheritdoc />
        protected override IReadOnlyCollection<ModelObjectGraph> GetSourceCollection(MocassinProjectGraph projectGraph)
        {
            return projectGraph?.ProjectModelGraph?.ParticleModelGraph?.Particles;
        }

        /// <inheritdoc />
        protected override ICollection<ModelObjectReferenceGraph<Particle>> GetTargetCollection(ParticleSetGraph sourceObject)
        {
            return sourceObject?.Particles;
        }
    }
}