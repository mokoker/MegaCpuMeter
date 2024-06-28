using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MegaCpuMeter
{
    public class MegaTreeViewItem : TreeViewItem
    {
        public MegaTreeViewItem(ISensor sensor)
        {
            Sensor = sensor;
        }
        private ISensor _sensor;
        public ISensor Sensor
        {
            get
            {
                return _sensor;
            }
            set
            {

                value.SensorValueChanged += Value_SensorValueChanged;
                _sensor = value;
                
                this.Header = "will change";

            }
        }

        private void Value_SensorValueChanged(float? value)
        {
            Dispatcher.BeginInvoke((Action)(() => ChangeVal(value)));
         
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
               var result =  DragDrop.DoDragDrop(this, Sensor, DragDropEffects.Copy | DragDropEffects.Move);
                if(result == DragDropEffects.None)
                {
                    
                }
                
            }
        }

        private void ChangeVal(float? value)
        {
            this.Header = $"{_sensor.Name} {value} ";
        }
    }
}
