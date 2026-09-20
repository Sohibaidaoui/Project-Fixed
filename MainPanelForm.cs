using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AotForms;
using Guna.UI2.WinForms;

namespace Client
{
    public class MainPanelForm : Form
    {
        private IntPtr _mainHandle;
        private bool _connected = false;

        private Guna2BorderlessForm _borderless = null!;
        private Guna2ShadowForm _shadow = null!;
        private Guna2Panel _topBar = null!;
        private Guna2Panel _mainPanel = null!;
        private Label _statusLabel = null!;
        private Guna2ToggleSwitch _adbToggle = null!;
        private Guna2CustomCheckBox _ffMaxCheckBox = null!;
        private System.ComponentModel.IContainer components = new System.ComponentModel.Container();

        public MainPanelForm(IntPtr mainHandle)
        {
            _mainHandle = mainHandle;
            InitializeUi();
        }

        private void InitializeUi()
        {
            this.AutoScaleMode = AutoScaleMode.None;
            this.ClientSize = new Size(680, 440);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(10, 13, 18);
            this.TopMost = true;

            _borderless = new Guna2BorderlessForm(this.components)
            {
                ContainerControl = this,
                BorderRadius = 14,
                HasFormShadow = true,
                TransparentWhileDrag = true
            };

            _shadow = new Guna2ShadowForm(this.components)
            {
                TargetForm = this,
                ShadowColor = Color.FromArgb(0, 229, 255)
            };

            // Top Bar
            _topBar = new Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 45,
                BackColor = Color.FromArgb(17, 22, 32)
            };
            this.Controls.Add(_topBar);

            var title = new Label
            {
                Text = "KRISHU X VIP PANEL",
                Font = new Font("Segoe UI Black", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 229, 255),
                Location = new Point(16, 12),
                AutoSize = true
            };
            _topBar.Controls.Add(title);

            var versionBadge = new Label
            {
                Text = "OB55 UPDATED",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 230, 118),
                BackColor = Color.FromArgb(33, 38, 45),
                Location = new Point(190, 14),
                Padding = new Padding(4, 1, 4, 1),
                AutoSize = true
            };
            _topBar.Controls.Add(versionBadge);

