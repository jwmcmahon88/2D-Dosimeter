using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace DosimeterController
{
    public enum PrinterStatus { Initializing, Idle, Moving, Error };
    public enum LaserStatus { Initializing, Disabled, Enabled };
    public enum CounterStatus { Initializing, Disabled, Enabled };

    public delegate void LogMessageHandler(string message);
    public delegate void PrinterStatusChangeDelegate(PrinterStatus status);
    public delegate void LaserStatusChangeDelegate(LaserStatus status);
    public delegate void CounterStatusChangeDelegate(CounterStatus status);

    public class HardwareController
    {
        // Public bindings
        public event PrinterStatusChangeDelegate OnPrinterStatusChange = _ => {};
        public event LaserStatusChangeDelegate OnLaserStatusChange = _ => { };
        public event CounterStatusChangeDelegate OnCounterStatusChange = _ => { };
        public event LogMessageHandler OnLogMessage = _ => {};

        PrinterStatus _printerStatus;
        public PrinterStatus PrinterStatus
        {
            get { return _printerStatus; }
            private set { _printerStatus = value; OnPrinterStatusChange(_printerStatus); }
        }

        LaserStatus _laserStatus;
        public LaserStatus LaserStatus
        {
            get { return _laserStatus; }
            private set { _laserStatus = value; OnLaserStatusChange(_laserStatus); }
        }

        CounterStatus _counterStatus;
        public CounterStatus CounterStatus
        {
            get { return _counterStatus; }
            private set { _counterStatus = value; OnCounterStatusChange(_counterStatus); }
        }

        Thread asyncControl;

        public void Initialize()
        {
            PrinterStatus = PrinterStatus.Initializing;
            LaserStatus = LaserStatus.Initializing;
            CounterStatus = CounterStatus.Initializing;

            asyncControl = new Thread(() =>
            {
                Thread.Sleep(10000);
                PrinterStatus = PrinterStatus.Idle;
                LaserStatus = LaserStatus.Disabled;
                CounterStatus = CounterStatus.Disabled;
            });

            asyncControl.Start();
        }

        public void StartScan(float x1, float x2, float y1, float y2, float speed)
        {
            if (asyncControl.IsAlive)
            {
                OnLogMessage("Invalid thread state. Aborting scan.");
                return;
            }

            asyncControl = new Thread(() =>
            {
                OnLogMessage("Moving to start position.");
                PrinterStatus = PrinterStatus.Moving;
                Thread.Sleep(5000);
                PrinterStatus = PrinterStatus.Idle;
                OnLogMessage("Starting scan.");
                PrinterStatus = PrinterStatus.Moving;
                LaserStatus = LaserStatus.Enabled;
                CounterStatus = CounterStatus.Enabled;
                Thread.Sleep(5000);
                OnLogMessage("Scan complete.");
                PrinterStatus = PrinterStatus.Idle;
                LaserStatus = LaserStatus.Disabled;
                CounterStatus = CounterStatus.Disabled;
            });

            asyncControl.Start();
        }

        public void Shutdown()
        {
            asyncControl.Abort();

            // TODO: Send homing command
            // TODO: Disable laser and counter
        }
    }
}
