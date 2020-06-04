using Mocassin.UI.GUI.Base.ViewModels;
using System;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.TransitionModel.GridControl
{
    /// <summary>
    ///     A <see cref="ViewModelBase"/> for a transition mechanism that can be translated into a connection pattern for the model processing system
    /// </summary>
    public class TransitionMechanismViewModel : ViewModelBase
    {
        private string name;
        private string description;
        private string connectionPattern;
        private string positionChainDescription;
        private bool hasAssociationFlagSupport;

        /// <summary>
        ///     Get or set a name for the mechanism
        /// </summary>
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        /// <summary>
        ///     Get or set a description
        /// </summary>
        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        /// <summary>
        ///     Get or set the connection pattern <see cref="string"/>
        /// </summary>
        public string ConnectionPattern
        {
            get => connectionPattern;
            set => SetProperty(ref connectionPattern, value);
        }

        /// <summary>
        ///     Get or set a description for the position chain
        /// </summary>
        public string PositionChainDescription
        {
            get => positionChainDescription;
            set => SetProperty(ref positionChainDescription, value);
        }

        /// <summary>
        ///     Get a set a boolean flag if the mechanism supports the association/dissociation treatment flag
        /// </summary>
        public bool HasAssociationFlagSupport
        {
            get => hasAssociationFlagSupport;
            set => SetProperty(ref hasAssociationFlagSupport, value);
        }
    }
}