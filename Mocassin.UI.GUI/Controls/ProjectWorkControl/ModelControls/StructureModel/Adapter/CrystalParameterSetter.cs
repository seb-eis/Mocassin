using System;
using Mocassin.Symmetry.CrystalSystems;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.Xml.StructureModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.StructureModel.Adapter
{
    /// <summary>
    ///     Adapter class that wraps a <see cref="Symmetry.CrystalSystems.CrystalSystem" /> and
    ///     <see cref="CellParametersData" /> into a
    ///     <see cref="ViewModelBase" />
    /// </summary>
    public class CrystalParameterSetter : ViewModelBase
    {
        /// <summary>
        ///     Get the <see cref="Symmetry.CrystalSystems.CrystalSystem" /> that provides the parameter constraints
        /// </summary>
        public CrystalSystem CrystalSystem { get; }

        /// <summary>
        ///     Get the <see cref="CellParametersData" /> that is used as a value target
        /// </summary>
        public CellParametersData ParametersData { get; }

        /// <summary>
        ///     Get a boolean flag if <see cref="Alpha" /> is read only
        /// </summary>
        public bool IsReadOnlyAlpha => CrystalSystem.Alpha.IsContextImmutable;

        /// <summary>
        ///     Get a boolean flag if <see cref="Beta" /> is read only
        /// </summary>
        public bool IsReadOnlyBeta => CrystalSystem.Beta.IsContextImmutable;

        /// <summary>
        ///     Get a boolean flag if <see cref="Gamma" /> is read only
        /// </summary>
        public bool IsReadOnlyGamma => CrystalSystem.Gamma.IsContextImmutable;

        /// <summary>
        ///     Get a boolean flag if <see cref="ParamA" /> is read only
        /// </summary>
        public bool IsReadOnlyParamA => CrystalSystem.ParamA.IsContextImmutable;

        /// <summary>
        ///     Get a boolean flag if <see cref="ParamB" /> is read only
        /// </summary>
        public bool IsReadOnlyParamB => CrystalSystem.ParamB.IsContextImmutable;

        /// <summary>
        ///     Get a boolean flag if <see cref="ParamC" /> is read only
        /// </summary>
        public bool IsReadOnlyParamC => CrystalSystem.ParamC.IsContextImmutable;


        /// <summary>
        ///     Get or set the parameter A in [Ang]
        /// </summary>
        public double ParamA
        {
            get => ParametersData.ParamA;
            set
            {
                ParametersData.ParamA = value;
                OnParameterChanged();
            }
        }

        /// <summary>
        ///     Get or set the parameter B in [Ang]
        /// </summary>
        public double ParamB
        {
            get => ParametersData.ParamB;
            set
            {
                ParametersData.ParamB = value;
                OnParameterChanged();
            }
        }

        /// <summary>
        ///     Get or set the parameter C in [Ang]
        /// </summary>
        public double ParamC
        {
            get => ParametersData.ParamC;
            set
            {
                ParametersData.ParamC = value;
                OnParameterChanged();
            }
        }

        /// <summary>
        ///     Get or set the angle alpha
        /// </summary>
        public double Alpha
        {
            get => ParametersData.Alpha;
            set
            {
                ParametersData.Alpha = value;
                OnParameterChanged();
            }
        }

        /// <summary>
        ///     Get or set the angle beta
        /// </summary>
        public double Beta
        {
            get => ParametersData.Beta;
            set
            {
                ParametersData.Beta = value;
                OnParameterChanged();
            }
        }

        /// <summary>
        ///     Get or set the angle gamma
        /// </summary>
        public double Gamma
        {
            get => ParametersData.Gamma;
            set
            {
                ParametersData.Gamma = value;
                OnParameterChanged();
            }
        }

        /// <summary>
        ///     Get or set a boolean flag if the system should use radian angles
        /// </summary>
        public bool IsRadian
        {
            get => ParametersData.IsRadian;
            set
            {
                ParametersData.IsRadian = value;
                OnParameterChanged();
            }
        }

        /// <summary>
        ///     Create new <see cref="CrystalParameterSetter" /> from <see cref="Mocassin.Symmetry.CrystalSystems.CrystalSystem" />
        ///     and <see cref="CellParametersData" />
        /// </summary>
        /// <param name="crystalSystem"></param>
        /// <param name="parametersData"></param>
        public CrystalParameterSetter(CrystalSystem crystalSystem, CellParametersData parametersData)
        {
            CrystalSystem = crystalSystem ?? throw new ArgumentNullException(nameof(crystalSystem));
            ParametersData = parametersData ?? throw new ArgumentNullException(nameof(parametersData));
        }

        /// <summary>
        ///     Corrects the current <see cref="CellParametersData" /> using the internal constraints
        /// </summary>
        private void CorrectParameterData()
        {
            var correctedSet = GetCorrectedParameterSet(ParametersData.GetParameterSet());
            ParametersData.PopulateFrom(correctedSet);
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
            CorrectParameterData();
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