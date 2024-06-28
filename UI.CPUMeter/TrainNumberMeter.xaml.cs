
using LibreHardwareMonitor.Hardware;
using System;

using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MegaCpuMeter
{
    /// <summary>
    /// Interaction logic for MeterVertical.xaml
    /// </summary>
    public partial class TrainNumberMeter : UserControl
    {
        private ISensor _value;
        private Field _parentField;
        private int divider = 1;
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

        }

        private void Value_SensorValueChanged(float? value)
        {
            Dispatcher.BeginInvoke((Action)(() => ChangeVal(value)));

        }



        //private void ChangeColors(int oldVal,int newVal)
        //{
        //    if (newVal > oldVal)
        //    {
        //        for (int i = oldVal; i < newVal; i++)
        //        {
        //            var x = (Rectangle)stckMain.Children[i];
        //            x.Fill = new SolidColorBrush(Colors.Blue);
        //        }
        //    }
        //    else
        //    {
        //        for (int i = oldVal; i > newVal; i--)
        //        {
        //            var x = (Rectangle)stckMain.Children[i];
        //            x.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF34B9A4"));
        //        }
        //    }
        //    _value = newVal;

        //}
        //protected override void OnMouseMove(MouseEventArgs e)
        //{
        //    base.OnMouseMove(e);
        //    if (e.LeftButton == MouseButtonState.Pressed)
        //    {


        //        // Initiate the drag-and-drop operation.
        //        var result = DragDrop.DoDragDrop(this, Sensor, DragDropEffects.Copy | DragDropEffects.Move);
        //        if (result != null)
        //        {
        //            _parentField.RemoveElement(this);
        //        }
        //    }
        //}

        public TrainNumberMeter(Field field)
        {
            _parentField = field;
            InitializeComponent();
        }

        public TrainNumberMeter(Field field, ISensor sensor) : this(field)
        {
            Sensor = sensor;
        }
    }
}
