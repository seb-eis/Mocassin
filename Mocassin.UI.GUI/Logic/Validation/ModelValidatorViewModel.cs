using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Events;
using Mocassin.Framework.Operations;
using Mocassin.Model.ModelProject;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Customization;
using Mocassin.UI.Xml.Helper;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.Model;

namespace Mocassin.UI.GUI.Logic.Validation
{
    /// <summary>
    ///     Flags for model validation status information
    /// </summary>
    [Flags]
    public enum ModelValidationStatus
    {
        /// <summary>
        ///     Signals no errors
        /// </summary>
        NoErrors = 0,

        /// <summary>
        ///     Signals a fatal exception
        /// </summary>
        CaughtException = 1,

        /// <summary>
        ///     Signals that the model is not ready for validation
        /// </summary>
        ModelNotReady = 1 << 1,

        /// <summary>
        ///     Signals that the model was rejected
        /// </summary>
        ModelRejected = 1 << 2,

        /// <summary>
        ///     Signals that the customization template could not be created
        /// </summary>
        CustomizationTemplateBuildFailure = 1 << 3
    }

    /// <summary>
    ///     A <see cref="PrimaryControlViewModel" /> live validator for <see cref="ProjectModelData" /> instances that
    ///     provides affiliated
    ///     update events and supports continuous validation cycles
    /// </summary>
    public class ModelValidatorViewModel : PrimaryControlViewModel
    {
        private readonly object lockObject = new object();
        private bool isIgnoreContentChange;
        private bool isValidating;

        /// <summary>
        ///     Get the <see cref="IModelProject" /> interface that provides the validation functionality
        /// </summary>
        private IModelProject ModelProject { get; }

        /// <summary>
        ///     Get or set the <see cref="ObjectGraphChangeObserver" /> for the observed model
        /// </summary>
        private ObjectGraphChangeObserver ModelGraphChangeObserver { get; }

        /// <summary>
        ///     Get or set a boolean value if the validator is disposed
        /// </summary>
        private bool IsDisposed { get; set; }

        /// <summary>
        ///     Get or set a boolean flag informing if a validation is in progress
        /// </summary>
        private bool IsValidating
        {
            get
            {
                lock (lockObject)
                {
                    return isValidating;
                }
            }
            set
            {
                lock (lockObject)
                {
                    isValidating = value;
                }
            }
        }

        /// <summary>
        ///     The <see cref="ReactiveEvent{TSubject}" /> for changes in the <see cref="IOperationReport" /> set
        /// </summary>
        private ReactiveEvent<IEnumerable<IOperationReport>> ReportSetChangedEvent { get; }

        /// <summary>
        ///     The <see cref="ReactiveEvent{TSubject}" /> for changes in
        /// </summary>
        private ReactiveEvent<ModelValidationStatus> ModelValidationStatusChangedEvent { get; }

        /// <summary>
        ///     Get the <see cref="ProjectModelData" /> that the validator is using
        /// </summary>
        public ProjectModelData ProjectModelData { get; }

        /// <summary>
        ///     Gee the <see cref="IObservable{T}" /> for changes in the set of <see cref="IOperationReport" /> instances
        /// </summary>
        public IObservable<IEnumerable<IOperationReport>> ReportSetChangedNotifications => ReportSetChangedEvent.AsObservable();

        /// <summary>
        ///     Get the <see cref="IObservable{T}" /> for notifications about changes in the <see cref="ModelValidationStatus" />
        /// </summary>
        public IObservable<ModelValidationStatus> StatusChangedNotifications => ModelValidationStatusChangedEvent.AsObservable();

        /// <summary>
        ///     Get the <see cref="AsyncRunValidationCommand" /> to start a single validation cycle
        /// </summary>
        public AsyncRunValidationCommand RunValidationCommand { get; }

        /// <summary>
        ///     Get or set a boolean flag if the validation system ignores received content change notifications
        /// </summary>
        public bool IsIgnoreContentChange
        {
            get => isIgnoreContentChange;
            set => SetProperty(ref isIgnoreContentChange, value);
        }

        /// <summary>
        ///     Creates new <see cref="ModelValidatorViewModel" /> for the passed <see cref="ProjectModelData" /> and
        ///     <see cref="IMocassinProjectControl" />
        /// </summary>
        /// <param name="projectModelData"></param>
        /// <param name="projectControl"></param>
        public ModelValidatorViewModel(ProjectModelData projectModelData, IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            ProjectModelData = projectModelData ?? throw new ArgumentNullException(nameof(projectModelData));
            ReportSetChangedEvent = new ReactiveEvent<IEnumerable<IOperationReport>>();
            ModelValidationStatusChangedEvent = new ReactiveEvent<ModelValidationStatus>();
            RunValidationCommand = new AsyncRunValidationCommand(this);
            ModelProject = ProjectControl.CreateModelProject();
            ModelGraphChangeObserver = new ObjectGraphChangeObserver(new[] {typeof(MocassinProject)});
            ModelGraphChangeObserver.SetObservationRoot(projectModelData);
            ModelGraphChangeObserver.ChangeEventNotifications.Subscribe(OnModelChanged);
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            DisposeInternal();
            base.Dispose();
        }

