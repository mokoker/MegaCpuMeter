using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MegaCpuMeter
{
    /// <summary>
    /// Interaction logic for ControlMenuWindow.xaml
    /// </summary>
    public partial class ControlMenuWindow : Window
    {
        public ControlMenuWindow(UIElement element)
        {
            InitializeComponent();
            grdMain.Children.Add(element);
            element.Visibility = Visibility.Visible;
            this.Closing += Window_Closing;
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}
