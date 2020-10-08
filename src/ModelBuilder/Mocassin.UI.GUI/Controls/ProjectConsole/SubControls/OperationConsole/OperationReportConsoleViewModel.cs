using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Mocassin.Framework.Operations;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.Collections;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectConsole.SubControls.OperationConsole.Commands;
using Mocassin.UI.GUI.Logic.Validation;
using Mocassin.UI.Data.Base;
using Mocassin.UI.Data.Main;
using Mocassin.UI.Data.ProjectLibrary;

namespace Mocassin.UI.GUI.Controls.ProjectConsole.SubControls.OperationConsole
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for <see cref="OperationReportConsoleView" /> that displays
    ///     and updates <see cref="IOperationReport" /> collections for automated model live checking
    /// </summary>
    public class OperationReportConsoleViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     Get a dummy <see cref="MocassinProject" /> that serves as a stop dummy for the live validation
        /// </summary>
        public static readonly MocassinProject DummyProject = new MocassinProject {ProjectName = "Empty"};

        private readonly object lockObject = new object();
        private bool isErrorContentFiltered;
        private bool isSoftUpdateStop;
        private MocassinProject selectedProject = DummyProject;
        private IOperationReport selectedReport;
        private ModelValidatorViewModel validatorViewModel;

        /// <summary>
        ///     Get or set the last received <see cref="IEnumerable{T}" /> of <see cref="IOperationReport" /> instances
        /// </summary>
        private IEnumerable<IOperationReport> LastReportSet { get; set; }

        /// <summary>
        ///     The <see cref="IDisposable" /> for an active report subscription
        /// </summary>
        private IDisposable LiveReportSubscription { get; set; }

        /// <summary>
        ///     The <see cref="ObservableCollectionViewModel{T}" /> for the set of <see cref="IOperationReport" />
        /// </summary>
        public ObservableCollectionViewModel<IOperationReport> OperationCollectionViewModel { get; }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> of selectable project graphs
        /// </summary>
        public IEnumerable<MocassinProject> ProjectGraphs => GetProjectGraphs();

        /// <summary>
        ///     Get or set a <see cref="ModelValidatorViewModel" /> instance that periodically checks its linked model definition
        /// </summary>
        public ModelValidatorViewModel ValidatorViewModel
        {
            get => validatorViewModel;
            private set => SetProperty(ref validatorViewModel, value);
        }

        /// <summary>
        ///     Get or set the currently selected <see cref="IOperationReport" />
        /// </summary>
        public IOperationReport SelectedReport
        {
            get => selectedReport;
            set => SetProperty(ref selectedReport, value);
        }

        /// <summary>
        ///     Get or set the selected <see cref="MocassinProject" /> that is live validated
        /// </summary>
        public MocassinProject SelectedProject
        {
            get => selectedProject;
            set => SetProperty(ref selectedProject, value);
        }

        /// <summary>
        ///     Get a set a boolean flag that controls if display requests will filter <see cref="IOperationReport" /> instances
        ///     that have nor relevant content
        /// </summary>
        public bool IsErrorContentFiltered
        {
            get => isErrorContentFiltered;
            set => SetProperty(ref isErrorContentFiltered, value);
        }

        /// <summary>
        ///     Get or set a boolean flag that defines if updating of received report lists is temporarily stopped
        /// </summary>
        public bool IsSoftUpdateStop
        {
            get => isSoftUpdateStop;
            set => SetProperty(ref isSoftUpdateStop, value);
        }

        /// <summary>
        ///     Get the <see cref="SetSoftUpdateStopCommand" /> to set/unset the affiliated flag
        /// </summary>
        public SetSoftUpdateStopCommand SoftUpdateStopCommand { get; }

        /// <inheritdoc />
        public OperationReportConsoleViewModel(IProjectAppControl projectControl)
            : base(projectControl)
        {
            OperationCollectionViewModel = new ObservableCollectionViewModel<IOperationReport>();
            SoftUpdateStopCommand = new SetSoftUpdateStopCommand(this);
            PropertyChanged += OnFilterChanged;
            PropertyChanged += OnProjectGraphChanged;
        }

        /// <summary>
        ///     Requests the displaying of the passed <see cref="IEnumerable{T}" /> sequence of <see cref="IOperationReport" />
        ///     with optional boolean flag to ignore the soft update stop
        /// </summary>
        /// <param name="reports"></param>
        /// <param name="ignoreSoftStop"></param>
        public void DisplayReports(IEnumerable<IOperationReport> reports, bool ignoreSoftStop = false)
        {
            lock (lockObject)
            {
                if (IsSoftUpdateStop && !ignoreSoftStop)
                {
                    AttachToPropertyChange(() => DisplayReports(reports, true),
                        (bool x) => !x,
                        nameof(IsSoftUpdateStop));
                    return;
                }

                LastReportSet = reports;
                OperationCollectionViewModel.Clear();
                OperationCollectionViewModel.AddItems(GetFilteredReports(LastReportSet));
            }
        }

        /// <summary>
        ///     Filters the passed <see cref="IEnumerable{T}" /> of <see cref="IOperationReport" /> instances using the internal
        ///     filter settings
        /// </summary>
        /// <param name="reports"></param>
        /// <returns></returns>
        public IEnumerable<IOperationReport> GetFilteredReports(IEnumerable<IOperationReport> reports)
        {
            return !IsErrorContentFiltered ? reports : reports?.Where(x => !x.IsGood);
        }

        /// <summary>
        ///     Subscribes the report console system to a <see cref="IObservable{T}" /> for <see cref="IOperationReport" />
        ///     sequences. Active subscription is disposed if it exists
        /// </summary>
        /// <param name="reportSource"></param>
        public void ChangeReportSubscription(IObservable<IEnumerable<IOperationReport>> reportSource)
        {
            lock (lockObject)
            {
                AsyncDisposeLiveValidation();
                DisplayReports(null);
                ValidatorViewModel = null;
                LiveReportSubscription = null;
                if (reportSource == null) return;

                LiveReportSubscription = reportSource.Subscribe(reports => DisplayReports(reports));
            }
        }

        /// <summary>
        ///     Starts a <see cref="Task" /> that awaits disposal of the current <see cref="ModelValidatorViewModel" /> and
        ///     affiliated
        ///     subscription
        /// </summary>
        private void AsyncDisposeLiveValidation()
        {
            var subscription = LiveReportSubscription;
            var validator = ValidatorViewModel;
            Task.Run(() =>
            {
                if (subscription == null && validator == null) return;
                subscription?.Dispose();
                validator.Dispose();
                PushInfoMessage($"Removed a change detector from the [{validator.ProjectModelData.Parent.ProjectName}] model tree.");
            });
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            AsyncDisposeLiveValidation();
            base.Dispose();
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> of selectable <see cref="MocassinProject" /> instances for live
        ///     validation
        /// </summary>
        /// <returns></returns>
        private IEnumerable<MocassinProject> GetProjectGraphs()
        {
            yield return DummyProject;
            if (ProjectControl.ProjectGraphs == null) yield break;
            foreach (var projectGraph in ProjectControl.ProjectGraphs) yield return projectGraph;
        }

        /// <summary>
        ///     Action that is called if the <see cref="IsErrorContentFiltered" /> property changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFilterChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsErrorContentFiltered)) DisplayReports(LastReportSet);
        }

        /// <summary>
        ///     Action that is called if the <see cref="SelectedProject" /> property changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnProjectGraphChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(SelectedProject)) return;
            await Task.Run(() => SwitchValidationTarget(SelectedProject?.ProjectModelData));
        }

        /// <summary>
        ///     Switches the <see cref="ModelValidatorViewModel" /> system to target the passed <see cref="ProjectModelData" /> or
        ///     stops the system if the argument is null
        /// </summary>
        /// <param name="targetProjectModelData"></param>
        public void SwitchValidationTarget(ProjectModelData targetProjectModelData)
        {
            if (targetProjectModelData == null || ReferenceEquals(targetProjectModelData, DummyProject.ProjectModelData))
            {
                ChangeReportSubscription(null);
                return;
            }

            var validator = new ModelValidatorViewModel(targetProjectModelData, ProjectControl);
            ChangeReportSubscription(validator.ReportSetChangedNotifications);
            ValidatorViewModel = validator;
            ValidatorViewModel.RunValidationCommand.Execute(null);
            PushInfoMessage($"Attached a change detector to the [{targetProjectModelData.Parent.ProjectName}] model tree.");
        }

        /// <inheritdoc />
        protected override void OnProjectLibraryChangedInternal(IMocassinProjectLibrary newProjectLibrary)
        {
            OnPropertyChanged(nameof(ProjectGraphs));
        }

        /// <inheritdoc />
        protected override void OnProjectContentChangedInternal()
        {
            OnPropertyChanged(nameof(ProjectGraphs));
        }
    }
}