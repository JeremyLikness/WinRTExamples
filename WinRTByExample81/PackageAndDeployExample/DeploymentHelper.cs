using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.Storage;

// When in debug mode, swap out the definition of CurrentApp to instead use the CurrentAppSimualtor
#if DEBUG
using CurrentApp = Windows.ApplicationModel.Store.CurrentAppSimulator;
#endif

namespace PackageAndDeployExample
{
    public class DeploymentHelper
    {
        #region Fields

        private ListingInformation _latestListingInformation;
        private LicenseInformation _latestLicenseInformation; 

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current Windows Store AppId value for the current app.
        /// </summary>
        /// <value>
        /// The application identifier.
        /// </value>
        public Guid AppId
        {
            get { return CurrentApp.AppId; }
        }

        /// <summary>
        /// Gets the current Windows Store link URI for the current app.
        /// </summary>
        /// <value>
        /// The link URI.
        /// </value>
        public Uri LinkUri
        {
            get { return CurrentApp.LinkUri; }
        }

        /// <summary>
        /// Gets the latest Windows Store listing information for the current app.
        /// </summary>
        /// <value>
        /// The latest listing information.
        /// </value>
        public ListingInformation LatestListingInformation
        {
            get { return _latestListingInformation; }
            private set
            {
                if (!Equals(_latestListingInformation, value))
                {
                    _latestListingInformation = value;
                    OnListingChanged();

                    // Update the license since the listing has been overhauled
                    LatestLicenseInformation = CurrentApp.LicenseInformation;
                }
            }
        }

        /// <summary>
        /// Gets the latest license information for the current app.
        /// </summary>
        /// <value>
        /// The latest license information.
        /// </value>
        public LicenseInformation LatestLicenseInformation
        {
            get { return _latestLicenseInformation; }
            private set
            {
                if (_latestLicenseInformation != null)
                {
                    _latestLicenseInformation.LicenseChanged -= OnLicenseChanged;
                }
                _latestLicenseInformation = value;
                if (_latestLicenseInformation != null)
                {
                    _latestLicenseInformation.LicenseChanged += OnLicenseChanged;
                }
                OnLicenseChanged();
            }
        } 

        #endregion

        #region Methods

        /// <summary>
        /// Resets the application's simulated license to a "purchased" state.
        /// </summary>
        public async Task SetAppPurchased()
        {
            // Load the file that simulates the app as if it has been purchased
            await LoadStoreProxyFile("WindowsStoreProxy_Purchased.xml");
        }

        /// <summary>
        /// Resets the application's simulated license to a "trial" state.
        /// </summary>
        public async Task SetAppTrial()
        {
            // Load the file that simulates the app as if it is in trial mode
            await LoadStoreProxyFile("WindowsStoreProxy_Trial.xml");
        }

        /// <summary>
        /// Triggers an attempt to upgrade the app from trial mode to full mode.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">
        /// Trial mode conversion failed with a COM exception.
        /// or
        /// Trial mode conversion failed with an argument exception.
        /// or
        /// Trial mode conversion failed with an out of memory exception.
        /// </exception>
        public async Task<String> TriggerTrialUpgrade()
        {
            // Request a purchase upgrade from the Trial Mode to the Full Feature Mode
            try
            {
                var receipt = await CurrentApp.RequestAppPurchaseAsync(true);
                return receipt;
            }
            catch (System.Runtime.InteropServices.COMException comException)
            {
                // Happens with E_FAIL
                throw new InvalidOperationException("Trial mode conversion failed with a COM exception.", comException);
            }
            catch (ArgumentException argumentException)
            {
                // Happens with E_INVALIDARG result
                throw new InvalidOperationException("Trial mode conversion failed with an argument exception.", argumentException);
            }
            catch (OutOfMemoryException outOfMemoryException)
            {
                // Happens with E_OUTOFMEMORY result
                throw new InvalidOperationException("Trial mode conversion failed with an out of memory exception.", outOfMemoryException);
            }
        }

        /// <summary>
        /// Attempts to execute an in-app purchase of the indicated item.
        /// </summary>
        /// <param name="selectedProductListing">The selected product listing.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">selectedProductListing</exception>
        public async Task<PurchaseResults> PurchaseItem(ProductListing selectedProductListing)
        {
            if (selectedProductListing == null) throw new ArgumentNullException("selectedProductListing");

            try
            {
                var result = await CurrentApp.RequestProductPurchaseAsync(selectedProductListing.ProductId);
                return result;

            }
            catch (System.Runtime.InteropServices.COMException comException)
            {
                // Happens with E_FAIL
                throw new InvalidOperationException("In-app purchase failed with a COM exception.", comException);
            }
            catch (ArgumentException argumentException)
            {
                // Happens with E_INVALIDARG result
                throw new InvalidOperationException("In-app purchase  failed with an argument exception.", argumentException);
            }
            catch (OutOfMemoryException outOfMemoryException)
            {
                // Happens with E_OUTOFMEMORY result
                throw new InvalidOperationException("In-app purchase  failed with an out of memory exception.", outOfMemoryException);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the latest app listing has been changed.
        /// </summary>
        public event EventHandler ListingChanged;

        private void OnListingChanged()
        {
            var handler = ListingChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the latest app license has been changed.
        /// </summary>
        public event EventHandler LicenseChanged;

        private void OnLicenseChanged()
        {
            var handler = LicenseChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        } 

        #endregion

        #region Helper methods

        private async Task LoadStoreProxyFile(String proxyProfileFileToLoad)
        {
// Only use the ReloadSimulatorAsync method when in debug mode
#if DEBUG
            // Load the proxy XML file to use
            var proxyFilePath = "ms-appx:///WindowsStoreProxy/" + proxyProfileFileToLoad;
            var baselineStoreProxyFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(proxyFilePath));
            // Update the simulator to take its values from those defined in the XML file
            await CurrentApp.ReloadSimulatorAsync(baselineStoreProxyFile);
#endif
            try
            {
                LatestListingInformation = await CurrentApp.LoadListingInformationAsync();
            }
            catch (System.Runtime.InteropServices.COMException comException)
            {
                // Happens with E_FAIL
                throw new InvalidOperationException("App listing request failed with a COM exception.", comException);
            }
            catch (ArgumentException argumentException)
            {
                // Happens with E_INVALIDARG result
                throw new InvalidOperationException("App listing request failed with an argument exception.", argumentException);
            }
            catch (OutOfMemoryException outOfMemoryException)
            {
                // Happens with E_OUTOFMEMORY result
                throw new InvalidOperationException("App listing request failed with an out of memory exception.", outOfMemoryException);
            }
        }

        #endregion
    }
}