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
                var printerPort = "COM3";
                var counterPort = "COM7";
                try
                {
                    counter = new CounterController(counterPort, 250000);
                    counter.OnLogMessage += OnLogMessage;
                }
                catch (Exception)
                {
                    OnLogMessage("Failed to open counter on " + counterPort);
                    Status = HardwareStatus.Error;
                    return;
                }

                try
                {
                    printer = new PrinterController(printerPort, 250000);
                    printer.OnLogMessage += OnLogMessage;
                }
                catch (Exception)
                {
                    OnLogMessage("Failed to open printer on " + printerPort);
                    Status = HardwareStatus.Error;
                    return;
                }

                Status = HardwareStatus.Idle;
            });

            asyncControl.Start();
        }

        public void StartScan(decimal x, decimal y, decimal z, decimal width, decimal height, decimal rowHeight, decimal velocity)
        {
            if (asyncControl.IsAlive)
            {
                OnLogMessage("Invalid thread state. Aborting scan.");
                return;
            }

            if (Status == HardwareStatus.Error)
            {
                OnLogMessage("Hardware status is error. Aborting scan.");
                return;
            }

            asyncControl = new Thread(() =>
            {
                try
                {
                    var startColumn = 100;
                    var endColumn = 300;

                    var dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "scan.fits");
                    var rows = (int)Math.Ceiling(height / rowHeight);
                    var columns = endColumn - startColumn + 1;

                    using (var fits = new MiniFits(dataPath, columns, rows, true))
                    {
                        fits.UpdateKey("TESTKEY", "TESTVALUE", "TESTCOMMENT");
                        var data = new ushort[rows * columns];

                        OnLogMessage("Moving to start position.");
                        Status = HardwareStatus.Homing;

                        printer.MoveToPosition(x, y, z, 2000);
                        counter.ZeroPositionCounter();

                        Status = HardwareStatus.Scanning;
                        OnLogMessage("Starting scan.");

                        // Scan rows
                        for (var i = 0; i < rows; i++)
                        {
                            counter.Start();
                            printer.MoveDeltaX(width * (i % 2 == 1 ? -1 : 1), velocity);
                            counter.Stop();

                            // Read and save data to file
                            var primary = counter.ReadHistogram(CounterChannel.Primary, startColumn, endColumn);
                            OnLogMessage(string.Join(", ", primary));

                            fits.SetImageRow(i, primary);
                            counter.ResetHistogram();
                            printer.MoveDeltaY(rowHeight, velocity);
                        }

                        OnLogMessage("Scan complete.");
                        Status = HardwareStatus.Homing;
                        printer.MoveToHome();

                        Status = HardwareStatus.Idle;
                    }
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
                catch (MiniFitsException e)
                {
                    OnLogMessage("File error: " + e.Message);
                    Status = HardwareStatus.Error;
                    // Turn off laser and home printer
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
