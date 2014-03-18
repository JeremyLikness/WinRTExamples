using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Store;
using Windows.UI.Core;
using Windows.UI.Popups;
using PackageAndDeployExample.Common;

namespace PackageAndDeployExample
{
    public class DeploymentViewModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly DeploymentHelper _trialHelper = new DeploymentHelper();
        private readonly CoreDispatcher _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

        private ListingInformation _listingInformation;
        private LicenseInformation _licenseInformation;

        private RelayCommand _resetToTrialCommand;
        private RelayCommand _resetToFullCommand;

        private RelayCommand _upgradeTrialCommand;

        private RelayCommand _purchaseInAppItemCommand;
        private ProductListing _selectedProductListing;

        #endregion

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="DeploymentViewModel"/> class.
        /// </summary>
        public DeploymentViewModel()
        {
            _trialHelper.ListingChanged += HandleTrialHelperListingChanged;
            _trialHelper.LicenseChanged += HandleTrialHelperLicenseChanged;
        }

        #endregion

        #region Properties

        public String AppId
        {
            get { return _trialHelper.AppId == Guid.Empty ? String.Empty : _trialHelper.AppId.ToString(); }
        }

        public String AppLink
        {
            get { return _trialHelper.AppId == Guid.Empty ? String.Empty : _trialHelper.LinkUri.ToString(); }
        }

        public ListingInformation ListingInformation
        {
            get { return _listingInformation; }
            private set
            {
                if (!Equals(_listingInformation, value))
                {
                    _listingInformation = value;

                    if (_listingInformation != null)
                    {
                        foreach (var foo in _listingInformation.ProductListings)
                        {
                            Debug.WriteLine(foo);
                        }
                    }

                    OnPropertyChanged();
                    OnPropertyChanged("AppId");
                    OnPropertyChanged("AppLink");
                }
            }
        }

        /// <summary>
        /// Gets or sets the current license information.
        /// </summary>
        /// <value>
        /// The license information.
        /// </value>
        public LicenseInformation LicenseInformation
        {
            get { return _licenseInformation; }
            private set
            {
                _licenseInformation = value;
                OnPropertyChanged();
            }
        }

        public ProductListing SelectedProductListing
        {
            get { return _selectedProductListing; }
            set
            {
                if (!Equals(_selectedProductListing, value))
                {
                    _selectedProductListing = value;
                    OnPropertyChanged();
                    PurchaseInAppItemCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

        #region Commands & Implementations

        public RelayCommand ResetToTrialCommand
        {
            get { return _resetToTrialCommand ?? (_resetToTrialCommand = new RelayCommand(ResetToTrial)); }
        }

        public RelayCommand ResetToFullCommand
        {
            get { return _resetToFullCommand ?? (_resetToFullCommand = new RelayCommand(ResetToFull)); }
        }

        public RelayCommand UpgradeTrialCommand
        {
            get { return _upgradeTrialCommand ?? (_upgradeTrialCommand = new RelayCommand(UpgradeTrial)); }
        }

        public RelayCommand PurchaseInAppItemCommand
        {
            get
            {
                return _purchaseInAppItemCommand ?? (_purchaseInAppItemCommand = new RelayCommand(PurchaseInAppItem, CanPurchaseInAppItem));
            }
        }

        private async void ResetToTrial()
        {
            try
            {
                await _trialHelper.SetAppTrial();
            }
            catch (InvalidOperationException e)
            {
                var errorDialog = new MessageDialog(e.Message, "Error configuring trial mode.");
                errorDialog.ShowAsync();
            }
        }

        private async void ResetToFull()
        {
            try
            {
                await _trialHelper.SetAppPurchased();
            }
            catch (InvalidOperationException e)
            {
                var errorDialog = new MessageDialog(e.Message, "Error configuring full mode.");
                errorDialog.ShowAsync();
            }
        }

        private async void UpgradeTrial()
        {
            try
            {
                await _trialHelper.TriggerTrialUpgrade();
                var successDialog = new MessageDialog("Successfully updgraded to full version from trial", "Trial Upgrade Success");
                successDialog.ShowAsync();
            }
            catch (InvalidOperationException e)
            {
                var errorDialog = new MessageDialog(e.Message, "Trial Upgrade Error");
                errorDialog.ShowAsync();
            }
        }

        private async void PurchaseInAppItem()
        {
            try
            {
                if (SelectedProductListing == null) return;
                var result = await _trialHelper.PurchaseItem(SelectedProductListing);
                var successDialog = new MessageDialog("In-app purchase completed - " + result.Status, "In-App Purchase");
                successDialog.ShowAsync();
            }
            catch (InvalidOperationException e)
            {
                var errorDialog = new MessageDialog(e.Message, "In App Purchase Error");
                errorDialog.ShowAsync();
            }
        }

        private Boolean CanPurchaseInAppItem()
        {
            return SelectedProductListing != null;
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        } 

        #endregion

        #region Helper methods

        private async void HandleTrialHelperListingChanged(Object sender, EventArgs e)
        {
            await _dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () => ListingInformation = _trialHelper.LatestListingInformation);
        }

        private async void HandleTrialHelperLicenseChanged(Object sender, EventArgs e)
        {
            await _dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () => LicenseInformation = _trialHelper.LatestLicenseInformation);
        }

        #endregion
    }
}