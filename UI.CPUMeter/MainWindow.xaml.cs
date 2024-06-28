using HidSharp.Utility;
using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Hardware.Cpu;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace MegaCpuMeter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Computer computer;
        private Timer timer;
        private UpdateVisitor updateVisitor;
        private TreeView tvComp;
        ControlMenuWindow window;

        public MainWindow()
        {

            tvComp = new TreeView();
            window = new ControlMenuWindow(tvComp);
            hardwareList = new List<IHardware>();

            updateVisitor = new UpdateVisitor();
            InitializeComponent();
            computer = new Computer();
            computer.HardwareAdded += new HardwareEventHandler(HardwareAdded);

            computer.IsCpuEnabled = true;
            computer.IsControllerEnabled = true;
            computer.IsMotherboardEnabled = true;
            computer.IsNetworkEnabled = true;
            computer.IsGpuEnabled = true;
            computer.IsStorageEnabled = true;
            computer.IsMemoryEnabled = true;

            computer.Open();
            timer = new Timer(CheckStatus, null, 2000, 1000);

        }


        public void CheckStatus(Object stateInfo)
        {
            computer.Accept(updateVisitor);
            // x1.Value = (int)hardwareList[0].Sensors[0].Value;
        }
        private List<IHardware> hardwareList;
        private void HardwareAdded(IHardware hardware)
        {
            hardwareList.Add(hardware);

            TreeViewItem item = new TreeViewItem();
            item.Header = hardware.Name;
            tvComp.Items.Add(item);
            foreach (IHardware subHardware in hardware.SubHardware)
                SubHardwareAdded(subHardware, item);
            AddHardwareSensors(hardware, item);




        }

        private void SensorAdded(ISensor sensor)
        {

            Dispatcher.BeginInvoke((Action)(() => FuckMe(sensor)));
         
        }

        private void FuckMe(ISensor sensor)
        {
            TreeViewItem result = null;
            IterateTree(tvComp.Items, sensor.Hardware.Name, ref result);
            if (result != null)
            {
                TreeViewItem subKey = null;
                IterateTree(result.Items, sensor.SensorType.ToString(), ref subKey);
                if(subKey == null)
                {
                    subKey = new TreeViewItem();
                    subKey.Header = sensor.SensorType.ToString();
                    result.Items.Add(subKey);
                }

                subKey.Items.Add(new MegaTreeViewItem(sensor));
            }
        }
        private void IterateTree(ItemCollection items, string key, ref TreeViewItem result)
        {
            //Iterate each item
            foreach (var item in items)
            {
                if (item is TreeViewItem)
                {
                    var tvi = item as TreeViewItem;
                    if (tvi.Header == key)
                    {
                        result = tvi;
                        break;
                    }
                }
                if (item is ItemsControl)
                {
                    ItemsControl ic = (ItemsControl)item;
                    IterateTree(ic.Items, key, ref result); //Recursive call
                }
            }
        }

        private void SubHardwareAdded(IHardware hardware, TreeViewItem item)
        {
            hardware.SensorAdded += new SensorEventHandler(SensorAdded);

            TreeViewItem itemc = new TreeViewItem();
            itemc.Header = hardware.Name;
            item.Items.Add(itemc);
            AddHardwareSensors(hardware, item);
            foreach (IHardware subHardware in hardware.SubHardware)
                SubHardwareAdded(subHardware, itemc);
        }

        private void AddHardwareSensors(IHardware hardware, TreeViewItem item)
        {
            foreach (ISensor sen in hardware.Sensors)
            {
                TreeViewItem subKey = null;
                IterateTree(item.Items, sen.SensorType.ToString(), ref subKey);
                if (subKey == null)
                {
                    subKey = new TreeViewItem();
                    subKey.Header = sen.SensorType.ToString();
                    item.Items.Add(subKey);
                }

                subKey.Items.Add(new MegaTreeViewItem(sen));

               
            }
        }
        private void OnShowItems(object sender, RoutedEventArgs e)
        {

            window.Show();
            window.Activate();
            
  
        }

        private void OnShowFilled(object sender, RoutedEventArgs e)
        {
            fieldMain.ShowFulls();
        }

        private void OnQuit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
    
        }

        private void OnHideBarClicked(object sender, RoutedEventArgs e)
        {
            if (this.WindowStyle == WindowStyle.SingleBorderWindow)
            {
                this.WindowStyle = WindowStyle.None;
                menuItemShowHideBar.Header = "Show Border";
                
            }
            else if (this.WindowStyle == WindowStyle.None)
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                menuItemShowHideBar.Header = "Hide Border";
            }

        }
    }
}