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

namespace ArduinoTray
{
    public partial class ArduinoService
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        const string PROCESS_VLC    = "vlc";
        const string PROCESS_MPC    = "mpc-hc";
        const string PROCESS_CHR    = "chrome";

        const string DATA_PLAY      = "FD08F7";
        const string DATA_PAUSE     = "FD8877";
        const string DATA_FORWARD   = "FD48B7";
        const string DATA_REWIND    = "FD30CF";

        const string DATA_PREV      = "FD28D7";        const string DATA_REC       = "FDA857";        const string DATA_STOP      = "FD6897";
        const string DATA_NEXT      = "FD18E7";
        const string DATA_REPEAT    = "FFFFFFFF";

        const string KEY_SPACE      = " ";
        const string KEY_RIGHT      = "{RIGHT}";
        const string KEY_LEFT       = "{LEFT}";
        const string KEY_CTRL       = "^";
        const string KEY_F11        = "{F11}";
        const string KEY_F          = "f";
        const string KEY_ESC        = "{ESC}";


        private Dictionary<string, Dictionary<string, string>> _processKeyMap;
        private bool _logsEnabled = false;

        private Timer timer1;
        private SerialPort serialPort1;

        public ArduinoService()
        {
            initializeService();
        }

        private void initializeService()
        {
            _processKeyMap = new Dictionary<string, Dictionary<string, string>> {
                { PROCESS_MPC, new Dictionary<string, string> { { DATA_PLAY, KEY_SPACE },               { DATA_PAUSE, KEY_SPACE },      { DATA_FORWARD, KEY_CTRL + KEY_RIGHT }, 
                                                                { DATA_REWIND, KEY_CTRL + KEY_LEFT },   { DATA_REC, KEY_F11 },          { DATA_STOP, KEY_F11 } } },

                { PROCESS_VLC, new Dictionary<string, string> { { DATA_PLAY, KEY_SPACE },               { DATA_PAUSE, KEY_SPACE },      { DATA_FORWARD, KEY_CTRL + KEY_RIGHT }, 
                                                                { DATA_REWIND, KEY_CTRL + KEY_LEFT },   { DATA_REC, KEY_F },            { DATA_STOP, KEY_F } } },

                { PROCESS_CHR, new Dictionary<string, string> { { DATA_PLAY, KEY_SPACE },               { DATA_PAUSE, KEY_SPACE },      { DATA_FORWARD, KEY_RIGHT }, 
                                                                { DATA_REWIND, KEY_LEFT },              { DATA_REC, KEY_F },            { DATA_STOP, KEY_ESC } } }
            };

            serialPort1 = new SerialPort();

            timer1 = new Timer();
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

            LineReceivedEvent eventFunc = new LineReceivedEvent(DataReceived);
            eventFunc(data);
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
                case DATA_REC:
                case DATA_STOP:
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

                if (dataMap.ContainsKey(data)) {
                    string key = dataMap[data];
                    if (sendKeyEventToProcess(process, key)) {
                        addLog("Forwarded key:(" + key + ") of data:(" + data + ") to process:(" + process + ")");
                        return;
                    }
                }
            }         
        }

        private bool sendKeyEventToProcess(string processName, string key)
        {
            bool sent = false;
            IntPtr activeWin;
            
            Process[] processlist = Process.GetProcessesByName(processName);
            foreach (Process theprocess in processlist) {
                activeWin = GetForegroundWindow();
                if (activeWin != theprocess.MainWindowHandle)
                    SetForegroundWindow(theprocess.MainWindowHandle);
                SendKeys.SendWait(key);
                sent = true;
            }

            return sent;
        }

        private void addLog(string logStr)
        {
            if (_logsEnabled)
            {
                System.Diagnostics.Debug.WriteLine(logStr);
            }
        }

        public string _lastData { get; set; }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (connectToPort())
                timer1.Enabled = false;
        }
    }
}
