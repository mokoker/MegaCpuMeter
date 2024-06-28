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
    public partial class MeterVerticalBold : UserControl
    {
        private int _thickness = 16;
        private int _value;
        public int Value
        {
            get
            {
                return _value;
            }

            set
            {

                Dispatcher.BeginInvoke((Action)(() => ChangeValue(value)));

            }
        }
        private void ChangeValue(int value)
        {
            var highlight = value / 5;
            var highlightOld = _value / 5;
            if (highlight > highlightOld)
                for (int i = highlightOld; i < highlight; i++)
                {
                    var x = (Rectangle)stckMain.Children[i + 1];
                    x.Fill = new SolidColorBrush(Colors.Blue);
                }
            else
            {
                for (int i = highlightOld; i > highlight; i--)
                {
                    var x = (Rectangle)stckMain.Children[i];
                    x.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF34B9A4"));
                }
            }
            var yPostion = value * -4;
            WriteNumber(value, yPostion - 5);
            _value = value;
        }
        private void WriteNumber(int number, int yPosition)
        {
            lblPercentage.Content = number.ToString();
            lblPercentage.RenderTransform = new TranslateTransform(0, yPosition);
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
        public MeterVerticalBold()
        {
            InitializeComponent();
            DrawBars(20);
        }
        private void DrawBars(int count)
        {
            for (int i = 0; i <= count; i++)
            {
                //     < Rectangle x: Name = "sampleBar" HorizontalAlignment = "Left" Height = "5" Margin = "10,2,10,2" Stroke = "#FF00FF90" VerticalAlignment = "Bottom" Width = "80" Fill = "#FF34B9A4" />
                Rectangle rectangle = new Rectangle();
                rectangle.Height = _thickness;
                rectangle.Width = 80;
                rectangle.Name = $"rect{i}";
                rectangle.Margin = new Thickness(10, 1.5, 10, 1.5);
                //  rectangle.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FF90"));
                rectangle.Fill = new SolidColorBrush(i == 0 ? Colors.Blue : (Color)ColorConverter.ConvertFromString("#FF34B9A4"));
                stckMain.Children.Add(rectangle);
            }
        }
    }
}
