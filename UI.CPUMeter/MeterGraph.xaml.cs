
using LibreHardwareMonitor.Hardware;
using System.Diagnostics.Eventing.Reader;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MegaCpuMeter
{
    /// <summary>
    /// Interaction logic for MeterVertical.xaml
    /// </summary>
    public partial class MeterGraph : UserControl
    {
        private ISensor _value;
        private Field _parentField;
        private int divider = 1;
        private int drawOffset = 0;
        private double last = 0;
        private Brush stroker = Brushes.BlueViolet;
        private int gridWithTimes = 5;
        private double max = 0;

        private int[] history = Enumerable.Repeat(0, 96).ToArray();
        public ISensor Sensor
        {
            get
            {
                return _value;
            }
            set
            {

                value.SensorValueChanged += Value_SensorValueChanged;
                _value = value;
                lblSensorName.Content = value.Name;
                lblPercentage.Content = value.Value / divider;
                lblHardware.Text = value.Hardware.Name;
                FillUnit(value.SensorType);
                DrawGraph();
            }
        }
        private double MaxGraphValue
        {
            
            get
            {
       
                switch (_value.SensorType)
                {
                    case SensorType.Voltage:
                        max = 5;
                        break;
                    case SensorType.Current:
                        max = 10;
                        break;
                    case SensorType.Power:
                        max = 500;
                        break;
                    case SensorType.Clock:
                        max = 7000;
                        break;
                    case SensorType.Temperature:
                        max = 120;
                        break;
                    case SensorType.Load:
                        max = 100;
                        break;
                    case SensorType.Frequency:
                        max = 7000;
                        break;
                    case SensorType.Fan:
                        max = 5000;
                        break;
                    case SensorType.Flow:
                        break;
                    case SensorType.Control:
                        max = 100;
                        break;
                    case SensorType.Level:
                        max = 100;
                        break;
                    case SensorType.Factor:
                        break;
                    case SensorType.Data:
                        break;
                    case SensorType.SmallData:
                        break;
                    case SensorType.Throughput:
                        break;
                    case SensorType.TimeSpan:
                        break;
                    case SensorType.Energy:
                        break;
                    case SensorType.Noise:
                        break;
                    case SensorType.Humidity:
                        break;
                }
                if(_value.Max.HasValue && _value.Max.Value > max)
                    max = _value.Max.Value*2;
                lblMax.Content = Math.Round(max,3);
                return max;
            }
        }

        private void DrawNext(double val)
        {
           
            var line = new Line();
            line.Stroke = stroker;
            line.X1 = Width+drawOffset;
            line.X2 = line.X1 + 2;
            line.Y1 = Height - last * Height / MaxGraphValue;
            line.Y2 = Height - val * Height / MaxGraphValue;

            last = val;
            cnvHistory.Children.Add(line);
            drawOffset += 2;

            cnvHistory.Margin = new System.Windows.Thickness(drawOffset*-1, 0, 0, 0);
            if(drawOffset > Width * (gridWithTimes-1))
            {
                cnvHistory.Children.Clear();
                drawOffset = 0;
                DrawGraph();
            }
        }

        private void DrawGraph()
        {
            int step = 2;

            double leftPosition = Width - Sensor.Values.Count() * step;
            double max = (double)(_value.Max.HasValue ? _value.Max : 100);
            if (_value.Values.Count() > 0)
            {
                double prev = _value.Values.First().Value;
                foreach (var x in _value.Values)
                {

                    var line = new Line();
                    line.Stroke = stroker;
                    line.X1 = leftPosition;
                    line.X2 = leftPosition + 2;
                    line.Y1 = Height - prev * Height / MaxGraphValue;
                    line.Y2 = Height - x.Value * Height / MaxGraphValue;
                    leftPosition += 2;
                    prev = x.Value;
                    cnvHistory.Children.Add(line);
                }
                last = _value.Values.Last().Value;
            }
        }

        private void DrawGrid()
        {
            for (int j = 0; j < Height; j += (int)Height/5)
            {
                Line line = new Line();
                line.StrokeThickness = 0.1;
                ///line.StrokeDashArray = new DoubleCollection([2, 2]);
                line.Stroke = Brushes.Gray;
                line.X1 = 0;
                line.X2 = Width - 1;
                line.Y1 = j;
                line.Y2 = j;
                cnvBack.Children.Add(line);
            }
        }

        private void FillUnit(SensorType type)
        {
            switch (type)
            {
                case SensorType.Voltage:
                    lblUnit.Content = "Volt";
                    break;
                case SensorType.Current:
                    lblUnit.Content = "Ampere";
                    break;
                case SensorType.Power:
                    lblUnit.Content = "Watt";
                    break;
                case SensorType.Clock:
                    lblUnit.Content = "MHz";
                    break;
                case SensorType.Temperature:
                    lblUnit.Content = "Centigrade";
                    break;
                case SensorType.Load:
                    lblUnit.Content = "Percent";
                    break;
                case SensorType.Frequency:
                    lblUnit.Content = "MHz";
                    break;
                case SensorType.Fan:
                    lblUnit.Content = "RPM";
                    break;
                case SensorType.Flow:
                    break;
                case SensorType.Control:
                    lblUnit.Content = "Percent";

                    break;
                case SensorType.Level:
                    break;
                case SensorType.Factor:
                    break;
                case SensorType.Data:
                    lblUnit.Content = "GB";
                    break;
                case SensorType.SmallData:
                    break;
                case SensorType.Throughput:
                    lblUnit.Content = "KB/s";
                    divider = 100;
                    break;
                case SensorType.TimeSpan:
                    break;
                case SensorType.Energy:
                    break;
                case SensorType.Noise:
                    break;
                case SensorType.Humidity:
                    break;
            }
        }
        private void ChangeVal(float? value)
        {
            if (value.HasValue && value != 0)
            {
                var x = value.Value / divider;
                lblPercentage.Content = x.ToString("#.##");
                DrawNext((double)value.Value);
            }
            else
            {
                lblPercentage.Content = 0;
            }

        }

        private void Value_SensorValueChanged(float? value)
        {
            Dispatcher.BeginInvoke((Action)(() => ChangeVal(value)));

        }

        public MeterGraph(Field field)
        {
            _parentField = field;
            InitializeComponent();
            DrawGrid();
            cnvHistory.Width = Width * gridWithTimes;

        }

        public MeterGraph(Field field, ISensor sensor) : this(field)
        {
            Sensor = sensor;
        }
    }
}
