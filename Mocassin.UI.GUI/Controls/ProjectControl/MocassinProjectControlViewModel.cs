using Mocassin.UI.Base.ViewModel;

namespace Mocassin.UI.GUI.Controls.ProjectControl
{
    /// <summary>
    /// View model for the control of a full Mocassin project and its components
    /// </summary>
    public class MocassinProjectControlViewModel : ViewModelBase
    {
        private ModelProjectControlViewModel _modelProjectControlViewModel;

        private ModelContextControlViewModel _modelContextControlViewModel;

        private JobDbCreatorControlViewModel _jobDbCreatorControlViewModel;
    }
}