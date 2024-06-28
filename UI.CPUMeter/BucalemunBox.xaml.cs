using LibreHardwareMonitor.Hardware;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MegaCpuMeter
{
    /// <summary>
    /// Interaction logic for BucalemunBox.xaml
    /// </summary>
    public partial class BucalemunBox : UserControl
    {
        private bool trainMode;
        private ControlType controlType;
        private Field _field;
        private BucalemunBox next;
        private BucalemunBox prev;
        private int size = 1;
        public int Size { get => size; set => size = value; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public void SetLocation(int x, int y)
        {
            if (X >= 0 && Y >= 0)
                _field.Grid[X, Y] = null;
            _field.Grid[x, y] = this;
            X = x;
            Y = y;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            Margin = GetLocation(x, y);
        }

        private Thickness GetLocation(int i, int j)
        {
            var x = i * Field.QuantSize;
            var y = j * Field.QuantSize;
            return new Thickness(x, y, 0, 0);
        }
        public ISensor Sensor { get; set; }
        public bool TrainMode
        {
            get => trainMode; set
            {
                ChangeControlType(ControlType.Number);
                trainMode = value;
            }
        }
        private void ChangeControlType(ControlType controlType)
        {
            mainGrid.Children.Clear();
            UserControl control = null;
            switch (controlType)
            {
                case ControlType.Number:
                    control = new TrainNumberMeter(_field, Sensor);
                    Width = Field.QuantSize * 1;
                    MoveTheRights(1 - size);
                    Size = 1;

                    break;
                case ControlType.Bar:
                    control = new MeterBars(_field, Sensor);
                    Width = Field.QuantSize * 1;
                    MoveTheRights(1 - size);
                    Size = 1;
                    break;
                case ControlType.Graph:
                    control = new MeterGraph(_field, Sensor);
                    MoveTheRights(3 - size);
                    Size = 3;
                    Width = Field.QuantSize * Size;
                    break;

            }
            mainGrid.Children.Add(control);

        }

        public BucalemunBox Next
        {
            get => next; set
            {
                next = value;
            }
        }
        public BucalemunBox Prev
        {
            get => prev; set
            {
                prev = value;
            }
        }
        public int LengthOfTrain
        {
            get
            {
                int count = 0;
                BucalemunBox pointer = this;
                while (pointer != null && pointer.Sensor.SensorType == Sensor.SensorType)
                {
                    count++;
                    pointer = pointer.Next;
                }
                return count;
            }

        }
        public void CreateBorderForGroup()
        {
            Brush brush = new SolidColorBrush(Colors.Green);
            BucalemunBox pointy = this;
            while (pointy.prev != null)
            {
                pointy = pointy.prev;
            }
            pointy.BorderBrush = brush;
            pointy.BorderThickness = new Thickness(2, 2, 1, 2);
            pointy = pointy.next;
            while (pointy.next != null)
            {
                pointy.BorderBrush = brush;
                pointy.BorderThickness = new Thickness(0, 2, 1, 2);
                pointy = pointy.next;
            }
            pointy.BorderThickness = new Thickness(0, 2, 2, 2);
            pointy.BorderBrush = brush;



        }

        public BucalemunBox(Field field, ISensor sensor, ControlType type = ControlType.Number)
        {
            X = -1;
            Y = -1;
            Sensor = sensor;
            _field = field;
            controlType = type;
            InitializeComponent();
            UserControl control = null;
            switch (type)
            {
                case ControlType.Number:
                    control = new MeterNumber(field, sensor);
                    break;
                case ControlType.Logo:
                    control = new LogoBox(sensor.Hardware);
                    break;
            }
            mainGrid.Children.Add(control);

        }

        private bool RemoveLabelIfEmpty()
        {

            if (prev != null && prev.controlType == ControlType.Logo && next == null)
            {
                _field.Grid[prev.X, prev.Y] = null;
                _field.RemoveElement(prev);
                prev = null;
                return true;
            }
            return false;
        }
        private void RemoveMeFromChain()
        {
            if (prev != null)
            {
                prev.next = next;
            }
            if (next != null)
            {
                next.prev = prev;
            }
        }

        private void MoveTheRights(int offset)
        {
            if (offset == 0) return;
            BucalemunBox pointer = next;

            while (pointer != null)
            {

                pointer.SetLocation(pointer.X + offset, pointer.Y);



                //if (pointer.next == null)
                //{
                //    _field.Grid[pointer.X, pointer.Y] = null;

                //}
                pointer = pointer.next;

            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                object pass;
                if (controlType == ControlType.Logo)
                {
                    pass = this;

                }
                else
                {
                    pass = Sensor;
                }
                var result = DragDrop.DoDragDrop(this, pass, DragDropEffects.None | DragDropEffects.Copy | DragDropEffects.Move);
                if (result != null && result != DragDropEffects.None)
                {
                    if (controlType == ControlType.Logo)
                    {


                    }
                    else
                    {

                        _field.Grid[X, Y] = null;
                        if (!RemoveLabelIfEmpty())
                            MoveTheRights(-1);
                        RemoveMeFromChain();


                        _field.RemoveElement(this);
                    }
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ChangeControlType(ControlType.Number);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            ChangeControlType(ControlType.Bar);
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            ChangeControlType(ControlType.Graph);

        }
    }
}