        /// <summary>
        ///     Internal <see cref="Dispose" /> implementation
        /// </summary>
        private void DisposeInternal()
        {
            lock (lockObject)
            {
                ModelGraphChangeObserver.Dispose();
                ReportSetChangedEvent.OnCompleted();
                ModelValidationStatusChangedEvent.OnCompleted();
                IsDisposed = true;
            }
        }

        /// <summary>
        ///     Performs a single validation cycle run on the internally set <see cref="ProjectModelData" />
        /// </summary>
        public void RunValidation()
        {
            if (IsValidating || ModelProject == null) return;

            IsValidating = true;
            ModelProject.ResetProject();

            if (!TryPrepareModelInput(out var inputObjects))
            {
                ModelValidationStatusChangedEvent.OnNext(ModelValidationStatus.ModelNotReady);
                IsValidating = false;
                return;
            }

            if (!TryPushModelInput(ModelProject, inputObjects, out var reports))
            {
                ModelValidationStatusChangedEvent.OnNext(ModelValidationStatus.CaughtException);
                IsValidating = false;
                return;
            }

            ReportSetChangedEvent.OnNext(reports);
            if (reports.Any(x => !x.IsGood))
            {
                ModelValidationStatusChangedEvent.OnNext(ModelValidationStatus.ModelRejected);
                IsValidating = false;
                return;
            }

            if (!TryGenerateModelCustomization(ModelProject, out _))
            {
                ModelValidationStatusChangedEvent.OnNext(ModelValidationStatus.CustomizationTemplateBuildFailure);
                IsValidating = false;
                return;
            }

            ModelValidationStatusChangedEvent.OnNext(ModelValidationStatus.NoErrors);
            IsValidating = false;
        }

        /// <summary>
        ///     Tries to create a new <see cref="ProjectCustomizationTemplate" /> for the current <see cref="ProjectModelData" />
        /// </summary>
        /// <param name="customization"></param>
        /// <returns></returns>
        public ModelValidationStatus TryCreateCustomization(out ProjectCustomizationTemplate customization)
        {
            ModelProject.ResetProject();
            customization = null;
            if (!TryPrepareModelInput(out var inputObjects)) return ModelValidationStatus.ModelNotReady;

            if (!TryPushModelInput(ModelProject, inputObjects, out var reports)) return ModelValidationStatus.CaughtException;

            if (reports.Any(x => !x.IsGood)) return ModelValidationStatus.ModelRejected;

            return !TryGenerateModelCustomization(ModelProject, out customization)
                ? ModelValidationStatus.CustomizationTemplateBuildFailure
                : ModelValidationStatus.NoErrors;
        }

        /// <summary>
        ///     Tries to prepare a <see cref="IList{T}" /> of model input <see cref="object" /> instances from the internally set
        ///     <see cref="ProjectModelData" />. Returns false if the data is currently not convertible
        /// </summary>
        /// <param name="inputObjects"></param>
        /// <returns></returns>
        private bool TryPrepareModelInput(out IList<object> inputObjects)
        {
            try
            {
                inputObjects = ProjectModelData.GetInputSequence().ToList();
                return inputObjects.All(x => x != null);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                inputObjects = null;
                return false;
            }
        }

        /// <summary>
        ///     Tries to push the passed <see cref="IList{T}" /> of model <see cref="object" /> instances to the passed
        ///     <see cref="IModelProject" />. Returns false if the push is aborted for any reason
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="inputObjects"></param>
        /// <param name="reports"></param>
        /// <returns></returns>
        private bool TryPushModelInput(IModelProject modelProject, IList<object> inputObjects, out IList<IOperationReport> reports)
        {
            try
            {
                reports = modelProject.InputPipeline.PushToProject(inputObjects);
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                reports = null;
                return false;
            }
        }

        /// <summary>
        ///     Tries to generate a <see cref="ProjectCustomizationTemplate" /> from the provided <see cref="IModelProject" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="customization"></param>
        /// <returns></returns>
        private bool TryGenerateModelCustomization(IModelProject modelProject, out ProjectCustomizationTemplate customization)
        {
            try
            {
                customization = ProjectCustomizationTemplate.Create(modelProject, ProjectModelData);
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                customization = null;
                return false;
            }
        }

        /// <summary>
        ///     Action that is invoked if a model change is detected
        /// </summary>
        /// <param name="eventItems"></param>
        protected async void OnModelChanged((object Sender, EventArgs Args) eventItems)
        {
            if (IsIgnoreContentChange || IsDisposed || RunValidationCommand == null) return;
            await RunValidationCommand.ExecuteAsync(null);
        }
    }
}