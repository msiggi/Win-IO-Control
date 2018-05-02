using System;

namespace WinIoRc.HardwareHandler
{
    public class ErrorEventArgs : EventArgs
    {
        public string MessageText { get; set; }
        public Exception Exception { get; set; }
    }
}