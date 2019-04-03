using System;
using Mocassin.Symmetry.CrystalSystems;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.Xml.StructureModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.StructureModel.Adapter
{
    /// <summary>
    ///     Adapter class that wraps a <see cref="Symmetry.CrystalSystems.CrystalSystem" /> and
    ///     <see cref="CellParametersGraph" /> into a
    ///     <see cref="ViewModelBase" />
    /// </summary>
    public class CrystalParameterSetter : ViewModelBase
    {
        /// <summary>
        ///     Get the <see cref="Symmetry.CrystalSystems.CrystalSystem" /> that provides the parameter constraints
        /// </summary>
        public CrystalSystem CrystalSystem { get; }

        /// <summary>
        ///     Get the <see cref="CellParametersGraph" /> that is used as a value target
        /// </summary>
        public CellParametersGraph ParametersGraph { get; }

        /// <summary>
        ///     Get a boolean flag if <see cref="Alpha" /> is read only
        /// </summary>
        public bool IsReadOnlyAlpha => CrystalSystem.Alpha.Fixed;

        /// <summary>
        ///     Get a boolean flag if <see cref="Beta" /> is read only
        /// </summary>
        public bool IsReadOnlyBeta => CrystalSystem.Beta.Fixed;

        /// <summary>
        ///     Get a boolean flag if <see cref="Gamma" /> is read only
        /// </summary>
        public bool IsReadOnlyGamma => CrystalSystem.Gamma.Fixed;

        /// <summary>
        ///     Get a boolean flag if <see cref="ParamA" /> is read only
        /// </summary>
        public bool IsReadOnlyParamA => CrystalSystem.ParamA.Fixed;

        /// <summary>
        ///     Get a boolean flag if <see cref="ParamB" /> is read only
        /// </summary>
        public bool IsReadOnlyParamB => CrystalSystem.ParamB.Fixed;

        /// <summary>
        ///     Get a boolean flag if <see cref="ParamC" /> is read only
        /// </summary>
        public bool IsReadOnlyParamC => CrystalSystem.ParamC.Fixed;


        /// <summary>
        ///     Get or set the parameter A in [Ang]
        /// </summary>
        public double ParamA
        {
            get => ParametersGraph.ParamA;
            set
            {
                ParametersGraph.ParamA = value;
                OnParameterChanged();
            }
        }

        /// <summary>
        ///     Get or set the parameter B in [Ang]
        /// </summary>
        public double ParamB
        {
            get => ParametersGraph.ParamB;
            set
            {
                ParametersGraph.ParamB = value;
                OnParameterChanged();
            }
        }

        /// <summary>
        ///     Get or set the parameter C in [Ang]
        /// </summary>
        public double ParamC
        {
            get => ParametersGraph.ParamC;
            set
            {
                ParametersGraph.ParamC = value;
                OnParameterChanged();
            }
        }

        /// <summary>
        ///     Get or set the angle alpha
        /// </summary>
        public double Alpha
        {
            get => ParametersGraph.Alpha;
            set
            {
                ParametersGraph.Alpha = value;
                OnParameterChanged();
            }
        }

        /// <summary>
        ///     Get or set the angle beta
        /// </summary>
        public double Beta
        {
            get => ParametersGraph.Beta;
            set
            {
                ParametersGraph.Beta = value;
                OnParameterChanged();
            }
        }

        /// <summary>
        ///     Get or set the angle gamma
        /// </summary>
        public double Gamma
        {
            get => ParametersGraph.Gamma;
            set
            {
                ParametersGraph.Gamma = value;
                OnParameterChanged();
            }
        }

        /// <summary>
        ///     Get or set a boolean flag if the system should use radian angles
        /// </summary>
        public bool IsRadian
        {
            get => ParametersGraph.IsRadian;
            set
            {
                ParametersGraph.IsRadian = value;
                OnParameterChanged();
            }
        }

        /// <summary>
        ///     Create new <see cref="CrystalParameterSetter" /> from <see cref="Mocassin.Symmetry.CrystalSystems.CrystalSystem" />
        ///     and <see cref="CellParametersGraph" />
        /// </summary>
        /// <param name="crystalSystem"></param>
        /// <param name="parametersGraph"></param>
        public CrystalParameterSetter(CrystalSystem crystalSystem, CellParametersGraph parametersGraph)
        {
            CrystalSystem = crystalSystem ?? throw new ArgumentNullException(nameof(crystalSystem));
            ParametersGraph = parametersGraph ?? throw new ArgumentNullException(nameof(parametersGraph));
        }

        /// <summary>
        ///     Corrects the current <see cref="CellParametersGraph" /> using the internal constraints
        /// </summary>
        private void CorrectParameterGraph()
        {
            var correctedSet = GetCorrectedParameterSet(ParametersGraph.GetParameterSet());
            ParametersGraph.PopulateFrom(correctedSet);
        }

        /// <summary>
        ///     Corrects the passed <see cref="CrystalParameterSet" /> to the internal constraints and dependencies
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public CrystalParameterSet GetCorrectedParameterSet(CrystalParameterSet set)
        {
            CrystalSystem.ApplyParameterDependencies(set);
            return set;
        }

        /// <summary>
        ///     Action to call when any parameter changes
        /// </summary>
        private void OnParameterChanged()
        {
            CorrectParameterGraph();
            OnPropertyChanged(nameof(ParamA));
            OnPropertyChanged(nameof(ParamB));
            OnPropertyChanged(nameof(ParamC));
            OnPropertyChanged(nameof(Alpha));
            OnPropertyChanged(nameof(Beta));
            OnPropertyChanged(nameof(Gamma));
            OnPropertyChanged(nameof(IsRadian));
        }
    }
}