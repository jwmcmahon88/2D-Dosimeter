using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DosimeterController
{
    public class PrinterException : Exception
    {
        public PrinterException() { }
        public PrinterException(string message) : base(message) { }
        public PrinterException(string message, Exception inner) : base(message, inner) { }
    }

    class PrinterController : IDisposable
    {
        public event LogMessageHandler OnLogMessage = _ => { };

        readonly SerialPort port;

        public PrinterController(string portName, int baud)
        {
            port = new SerialPort(portName, baud);
            try
            {
                port.Open();

                // Wait for the printer to become responsive
                Thread.Sleep(1000);

                SendCheckedCommand("G91"); // Set relative positioning
                SendCheckedCommand("M204 S0 T0"); // Disable acceleration
                SendCheckedCommand("M84 S5"); // Disable idle motor hold after 5 seconds of inactivity

                MoveToHome();
            }
            catch (Exception e)
            {
                throw new PrinterException("Unable to initialize printer", e);
            }
        }

        public void Dispose()
        {
            port.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>Move to the home position. Blocks until the move is complete.</summary>
        public void MoveToHome()
        {
            // Need to move up in Z and home separately to prevent jamming
            // MoveDeltaZ(1, 50);
            // SendCheckedCommand("G28 Z0");
            SendCheckedCommand("G28 X0 Y0");
        }

        /// <summary>Move to an absolute position (in mm, mm/s). Blocks until the move is complete.</summary>
        public void MoveToPosition(decimal dx, decimal dy, decimal dz, decimal velocity)
        {
            // Set absolute positioning
            SendCheckedCommand("G90");

            // Move
            SendCheckedCommand(string.Format("G1 X{0:F2} Y{1:F2} Z{2:F2} F{3:F2}", dx, dy, dz, velocity));

            // Switch back to relative positioning
            SendCheckedCommand("G91");

            // Wait for move to complete
            SendCheckedCommand("M400");
        }

        /// <summary>Move a relative distance in the X direction (in mm, mm/s). Blocks until the move is complete.</summary>
        public void MoveDeltaX(decimal dx, decimal vx)
        {
            SendCheckedCommand(string.Format("G1 X{0:F2} F{1:F2}", dx, vx));

            // Wait for move to complete
            SendCheckedCommand("M400");
        }

        /// <summary>Move a relative distance in the Y direction (in mm, mm/s). Blocks until the move is complete.</summary>
        public void MoveDeltaY(decimal dy, decimal vy)
        {
            SendCheckedCommand(string.Format("G1 Y{0:F2} F{1:F2}", dy, vy));

            // Wait for move to complete
            SendCheckedCommand("M400");
        }

        /// <summary>Move a relative distance in the Z direction (in mm, mm/s). Blocks until the move is complete.</summary>
        public void MoveDeltaZ(decimal dz, decimal vz)
        {
            SendCheckedCommand(string.Format("G1 Z{0:F2} F{1:F2}", dz, vz));

            // Wait for move to complete
            SendCheckedCommand("M400");
        }

        /// <summary>Send a gcode string to the printer and wait for an "ok" response. Throws if a different response is recieved.</summary>
        void SendCheckedCommand(string gcode)
        {
            string response;
            try
            {
                port.WriteLine(gcode);
                response = port.ReadLine();
            }
            catch (Exception e)
            {
                throw new PrinterException("I/O error: " + e);
            }

            if (response != "ok")
                throw new PrinterException("Recieved unexpected response: " + response);
        }

        /// <summary>Write a message to the debug log.</summary>
        void Log(string message)
        {
            OnLogMessage(message);
        }
    }
}
