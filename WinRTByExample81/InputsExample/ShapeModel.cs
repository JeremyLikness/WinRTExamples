using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Input;
using InputsExample.Annotations;

namespace InputsExample
{
    public abstract class ShapeModel : INotifyPropertyChanged
    {
        private const Int32 InitialShapeSize = 128;
        private const Int32 MinShapeSize = 32;

        private Color _color;
        private Double _size = InitialShapeSize;
        private Point _position;
        private Point _direction;
        private Double _rate;
        private Double _rotation;
        private Boolean _isHot;
        private ManipulationModes _manipulationMode;
        private Boolean _isTapEnabled;
        private Boolean _isRightTapEnabled;
        private Boolean _isDoubleTapEnabled;
        private Boolean _isHoldingEnabled;

        private readonly IList<PointerDeviceType> _supportedDeviceTypes = new List<PointerDeviceType>();

        protected ShapeModel(ShapeType shapeType)
        {
            Shape = shapeType;
            SetRandomColor();
            SetRandomDirection();
            // Rate = 8,
        }

        public ShapeType Shape { get; private set; }

        public enum ShapeType
        {
            Ball,
            Square
        }

        public Color Color
        {
            get { return _color; }
            set
            {
                if (!_color.Equals(value))
                {
                    _color = value;
                    OnPropertyChanged();
                }
            }
        }

        public Double Size
        {
            get { return _size; }
            set
            {
                if (!_size.Equals(value))
                {
                    _size = value;
                    OnPropertyChanged();
                }
            }
        }

        public Double LeftPos
        {
            get { return Position.X - (Size / 2.0); }
        }

        public Double TopPos
        {
            get { return Position.Y - (Size / 2.0); }
        }

        public Point MinPosPoint { get; set; }

        public Point MaxPosPoint { get; set; }

        public Point Position
        {
            get { return _position; }
            set
            {
                if (!_position.Equals(value))
                {
                    _position = value;
                    OnPropertyChanged();
                    OnPropertyChanged("LeftPos");
                    OnPropertyChanged("TopPos");
                }
            }
        }

        public Double Rotation
        {
            get { return _rotation; }
            set
            {
                if (!_rotation.Equals(value))
                {
                    _rotation = value;
                    OnPropertyChanged();
                }
            }
        }

        public Point Direction
        {
            get { return _direction; }
            set
            {
                if (!_direction.Equals(value))
                {
                    _direction = value;
                    OnPropertyChanged();
                }
            }
        }

        public Double Rate
        {
            get { return _rate; }
            set
            {
                if (!_rate.Equals(value))
                {
                    _rate = value;
                    OnPropertyChanged();
                }
            }
        }

        public Boolean IsHot
        {
            get { return _isHot; }
            set
            {
                if (!_isHot.Equals(value))
                {
                    _isHot = value;
                    OnPropertyChanged();
                }
            }
        }

        public ManipulationModes ManipulationMode
        {
            get { return _manipulationMode; }
            set
            {
                if (!_manipulationMode.Equals(value))
                {
                    _manipulationMode = value;
                    OnPropertyChanged();
                }
            }
        }

        public Boolean IsTapEnabled
        {
            get { return _isTapEnabled; }
            set
            {
                if (value.Equals(_isTapEnabled)) return;
                _isTapEnabled = value;
                OnPropertyChanged();
            }
        }

        public Boolean IsRightTapEnabled
        {
            get { return _isRightTapEnabled; }
            set
            {
                if (value.Equals(_isRightTapEnabled)) return;
                _isRightTapEnabled = value;
                OnPropertyChanged();
            }
        }

        public Boolean IsDoubleTapEnabled
        {
            get { return _isDoubleTapEnabled; }
            set
            {
                if (value.Equals(_isDoubleTapEnabled)) return;
                _isDoubleTapEnabled = value;
                OnPropertyChanged();
            }
        }

