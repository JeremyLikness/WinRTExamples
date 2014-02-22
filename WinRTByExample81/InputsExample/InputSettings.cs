using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Devices.Input;
using InputsExample.Annotations;

namespace InputsExample
{
    public class InputSettings : INotifyPropertyChanged
    {
        #region Fields

        private Boolean _isAnimationOn = true;
        private Boolean _pointerIntegratedDevicesOnly;
        private Boolean _isMouseAvailable = true;
        private Boolean _isTouchAvailable = true;
        private Boolean _isPenAvailable = true;
        private Boolean _pointerSupportMouse = true;
        private Boolean _pointerSupportTouch = true;
        private Boolean _pointerSupportPen = true;
        private Boolean _manipulationTransformInertia = true;
        private Boolean _manipulationTransform = true;
        private Boolean _manipulationRotation = true;
        private Boolean _manipulationRotationInertia = true;
        private Boolean _manipulationScaling = true;
        private Boolean _manipulationScalingInertia = true;
        private Boolean _tapEnabled = true;
        private Boolean _rightTapEnabled = true;
        private Boolean _doubleTapEnabled = true;
        private Boolean _holdingEnabled = true;

        #endregion

        #region Constructor(s) and Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="InputSettings"/> class.
        /// </summary>
        public InputSettings()
        {
            UpdatePointerCapabilities();
            _pointerSupportMouse = IsMouseAvailable;
            _pointerSupportTouch = IsTouchAvailable;
            _pointerSupportPen = IsPenAvailable;
        } 

        #endregion

        public Boolean IsAnimationOn
        {
            get { return _isAnimationOn; }
            set
            {
                if (value.Equals(_isAnimationOn)) return;
                _isAnimationOn = value;
                OnPropertyChanged();
            }
        }

        #region Pointers

        public Boolean PointerIntegratedDevicesOnly
        {
            get { return _pointerIntegratedDevicesOnly; }
            set
            {
                if (value.Equals(_pointerIntegratedDevicesOnly)) return;
                _pointerIntegratedDevicesOnly = value;
                OnPropertyChanged();
                UpdatePointerCapabilities();
            }
        }

        public Boolean IsMouseAvailable
        {
            get { return _isMouseAvailable; }
            private set
            {
                if (value.Equals(_isMouseAvailable)) return;
                _isMouseAvailable = value;
                OnPropertyChanged();
            }
        }

        public Boolean IsTouchAvailable
        {
            get { return _isTouchAvailable; }
            private set
            {
                if (value.Equals(_isTouchAvailable)) return;
                _isTouchAvailable = value;
                OnPropertyChanged();
            }
        }

        public Boolean IsPenAvailable
        {
            get { return _isPenAvailable; }
            private set
            {
                if (value.Equals(_isPenAvailable)) return;
                _isPenAvailable = value;
                OnPropertyChanged();
            }
        }

        public Boolean PointerSupportMouse
        {
            get { return _pointerSupportMouse; }
            set
            {
                if (value.Equals(_pointerSupportMouse)) return;
                _pointerSupportMouse = value;
                OnPropertyChanged();
            }
        }

        public Boolean PointerSupportTouch
        {
            get { return _pointerSupportTouch; }
            set
            {
                if (value.Equals(_pointerSupportTouch)) return;
                _pointerSupportTouch = value;
                OnPropertyChanged();
            }
        }

        public Boolean PointerSupportPen
        {
            get { return _pointerSupportPen; }
            set
            {
                if (value.Equals(_pointerSupportPen)) return;
                _pointerSupportPen = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Manipulations

        public Boolean ManipulationTransform
        {
            get { return _manipulationTransform; }
            set
            {
                if (value.Equals(_manipulationTransform)) return;
                _manipulationTransform = value;
                OnPropertyChanged();
            }
        }

        public Boolean ManipulationTransformInertia
        {
            get { return _manipulationTransformInertia; }
            set
            {
                if (value.Equals(_manipulationTransformInertia)) return;
                _manipulationTransformInertia = value;
                OnPropertyChanged();
            }
        }

        public Boolean ManipulationRotation
        {
            get { return _manipulationRotation; }
            set
            {
                if (value.Equals(_manipulationRotation)) return;
                _manipulationRotation = value;
                OnPropertyChanged();
            }
        }

        public Boolean ManipulationRotationInertia
        {
            get { return _manipulationRotationInertia; }
            set
            {
                if (value.Equals(_manipulationRotationInertia)) return;
                _manipulationRotationInertia = value;
                OnPropertyChanged();
            }
        }

        public Boolean ManipulationScaling
        {
            get { return _manipulationScaling; }
            set
            {
                if (value.Equals(_manipulationScaling)) return;
                _manipulationScaling = value;
                OnPropertyChanged();
            }
        }

        public Boolean ManipulationScalingInertia
        {
            get { return _manipulationScalingInertia; }
            set
            {
                if (value.Equals(_manipulationScalingInertia)) return;
                _manipulationScalingInertia = value;
                OnPropertyChanged();
            }
        } 

        #endregion

        #region Gestures

        public Boolean TapEnabled
        {
            get { return _tapEnabled; }
            set
            {
                if (value.Equals(_tapEnabled)) return;
                _tapEnabled = value;
                OnPropertyChanged();
            }
        }

        public Boolean RightTapEnabled
        {
            get { return _rightTapEnabled; }
            set
            {
                if (value.Equals(_rightTapEnabled)) return;
                _rightTapEnabled = value;
                OnPropertyChanged();
            }
        }

        public Boolean DoubleTapEnabled
        {
            get { return _doubleTapEnabled; }
            set
            {
                if (value.Equals(_doubleTapEnabled)) return;
                _doubleTapEnabled = value;
                OnPropertyChanged();
            }
        }

        public Boolean HoldingEnabled
        {
            get { return _holdingEnabled; }
            set
            {
                if (value.Equals(_holdingEnabled)) return;
                _holdingEnabled = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        } 

        #endregion

        private void UpdatePointerCapabilities()
        {
            var devices = PointerDevice.GetPointerDevices();
            if (PointerIntegratedDevicesOnly)
            {
                devices = devices.Where(x => x.IsIntegrated).ToList();
            }
            IsTouchAvailable = devices.Any(x => x.PointerDeviceType == PointerDeviceType.Touch); 
            IsMouseAvailable = devices.Any(x => x.PointerDeviceType == PointerDeviceType.Mouse);
            IsPenAvailable = devices.Any(x => x.PointerDeviceType == PointerDeviceType.Pen);

            PointerSupportTouch = IsTouchAvailable;
            PointerSupportMouse = IsMouseAvailable;
            PointerSupportPen = IsPenAvailable;
        }
    }
}