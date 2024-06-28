using LibreHardwareMonitor.Hardware;
using System.Windows.Media.Imaging;

namespace MegaCpuMeter
{
    public static class LogoSelector
    {
        public static BitmapImage Select(HardwareType type)
        {
            switch (type)
            {
                case HardwareType.Motherboard:
                    return new BitmapImage(new Uri("imgs/oa_mother.png", UriKind.Relative));

                case HardwareType.SuperIO:
                    return new BitmapImage(new Uri("imgs/oa_input.png", UriKind.Relative));

                case HardwareType.Cpu:
                    return new BitmapImage(new Uri("imgs/oa_cpu.png", UriKind.Relative));

                case HardwareType.Memory:
                    return new BitmapImage(new Uri("imgs/oa_ram.png", UriKind.Relative));

                case HardwareType.GpuNvidia:
                case HardwareType.GpuAmd:
                case HardwareType.GpuIntel:
                    return new BitmapImage(new Uri("imgs/oa_pc.png", UriKind.Relative));

                case HardwareType.Storage:
                    return new BitmapImage(new Uri("imgs/oa_disc.png", UriKind.Relative));

                case HardwareType.Network:
                    return new BitmapImage(new Uri("imgs/oa_network.png", UriKind.Relative));

                case HardwareType.Cooler:
                    return new BitmapImage(new Uri("imgs/oa_fan.png", UriKind.Relative));

                case HardwareType.EmbeddedController:
                    return new BitmapImage(new Uri("imgs/oa_head2.png", UriKind.Relative));

                case HardwareType.Psu:
                    return new BitmapImage(new Uri("imgs/oa_head.png", UriKind.Relative));

                case HardwareType.Battery:
                    return new BitmapImage(new Uri("imgs/oa_battery.png", UriKind.Relative));

            }
            throw new NotImplementedException();
        }
    }
}
