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
            controller.OnHardwareStatusChange += new HardwareStatusChangeDelegate(s => status.Invoke((Action)(() => UpdateStatusLabel(status, s))));
            controller.Initialize();
        }

        void UpdateStatusLabel(Label l, HardwareStatus s)
        {
            l.Text = s.ToString();

            switch (s)
            {
                case HardwareStatus.Initializing: l.ForeColor = Color.Gray; break;
                case HardwareStatus.Error: l.ForeColor = Color.Red; break;
                case HardwareStatus.Idle: l.ForeColor = Color.Black; break;
                case HardwareStatus.Homing: l.ForeColor = Color.Yellow; break;
                case HardwareStatus.Scanning: l.ForeColor = Color.Green; break;
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
