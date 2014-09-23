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
        readonly Configuration configuration = new Configuration();

        public Form1()
        {
            InitializeComponent();
        }

        void Form1_Load(object sender, EventArgs e)
        {
            // The controller may call these events from a separate thread,
            // so they need to be marshalled back onto the UI thread
            controller.OnLogMessage += new LogMessageHandler(s => logText.Invoke((Action<string>)(ss => logText.AppendText(ss+"\n")), s));
            controller.OnHardwareStatusChange += new HardwareStatusChangeDelegate(s => status.Invoke((Action)(() => UpdateStatus(s))));
            controller.Initialize();

            propertyGrid.SelectedObject = configuration;
        }

        void UpdateStatus(HardwareStatus s)
        {
            status.Text = s.ToString();

            switch (s)
            {
                case HardwareStatus.Initializing:
                    propertyGrid.Enabled = true;
                    loadButton.Enabled = true;
                    startButton.Enabled = false;
                    startButton.Visible = true;
                    reinitializeButton.Enabled = false;
                    reinitializeButton.Visible = false;
                    status.ForeColor = Color.Gray;
                    break;
                case HardwareStatus.Error:
                    propertyGrid.Enabled = true;
                    loadButton.Enabled = true;
                    startButton.Enabled = false;
                    startButton.Visible = false;
                    reinitializeButton.Enabled = true;
                    reinitializeButton.Visible = true;
                    status.ForeColor = Color.Red;
                    break;
                case HardwareStatus.Idle:
                    propertyGrid.Enabled = true;
                    loadButton.Enabled = true;
                    startButton.Enabled = true;
                    startButton.Visible = true;
                    reinitializeButton.Enabled = false;
                    reinitializeButton.Visible = false;
                    status.ForeColor = Color.Black;
                    break;
                case HardwareStatus.Homing:
                    propertyGrid.Enabled = false;
                    loadButton.Enabled = false;
                    startButton.Enabled = false;
                    startButton.Visible = true;
                    reinitializeButton.Enabled = false;
                    reinitializeButton.Visible = false;
                    status.ForeColor = Color.Blue;
                    break;
                case HardwareStatus.Scanning:
                    propertyGrid.Enabled = false;
                    loadButton.Enabled = false;
                    startButton.Enabled = false;
                    startButton.Visible = true;
                    reinitializeButton.Enabled = false;
                    reinitializeButton.Visible = false;
                    status.ForeColor = Color.Green; break;
            }
        }

        void StartButtonClicked(object sender, EventArgs e)
        {
            logText.AppendText("Clicked action button\n");

            if (controller.Status != HardwareStatus.Idle)
            {
                logText.AppendText("Error: Start button was pressed when status was " + controller.Status.ToString());
                return;
            }

            controller.StartScan(configuration);
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            controller.Shutdown();
        }

        private void LoadButtonClicked(object sender, EventArgs e)
        {

        }

        private void ReinitializeButtonClicked(object sender, EventArgs e)
        {
            controller.Initialize();
        }
    }
}
