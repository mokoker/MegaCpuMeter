
using LibreHardwareMonitor.Hardware;
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
    public partial class LogoBox : UserControl
    {
        public LogoBox(IHardware hardware)
        {
            InitializeComponent();
            lblSensorName.Content = hardware.Name;
            imgHard.Source = LogoSelector.Select(hardware.HardwareType);
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
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {


                //// Initiate the drag-and-drop operation.
                //var result = DragDrop.DoDragDrop(this, Sensor, DragDropEffects.Copy | DragDropEffects.Move);
                //if (result != null)
                //{
                //    _parentField.RemoveElement(this);
                //}
            }
        }

     

    }
}