        public Boolean IsHoldingEnabled
        {
            get { return _isHoldingEnabled; }
            set
            {
                if (value.Equals(_isHoldingEnabled)) return;
                _isHoldingEnabled = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<PointerDeviceType> SupportedDeviceTypes
        {
            get { return _supportedDeviceTypes;  }
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public void SetRandomColor()
        {
            var random = new Random((Int32)DateTime.UtcNow.Ticks % Int32.MaxValue);
            var colorR = (Byte)random.Next(0, 0xff);
            var colorG = (Byte)random.Next(0, 0xff);
            var colorB = (Byte)random.Next(0, 0xff);
            Color = new Color { R = colorR, G = colorG, B = colorB, A = 0xff };
        }

        public void SetRandomDirection()
        {
            var random = new Random((Int32)DateTime.UtcNow.Ticks % Int32.MaxValue);
            var angle = random.Next(0, 359);
            var radians = Math.PI * angle / 180.0;
            Direction = new Point(Math.Cos(radians), Math.Sin(radians));
        }

        public void UpdateExtents(Point minPosPoint, Point maxPosPoint)
        {
            MinPosPoint = minPosPoint;
            MaxPosPoint = maxPosPoint;

            FixExtents();
        }

        private void FixExtents()
        {
            var xPos = Position.X;
            xPos = Math.Max(xPos, MinPosPoint.X + (Size / 2.0));
            xPos = Math.Min(xPos, MaxPosPoint.X - (Size / 2.0));

            var yPos = Position.Y;
            yPos = Math.Max(yPos, MinPosPoint.Y + (Size / 2.0));
            yPos = Math.Min(yPos, MaxPosPoint.Y - (Size / 2.0));
            Position = new Point(xPos, yPos);
        }

        public void UpdateShapePosition()
        {
            var xOffset = Direction.X * Rate;
            var yOffset = Direction.Y * Rate;

            MoveShape(xOffset, yOffset);
        }

        public void MoveShape(Double xOffset, Double yOffset)
        {
            var xPos = Position.X;
            if (xPos + xOffset - (Size / 2.0) <= MinPosPoint.X || xPos + (Size / 2.0) + xOffset >= MaxPosPoint.X)
            {
                Direction = new Point(Direction.X * -1, Direction.Y);
                xOffset = Direction.X * Rate;
            }

            var yPos = Position.Y;
            if (yPos - (Size / 2.0) + yOffset <= MinPosPoint.Y || yPos + (Size / 2.0) + yOffset >= MaxPosPoint.Y)
            {
                Direction = new Point(Direction.X, Direction.Y * -1);
                yOffset = Direction.Y * Rate;
            }

            Position = new Point(xPos + xOffset, yPos + yOffset);
        }

        public void UpdateRate(Double xRate, Double yRate)
        {
            // TODO (remember, both rate and direction, and units are per 30 ms...so that has to be translated somewhere)
        }

        public void ResizeShape(Double expansionPercent)
        {
            var newSize = Size * expansionPercent;
            newSize = Math.Max(newSize, MinShapeSize);
            newSize = Math.Min(newSize, Math.Min(MaxPosPoint.Y - MinPosPoint.Y, MaxPosPoint.X - MinPosPoint.X));
            Size = newSize;
            FixExtents();
        }

        public void RotateShape(Double degrees)
        {
            Rotation += degrees;
        }

        public void UpdateInputSettings(InputSettings inputSettings)
        {
            var manipulationMode = ManipulationModes.All;

            if (!inputSettings.ManipulationTransform) manipulationMode ^= ManipulationModes.TranslateX;
            if (!inputSettings.ManipulationTransform) manipulationMode ^= ManipulationModes.TranslateY;
            if (!inputSettings.ManipulationTransformInertia) manipulationMode ^= ManipulationModes.TranslateInertia;

            if (!inputSettings.ManipulationScaling) manipulationMode ^= ManipulationModes.Scale;
            if (!inputSettings.ManipulationScalingInertia) manipulationMode ^= ManipulationModes.ScaleInertia;

            if (!inputSettings.ManipulationRotation) manipulationMode ^= ManipulationModes.Rotate;
            if (!inputSettings.ManipulationRotationInertia) manipulationMode ^= ManipulationModes.RotateInertia;

            ManipulationMode = manipulationMode;

            IsTapEnabled = inputSettings.TapEnabled;
            IsRightTapEnabled = inputSettings.RightTapEnabled;
            IsDoubleTapEnabled = inputSettings.DoubleTapEnabled;
            IsHoldingEnabled = inputSettings.HoldingEnabled;

            _supportedDeviceTypes.Clear();
            if (inputSettings.PointerSupportMouse) _supportedDeviceTypes.Add(PointerDeviceType.Mouse);
            if (inputSettings.PointerSupportPen) _supportedDeviceTypes.Add(PointerDeviceType.Pen);
            if (inputSettings.PointerSupportTouch) _supportedDeviceTypes.Add(PointerDeviceType.Touch);
        }
    }

    public class SquareShapeModel : ShapeModel
    {
        public SquareShapeModel()
            : base(ShapeType.Square)
        {
        }
    }

    public class BallShapeModel : ShapeModel
    {
        public BallShapeModel()
            : base(ShapeType.Ball)
        {
        }
    }
}