// ██████╗░███████╗██████╗░░░░░░░██╗░░██╗  ░░░░░██╗██╗████████╗██╗░░░██╗██╗░░██╗
// ██╔══██╗██╔════╝██╔══██╗░░░░░░╚██╗██╔╝  ░░░░░██║██║╚══██╔══╝██║░░░██║╚██╗██╔╝
// ██████╔╝█████╗░░██║░░██║█████╗░╚███╔╝░  ░░░░░██║██║░░░██║░░░██║░░░██║░╚███╔╝░
// ██╔══██╗██╔══╝░░██║░░██║╚════╝░██╔██╗░  ██╗░░██║██║░░░██║░░░██║░░░██║░██╔██╗░
// ██║░░██║███████╗██████╔╝░░░░░░██╔╝╚██╗  ╚█████╔╝██║░░░██║░░░╚██████╔╝██╔╝╚██╗
// ╚═╝░░╚═╝╚══════╝╚═════╝░░░░░░░╚═╝░░╚═╝  ░╚════╝░╚═╝░░░╚═╝░░░░╚═════╝░╚═╝░░╚═╝

// All necessary imports for Forms, Memory Handling, DLL Injection, etc.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace RED_X_CLOUD_CONTROL_V2
{
    public partial class Form1 : Form
    {
        // Local web server for mobile remote control
        private HttpListener httpListener;
        private Thread listenerThread;

        // Constants for Windows API interaction
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 256;

        // Hook variables for listening to keyboard keys
        private static IntPtr hookID = IntPtr.Zero;
        private static IntPtr hookID1 = IntPtr.Zero;
        private static IntPtr hookID2 = IntPtr.Zero;
        private static IntPtr hookID3 = IntPtr.Zero;
        private static IntPtr hookID4 = IntPtr.Zero;
        private static IntPtr hookID5 = IntPtr.Zero;
        private static IntPtr hookID6 = IntPtr.Zero;
        private static IntPtr hookID7 = IntPtr.Zero;
        private static IntPtr hookID8 = IntPtr.Zero;
        private static IntPtr hookID9 = IntPtr.Zero;

        // Delegates for multiple key hook callbacks
        private Form1.LowLevelKeyboardProc hookCallback;
        private Form1.LowLevelKeyboardProc hookCallback1;
        private Form1.LowLevelKeyboardProc hookCallback2;
        private Form1.LowLevelKeyboardProc hookCallback3;
        private Form1.LowLevelKeyboardProc hookCallback4;
        private Form1.LowLevelKeyboardProc hookCallback5;
        private Form1.LowLevelKeyboardProc hookCallback6;
        private Form1.LowLevelKeyboardProc hookCallback7;
        private Form1.LowLevelKeyboardProc hookCallback8;
        private Form1.LowLevelKeyboardProc hookCallback9;

        // Keybind listener flags
        private bool waitPressKey;
        private bool waitPressKey1;
        private bool waitPressKey2;
        private bool waitPressKey3;
        private bool waitPressKey4;

        private const int WM_NCLBUTTONDOWN = 161;
        private const int HT_CAPTION = 2;

        // DLL imports for key hook setup
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private bool isAnimating = false;

        private readonly tenzo32 TXCmem = new tenzo32(); // Main memory handler

        // Emulator process info and AoB pattern for aimbot detection
        private readonly string[] TaskName = { "HD-Player" };
        private readonly int ReadOffset = 0xAA;
        private readonly int WriteOffset = 0xA6;
        private readonly string AimbotPattern = "FF FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF FF FF FF FF FF FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 A5 43"; // Truncated for clarity

        public Form1()
        {
            InitializeComponent();

            this.hookCallback = new Form1.LowLevelKeyboardProc(this.HookCallback);

            // Set all keyboard hooks (bind, toggle, etc.)
            Form1.hookID1 = this.SetHook(this.hookCallback1);
            Form1.hookID2 = this.SetHook(this.hookCallback2);
            Form1.hookID3 = this.SetHook(this.hookCallback3);
            Form1.hookID4 = this.SetHook(this.hookCallback4);
            Form1.hookID5 = this.SetHook(this.hookCallback5);
            Form1.hookID6 = this.SetHook(this.hookCallback6);
            Form1.hookID7 = this.SetHook(this.hookCallback7);
            Form1.hookID8 = this.SetHook(this.hookCallback8);
            Form1.hookID9 = this.SetHook(this.hookCallback9);
            Form1.hookID = this.SetHook(this.hookCallback);

            Application.ApplicationExit += new EventHandler(this.Application_ApplicationExit);
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            Form1.UnhookWindowsHookEx(Form1.hookID);
        }

        private IntPtr SetHook(Form1.LowLevelKeyboardProc proc)
        {
            using (Process currentProcess = Process.GetCurrentProcess())
            {
                using (currentProcess.MainModule)
                {
                    IntPtr moduleHandle = Form1.GetModuleHandle((string)null);
                    return Form1.SetWindowsHookEx(13, proc, moduleHandle, 0U);
                }
            }
        }

        // Captures key input to set keybind or trigger toggle
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)256) // WM_KEYDOWN
            {
                KeysConverter keysConverter = new KeysConverter();
                string str = keysConverter.ConvertToString((Keys)Marshal.ReadInt32(lParam));

                if (this.waitPressKey)
                {
                    ((Control)this.bindBtn).ForeColor = Color.Red;
                    ((Control)this.bindBtn).Text = str.Equals("Escape") ? "None" : str;
                    this.waitPressKey = false;
                }
                else
                {
                    Keys keys = (Keys)keysConverter.ConvertFromString(((Control)this.bindBtn).Text.Replace("...", ""));
                    if (keys != Keys.None && (Keys)Marshal.ReadInt32(lParam) == keys)
                    {
                        checkBox1.Checked = !checkBox1.Checked;
                    }
                }
            }
            return Form1.CallNextHookEx(Form1.hookID1, nCode, wParam, lParam);
        }

        // Memory dictionaries for backup and restore
        private readonly Dictionary<long, byte[]> OriginalValue1 = new Dictionary<long, byte[]>();
        private readonly Dictionary<long, byte[]> OriginalValue2 = new Dictionary<long, byte[]>();
        private readonly Dictionary<long, byte[]> ReplacedValue1 = new Dictionary<long, byte[]>();
        private readonly Dictionary<long, byte[]> ReplacedValue2 = new Dictionary<long, byte[]>();

        public bool Aimbot = false;

        // Button to activate Aimbot
        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!TXCmem.getTask(TaskName))
                {
                    sta.Text = "STATUS: Emulator Not Found!!";
                    sta.ForeColor = Color.Red;
                    return;
                }

                Process targetProcess = Process.GetProcessesByName("HD-Player").FirstOrDefault();
                if (targetProcess == null)
                {
                    sta.Text = "STATUS: Emulator Not Found!!";
                    sta.ForeColor = Color.Red;
                    return;
                }

                TXCmem.OpenProcess(targetProcess.Id);
                sta.Text = "STATUS: Activating....";
                sta.ForeColor = Color.Green;
                var stopwatch = Stopwatch.StartNew();

                OriginalValue1.Clear();
                OriginalValue2.Clear();
                ReplacedValue1.Clear();
                ReplacedValue2.Clear();

                IEnumerable<long> addresses = await TXCmem.Trace(AimbotPattern);
                if (addresses == null || !addresses.Any())
                {
                    sta.Text = "STATUS: Error !!!!!";
                    sta.ForeColor = Color.Red;
                    return;
                }

                foreach (long addr in addresses)
                {
                    long readAddr = addr + ReadOffset;
                    long writeAddr = addr + WriteOffset;

                    byte[] readBytes = TXCmem.TraceHead(readAddr.ToString("X"), 4);
                    byte[] writeBytes = TXCmem.TraceHead(writeAddr.ToString("X"), 4);

                    if (readBytes == null || writeBytes == null)
                    {
                        sta.Text = "STATUS: Failed to read memory.";
                        sta.ForeColor = Color.Red;
                        continue;
                    }

                    int readValue = BitConverter.ToInt32(readBytes, 0);
                    int writeValue = BitConverter.ToInt32(writeBytes, 0);

                    OriginalValue1[writeAddr] = writeBytes;
                    OriginalValue2[readAddr] = readBytes;

                    TXCmem.SetHeadBytes(writeAddr.ToString("X"), "int", readValue.ToString());
                    TXCmem.SetHeadBytes(readAddr.ToString("X"), "int", writeValue.ToString());

                    ReplacedValue1[writeAddr] = BitConverter.GetBytes(readValue);
                    ReplacedValue2[readAddr] = BitConverter.GetBytes(writeValue);
                }

                sta.Text = $"STATUS: Aimbot loaded | Time: {stopwatch.Elapsed.TotalSeconds:F2}s";
                sta.ForeColor = Color.Green;
            }
            catch (Exception)
            {
                sta.Text = "STATUS: ERROR";
                sta.ForeColor = Color.Red;
            }
        }

        // Restore original memory (turn off aimbot)
        public void AimbotOFF()
        {
            RestoreValues1(OriginalValue1);
            RestoreValues1(OriginalValue2);
            sta.Text = "STATUS: Aimbot disabled";
            sta.ForeColor = Color.Red;
        }

        // Restore swapped memory (turn on aimbot)
        public void AimbotON()
        {
            RestoreValues1(ReplacedValue1);
            RestoreValues1(ReplacedValue2);
            sta.Text = "STATUS: Aimbot Enabled <3";
            sta.ForeColor = Color.Green;
        }

        // Helper to apply memory values
        private void RestoreValues1(Dictionary<long, byte[]> dictionary)
        {
            foreach (var entry in dictionary)
            {
                int value = BitConverter.ToInt32(entry.Value, 0);
                TXCmem.SetHeadBytes(entry.Key.ToString("X"), "int", value.ToString());
            }
        }

        private bool AimbotToggle = false;

        // Toggle aimbot manually via checkbox or bind
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!AimbotToggle)
            {
                AimbotOFF();
                AimbotToggle = true;
            }
            else
            {
                AimbotON();
                AimbotToggle = false;
            }
        }

        // When bind button is clicked, listen for key input
        private void bindBtn_Click(object sender, EventArgs e)
        {
            bindBtn.ForeColor = Color.Red;
            bindBtn.Text = "...";
            sta.Text = "Press a key to keybind";
            waitPressKey = true;
        }

        // On form load, hide window and start local web server
        private void Form1_Load(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.Opacity = 0;
            this.Hide();

            string localIP = GetLocalIPv4();
            string ipWithPortAndPath = localIP + ":6969/redx"; // Append port and path
            MessageBox.Show("Your local address is: " + ipWithPortAndPath, "IP Address");


            httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://+:6969/");
            httpListener.Start();

            listenerThread = new Thread(HandleIncomingRequests);
            listenerThread.IsBackground = true;
            listenerThread.Start();
        }

        // Handles HTTP requests from phone (remote control)
        private void HandleIncomingRequests()
        {
            while (httpListener.IsListening)
            {
                try
                {
                    var context = httpListener.GetContext();
                    var request = context.Request;
                    var response = context.Response;

                    response.Headers.Add("Access-Control-Allow-Origin", "*");
                    response.Headers.Add("Access-Control-Allow-Methods", "GET");

                    string result = "";

                    switch (request.RawUrl.ToLower())
                    {
                        case "/redx":
                            string resourceName = "RED_X_CLOUD_CONTROL_V2.form.html"; // embedded resource name
                            string html = LoadEmbeddedHtml(resourceName);
                            result = html;
                            response.ContentType = "text/html; charset=UTF-8";
                            break;
                        case "/load":
                            this.Invoke((MethodInvoker)(() => {
                                button1.PerformClick();
                                Application.DoEvents();
                                Thread.Sleep(50);
                                result = sta.Text;
                            }));
                            break;

                        case "/toggle":
                            this.Invoke((MethodInvoker)(() => {
                                checkBox1.Checked = !checkBox1.Checked;
                                Application.DoEvents();
                                Thread.Sleep(50);
                                result = sta.Text;
                            }));
                            break;

                        case "/bind":
                            this.Invoke((MethodInvoker)(() => {
                                bindBtn.PerformClick();
                                Application.DoEvents();
                                Thread.Sleep(50);
                                result = sta.Text;
                            }));
                            break;

                        case "/exit":
                            this.Invoke((MethodInvoker)(() => {
                                button2.PerformClick();
                                Application.DoEvents();
                                Thread.Sleep(50);
                                result = sta.Text;
                            }));
                            break;

                        case "/location":
                            this.Invoke((MethodInvoker)(() => {
                                button3.PerformClick();
                                Application.DoEvents();
                                Thread.Sleep(50);
                                result = sta.Text;
                            }));
                            break;

                        case "/status":
                            this.Invoke((MethodInvoker)(() => result = sta.Text));
                            break;

                        case "/ping":
                            result = "Pong";
                            break;

                        default:
                            result = "Invalid Endpoint";
                            break;
                    }

                    byte[] buffer = Encoding.UTF8.GetBytes(result);
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    response.OutputStream.Close();
                }
                catch (Exception)
                {
                    // Optional logging here
                }
            }
        }


        private string LoadEmbeddedHtml(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new FileNotFoundException($"Resource '{resourceName}' not found.");

                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }


        //ip show
        private string GetLocalIPv4()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Check for operational network interface
                if (ni.OperationalStatus == OperationalStatus.Up &&
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                     ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet))
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return ip.Address.ToString(); // Return IPv4 address
                        }
                    }
                }
            }

            return "No IPv4 address found";
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // === DLL Injection Methods ===
        const int PROCESS_CREATE_THREAD = 0x0002;
        const int PROCESS_QUERY_INFORMATION = 0x0400;
        const int PROCESS_VM_OPERATION = 0x0008;
        const int PROCESS_VM_WRITE = 0x0020;
        const int PROCESS_VM_READ = 0x0010;
        const int PROCESS_ALL_ACCESS = 0x1F0FFF;

        const uint MEM_COMMIT = 0x1000;
        const uint PAGE_READWRITE = 0x04;

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, uint size, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, IntPtr dwStackSize,
            IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr hObject);

        // Extracts DLL from embedded resources
        private void ExtractEmbeddedResource(string resourceName, string outputPath)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    MessageBox.Show("Failed to find embedded resource: " + resourceName);
                    return;
                }

                using (FileStream fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }

        // DLL injection to emulator
        private void button3_Click(object sender, EventArgs e)
        {
            string processName = "HD-Player";
            string dllName = "gncbvs.dll";
            string dllResourceName = "RED_X_CLOUD_CONTROL_BASIC.gncbvs.dll";
            string tempDllPath = Path.Combine(Path.GetTempPath(), dllName);

            if (Process.GetProcessesByName(processName).Length == 0)
            {
                sta.Text = "STATUS: Emulator Not Found!!";
                sta.ForeColor = Color.Red;
                return;
            }

            Process targetProcess = Process.GetProcessesByName(processName)[0];
            foreach (ProcessModule module in targetProcess.Modules)
            {
                if (module.ModuleName.Equals(dllName, StringComparison.OrdinalIgnoreCase))
                {
                    sta.Text = "STATUS: CHAMES MENU ALREADY INJECTED BEFORE";
                    sta.ForeColor = Color.Orange;
                    return;
                }
            }

            ExtractEmbeddedResource(dllResourceName, tempDllPath);

            IntPtr hProcess = OpenProcess(
                PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ,
                false,
                targetProcess.Id
            );

            IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            IntPtr allocMemAddress = VirtualAllocEx(hProcess, IntPtr.Zero, (IntPtr)tempDllPath.Length, MEM_COMMIT, PAGE_READWRITE);

            IntPtr bytesWritten;
            WriteProcessMemory(hProcess, allocMemAddress, Encoding.ASCII.GetBytes(tempDllPath), (uint)tempDllPath.Length, out bytesWritten);

            CreateRemoteThread(hProcess, IntPtr.Zero, IntPtr.Zero, loadLibraryAddr, allocMemAddress, 0, IntPtr.Zero);

            sta.Text = "STATUS: CHAMES MENU INJECTED";
            sta.ForeColor = Color.Green;
        }
    }
}


