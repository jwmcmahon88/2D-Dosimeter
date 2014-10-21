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
    public delegate void HardwareStatusChangeDelegate(HardwareStatus status, decimal scanPercentage);

    public class HardwareController : IDisposable
    {
        // Public bindings
        public event HardwareStatusChangeDelegate OnHardwareStatusChange = (a, b) => {};
        public event LogMessageHandler OnLogMessage = _ => {};
        public HardwareStatus Status { get; private set; }

        PrinterController printer;
        CounterController counter;

        bool cancelScan;

        void UpdateStatus(HardwareStatus status, decimal scanPercentage = 0)
        {
            Status = status;
            OnHardwareStatusChange(status, scanPercentage);
        }

        Thread asyncControl;

        public void Initialize()
        {
            UpdateStatus(HardwareStatus.Initializing);

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
                    UpdateStatus(HardwareStatus.Error);
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
                    UpdateStatus(HardwareStatus.Error);
                    CloseSerialConnections();

                    return;
                }

                UpdateStatus(HardwareStatus.Idle);
            });

            asyncControl.Start();
        }

        public void Dispose()
        {
            CloseSerialConnections();
            GC.SuppressFinalize(this);
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

            cancelScan = false;

            asyncControl = new Thread(() =>
            {
                try
                {
                    var startColumn = (int)((config.Origin.X - config.RowOverscan) * config.XStepsPerMM);
                    var endColumn = (int)((config.Origin.X + config.Size.Width + config.RowOverscan) * config.XStepsPerMM);

                    OnLogMessage(string.Format("Counter columns {0} to {1}", startColumn, endColumn));
                    var rows = (int)Math.Ceiling(config.Size.Height / config.RowStride);
                    var columns = endColumn - startColumn + 1;

                    var dimensions = new[] { columns, rows, 2 };
                    using (var fits = new MiniFits(config.DataFile, dimensions, MiniFitsType.U16, true))
                    {
                        fits.WriteKey("OPERATOR", config.Operator, null);
                        fits.WriteKey("DATETIME", DateTime.Now.ToString("G"), "Time at the start of the scan");
                        fits.WriteKey("DESCRIPT", config.Description, null);
                        fits.WriteKey("SCANAREA", string.Format("{0:F3} {1:F3} {2:F3} {3:F3}", config.Origin.X, config.Origin.Y, config.Size.Width, config.Size.Height), "The requested scan area (X Y W H in mm)");
                        fits.WriteKey("ROWSTART", startColumn, "The step count of the first data column");
                        fits.WriteKey("ROWSTRID", config.RowStride, 6, "The step size between rows (in mm)");
                        fits.WriteKey("ROWSPEED", config.RowSpeed, 4, "The row scan speed (in mm/minute)");
                        fits.WriteKey("COLSTRID", config.ColumnStride, 6, "The step size between columns (in mm)");
                        fits.WriteKey("COLSPEED", config.ColumnSpeed, 4, "The row-change speed (in mm/minute)");

                        var data = new ushort[rows * columns * 2];

                        OnLogMessage("Moving to start position.");
                        UpdateStatus(HardwareStatus.Homing);

                        printer.MoveToHome();
                        counter.ZeroPositionCounter();

                        printer.MoveToPosition(0, config.Origin.Y, config.FocusHeight, 2000);
                        printer.MoveToPosition(config.Origin.X - config.RowOverscan, config.Origin.Y, config.FocusHeight, 2000);

                        UpdateStatus(HardwareStatus.Scanning);
                        OnLogMessage("Starting scan.");

                        // Scan rows
                        for (var i = 0; i < rows; i++)
                        {
                            if (cancelScan)
                                break;

                            OnLogMessage(string.Format("Scanning row {0} of {1} ({2:F0}%)", i + 1, rows, i * 100 / rows));
                            counter.Start();
                            printer.MoveDeltaX((config.Size.Width + 2 * config.RowOverscan) * (i % 2 == 1 ? -1 : 1), config.RowSpeed);
                            counter.Stop();

                            // Read and save data to file
                            var primary = counter.ReadHistogram(CounterChannel.Primary, startColumn, endColumn);
                            var secondary = counter.ReadHistogram(CounterChannel.Secondary, startColumn, endColumn);

                            Array.Copy(primary, 0, data, i * columns, columns);
                            Array.Copy(secondary, 0, data, rows * columns + i * columns, columns);

                            OnLogMessage(string.Join(" ", primary));

                            fits.WriteImageData(data);

                            counter.ResetHistogram();
                            UpdateStatus(HardwareStatus.Scanning, i * 100m / rows);
                            printer.MoveDeltaY(config.RowStride, config.ColumnSpeed);
                        }

                        OnLogMessage("Scan complete.");
                        UpdateStatus(HardwareStatus.Homing);
                        printer.MoveToHome();

                        UpdateStatus(HardwareStatus.Idle);
                    }
                }
                catch (PrinterException e)
                {
                    OnLogMessage("Printer error: " + e.Message);
                    UpdateStatus(HardwareStatus.Error);
                    CloseSerialConnections();
                }
                catch (CounterException e)
                {
                    OnLogMessage("Counter error: " + e.Message);
                    UpdateStatus(HardwareStatus.Error);

                    // TODO: Send emergency stop (M112)
                    CloseSerialConnections();
                }
                catch (MiniFitsException e)
                {
                    OnLogMessage("File error: " + e.Message);
                    UpdateStatus(HardwareStatus.Error);

                    // TODO: Turn off laser and home printer
                    // TODO: Send emergency stop (M112)
                    CloseSerialConnections();
                }
            });

            asyncControl.Start();
        }

        void CloseSerialConnections()
        {
            printer.Dispose();
            printer = null;

            counter.Dispose();
            counter = null;
        }

        public void CancelScan()
        {
            cancelScan = true;
        }
    }
}
