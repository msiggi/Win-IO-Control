using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace WinIoRc.GpioHandler
{
    public static class GpioHandler
    {


        public static void Set(GpioPin pin, bool state)
        {
            pin.Write(GpioPinValue.High);
        }

        public static bool Get(GpioPin pin)
        {
            var val = pin.Read();

            return (val == GpioPinValue.High) ? true : false;
        }
    }
}
