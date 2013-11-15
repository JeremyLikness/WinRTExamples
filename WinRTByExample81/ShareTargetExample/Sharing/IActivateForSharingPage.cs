using Windows.ApplicationModel.Activation;

namespace ShareTargetExample
{
    public interface IActivateForSharingPage
    {
        /// <summary>
        /// Invoked when another application wants to share content through this application.
        /// </summary>
        /// <param name="e">Activation data used to coordinate the process with Windows.</param>
        void Activate(ShareTargetActivatedEventArgs e);
    }
}