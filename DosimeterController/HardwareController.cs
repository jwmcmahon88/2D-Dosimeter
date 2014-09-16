using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace DosimeterController
{
    public enum HardwareStatus { Initializing, Idle, Homing, Scanning, Error }

    public delegate void LogMessageHandler(string message);
    public delegate void HardwareStatusChangeDelegate(HardwareStatus status);

    public class HardwareController
    {
        // Public bindings
        public event HardwareStatusChangeDelegate OnHardwareStatusChange = _ => {};
        public event LogMessageHandler OnLogMessage = _ => {};

        PrinterController printer;
        CounterController counter;

        HardwareStatus _status;
        public HardwareStatus Status
        {
            get { return _status; }
            private set { _status = value; OnHardwareStatusChange(_status); }
        }

        Thread asyncControl;

        public void Initialize()
        {
            Status = HardwareStatus.Initializing;

            asyncControl = new Thread(() =>
            {
                try
                {
                    counter = new CounterController("COM5", 250000);
                    counter.OnLogMessage += OnLogMessage;
                }
                catch (Exception)
                {
                    OnLogMessage("Failed to open counter port");
                    Status = HardwareStatus.Error;
                    return;
                }

                try
                {
                    printer = new PrinterController("COM3", 250000);
                    printer.OnLogMessage += OnLogMessage;
                }
                catch (Exception)
                {
                    OnLogMessage("Failed to open printer port");
                    Status = HardwareStatus.Error;
                    return;
                }

                Status = HardwareStatus.Idle;
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
                try
                {

                    OnLogMessage("Moving to start position.");
                    Status = HardwareStatus.Homing;

                    printer.MoveToPosition(100, 100, 0, 2000);
                    counter.ZeroPositionCounter();

                    Status = HardwareStatus.Scanning;
                    OnLogMessage("Starting scan.");

                    // Scan rows
                    for (var i = 0; i < 10; i++)
                    {
                        counter.Start();
                        printer.MoveDeltaX(30 * (i % 2 == 1 ? -1 : 1), 200);
                        counter.Stop();

                        // Read and save data to file

                        printer.MoveDeltaY(1, 200);
                    }

                    OnLogMessage("Scan complete.");
                    Status = HardwareStatus.Homing;
                    printer.MoveToHome();

                    Status = HardwareStatus.Idle;
                }
                catch (PrinterException e)
                {
                    OnLogMessage("Printer error: " + e.Message);
                    Status = HardwareStatus.Error;
                    // Turn off laser
                }
                catch (CounterException e)
                {
                    OnLogMessage("Counter error: " + e.Message);
                    Status = HardwareStatus.Error;
                }
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
