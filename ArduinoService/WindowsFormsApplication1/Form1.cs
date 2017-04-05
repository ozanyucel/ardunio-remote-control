using ArduinoGUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        // Activate an application window.
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        const string PROCESS_VLC = "vlc";
        const string PROCESS_MPC = "mpc-hc";

        const string DATA_PLAY      = "FD08F7";
        const string DATA_PAUSE     = "FD8877";
        const string DATA_FORWARD   = "FD48B7";
        const string DATA_REWIND    = "FD30CF";
        const string DATA_REPEAT    = "FFFFFFFF";

        const string KEY_SPACE      = " ";
        const string KEY_CTRL_RIGHT = "^{RIGHT}";
        const string KEY_CTRL_LEFT  = "^{LEFT}";

        private Dictionary<string, Dictionary<string, string>> _processKeyMap;
        private bool _logsEnabled = false;

        public Form1()
        {
            InitializeComponent();

            initializeService();
        }

        private void initializeService()
        {
            _processKeyMap = new Dictionary<string, Dictionary<string, string>> {
                { PROCESS_VLC, new Dictionary<string, string> { { DATA_PLAY, KEY_SPACE }, { DATA_PAUSE, KEY_SPACE }, { DATA_FORWARD, KEY_CTRL_RIGHT }, { DATA_REWIND, KEY_CTRL_LEFT } } },
                { PROCESS_MPC, new Dictionary<string, string> { { DATA_PLAY, KEY_SPACE }, { DATA_PAUSE, KEY_SPACE }, { DATA_FORWARD, KEY_CTRL_RIGHT }, { DATA_REWIND, KEY_CTRL_LEFT } } }
            };

            timer1.Interval = 10000;
            timer1.Enabled = true;

            if (connectToPort())
                timer1.Enabled = false;
        }


        private bool connectToPort()
        {
            try
            {
                serialPort1.PortName = "COM3";
                serialPort1.BaudRate = 9600;
                serialPort1.DtrEnable = true;
                serialPort1.Open();

                serialPort1.DataReceived += serialPort1_DataReceived;

                addLog("Connected to COM3");

                return true;
            }
            catch (Exception)
            {
                addLog("COM3 is not available!");
                
                return false;
            }

       }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            string line = serialPort1.ReadLine();
            string data = Regex.Replace(line, @"\t|\n|\r", "");
            this.BeginInvoke(new LineReceivedEvent(DataReceived), data);
        }

        private delegate void LineReceivedEvent(string line);

        private void DataReceived(string data)
        {
            switch (data)
            {
                case DATA_PLAY:
                case DATA_PAUSE:
                case DATA_FORWARD:
                case DATA_REWIND:
                    forwardData(data);
                    _lastData = data;
                    break;
                case DATA_REPEAT:
                    forwardLastData();
                    break;
                default:
                    addLog("Unknown: " + data);
                    break;
            }
        }

        private void forwardLastData()
        {
            if (_lastData == DATA_FORWARD || _lastData == DATA_REWIND) {
                   forwardData(_lastData);
            }
        }

        private void forwardData(string data)
        {
            foreach (string process in _processKeyMap.Keys) {

                Dictionary<string, string> dataMap = _processKeyMap[process];

                string key = dataMap[data];
                sendKeyEventToProcess(process, key);

                addLog("Forward key:(" + key + ") of data:(" + data + ") to process:(" + process + ")");
            }
                
        }

        private void sendKeyEventToProcess(string processName, string key)
        {
            Process[] processlist = Process.GetProcessesByName(processName);
            foreach (Process theprocess in processlist) {
                SetForegroundWindow(theprocess.MainWindowHandle);
                SendKeys.SendWait(key);                
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
        }

        private void addLog(string logStr)
        {
            if (_logsEnabled)
            {
                System.Diagnostics.Debug.WriteLine(logStr);
                textBox1.Text += logStr + Environment.NewLine; 
            }
        }

        public string _lastData { get; set; }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (connectToPort())
                timer1.Enabled = false;
        }

        private void cbEnableLogs_CheckedChanged(object sender, EventArgs e)
        {
            _logsEnabled = cbEnableLogs.Checked;
        }

        /*private void setCheckboxText()
        {
            cbEnableLogs.Text = _logsEnabled ? "Logs Enabled" : "Logs Disabled";
        }*/
    }
}
