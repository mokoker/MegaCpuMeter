using HidSharp.Utility;
using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Hardware.Cpu;
using System.Formats.Tar;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace MegaCpuMeter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Computer computer;
        private Timer timer, timerx;
        private UpdateVisitor updateVisitor;
        private TreeView tvComp;
        ControlMenuWindow window;
        private Dictionary<string, ISensor> sensors= new Dictionary<string,ISensor>();
        private List<StoredSquare> forgottenSquares = new List<StoredSquare>();
        public MainWindow()
        {
            IsNetFramework45Installed();
            tvComp = new TreeView();
            window = new ControlMenuWindow(tvComp);
            hardwareList = new List<IHardware>();

            updateVisitor = new UpdateVisitor();
            InitializeComponent();
            computer = new Computer();
            this.Closed += MainWindow_Closed;
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

            //  timerx = new Timer(FillMemory, null, 100,2000);
            FillMemory(null);
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private static bool IsNetFramework45Installed()
        {
            Type type;
            try
            {
                type = TryGetDefaultDllImportSearchPathsAttributeType();
            }
            catch (TypeLoadException)
            {
                MessageBox.Show(
                  "This application requires the .NET Framework 4.5 or a later version.\n" +
                  "Please install the latest .NET Framework. For more information, see\n\n" +
                  "https://dotnet.microsoft.com/download/dotnet-framework",
                  "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return type != null;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Type TryGetDefaultDllImportSearchPathsAttributeType()
        {
            return typeof(DefaultDllImportSearchPathsAttribute);
        }

        public void FillMemory(Object stateInfo)
        {
            if (File.Exists("memory.mem"))
            {
                using (StreamReader readtext = new StreamReader("memory.mem"))
                {
                    string readText = readtext.ReadToEnd();
                    var xx = JsonSerializer.Deserialize<List<StoredSquare>>(readText);

                    foreach (var x in xx)
                    {
                        if (x.Type != ControlType.Logo)
                        {
                            
                            if (!sensors.ContainsKey(x.Name))
                            {
                                forgottenSquares.Add(x);
                            }
                            else {
                                Dispatcher.BeginInvoke((Action)(() => fieldMain.AddSensor(x.X, x.Y, sensors[x.Name], x.Type)));
                            }

                        }
                    }
                }   
            }
           // timerx.Dispose();

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
            var xy =  forgottenSquares.Find(x => x.Name == sensor.Identifier.ToString());
            if (xy != null)
            {
                Dispatcher.BeginInvoke((Action)(() => fieldMain.AddSensor(xy.X, xy.Y, sensor, xy.Type)));
            }

            TreeViewItem result = null;
            IterateTree(tvComp.Items, sensor.Hardware.Name, ref result);
            if (result != null)
            {
                TreeViewItem subKey = null;
                IterateTree(result.Items, sensor.SensorType.ToString(), ref subKey);
                if (subKey == null)
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
                sensors[sen.Identifier.ToString()]= sen;

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

        private void OnSaveLayout(object sender, RoutedEventArgs e)
        {
            var result = fieldMain.ExportData();
            string jsonString = JsonSerializer.Serialize(result);
            File.WriteAllText("memory.mem", jsonString);

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