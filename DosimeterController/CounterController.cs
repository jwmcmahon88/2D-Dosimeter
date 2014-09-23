using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DosimeterController
{
    public enum CounterChannel { Primary = 0, Secondary = 1 }

    public class CounterException : Exception
    {
        public CounterException() { }
        public CounterException(string message) : base(message) { }
        public CounterException(string message, Exception inner) : base(message, inner) { }
    }

    class CounterController
    {
        public event LogMessageHandler OnLogMessage = _ => { };

        readonly SerialPort port;

        public CounterController(string portName, int baud)
        {
            port = new SerialPort(portName, baud);

            try
            {
                port.Open();

                // Wait for the printer to become responsive
                Thread.Sleep(1000);
            }
            catch (Exception e)
            {
                throw new CounterException("Unable to initialize counter", e);
            }
        }

        /// <summary>Enable the laser and start recording the pulse histogram.</summary>
        public void Start()
        {
            ClearReadBuffer();
            SendCheckedCommand("M1003");
        }

        /// <summary>Disable the laser and stop recording the pulse histogram.</summary>
        public void Stop()
        {
            ClearReadBuffer();
            SendCheckedCommand("M1004");
        }

        /// <summary>Read the specified histogram channel range.</summary>
        public ushort[] ReadHistogram(CounterChannel channel, int minValue, int maxValue)
        {
            ClearReadBuffer();
            SendCheckedCommand(string.Format("M1005 {0} {1} {2}", (int)channel, minValue, maxValue));

            var data = ReadResponse();

            var final = ReadResponse();
            if (final != "ok")
                throw new CounterException("Recieved unexpected response: " + final);

            try
            {
                return data.Split(' ')
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Select(s => ushort.Parse(s))
                    .ToArray();
            }
            catch (Exception e)
            {
                throw new CounterException("Recieved unexpected data: " + data + "\n\n" + e.Message);
            }
        }

        /// <summary>Clear the pulse histogram.</summary>
        public void ResetHistogram()
        {
            ClearReadBuffer();
            SendCheckedCommand("M1006");
        }

        public uint ReadPositionCounter()
        {
            ClearReadBuffer();
            SendCheckedCommand("M1001");

            uint position;
            var response = ReadResponse();
            if (!uint.TryParse(response, out position))
                throw new CounterException("Recieved unexpected response: " + response);

            return position;
        }

        /// <summary>Set the current printer head position as the zero</summary>
        public void ZeroPositionCounter()
        {
            ClearReadBuffer();
            SendCheckedCommand("M1002");
        }

        /// <summary>
        /// Send a gcode string to the counter and wait for an "ok" response. Throws if a different response is recieved.
        /// </summary>
        void SendCheckedCommand(string command)
        {
            SendCommand(command);
            var response = ReadResponse();

            if (response != "ok")
                throw new CounterException("Recieved unexpected response: '" + response + "'");
        }

        /// <summary>
        /// Send a command to the counter
        /// </summary>
        void SendCommand(string command)
        {
            try
            {
                port.WriteLine(command);
            }
            catch (Exception e)
            {
                throw new CounterException("I/O error: " + e);
            }
        }

        /// <summary>
        /// Read a response from the counter
        /// </summary>
        string ReadResponse()
        {
            string response;
            try
            {
                response = port.ReadLine();
            }
            catch (Exception e)
            {
                throw new CounterException("I/O error: " + e);
            }

            return response;
        }

        void ClearReadBuffer()
        {
            try
            {
                port.ReadExisting();
            }
            catch (Exception e)
            {
                throw new CounterException("I/O error: " + e);
            }
        }

        void Log(string message)
        {
            OnLogMessage(message);
        }
    }
}
