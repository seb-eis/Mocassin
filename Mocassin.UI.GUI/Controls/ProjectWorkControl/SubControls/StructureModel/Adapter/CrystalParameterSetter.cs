using System;
using Mocassin.Symmetry.CrystalSystems;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.Xml.StructureModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.StructureModel.Adapter
{
    /// <summary>
    ///     Adapter class that wraps a <see cref="Symmetry.CrystalSystems.CrystalSystem"/> and <see cref="CellParametersGraph"/> into a
    ///     <see cref="ViewModelBase"/>
    /// </summary>
    public class CrystalParameterSetter : ViewModelBase
    {
        public CrystalSystem CrystalSystem { get; }

        public CellParametersGraph ParametersGraph { get; }

        public bool IsReadOnlyAlpha => CrystalSystem.Alpha.Fixed;
        public bool IsReadOnlyBeta => CrystalSystem.Beta.Fixed;
        public bool IsReadOnlyGamma => CrystalSystem.Gamma.Fixed;
        public bool IsReadOnlyParamA => CrystalSystem.ParamA.Fixed;
        public bool IsReadOnlyParamB => CrystalSystem.ParamB.Fixed;
        public bool IsReadOnlyParamC => CrystalSystem.ParamC.Fixed;

        public double ParamA
        {
            get => ParametersGraph.ParamA;
            set
            {
                ParametersGraph.ParamA = value;
                OnParameterChanged();
            }
        }

        public double ParamB
        {
            get => ParametersGraph.ParamB;
            set
            {
                ParametersGraph.ParamB = value;
                OnParameterChanged();
            }
        }

        public double ParamC
        {
            get => ParametersGraph.ParamC;
            set
            {
                ParametersGraph.ParamC = value;
                OnParameterChanged();
            }
        }

        public double Alpha
        {
            get => ParametersGraph.Alpha;
            set
            {
                ParametersGraph.Alpha = value;
                OnParameterChanged();
            }
        }

        public double Beta
        {
            get => ParametersGraph.Beta;
            set
            {
                ParametersGraph.Beta = value;
                OnParameterChanged();
            }
        }

        public double Gamma
        {
            get => ParametersGraph.Gamma;
            set
            {
                ParametersGraph.Gamma = value;
                OnParameterChanged();
            }
        }

        public bool IsRadian
        {
            get => ParametersGraph.IsRadian;
            set
            {
                ParametersGraph.IsRadian = value;
                OnParameterChanged();
            }
        }


        public CrystalParameterSetter(CrystalSystem crystalSystem, CellParametersGraph parametersGraph)
        {
            CrystalSystem = crystalSystem ?? throw new ArgumentNullException(nameof(crystalSystem));
            ParametersGraph = parametersGraph ?? throw new ArgumentNullException(nameof(parametersGraph));
        }

        private void CorrectParameterGraph()
        {
            ParametersGraph.PopulateFrom(GetCorrectedParameterSet(ParametersGraph.GetParameterSet()));
        }

        private CrystalParameterSet GetCorrectedParameterSet(CrystalParameterSet set)
        {
            CrystalSystem.ApplyParameterDependencies(set);
            return set;
        }

        private void OnParameterChanged()
        {
            CorrectParameterGraph();
            OnPropertyChanged(nameof(ParamA));
            OnPropertyChanged(nameof(ParamB));
            OnPropertyChanged(nameof(ParamC));
            OnPropertyChanged(nameof(Alpha));
            OnPropertyChanged(nameof(Beta));
            OnPropertyChanged(nameof(Gamma));
        }
    }
}