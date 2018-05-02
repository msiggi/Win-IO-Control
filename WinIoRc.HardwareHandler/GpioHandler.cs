using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace WinIoRc.HardwareHandler
{
    public static class GpioHandler
    {
        public static EventHandler<ErrorEventArgs> ErrorOccured;

        private static GpioController gpioController;
        private static List<GpioPin> UsedPins { get; set; } = new List<GpioPin>();

        public static void Init()
        {
            gpioController = GpioController.GetDefault();
        }

        public static void Set(int pinNumber, bool state)
        {
            try
            {
                GpioPin pin = null;

                if (!UsedPins.Any(p=>p.PinNumber == pinNumber))
                {
                    pin = gpioController.OpenPin(pinNumber);
                    UsedPins.Add(pin);
                }
                else
                {
                    pin = UsedPins.First(p => p.PinNumber == pinNumber);
                }

                
                if (pin.GetDriveMode() != GpioPinDriveMode.Output)
                {
                    pin.SetDriveMode(GpioPinDriveMode.Output);
                }

                pin.Write(state ? GpioPinValue.High : GpioPinValue.Low);
            }
            catch (Exception ex)
            {
                ErrorOccured?.Invoke(null, new ErrorEventArgs
                {
                    Exception = ex,
                    MessageText = "Error at SetGpio"
                });

            }
        }
        public static bool Toggle(int pinNumber)
        {
            try
            {
                GpioPin pin = null;

                if (!UsedPins.Any(p => p.PinNumber == pinNumber))
                {
                    pin = gpioController.OpenPin(pinNumber);
                    UsedPins.Add(pin);
                }
                else
                {
                    pin = UsedPins.First(p => p.PinNumber == pinNumber);
                }


                if (pin.GetDriveMode() != GpioPinDriveMode.Output)
                {
                    pin.SetDriveMode(GpioPinDriveMode.Output);
                }

                if (pin.Read() == GpioPinValue.High)
                {
                    pin.Write(GpioPinValue.Low);
                    return false;
                }
                else
                {
                    pin.Write(GpioPinValue.High);
                    return true;
                }

            }
            catch (Exception ex)
            {
                ErrorOccured?.Invoke(null, new ErrorEventArgs
                {
                    Exception = ex,
                    MessageText = "Error at ToggleGpio"
                });
                return false;
            }
        }

        public static bool Get(GpioPin pin)
        {
            var val = pin.Read();

            return (val == GpioPinValue.High) ? true : false;
        }

    }
}
