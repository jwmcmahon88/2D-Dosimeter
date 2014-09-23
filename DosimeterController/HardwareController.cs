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

        public void StartScan(Configuration config)
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
                    // Add an additional overscan on each end
                    var overscan = 1m; // in mm
                    var xStepsPermm = 32;

                    var startColumn = (int)((config.Origin.X - overscan) * xStepsPermm);
                    var endColumn = (int)((config.Origin.X + config.Size.Width + overscan) * xStepsPermm);

                    OnLogMessage(string.Format("Counter columns {0} to {1}", startColumn, endColumn));
                    var rows = (int)Math.Ceiling(config.Size.Height / config.RowStride);
                    var columns = endColumn - startColumn + 1;

                    using (var fits = new MiniFits(config.DataFile, columns, rows, true))
                    {
                        fits.UpdateKey("OPERATOR", config.Operator, "T");
                        fits.UpdateKey("DATETIME", DateTime.Now.ToString("G"), "Time at the start of the scan");
                        fits.UpdateKey("TEST", "this is a string that has more than 68 characters. this is a string that has more than 68 characters. this is a string that has more than 68 characters. this is a string that has more than 68 characters.", "test");
                        fits.UpdateKey("SCANAREA", string.Format("{0:F2} {1:F2} {2:F2} {3:F2}", config.Origin.X, config.Origin.Y, config.Size.Width, config.Size.Height), "The requested scan area (X Y W H in mm)");
                        fits.UpdateKey("ROWSTART", startColumn, "The step count of the first data column");
                        fits.UpdateKey("ROWSTRID", config.RowStride, "The step size between rows (in mm)");
                        fits.UpdateKey("ROWSPEED", config.RowSpeed, "The row scan speed (in mm/minute)");

                        var data = new ushort[rows * columns];

                        OnLogMessage("Moving to start position.");
                        Status = HardwareStatus.Homing;

                        printer.MoveToHome();
                        counter.ZeroPositionCounter();

                        printer.MoveToPosition(config.Origin.X - overscan, config.Origin.Y, config.FocusHeight, 2000);

                        Status = HardwareStatus.Scanning;
                        OnLogMessage("Starting scan.");

                        // Scan rows
                        for (var i = 0; i < rows; i++)
                        {
                            Thread.Sleep(10);
                            counter.Start();
                            Thread.Sleep(10);

                            printer.MoveDeltaX((config.Size.Width + 2 * overscan) * (i % 2 == 1 ? -1 : 1), config.RowSpeed);
                            Thread.Sleep(10);
                            counter.Stop();
                            Thread.Sleep(10);

                            // Read and save data to file
                            var primary = counter.ReadHistogram(CounterChannel.Primary, startColumn, endColumn);

                            fits.SetImageRow(i, primary);
                            counter.ResetHistogram();
                            printer.MoveDeltaY(config.RowStride, config.RowSpeed);
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

                    // Send emergency stop (M112)
                }
                catch (MiniFitsException e)
                {
                    OnLogMessage("File error: " + e.Message);
                    Status = HardwareStatus.Error;
                    // Turn off laser and home printer
                    // Send emergency stop (M112)
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
