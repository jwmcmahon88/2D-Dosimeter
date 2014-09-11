using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DosimeterController
{
    public partial class Form1 : Form
    {
        readonly HardwareController controller = new HardwareController();

        public Form1()
        {
            InitializeComponent();
        }

        void Form1_Load(object sender, EventArgs e)
        {
            // Bind update events to the UI
            controller.OnLogMessage += new LogMessageHandler(s => logText.Invoke((Action<string>)(ss => logText.AppendText(ss+"\n")), s));
            controller.OnPrinterStatusChange += new PrinterStatusChangeDelegate(s => statusPrinter.Invoke((Action)(() => UpdatePrinterLabel(statusPrinter, s))));
            controller.OnLaserStatusChange += new LaserStatusChangeDelegate(s => statusLaser.Invoke((Action)(() => UpdateLaserLabel(statusLaser, s))));
            controller.OnCounterStatusChange += new CounterStatusChangeDelegate(s => statusCounter.Invoke((Action)(() => UpdateCounterLabel(statusCounter, s))));

            controller.Initialize();
        }

        void UpdatePrinterLabel(Label l, PrinterStatus s)
        {
            l.Text = s.ToString();

            switch (s)
            {
                case PrinterStatus.Initializing: l.ForeColor = Color.Gray; break;
                case PrinterStatus.Error: l.ForeColor = Color.Red; break;
                case PrinterStatus.Idle: l.ForeColor = Color.Black; break;
                case PrinterStatus.Moving: l.ForeColor = Color.Green; break;
            }
        }

        void UpdateLaserLabel(Label l, LaserStatus s)
        {
            l.Text = s.ToString();

            switch (s)
            {
                case LaserStatus.Initializing: l.ForeColor = Color.Gray; break;
                case LaserStatus.Disabled: l.ForeColor = Color.Black; break;
                case LaserStatus.Enabled: l.ForeColor = Color.Green; break;
            }
        }

        void UpdateCounterLabel(Label l, CounterStatus s)
        {
            l.Text = s.ToString();

            switch (s)
            {
                case CounterStatus.Initializing: l.ForeColor = Color.Gray; break;
                case CounterStatus.Disabled: l.ForeColor = Color.Black; break;
                case CounterStatus.Enabled: l.ForeColor = Color.Green; break;
            }
        }

        void StartButtonClicked(object sender, EventArgs e)
        {
            logText.AppendText("Clicked start button\n");
            controller.StartScan(0, 0, 0, 0, 0);
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            controller.Shutdown();
        }
    }
}
