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
            {
                for(int i = 0;i<Size;i++)
                {
                    _field.Grid[X+i, Y] = null;
                }
            }
            for (int i = 0; i < Size; i++)
            {
                _field.Grid[x+i, y] = this;
            }
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
              
                if(controlType == ControlType.NumberLogo)
                  ChangeControlType(ControlType.Number);
                trainMode = value;
            }
        }

        private void FillEmptyGrid(int count)
        {
            if (count > 0)
            {
                MoveTheRights(count);
                FillGrid(count);
            }
            else
            {
                EmptyGrid(count*-1);
                MoveTheRights(count);
            }
        }
        private void FillGrid(int count)
        { 
            for(int i = 1; i <= count; i++)
            {
                if(_field.Grid[X+i,Y] == null)
                {
                    _field.Grid[X + i, Y] = this;
                }
                else
                {
                    throw new Exception();
                }
            }
        }
        private void EmptyGrid(int count)
        {
            for (int i = count; i > 0; i--)
            {
                if (_field.Grid[X + i, Y] != null)
                {
                    _field.Grid[X + i, Y] = null;
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        private void ChangeControlType(ControlType ctrlType)
        {
            mainGrid.Children.Clear();
            UserControl control = null;
            switch (ctrlType)
            {
                case ControlType.Number:
                    control = new TrainNumberMeter(_field, Sensor);
                    Width = Field.QuantSize * 1;

                    FillEmptyGrid(1 - size);
                    Size = 1;
                    ctrlType = ControlType.Number;
                    break;
                case ControlType.Bar:
                    control = new MeterBars(_field, Sensor);
                    Width = Field.QuantSize * 1;
                    FillEmptyGrid(1 - size);
                    Size = 1;
                    ctrlType = ControlType.Bar;
                    break;
                case ControlType.Graph:
                    control = new MeterGraph(_field, Sensor);
                    FillEmptyGrid(3 - size);
                    Size = 3;
                    Width = Field.QuantSize * Size;
                    controlType = ControlType.Graph;
                    break;
                case ControlType.NumberLogo:
                    control = new MeterNumber(_field, Sensor);
                    Width = Field.QuantSize * 1;
                    FillEmptyGrid(1 - size);
                    ctrlType = ControlType.NumberLogo;
                    Size = 1;
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
                case ControlType.NumberLogo:
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
                        RemoveThis();
                        
                    }
                }
            }
        }
        private void RemoveThis()
        {
            for (int i = 0; i < Size; i++)
            {
                _field.Grid[X + i, Y] = null;
            }
           
            if (!RemoveLabelIfEmpty())
                MoveTheRights(-1 * Size);
            RemoveMeFromChain();

            _field.RemoveElement(this);
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

        public void RemoveMe()
        {
            _field.RemoveElement(this);
            _field.Grid[X, Y] = null;
        }
        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
          
            if (controlType == ControlType.Logo)
            {
                BucalemunBox pointer = this;
                while (pointer != null)
                {
                    pointer.RemoveMe();
                    pointer = pointer.next;
                }
            }
            else
            {
                RemoveThis();
            }    
        }
    }
}