            var closeBox = new Guna2ControlBox
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(638, 8),
                Size = new Size(30, 30),
                FillColor = Color.Transparent,
                IconColor = Color.FromArgb(255, 82, 82),
                HoverState = { FillColor = Color.FromArgb(255, 23, 68), IconColor = Color.White }
            };
            closeBox.Click += (s, e) => { Environment.Exit(0); };

            var minBox = new Guna2ControlBox
            {
                ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(602, 8),
                Size = new Size(30, 30),
                FillColor = Color.Transparent,
                IconColor = Color.FromArgb(139, 148, 158)
            };
            _topBar.Controls.Add(closeBox);
            _topBar.Controls.Add(minBox);

            // Drag topbar
            var drag = new Guna2DragControl(this.components)
            {
                TargetControl = _topBar
            };

            // Main Content Panel
            _mainPanel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(14, 18, 25),
                Padding = new Padding(20)
            };
            this.Controls.Add(_mainPanel);
            _mainPanel.BringToFront();

            // Card: Connect ADB (The Main Requested Function)
            var adbCard = new Guna2Panel
            {
                Location = new Point(20, 15),
                Size = new Size(640, 100),
                BorderRadius = 10,
                BorderColor = Color.FromArgb(48, 54, 61),
                BorderThickness = 1,
                FillColor = Color.FromArgb(22, 27, 34)
            };
            _mainPanel.Controls.Add(adbCard);

            var adbTitle = new Label
            {
                Text = "CONNECT ADB & HOOK",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(18, 16),
                AutoSize = true
            };
            adbCard.Controls.Add(adbTitle);

            var adbDesc = new Label
            {
                Text = "Initializes HD-Player emulator, executes HD-Adb, maps libil2cpp.so & starts ESP.",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(139, 148, 158),
                Location = new Point(18, 44),
                AutoSize = true
            };
            adbCard.Controls.Add(adbDesc);

            // Toggle Switch for Connect ADB
            _adbToggle = new Guna2ToggleSwitch
            {
                Location = new Point(560, 28),
                Size = new Size(55, 28),
                CheckedState = { FillColor = Color.FromArgb(0, 230, 118) },
                UncheckedState = { FillColor = Color.FromArgb(48, 54, 61) },
                Cursor = Cursors.Hand
            };
            _adbToggle.CheckedChanged += AdbToggle_CheckedChanged;
            adbCard.Controls.Add(_adbToggle);

            // Status Bar Card
            var statusCard = new Guna2Panel
            {
                Location = new Point(20, 130),
                Size = new Size(640, 48),
                BorderRadius = 8,
                BorderColor = Color.FromArgb(33, 38, 45),
                BorderThickness = 1,
                FillColor = Color.FromArgb(17, 22, 32)
            };
            _mainPanel.Controls.Add(statusCard);

            var statusPrefix = new Label
            {
                Text = "STATUS:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(110, 118, 129),
                Location = new Point(16, 14),
                AutoSize = true
            };
            statusCard.Controls.Add(statusPrefix);

            _statusLabel = new Label
            {
                Text = "Idle - Ready to Connect",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(139, 148, 158),
                Location = new Point(85, 14),
                AutoSize = true
            };
            statusCard.Controls.Add(_statusLabel);

            // FF MAX Toggle
            _ffMaxCheckBox = new Guna2CustomCheckBox
            {
                Location = new Point(500, 14),
                Size = new Size(20, 20),
                CheckedState = { FillColor = Color.FromArgb(0, 229, 255) },
                UncheckedState = { FillColor = Color.FromArgb(33, 38, 45) }
            };
            statusCard.Controls.Add(_ffMaxCheckBox);

            var ffMaxLbl = new Label
            {
                Text = "Free Fire MAX",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(139, 148, 158),
                Location = new Point(526, 15),
                AutoSize = true
            };
            statusCard.Controls.Add(ffMaxLbl);

            // Features Grid
            int startY = 195;
            CreateFeatureToggle("Box ESP", 20, startY, Config.ESPBox, (val) => Config.ESPBox = val);
            CreateFeatureToggle("Snaplines ESP", 240, startY, Config.ESPLine, (val) => Config.ESPLine = val);
            CreateFeatureToggle("Skeleton ESP", 460, startY, Config.ESPSkeleton, (val) => Config.ESPSkeleton = val);

            CreateFeatureToggle("Player Name ESP", 20, startY + 45, Config.ESPName, (val) => Config.ESPName = val);
            CreateFeatureToggle("Health Bar ESP", 240, startY + 45, Config.ESPHealth, (val) => Config.ESPHealth = val);
            CreateFeatureToggle("Distance ESP", 460, startY + 45, Config.ESPDistance, (val) => Config.ESPDistance = val);

            CreateFeatureToggle("Enable Aimbot", 20, startY + 90, Config.AimBot, (val) => Config.AimBot = val);
            CreateFeatureToggle("Draw FOV Circle", 240, startY + 90, Config.Aimfovc, (val) => Config.Aimfovc = val);
            CreateFeatureToggle("Ignore Knocked", 460, startY + 90, Config.IgnoreKnocked, (val) => Config.IgnoreKnocked = val);

            CreateFeatureToggle("Streamer Mode", 20, startY + 135, Config.StreamMode, (val) => Config.StreamMode = val);
            CreateFeatureToggle("Name Background", 240, startY + 135, Config.espbg, (val) => Config.espbg = val);
            CreateFeatureToggle("No Recoil", 460, startY + 135, Config.NoRecoil, (val) => Config.NoRecoil = val);
        }

        private void CreateFeatureToggle(string title, int x, int y, bool initial, Action<bool> onChange)
        {
            var pnl = new Guna2Panel
            {
                Location = new Point(x, y),
                Size = new Size(200, 36),
                BorderRadius = 6,
                BorderColor = Color.FromArgb(33, 38, 45),
                BorderThickness = 1,
                FillColor = Color.FromArgb(22, 27, 34)
            };
            _mainPanel.Controls.Add(pnl);

            var cb = new Guna2CustomCheckBox
            {
                Location = new Point(10, 8),
                Size = new Size(18, 18),
                Checked = initial,
                CheckedState = { FillColor = Color.FromArgb(0, 230, 118) },
                UncheckedState = { FillColor = Color.FromArgb(33, 38, 45) },
                Cursor = Cursors.Hand
            };
            cb.CheckedChanged += (s, e) => onChange(cb.Checked);
            pnl.Controls.Add(cb);

            var lbl = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(230, 237, 243),
                Location = new Point(34, 9),
                AutoSize = true,
                Cursor = Cursors.Hand
            };
            lbl.Click += (s, e) => cb.Checked = !cb.Checked;
            pnl.Controls.Add(lbl);
        }

        private async void AdbToggle_CheckedChanged(object? sender, EventArgs e)
        {
            if (_adbToggle.Checked)
            {
                await Task.Delay(1500);
                await EnableHookAsync();
            }
            else
            {
                _statusLabel.Text = "Disconnected";
                _statusLabel.ForeColor = Color.FromArgb(139, 148, 158);
                _connected = false;
            }
        }

        public async Task EnableHookAsync()
        {
            if (_connected) return;

            UpdateStatus("Connecting ...", Color.FromArgb(255, 179, 0));

            var processes = Process.GetProcessesByName("HD-Player");
            if (processes.Length != 1)
            {
                UpdateStatus("Error: HD-Player not running (Single instance required)", Color.FromArgb(255, 23, 68));
                _adbToggle.Checked = false;
                return;
            }

            var process = processes[0];
            var mainModulePath = Path.GetDirectoryName(process.MainModule?.FileName);
            if (string.IsNullOrEmpty(mainModulePath))
            {
                UpdateStatus("Error: Unable to locate module path", Color.FromArgb(255, 23, 68));
                _adbToggle.Checked = false;
                return;
            }

            var adbPath = Path.Combine(mainModulePath, "HD-Adb.exe");
            if (!File.Exists(adbPath))
            {
                UpdateStatus("Error: HD-Adb.exe Not Found", Color.FromArgb(255, 23, 68));
                _adbToggle.Checked = false;
                return;
            }

            var adb = new Adb(adbPath);
            await adb.Kill();

            var started = await adb.Start();
            if (!started)
            {
                UpdateStatus("Error: Failed to start ADB server", Color.FromArgb(255, 23, 68));
                _adbToggle.Checked = false;
                return;
            }

            string pkg = _ffMaxCheckBox.Checked ? "com.dts.freefiremax" : "com.dts.freefireth";
            string lib = "libil2cpp.so";

            UpdateStatus($"Searching {lib} in {pkg}...", Color.FromArgb(0, 229, 255));

            var moduleAddr = await adb.FindModule(pkg, lib);
            if (moduleAddr == 0)
            {
                UpdateStatus("Module Not Found (Ensure game is running in lobby)", Color.FromArgb(255, 23, 68));
                _adbToggle.Checked = false;
                return;
            }

            Offsets.Il2Cpp = moduleAddr;
            Core.Handle = FindRenderWindow(_mainHandle);

            UpdateStatus("Starting ESP Overlay...", Color.FromArgb(0, 229, 255));

            var esp = new ESP();
            await esp.Start();

            new Thread(Data.Work) { IsBackground = true }.Start();

            UpdateStatus("Connected & Hooked!", Color.FromArgb(0, 230, 118));
            _connected = true;
        }

        private void UpdateStatus(string message, Color color)
        {
            if (this.IsHandleCreated)
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    _statusLabel.Text = message;
                    _statusLabel.ForeColor = color;
                }));
            }
        }

        private static IntPtr FindRenderWindow(IntPtr parent)
        {
            IntPtr renderWindow = IntPtr.Zero;
            WinAPI.EnumChildWindows(parent, (hWnd, lParam) =>
            {
                StringBuilder sb = new StringBuilder(256);
                WinAPI.GetWindowText(hWnd, sb, sb.Capacity);
                string windowName = sb.ToString();
                if (!string.IsNullOrEmpty(windowName))
                {
                    if (windowName != "HD-Player")
                    {
                        renderWindow = hWnd;
                    }
                }
                return true;
            }, IntPtr.Zero);

            return renderWindow;
        }
    }
}
