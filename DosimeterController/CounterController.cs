using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DosimeterController
{
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
            return;
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

        }

        /// <summary>Disable the laser and stop recording the pulse histogram.</summary>
        public void Stop()
        {

        }

        /// <summary>Read the specified histogram channel range.</summary>
        public byte[] ReadHistogram(int minValue, int maxValue)
        {
            return new byte[0];
        }

        /// <summary>Clear the pulse histogram.</summary>
        public void ResetHistogram()
        {

        }

        /// <summary>Set the current printer head position as the zero</summary>
        public void ZeroPositionCounter()
        {

        }

        /// <summary>
        /// Send a gcode string to the counter and wait for an "ok" response. Throws if a different response is recieved.
        /// </summary>
        void SendCheckedCommand(string gcode)
        {
            Log("tx: " + gcode);
            string response;
            try
            {
                port.WriteLine(gcode);
                response = port.ReadLine();
            }
            catch (Exception e)
            {
                throw new CounterException("I/O error: " + e);
            }

            Log("rx: " + response);
            if (response != "ok")
                throw new CounterException("Recieved unexpected response: " + response);
        }

        void Log(string message)
        {
            OnLogMessage(message);
        }
    }
}
