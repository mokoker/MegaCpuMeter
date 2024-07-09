using LibreHardwareMonitor.Hardware;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using static LibreHardwareMonitor.Hardware.Storage.NVMeGeneric;

namespace MegaCpuMeter
{
    /// <summary>
    /// Interaction logic for Field.xaml
    /// </summary>
    public partial class Field : UserControl
    {
        private BucalemunBox[,] grid = new BucalemunBox[20, 20];
        private static int quantSize = 96;

        public BucalemunBox[,] Grid { get => grid; set => grid = value; }
        public static int QuantSize { get => quantSize; set => quantSize = value; }

        public Field()
        {
            InitializeComponent();

            gridMain.DragOver += Field_DragOver;
            gridMain.Drop += Field_Drop;
            gridMain.PreviewDragLeave += GridMain_PreviewDragLeave;

            HideGrid();
        }



        private void GridMain_PreviewDragLeave(object sender, DragEventArgs e)
        {
            HideGrid();

        }

        public void ShowFulls()
        {
            string result = "";
            Brush brush = new SolidColorBrush(Colors.AliceBlue);
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] != null)
                    {
                        grid[i, j].Background = brush;
                        result += $"[{i}={j}]";

                    }
                }
            }
            MessageBox.Show(result);
        }


        private void Field_Drop(object sender, DragEventArgs e)
        {
            var position = e.GetPosition(gridMain);
            int i = (int)Math.Floor(position.X / QuantSize);
            int j = (int)Math.Floor(position.Y / QuantSize);
           
            ISensor droppedThingie = null;
            if (e.Data.GetDataPresent(typeof(BucalemunBox)))//Move train
            {
                var box = e.Data.GetData(typeof(BucalemunBox)) as BucalemunBox;

                BucalemunBox pointer = box;
                int locx = i;
                while (pointer != null)
                {
                    pointer.SetLocation(locx, j);
                    locx += pointer.Size;
                    pointer = pointer.Next;
                    
                }
            }

            if (e.Data.GetDataPresent(typeof(Sensor)))
            {
                droppedThingie = e.Data.GetData(typeof(Sensor)) as Sensor;

            }
            else if (e.Data.GetDataPresent(typeof(NVMeSensor)))
            {
                droppedThingie = e.Data.GetData(typeof(NVMeSensor)) as NVMeSensor;
            }
            if (droppedThingie != null)
            {

                if (Grid[i, j] == null)
                {
                    BucalemunBox mn = new BucalemunBox(this, droppedThingie,ControlType.NumberLogo);
                    
                    gridMain.Children.Add(mn);

                    mn.SetLocation(i, j);

           
                    CheckTrainX(i, j, mn);


                }
                else
                {

                }

            }
            HideGrid();
        }

        private BucalemunBox GenerateLogoBox(int i, int j, ISensor type)
        {
            BucalemunBox lb = new BucalemunBox(this, type, ControlType.Logo);

            gridMain.Children.Add(lb);
            lb.SetLocation(i, j);

            return lb;
        }
   
        private bool CheckTrainX(int x, int y, BucalemunBox droppedThingie)
        {
            if (x == 0)//En soldayiz tren olamaz
                return false;

            var leftOne = Grid[x - 1, y];
            if (leftOne != null)
            {
                if (leftOne.Sensor.Hardware.HardwareType == droppedThingie.Sensor.Hardware.HardwareType)
                {
                    if (leftOne.TrainMode)
                    {
                        droppedThingie.TrainMode = true;
                        leftOne.Next = droppedThingie;
                        droppedThingie.Prev = leftOne;

                    }
                    else
                    {
                        BucalemunBox lb = null;
                        if (x == 1)
                        {
                            lb = GenerateLogoBox(0, y, droppedThingie.Sensor);
                            leftOne.Margin = droppedThingie.Margin;
                            grid[1, y] = leftOne;
                            
                            droppedThingie.SetLocation(2, y);
                        }
                        else
                        {
                            lb = GenerateLogoBox(x - 1-leftOne.Size, y, droppedThingie.Sensor);
                        }
                        leftOne.Prev = lb;
                        lb.Next = leftOne;
                        leftOne.Next = droppedThingie;
                        droppedThingie.Prev = leftOne;
                        leftOne.TrainMode = true;
                        droppedThingie.TrainMode = true;

                    }
                    droppedThingie.CreateBorderForGroup();
                    return true;
                }
            }
            return false;
        }

        private void CheckTrainY(int x, int y)
        {

        }

        private void Field_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Sensor)) || e.Data.GetDataPresent(typeof(NVMeSensor)) || e.Data.GetDataPresent(typeof(BucalemunBox)))
            {
                ShowGrid();
                var position = e.GetPosition(gridMain);
                int i = (int)Math.Floor(position.X / QuantSize);
                int j = (int)Math.Floor(position.Y / QuantSize);

                if (Grid[i, j] != null)
                {
                    e.Effects = DragDropEffects.None;
                }
                else
                {
                    e.Effects = DragDropEffects.Move;

                }

            }
            else
            {
                e.Effects = DragDropEffects.None;

            }
            e.Handled = true;
        }



        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            cnvBack.Children.Clear();
            for (int j = 0; j < this.ActualHeight; j += QuantSize)
            {
                Line line = new Line();
                line.StrokeThickness = 1;
                line.Visibility = Visibility.Visible;
                line.StrokeDashArray = new DoubleCollection([2, 2]);
                line.Stroke = Brushes.Black;
                line.X1 = 0;
                line.X2 = this.ActualWidth - 1;
                line.Y1 = j;
                line.Y2 = j;
                cnvBack.Children.Add(line);
            }

            for (int i = 0; i < this.ActualWidth; i += QuantSize)
            {
                Line line = new Line();
                line.StrokeThickness = 1;
                line.Visibility = Visibility.Visible;
                line.StrokeDashArray = new DoubleCollection([2, 2]);

                line.Stroke = Brushes.Black;
                line.X1 = i;
                line.X2 = i;
                line.Y1 = 0;
                line.Y2 = this.ActualHeight - 1;
                cnvBack.Children.Add(line);
            }
        }
        public void HideGrid()
        {
            cnvBack.Visibility = Visibility.Hidden;
        }

        public void ShowGrid()
        {
            cnvBack.Visibility = Visibility.Visible;
        }
        public void RemoveElement(UIElement obj)
        {
            gridMain.Children.Remove(obj);
        }
    }
}