// ░░░░░██╗░█████╗░██╗███╗░░██╗  ░█████╗░██╗░░░██╗██████╗░
// ░░░░░██║██╔══██╗██║████╗░██║  ██╔══██╗██║░░░██║██╔══██╗
// ░░░░░██║██║░░██║██║██╔██╗██║  ██║░░██║██║░░░██║██████╔╝
// ██╗░░██║██║░░██║██║██║╚████║  ██║░░██║██║░░░██║██╔══██╗
// ╚█████╔╝╚█████╔╝██║██║░╚███║  ╚█████╔╝╚██████╔╝██║░░██║
// ░╚════╝░░╚════╝░╚═╝╚═╝░░╚══╝  ░╚════╝░░╚═════╝░╚═╝░░╚═╝

// ██████╗░██╗░██████╗░█████╗░░█████╗░██████╗░██████╗░
// ██╔══██╗██║██╔════╝██╔══██╗██╔══██╗██╔══██╗██╔══██╗
// ██║░░██║██║╚█████╗░██║░░╚═╝██║░░██║██████╔╝██║░░██║
// ██║░░██║██║░╚═══██╗██║░░██╗██║░░██║██╔══██╗██║░░██║
// ██████╔╝██║██████╔╝╚█████╔╝╚█████╔╝██║░░██║██████╔╝
// ╚═════╝░╚═╝╚═════╝░░╚════╝░░╚════╝░╚═╝░░╚═╝╚═════╝░
// https://discord.gg/f7KPc9JyeY
