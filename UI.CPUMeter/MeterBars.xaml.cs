
using LibreHardwareMonitor.Hardware;
using System.Windows.Controls;
using System.Windows.Media;

namespace MegaCpuMeter
{
    /// <summary>
    /// Interaction logic for MeterVertical.xaml
    /// </summary>
    public partial class MeterBars : UserControl
    {
        private ISensor _value;
        private Field _parentField;
        private int divider = 1;
        private GradientStopCollection colorScale;
        private double max = 0;
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
         
                FillUnit(value.SensorType);
             
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
                if (_value.Max.HasValue && _value.Max.Value > max)
                    max = _value.Max.Value * 2;
               
                return max;
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
            }
            else
            {
                lblPercentage.Content = 0;
            }

           

            rctMain.Height =(double) value/ MaxGraphValue * 0.96*this.Height;

     
            rctMain.Fill = new SolidColorBrush(GetColor((double)value/MaxGraphValue));
        }

        private Color GetColor(double value)
        {
            return colorScale.GetRelativeColor(value);
        }
        private void Value_SensorValueChanged(float? value)
        {
            Dispatcher.BeginInvoke((Action)(() => ChangeVal(value)));

        }


    

        public MeterBars(Field field)
        {
            _parentField = field;
            InitializeComponent();
            colorScale =
        [
            new GradientStop((Color)ColorConverter.ConvertFromString("#FF0033FF"), 0),
                new GradientStop((Color)ColorConverter.ConvertFromString("#FFF1F600"), 0.5),
                new GradientStop(Colors.Red,1),
                new GradientStop((Color)ColorConverter.ConvertFromString("#FF00FF56"), 0.2),
            ];
        }

        public MeterBars(Field field, ISensor sensor) : this(field)
        {
            Sensor = sensor;
 
        }
    }
}
